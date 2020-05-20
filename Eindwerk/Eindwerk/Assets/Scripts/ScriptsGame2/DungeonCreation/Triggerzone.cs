using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerzone : MonoBehaviour
{
    private bool entered;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KeyPressed();
    }
    private void KeyPressed()
    {
        if (entered)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject.Find("DungeonCreator").GetComponent<DungeonCreator>().CreateDungeon();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
                if (this.CompareTag("EndPoint"))
                {
                    entered = true;
                    this.transform.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                }
                break;
            default:
                entered = false;
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Player":
                if (this.CompareTag("EndPoint"))
                {
                    entered = false;
                    this.transform.GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                }
                break;
            default:
                break;
        }
    }
}
