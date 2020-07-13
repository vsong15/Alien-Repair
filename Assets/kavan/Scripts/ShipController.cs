using UnityEngine;

public class ShipController : MonoBehaviour
{
    public GameObject[] Rooms;
    public bool m_CreateLeak = false;
    public float m_FloodedPercentage;
    public float m_LeakPercentage;

    private void Update()
    {
        ShipStatus();

        if (m_CreateLeak)
        {
            Leak();
        }
    }

    private void ShipStatus()
    {
        float roomCount = Rooms.Length;
        float roomsFlooded = 0;
        float roomsLeakPercentage = 0;
        for (int i = 0; i < roomCount; i++)
        {
            var currentRoom = Rooms[i].GetComponent<RoomController>();
            if (currentRoom.m_RoomFlooded)
            {
                roomsFlooded += 1;
            }
            if (currentRoom.m_RoomFlooding)
            {
                roomsLeakPercentage += currentRoom.m_AveragePercentFlooded;
            }
        }
        m_FloodedPercentage = roomsFlooded / roomCount;
        m_LeakPercentage = roomsLeakPercentage / roomCount;
    }

    public void Leak()
    {

        var num = Random.Range(0, Rooms.Length);
        var randomRoom = Rooms[num].GetComponent<RoomController>();
        //where NOT to flood?
        var isRoomFlooding = randomRoom.m_RoomFlooding;
        var isRoomIntro = randomRoom.m_introRoom;

        if (isRoomFlooding == true || isRoomIntro == true)
        {
            //Debug.Log("The random room chosen is already flooding, status : " + isRoomFlooding + ", trying again...");
            return;
        }

        //Breach
        randomRoom.BreachRoom();

        //CALL LEAK SOUND
        GameSounds.instance.PipeBurst();

        //Stop Trying to CreateLeaks
        m_CreateLeak = false;
    }
}
