using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenuAttribute()]
public class GameConfig : ScriptableObject
{
    // feel free to put global game settings in here
    public float walkSpeed = 2;
    public float climbSpeed = 1;
    public float directionInputMinThreshold = 0.1f;
    // doing our own gravity so we can use Rigidbody2D.MovePosition()
    public float gravity = 20;
    // when a dead player's head hits the surface the normal gravity is too high and pushes
    // them under a lot, so this is a quick attempt to fix that by using a smaller value
    public float deadGravity = 10;
    public float buoyancy = -0.1f; // floats characters when dead
    public float descendTimeInSeconds = 90;
    public float o2Seconds = 10;
    public float flashIntervalSeconds = 0.5f;
    public float secondsToShowPopupTips = 30;
    // when the game starts, how long do we want to take to pan down to the ship?
    public float panToShipSeconds = 10;
    public int repairHitsNeeded = 5; // how many hammer hits to repair a breach
    public float percentageForSink = 0.1f; // percentage of rooms leaky for sinking
    public float maxSinkPercentPerSecond = 0.01f; // sink one percentage of the depth range per second at max
    public float minLeakEventSeconds = 25;
    public float maxLeakEventSeconds = 35;
    [Range(0.1f, 1)]
    public float depthLeakEventMultiplier = 0.5f; // a lower value means more frequent leak events
    // for each additional player beyond 1 we subtract this many seconds for leaks
    public float perPlayerPenaltySeconds = 5f; 
    // wait a few seconds before showing the win/loss card
    public float secondsToWaitBeforeFinalScreen = 5;
    // wait on the win/loss card
    public float secondsToWaitOnFinalScreen = 10;

    static GameConfig _instance;
    public static GameConfig instance { get {
        if (!_instance)
            _instance = Resources.Load<GameConfig>("Game Config");
        if (_instance == null)
            throw new System.Exception("Missing GameConfig file");
        return _instance;
    } }
}
