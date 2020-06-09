using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    public float yOffset;
    public float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.Find("Player(Clone)");
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (gameObject.CompareTag("PlayerCamera"))
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + yOffset, player.transform.position.z + zOffset);
        }
        else
        {
            transform.position = new Vector3(player.transform.position.x, gameObject.transform.position.y, player.transform.position.z);
        }
        
    }
}
