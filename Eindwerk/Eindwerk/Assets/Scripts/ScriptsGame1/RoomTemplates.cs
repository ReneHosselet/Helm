using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bRooms;
    public GameObject[] tRooms;
    public GameObject[] lRooms;
    public GameObject[] rRooms;
    //public GameObject[] cRooms;

    public List<GameObject> rooms;

    public float waitTime;
    private bool eRoomSpawned;
    public GameObject eRoom;

    //test var
    public GameObject closedRoom;
    private Vector3 offset = new Vector3(0,1,0);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ERoomSpawn();
    }
    private void ERoomSpawn()
    {
        if (waitTime <= 0 && !eRoomSpawned)
        {
            Instantiate(eRoom, rooms[rooms.Count - 1].transform.position + offset, Quaternion.identity);
            eRoomSpawned = true;
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
