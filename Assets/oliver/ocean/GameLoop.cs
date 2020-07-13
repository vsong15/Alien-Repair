using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLoop : MonoBehaviour
{
    public Transform oceanContainer, ocean, sky;
    public ShipController ship;
    public InputManager input;

    public enum GameState {
        AtTitle,
        PanningToShip,
        WaitingForFirstRepair,
        InGame,
        Won,
        Lost
    }
    public GameState state { get; private set; }

    // assume no scaling/rotations on these!
    public float oceanLocalTop { get; private set; }
    public float skyLocalTop { get; private set; }
    public float oceanLocalBottom { get; private set; }
    public float skyLocalBottom { get; private set; }
    public float oceanHeight { get { return oceanLocalTop - oceanLocalBottom; } }
    public float skyHeight { get { return skyLocalTop - skyLocalBottom; } }
    public float skyWorldTop { get { return sky.position.y + skyLocalTop; } }
    public float oceanWorldBottom { get { return ocean.position.y + oceanLocalBottom; } }
    public float oceanContainerMinY { get { return ship.transform.position.y + Camera.main.orthographicSize; } }
    public float oceanContainerMaxY { get { return ship.transform.position.y + Camera.main.orthographicSize + cameraPanHeight; } }
    // 1 == bottom
    public float normalizedDepth { get { 
        if (state == GameState.AtTitle || state == GameState.PanningToShip) {
            return (cameraTopBorder - Camera.main.transform.position.y) / (cameraTopBorder - cameraBottomBorder);
        } else {
            return (cameraTopBorder - ship.transform.position.y) / (cameraTopBorder - cameraBottomBorder);
        }
    } }

    public MusicManager musicManager;
    
    public NeoDragonCP.GameEvent createLeak;

    public AnimationCurve panToShipCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public Slider depthGauge;

    public float nextLeakTimer;

    public GameObject victoryScreen, gameOverScreen;

    public float cameraTopBorder {
        get {
            return skyWorldTop - Camera.main.orthographicSize;
        }
    }

    public float cameraBottomBorder {
        get {
            return oceanWorldBottom + Camera.main.orthographicSize;
        }
    }

    public float cameraPanHeight {
        get {
            return cameraTopBorder - cameraBottomBorder;
        }
    }

    static GameLoop _instance;
    public static GameLoop instance { get {
        if (!_instance)
            _instance = GameObject.FindObjectOfType<GameLoop>();
        if (_instance == null)
            throw new System.Exception("Missing GameLoop instance");
        return _instance;
    } }

    bool hitBottom;

    void Start() {
        state = GameState.AtTitle;
        var p = Camera.main.transform.position;
        p.x = ocean.transform.position.x;
        p.y = cameraTopBorder;
        Camera.main.transform.position = p;

        var positions = new Vector3[10]; 
        var count = ocean.GetComponent<LineRenderer>().GetPositions(positions);
        oceanLocalTop = positions[0].y;
        oceanLocalBottom = positions[count - 1].y;
        count = sky.GetComponent<LineRenderer>().GetPositions(positions);
        skyLocalTop = positions[0].y;
        skyLocalBottom = positions[count - 1].y;
    }

    IEnumerator PanToShip() {
        float timer = 0;
        var start = Camera.main.transform.position.y;
        var dest = ship.transform.position.y;
        var distance = start - dest;
        do {
            timer += Time.deltaTime;
            var p = Camera.main.transform.position;
            p.y = cameraTopBorder - panToShipCurve.Evaluate(timer / GameConfig.instance.panToShipSeconds) * distance;
            Camera.main.transform.position = p;
            yield return new WaitForEndOfFrame();
        } while (timer < GameConfig.instance.panToShipSeconds);
        var b = Camera.main.transform.position;
        b.y = dest;
        Camera.main.transform.position = b;
        state = GameState.WaitingForFirstRepair;
    }

    public void OnFirstRepair() {
        state = GameState.InGame;
        createLeak.Raise();
        createLeak.Raise();
        createLeak.Raise();
    }

    IEnumerator Win() {
        GameSounds.instance.GameWin();
        state = GameState.Won;
        victoryScreen.SetActive(true);
        yield return new WaitForSeconds(GameConfig.instance.secondsToWaitOnFinalScreen);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator Lose() {
        yield return new WaitForSeconds(GameConfig.instance.secondsToWaitBeforeFinalScreen);
        // GameSounds.instance.GameLose();
        state = GameState.Lost;
        gameOverScreen.SetActive(true);
        yield return new WaitForSeconds(GameConfig.instance.secondsToWaitOnFinalScreen);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (state == GameState.AtTitle && input.players.Count > 0) {
            state = GameState.PanningToShip;
            StartCoroutine("PanToShip");
            musicManager.PlayAmbientMusic();
        }
        depthGauge.value = 1 - normalizedDepth;
        if (state == GameState.InGame) {
            nextLeakTimer -= Time.deltaTime;
            if (nextLeakTimer <= 0) {
                createLeak.Raise();
                nextLeakTimer = Random.Range(GameConfig.instance.minLeakEventSeconds, 
                        GameConfig.instance.maxLeakEventSeconds);
                // nextLeakTimer *= (1 - normalizedDepth * GameConfig.instance.depthLeakEventMultiplier);
                nextLeakTimer -= GameConfig.instance.perPlayerPenaltySeconds * (input.players.Count - 1);
                nextLeakTimer = Mathf.Max(nextLeakTimer, 1);
                Debug.Log($"Next leak in {nextLeakTimer}");
            }
            var maxDepthChangePerSecond = GameConfig.instance.maxSinkPercentPerSecond * cameraPanHeight;
            var risePerSecond = maxDepthChangePerSecond - (ship.m_FloodedPercentage * 40 * maxDepthChangePerSecond);
            risePerSecond = Mathf.Clamp(risePerSecond, -maxDepthChangePerSecond, maxDepthChangePerSecond);
            // moving the ocean up means sinking
            // moving the ocean down means rising
            var pos = oceanContainer.transform.position;
            var newY = pos.y - risePerSecond * Time.deltaTime;
            newY = Mathf.Clamp(newY, oceanContainerMinY, oceanContainerMaxY);
            pos.y = newY;
            oceanContainer.transform.position = pos;
            if (pos.y == oceanContainerMaxY && !hitBottom) {
                hitBottom = true;
                GameSounds.instance.StartCrash();
                StartCoroutine("Lose");
            }
            if (pos.y == oceanContainerMinY)
                StartCoroutine("Win");
            bool alive = false;
            foreach (var p in input.players) {
                if (!p.isDead) {
                    alive = true;
                    break;
                }
            }
            if (!alive)
                StartCoroutine("Lose");
        }
    }
}
