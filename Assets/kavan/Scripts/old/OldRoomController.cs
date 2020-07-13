using UnityEngine;
using UnityEngine.UI;

public class OldRoomController : MonoBehaviour
{
    [Header("Room Settings")]
    public bool m_introBreach = false;
    public bool m_glassRoom = false;
    public bool m_Breached = false;
    public bool m_RoomFlooding = false;
    public bool m_RoomFlooded = false;
    public bool m_RoomDraining = false;
    public bool m_RoomLocked = false;
    public OldRoomController[] m_ConnectedRooms;
    public Animator[] m_ConnectedDoors;

    [Header("Flood Settings")]
    public float m_TimeToFlood = 10f;
    private float m_timer;
    public Slider m_WaterSlider;
    public GameObject m_WaterHandle;
    [Range(0, 1)]
    public float m_PercentFlooded;
    public GameObject m_BreachGO;
    public GameObject m_BreachOnElements;
    public Sprite[] BreachSprites;
    public Sprite[] RepairSprites;
    private SpriteRenderer BreachSR;

    [Header("Events")]
    public NeoDragonCP.GameEvent StartGameEvent;


    public void RandomBreachSprite()
    {
        int randomNum = Random.Range(0, BreachSprites.Length);
        BreachSR.sprite = BreachSprites[randomNum];
    }
    public void RandomRepairSprite()
    {
        int randomNum = Random.Range(0, RepairSprites.Length);
        BreachSR.sprite = RepairSprites[randomNum];
    }

    public void Repair()
    {
        GameSounds.instance.LeverLatch();

        if (m_introBreach)
        {
            StartGameEvent.Raise();
            m_introBreach = false;
            m_timer = 10f;
        }
        RandomRepairSprite();
        m_Breached = false;
        m_RoomDraining = true;

        //Overflow Drain
        for (int i = 0; i < m_ConnectedRooms.Length; i++)
        {
            if (m_ConnectedRooms[i].m_Breached == false)
            {
                m_ConnectedRooms[i].m_RoomFlooding = false;
                m_ConnectedRooms[i].m_RoomDraining = true;
            }
        }
        
    }

    private void CheckLockedStatus()
    {
        //Are all connected doors closed?
        int doorCount = 0;
        int numClosed = 0;

        if (m_ConnectedDoors.Length != 0)
        {
            doorCount = m_ConnectedDoors.Length;
            for (int i = 0; i < m_ConnectedDoors.Length; i++)
            {
                var isclosed = m_ConnectedDoors[i].GetBool("closed");
                if (isclosed)
                {
                    numClosed += 1;
                }
            }
        }

        if (doorCount != 0)
        {
            if (numClosed == doorCount)
            {
                m_RoomLocked = true;
            }
            else
            {
                m_RoomLocked = false;
            }
        }
        else
        {
            m_RoomLocked = false;
        }
    }
    private void CheckRoomFloodedStatus()
    {
        if (m_PercentFlooded > 1)
        {
            m_RoomFlooded = true;
            m_WaterHandle.SetActive(false);
        }
        else
        {
            m_RoomFlooded = false;
            m_WaterHandle.SetActive(true);
        }
    }
    private void Overflow()
    {
        for (int i = 0; i < m_ConnectedRooms.Length; i++)
        {
            if (m_ConnectedRooms[i].m_RoomLocked == false)
            {
                m_ConnectedRooms[i].m_RoomFlooding = true;
            }
        }
    }

    private bool AreNeighborsBreached()
    {   
        int numBreached = 0;

        if (m_ConnectedRooms.Length != 0)
        {
            for (int i = 0; i < m_ConnectedRooms.Length; i++)
            {
                if (m_ConnectedRooms[i].m_Breached == true)
                {
                    numBreached += 1;
                }
            }
        }

        if (numBreached != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    private void Awake()
    {
        BreachSR = m_BreachGO.GetComponentInChildren<SpriteRenderer>();
        m_timer = 0f;
    }

    private void Update()
    {
        if (m_introBreach)
        {
            return; //go no further if intro...
        }

        //A LEAK() has been created in ShipController
        if (m_Breached)
        {
            m_BreachGO.SetActive(true);
            m_BreachOnElements.SetActive(true);
            m_RoomFlooding = true;
        }
        else //Every frame make sure breach elements are off if you're !m_Breached
        {
            m_BreachOnElements.SetActive(false);
        }

        //Status set by Repair() & below if locked up
        if (m_RoomDraining)
        {
            if (m_timer <= 0)
            {
                m_RoomDraining = false;
                m_PercentFlooded = 0;
            }
            else
            {
                m_timer -= Time.deltaTime * 2;
                m_PercentFlooded = m_timer / m_TimeToFlood;
                m_WaterSlider.value = m_PercentFlooded;
            }           
        }

        CheckLockedStatus();

        // if im locked in, and theres no breach here, stop flooding and drain.
        if (m_RoomLocked && !m_Breached)
        {
            m_RoomFlooding = false;
            m_RoomDraining = true;
        }

        CheckRoomFloodedStatus();

        //FLOOD
        if (!m_RoomDraining && !m_RoomFlooded && m_RoomFlooding)
        {
            m_timer += Time.deltaTime;
            m_PercentFlooded = m_timer / m_TimeToFlood;
            m_WaterSlider.value = m_PercentFlooded;
        }

        //Overflow if breached and not locked down
        if (m_Breached && !m_RoomLocked)
        {
            Overflow();
        }

        if(m_RoomFlooded && m_RoomFlooding && !m_Breached)
        {
            m_RoomFlooding = false;
            m_RoomDraining = true;
        }

        if (m_RoomFlooding && !m_Breached && !AreNeighborsBreached())
        {
            m_RoomFlooding = false;
            m_RoomDraining = true;
        }
    }
}