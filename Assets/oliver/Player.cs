using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Character character;
    Vector2 dirInput;
    Flasher flasher;
    float o2Seconds;
    public bool isDead { get { return character.isDead; } }
    bool interactHold;

    // Start is called before the first frame update
    void Start()
    {
        o2Seconds = GameConfig.instance.o2Seconds;
        character = GetComponent<Character>();
        flasher = GetComponent<Flasher>();
        name = $"Character {Random.Range(0, 20)}";
    }

    void OnMove(InputValue value) {
        dirInput = (Vector2) value.Get();
        if (dirInput.x > 0 && dirInput.x < GameConfig.instance.directionInputMinThreshold)
            dirInput.x = 0;
        if (dirInput.x < 0 && dirInput.x > -GameConfig.instance.directionInputMinThreshold)
            dirInput.x = 0;
        if (dirInput.y > 0 && dirInput.y < GameConfig.instance.directionInputMinThreshold)
            dirInput.y = 0;
        if (dirInput.y < 0 && dirInput.y > -GameConfig.instance.directionInputMinThreshold)
            dirInput.y = 0;
    }

    void OnInteract(InputValue value) {
        if (!character) // first button press can get here before we've Start()ed
            return;
        interactHold = value.isPressed;
        if (!interactHold) {
            if (character.isRepairing)
                character.StopRepairing();
            return;
        }
        if (character.breach != null) {
            character.StartRepairing();
        } else if (character.door != null) {
            character.ToggleDoor();
        }
    }

    // todo - multiplayer/rewired?
    void GetInput() {
        dirInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        // if the player's directional input doesn't exceed a certain threshold, stop moving 
        // (could also do this in the input settings maybe)
        if (dirInput.magnitude < GameConfig.instance.directionInputMinThreshold)
            dirInput = Vector2.zero;
    }

    public void SetColor(Color color) {
        transform.Find("graphics_shirt").GetComponent<SpriteRenderer>().color = color;
    }

    public void Die() {
        if (isDead)
            return;
        flasher.flash = false;
        character.SetDead();
    }

    // Update is called once per frame
    void Update()
    {  
        if (isDead)
            return;

        // this handles players holding the repair button down before they walk into the breach trigger
        if (interactHold && !character.isRepairing && character.breach)
            character.StartRepairing();

        if (character.headUnderWater) {
            o2Seconds -= Time.deltaTime;
            flasher.flash = true;
            if (o2Seconds <= 0) 
                Die();
        } else {
            o2Seconds = GameConfig.instance.o2Seconds;
            flasher.flash = false;
        }

        // GetInput();
        // timer += Time.deltaTime;
        // if (timer >= 5) {
        //     timer = 0;
        //     Debug.Log($"Player {name} == {GetComponent<PlayerInput>().devices.Count}");
        // }
    }

    void HandleMovement() {
        // if the player is pushing up or down and we're overlapping a ladder, start climbing it
        if (!character.isClimbingLadder && character.ladder != null && dirInput.y != 0 && character.touchingGround) {
            // test to make sure the player isn't pushing up at the top of a ladder or down at the 
            // bottom of one, in which case they should not start climbing
            var ladderDir = character.ladder.GetDirection(character.feet.position);
            if (dirInput.y > 0 && ladderDir > 0 || dirInput.y < 0 && ladderDir < 0)
                character.StartClimbing();
        }

        if (character.isClimbingLadder) {
            // if the player is already climbing a ladder, we can move vertically as well as horizontally
            if (dirInput != Vector2.zero) {
                var desiredPosition = transform.position 
                        + new Vector3(dirInput.x * GameConfig.instance.walkSpeed, 
                            dirInput.y * GameConfig.instance.climbSpeed, 0) * Time.fixedDeltaTime;
                character.ClimbTo(desiredPosition);
            } else {
                character.IdleTo(transform.position);
            }
        } else if (character.isRepairing) {
            var desiredPosition = transform.position;
            desiredPosition += new Vector3(0, -GameConfig.instance.gravity * Time.fixedDeltaTime, 0);
            // send a position even if repairing (probably don't need to but it keeps us consistent)
            // actually don't because it makes things janky
            // character.IdleTo(desiredPosition);
        } else {
            var desiredPosition = transform.position 
                    + new Vector3(dirInput.x, 0, 0) * GameConfig.instance.walkSpeed * Time.fixedDeltaTime;
            desiredPosition += new Vector3(0, -GameConfig.instance.gravity * Time.fixedDeltaTime, 0);
            if (dirInput.x != 0) {
                character.WalkTo(desiredPosition);
            } else {
                // send a position even if idling in case the character is falling
                character.IdleTo(desiredPosition);
            }
        }
    }

    void FixedUpdate() {
        if (isDead)
            return;
        HandleMovement();
    }
}
