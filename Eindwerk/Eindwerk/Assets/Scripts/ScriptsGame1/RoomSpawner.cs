using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    //1 = need T
    //2 = need B
    //3 = need R
    //4 = need L

    private RoomTemplates templates;
    private int random;
    private bool spawned = false;

    public float waitTime = 4f;

    private void Start()
    {
        Destroy(gameObject, waitTime);
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn",0.1f);
    }
    private void Spawn()
    {
        if (!spawned)
        {
            switch (openingDirection)
            {
                case 1: //need room with top opening
                    random = Random.Range(0, templates.tRooms.Length);
                    Instantiate(templates.tRooms[random], transform.position, Quaternion.identity);
                    break;
                case 2: //need room with bottom opening
                    random = Random.Range(0, templates.bRooms.Length);
                    Instantiate(templates.bRooms[random], transform.position, Quaternion.identity);
                    break;
                case 3: //need room with right opening
                    random = Random.Range(0, templates.rRooms.Length);
                    Instantiate(templates.rRooms[random], transform.position, Quaternion.identity);
                    break;
                case 4: //need room with left opening
                    random = Random.Range(0, templates.lRooms.Length);
                    Instantiate(templates.lRooms[random], transform.position, Quaternion.identity);
                    break;
                default:
                    break;
            }
            spawned = true;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if (!other.GetComponent<RoomSpawner>().spawned && !spawned)
            {
                // spawns wall blocking off openings
                //random = Random.Range(0, templates.cRooms.Length);
                //Instantiate(templates.cRooms[random], transform.position, Quaternion.identity);
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                //Destroy(gameObject);
            }
            spawned = true;
        }
    }
}
