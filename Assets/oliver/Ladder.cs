using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public EdgeCollider2D transitionFloor;

    // returns -1 or 1 depending on if this ladder goes up or down from a given ground position.  i.e.,
    // if the player is standing at pos and encounters this ladder, does it go up or down?
    public int GetDirection(Vector2 pos) {
        return (transitionFloor.ClosestPoint(pos).y > pos.y) ? 1 : -1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
