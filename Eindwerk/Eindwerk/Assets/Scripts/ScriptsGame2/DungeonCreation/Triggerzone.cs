using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triggerzone : MonoBehaviour
{
    private bool entered;
    private DungeonCreator dC;
    // Start is called before the first frame update
    void Start()
    {
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
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
                if (gameObject.name.Substring(0,5) == "Level")
                {
                    dC.CreateDungeon(0);
                }
                else if (gameObject.name.Substring(0,4) == "Shop")
                {
                    dC.CreateDungeon(1);
                }
               dC.ShowInstructionText(null);
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
                    dC.ShowInstructionText(gameObject);
                }
                //start animation if this is boss
                else if (this.transform.parent.name.Substring(0,4)=="Boss")
                {
                    this.transform.parent.Find("Canvas").gameObject.SetActive(true);
                                        
                    this.transform.parent.GetComponent<Collider>().enabled = true;
                    this.GetComponent<Collider>().enabled = false;
                    this.transform.parent.GetComponent<Animator>().SetTrigger("Start");
                }
                break;
            default:
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
                    dC.ShowInstructionText(null);
                }
                break;
            default:
                break;
        }
    }
}
