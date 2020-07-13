using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// this is the visuals/etc of the character sprite/physics.  it is used by Player which is
// in charge of getting user input and making this character do something.
public class Character : MonoBehaviour
{
    Animator animator;
    CapsuleCollider2D capsule;
    Rigidbody2D rb;
    SpriteRenderer[] spriteRenderers;
    // if we're overlapping a ladder, it will be here (regardless of whether or not we're climbing it)
    public Ladder ladder { get; private set; }
    // any non-ladder interactable colliders we're overlapping will show up here
    List<Collider2D> interactables = new List<Collider2D>();
    // the closest/most appropriate non-ladder interactable we are overlapping
    public Collider2D currentInteractable { get {
        var right = spriteRenderers[0].flipX ? transform.right : -transform.right;
        Collider2D closest = null;
        float closestDot = -9999;
        foreach (var i in interactables) {
            if (!i.name.ToLower().StartsWith("door")) {
                // it's a breach, so do this hack
                // if we're right on top of a breach, use that even if we're not looking at it
                if (Mathf.Abs(i.transform.position.x - transform.position.x) <= 1)
                    return i;
            }
            var dot = Vector2.Dot(right, ((Vector2) transform.position) - ((Vector2) i.transform.position));
            if (dot > closestDot) {
                closest = i;
                closestDot = dot;
            }
        }
        return closest;
    }}
    // if currentInteractable is a breach, it will be here (regardless of whether or not we're repairing it)
    public Collider2D breach { get {
        var i = currentInteractable;
        if (i != null && !i.name.ToLower().StartsWith("door"))
            return i;
        return null;
    } }
    // if currentInteractable is a door, it will be here
    public Collider2D door { get {
        var i = currentInteractable;
        if (i != null && i.name.ToLower().StartsWith("door"))
            return i;
        return null;
    } }
    public Transform feet, head;
    public bool touchingGround { get { return animator.GetBool("touchingGround"); } }
    public bool isRepairing { get { return animator.GetBool("isRepairing"); } }
    List<Image> waterAreas = new List<Image>();
    public bool feetUnderWater { get; private set; }
    public bool headUnderWater { get; private set; }
    public bool isDead { get { return animator.GetBool("isDead"); } }
    // store the number of hammer hits the player has done
    int currentRepairCount;
    public GameObject instructionsHold, instructionsTap;

    public bool isClimbingLadder {
        get {
            return animator.GetBool("isClimbing");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        ScanShip();
    }

    void ScanShip() {
        Debug.Log("Scanning ship...");
        var ship = GameObject.FindObjectOfType<ShipController>();
        if (ship == null) 
            Debug.LogWarning("Could not find ShipController");
        
        foreach (var i in FindObjectsOfType<Image>()) {
            if (i.name != "Fill")
                continue;
            waterAreas.Add(i);
        }
        Debug.Log($"Found {waterAreas.Count} water fill images");
    }

    void CheckForWater() {
        var corners = new Vector3[4];
        var feetPos = feet.position;
        var headPos = head.position;
        feetUnderWater = false;
        headUnderWater = false;
        foreach (var w in waterAreas) {
            w.rectTransform.GetWorldCorners(corners);
            var bl = corners[0];
            var tl = corners[1];
            var tr = corners[2];
            var br = corners[3];
            if (feetPos.x > bl.x && feetPos.x < br.x && feetPos.y > bl.y && feetPos.y < tl.y)
                feetUnderWater = true;
            if (headPos.x > bl.x && headPos.x < br.x && headPos.y > bl.y && headPos.y < tl.y)
                headUnderWater = true;
        }
    }

    void RefreshInstructions() {
        instructionsHold.SetActive(breach != null);
        instructionsTap.SetActive(door != null);
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (isDead)
            return;
        // store ladders separately from other interactables
        var l = coll.GetComponent<Ladder>();
        if (l) {
            ladder = l;
            return;
        }
        if (coll.gameObject.layer == LayerMask.NameToLayer("Interactable")) {
            Debug.Log($"Adding {coll.name}");
            interactables.Add(coll);
            // if (coll.name.ToLower().StartsWith("door")) {
            //     door = coll;
            //     Debug.Log("Got door", door);
            // } else {
            //     // must be a breach
            //     Debug.Log($"Found breach: {coll.name}");
            //     breach = coll;
            // }
            RefreshInstructions();
        }
    }

    void OnTriggerExit2D(Collider2D coll) {
        if (isDead)
            return;
        // if (coll == door) {
        //     door = null;
        //     Debug.Log("Left door", door);
        // } else if (coll == breach) {
        //     Debug.Log($"Left breach: {breach.name}");
        //     breach = null;
        // } else if (ladder != null && coll.gameObject == ladder.gameObject) {
        if (ladder != null && coll.gameObject == ladder.gameObject) {
            if (isClimbingLadder)
                StopClimbing();
            ladder = null;
        } else {
            Debug.Log($"Removing {coll.name}");
            interactables.Remove(coll);
            RefreshInstructions();
        }
    }

    // this walks to a position over the next physics update - meant to be called every FixedUpdate that
    // the player is walking
    public void WalkTo(Vector2 pos) {
        if (isClimbingLadder)
            Debug.LogError("Player can't walk when climbing");
        if (isRepairing)
            Debug.LogError("Player can't walk when repairing");
        foreach (var sr in spriteRenderers)
            sr.flipX = pos.x < transform.position.x;
        rb.MovePosition(pos);
        animator.SetBool("isClimbing", false);
        animator.SetFloat("speed", 1);
    }

    // this falls to a position over the next physics update - meant to be called every FixedUpdate that
    // the player is falling (i.e. not climbing and delta y < 0)
    // public void FallTo(Vector2 pos) {
    //     if (isClimbingLadder)
    //         Debug.LogError("Player can't fall when climbing");
    //     rb.MovePosition(pos);
    //     animator.SetBool("isClimbing", false);
    //     // todo - set falling
    //     animator.SetFloat("speed", 1);
    // }

    public void StartRepairing() {
        if (isDead || isRepairing)
            return;
        if (isClimbingLadder)
            throw new System.Exception("Can't repair when climbing a ladder");
        currentRepairCount = 0;
        animator.SetBool("isRepairing", true);
    }

    void CS_PlayFootstep() {
        if (feetUnderWater)
            GameSounds.instance.PlayerFootstepsDry();
        else
            GameSounds.instance.PlayerFootstepsWet();
    }

    void CS_PlayHammer() {
        GameSounds.instance.LeverLatch();
        currentRepairCount ++;
        if (currentRepairCount >= GameConfig.instance.repairHitsNeeded) {
            RepairBreach();
        }
    }

    void RepairBreach() {
        if (breach == null)
            throw new System.Exception("No breach for RepairBreach()");
        var room = breach.GetComponentInParent<RoomController>();
        if (room != null) {
            StopRepairing();
            // after the player does a repair, assume they don't need instructions anymore
            if (GameLoop.instance.state == GameLoop.GameState.InGame)
                instructionsHold.transform.parent.gameObject.SetActive(false);
            room.RepairBreach();
        } else
            Debug.LogWarning("Found breach with no room", breach.gameObject);
    }

    public void StopRepairing() {
        if (isDead)
            return;
        if (!isRepairing)
            throw new System.Exception("Can't stop repairing when not repairing");
        animator.SetBool("isRepairing", false);
    }

    public void SetDead() {
        // hack to turn off the parent canvas of the instructions popups
        instructionsHold.transform.parent.gameObject.SetActive(false);
        if (isDead)
            return;
        gameObject.layer = LayerMask.NameToLayer("DeadPlayer");
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddTorque(1); // make them rotate a bit
        animator.SetFloat("speed", 0);
        animator.SetBool("isDead", true);
        if (isClimbingLadder)
            StopClimbing();
        GameSounds.instance.DeathDrown();
    }

    public void StartClimbing() {
        if (isDead)
            return;
        if (isRepairing)
            throw new System.Exception("Player can't climb when repairing");
        if (ladder == null)
            throw new System.Exception("Can't start climbing when not overlapping a ladder");
        // make it so the player doesn't collide with the ladder's transition floor
        Physics2D.IgnoreCollision(capsule, ladder.transitionFloor);
        animator.SetBool("isClimbing", true);
    }

    public void StopClimbing() {
        if (!isClimbingLadder)
            throw new System.Exception("Can't stop climbing when not climbing");
        // make the player collide with the transition floor again
        Physics2D.IgnoreCollision(capsule, ladder.transitionFloor, false);
        animator.SetBool("isClimbing", false);
    }

    // this climbs to a position over the next physics update - meant to be called every FixedUpdate that
    // the player is climbing.  call StartClimbing() first.
    public void ClimbTo(Vector2 pos) {
        if (!isClimbingLadder)
            throw new System.Exception("Player can't climb when not climbing");
        foreach (var sr in spriteRenderers)
            sr.flipX = pos.x < transform.position.x;
        rb.MovePosition(pos);
        animator.SetFloat("speed", 1);
    }

    // should be called in a fixed update when the user has no directional input (but the character might
    // be falling due to gravity).  they could also be repairing here.
    public void IdleTo(Vector2 pos) {
        rb.MovePosition(pos);
        animator.SetFloat("speed", 0);
    }

    public void ToggleDoor() {
        if (door == null)
            throw new System.Exception("No door for ToggleDoor()");
        var animator = door.GetComponent<Animator>();
        if (animator != null) {
            if (animator.GetBool("closed")) {
                animator.SetBool("closed", false);
                GameSounds.instance.DoorBurst();
            } else {
                animator.SetBool("closed", true);
                GameSounds.instance.DoorClose();
            }
        } else
            Debug.LogWarning("Found door with no animator", door.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckForWater();
        if (isDead) {
            if (headUnderWater) {
                rb.gravityScale = GameConfig.instance.buoyancy;
            } else {
                rb.gravityScale = GameConfig.instance.deadGravity;
            }
        } else {
            var touchingGround = Physics2D.Raycast(feet.position, Vector2.down, 0.1f, LayerMask.GetMask("Ground"));
            animator.SetBool("touchingGround", touchingGround);
        }
    }
}
