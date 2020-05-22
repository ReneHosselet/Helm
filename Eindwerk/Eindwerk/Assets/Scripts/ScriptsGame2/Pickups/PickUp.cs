using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    private DungeonCreator dC;
    public int currencyValue;

    // Start is called before the first frame update
    void Start()
    {
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
        CheckPickUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void CheckPickUp()
    {
        switch (gameObject.tag)
        {
            case "Currency":
                currencyValue = 1;
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("touch");
            if (gameObject.CompareTag("Currency"))
            {
                Debug.Log("hier");
                dC.CalculateCurrency(currencyValue);
                Destroy(gameObject);
            }
        }
    }
}
