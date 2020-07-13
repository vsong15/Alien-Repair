using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomController : MonoBehaviour
{
    [Header("Room Settings")]
    public bool m_hallway = false;
    public bool m_introRoom = false;
    public bool m_glassRoom = false;
    public float m_glassRoomBScale = 1.5f;

    public TileManager[] m_ConnectedTiles;
    public TileManager[] m_ConnectedDoors;

    [Header("Breach Variables")]
    public bool m_RoomBreached = false;
    public float minBScale = .5f;
    public float maxBScale = 1f;
    public TileManager m_BreachedTile;
    public GameObject m_BreachGO;
    public GameObject m_BreachOnElements;
    public Sprite[] BreachSprites;
    public Sprite[] RepairSprites;
    private SpriteRenderer BreachSR;


    [Header("Flood Settings")]
    public float m_TimeToFloodRoom = 10;
    public bool m_RoomFlooding = false;
    public bool m_RoomFlooded = false;
    [Range(0, 1)]
    public float m_PercentTilesFlooding;
    [Range(0, 1)]
    public float m_AveragePercentFlooded;


    [Header("Events")]
    public NeoDragonCP.GameEvent StartGameEvent;


    public void SetRandomBreachSprite()
    {
        int randomNum = Random.Range(0, BreachSprites.Length);
        BreachSR.sprite = BreachSprites[randomNum];
    }
    public void SetRandomRepairSprite()
    {
        int randomNum = Random.Range(0, RepairSprites.Length);
        BreachSR.sprite = RepairSprites[randomNum];
    }

    public void BreachRoom()
    {
        //SetRandomRotation
        Vector3 euler = m_BreachGO.transform.eulerAngles;
        euler.z = Random.Range(0f, 360f);
        m_BreachGO.transform.eulerAngles = euler;
        //SetRandomScale
        if (m_glassRoom)
        {
            m_BreachGO.transform.localScale = new Vector3(m_glassRoomBScale, m_glassRoomBScale, m_glassRoomBScale);
        }
        else //all other rooms
        {
            var randomScale = Random.Range(minBScale, maxBScale);
            m_BreachGO.transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        }

        SetRandomBreachSprite();
        m_RoomBreached = true;
        m_BreachGO.SetActive(true);
        m_BreachOnElements.SetActive(true);
        m_BreachedTile.m_isBreached = true;
        
        //m_RoomFlooding = true;
        // m_BreachedTile.StartCoroutine("Flooding");

        m_BreachedTile.m_isDraining = false;
    }  

    public void RepairBreach()
    {
        if (m_introRoom)
        {
            StartGameEvent.Raise();
            m_introRoom = false;

            //Drain ALL TILES (intro only)
            for (int i = 0; i < m_ConnectedTiles.Length; i++)
            {
                m_ConnectedTiles[i].m_timer = 0.8f;
                m_ConnectedTiles[i].m_isDraining = true;
            }
        }


        //for all repairs
        m_RoomBreached = false;
        m_BreachOnElements.SetActive(false);
       // m_RoomFlooding = false;

        SetRandomRepairSprite();

        //Call on breached tile to InitiateDrain();
        for (int i = 0; i < m_ConnectedTiles.Length; i++)
        {
            if (m_ConnectedTiles[i].m_isBreached)
            {
                m_ConnectedTiles[i].InitiateDrain();
            }
        }
    }


    private void CheckRoomFloodedStatus()
    {
        float numFloodingTiles = 0;
        float totalAmountFlooded = 0;
        int numOfConnectedTiles = m_ConnectedTiles.Length;

        for (int i = 0; i < numOfConnectedTiles; i++)
        {
            if (m_ConnectedTiles[i].m_isFlooding)
            {
                totalAmountFlooded += m_ConnectedTiles[i].m_PercentFlooded;
                numFloodingTiles += 1;
            }

            if (numFloodingTiles >= 1)
            {
                m_RoomFlooding = true;
            }
            else
            {
                m_RoomFlooding = false;
            }
        }

        m_AveragePercentFlooded = totalAmountFlooded / (numOfConnectedTiles * 100);
        m_PercentTilesFlooding = numFloodingTiles / numOfConnectedTiles;

        //Room is considered Flooded if over 50% of the contained tiles capacity is filled
        if (m_AveragePercentFlooded > .5f)
        {
            m_RoomFlooded = true;
        }
        else
        {
            m_RoomFlooded = false;
        }
    }

    public IEnumerator CheckRoomDrainStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            //Debug.Log(this.name + " is running Drain Check");
            float totalClosedDoors = 0;
            int numDoors = m_ConnectedDoors.Length;

            for (int i = 0; i < m_ConnectedDoors.Length; i++)
            {
                if (m_ConnectedDoors[i].m_doorClosed == true)
                {
                    totalClosedDoors += 1;
                }
            }
            if (totalClosedDoors == numDoors && !m_RoomBreached)
            {
                //Drain Room
                for (int i = 0; i < m_ConnectedTiles.Length; i++)
                {
                    m_ConnectedTiles[i].m_isDraining = true;
                }
            }
        }
    }

    private void Awake()
    {
        if (m_ConnectedDoors.Length > 0) // all rooms EXCEPT intro
        {
            StartCoroutine("CheckRoomDrainStatus");
        }
       
        BreachSR = m_BreachGO.GetComponentInChildren<SpriteRenderer>();

        for (int i = 0; i < m_ConnectedTiles.Length; i++)
        {
            m_ConnectedTiles[i].m_TimeToFlood = m_TimeToFloodRoom;
        }

        if (m_hallway)
        {
            for (int i = 0; i < m_ConnectedTiles.Length; i++)
            {
                m_ConnectedTiles[i].m_isHallway = true;
            }
        }

        if (m_introRoom)
        {
            for (int i = 0; i < m_ConnectedTiles.Length; i++)
            {
                m_ConnectedTiles[i].m_WaterSlider.value = 5f;
            }
        }

    }

    private void Update()
    {
        CheckRoomFloodedStatus();    
    }
}
