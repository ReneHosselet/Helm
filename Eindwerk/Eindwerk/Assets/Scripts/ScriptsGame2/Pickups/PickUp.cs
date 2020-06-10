using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUp : MonoBehaviour
{
    private DungeonCreator dC;
    public int currencyValue;
    public int pickUpValue;

    private int pickUpModifier = 0;
    private int rand;
    private int modifyPerXLevels = 3;
    private Text currencyTxt;
    private Image currencyIcon;
    private bool enteredArea;

    // Start is called before the first frame update
    void Start()
    {
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
        
        pickUpModifier = dC.currentDungeonLevel / modifyPerXLevels;
        rand = Random.Range(1, pickUpModifier+2);

        CheckPickUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && enteredArea)
        {
            if (gameObject.CompareTag("PickUp"))
            {
                dC.ApplyPickUp(gameObject,currencyValue,pickUpValue);
            }
        }
    }
    private void CheckPickUp()
    {
        switch (gameObject.tag)
        {
            case "Currency":
                currencyValue = 1;
                break;
            case "PickUp":
                SetPickupValues();
                break;
            default:
                break;
        }
    }
    private void SetPickupValues()
    {
        if (pickUpModifier >= 1)
        {
            currencyValue *= rand;
            pickUpValue *= rand;
        }
        ShowCost(currencyValue);
    }
    private void ShowCost(int value)
    {
        currencyTxt = transform.GetComponentInChildren<Text>();
        currencyIcon = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        if (value == 0)
        {
            currencyTxt.text ="";
            currencyIcon.enabled = false;
        }
        else
        {
            currencyTxt.text = value.ToString();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enteredArea = true;
            if (gameObject.CompareTag("Currency"))
            {
                //0 == coin sound
                
                dC.CalculateCurrency(currencyValue);
                dC.PlaySound(dC.audioPickupList[0]);
                Destroy(gameObject);
            }
            else
            {
                dC.ShowInstructionText(gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enteredArea = false;
            dC.ShowInstructionText(null);
        }
    }
}
