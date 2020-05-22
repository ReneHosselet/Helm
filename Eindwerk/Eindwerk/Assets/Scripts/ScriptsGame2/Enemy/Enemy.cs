using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health;
    public float unmodifiedHealth;
    private float maxHealth;

    //healthmodifier per level
    public float healthModifier = 5f;
    //weapon
    public GameObject weapon;
    private WeaponScript heldWeapon;
    private GameObject weaponAnchorPoint;
    //damage modifier
    private float damageModifiers;

    private GameObject canvas;
    private GameObject sliderObj;
    private Slider slider;

    private DungeonCreator dC;

    private void Start()
    {
        weaponAnchorPoint = transform.Find("body/hand/weaponAnchorPoint").gameObject;
        //healthbar on start
        canvas = gameObject.transform.Find("Canvas").gameObject;
        sliderObj = canvas.transform.GetChild(0).GetComponentInChildren<Slider>().gameObject;
        slider = sliderObj.GetComponentInChildren<Slider>();
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();

        //give enemy weapon
        heldWeapon = Instantiate(weapon, weaponAnchorPoint.transform).GetComponent<WeaponScript>();

        //start health
        maxHealth = unmodifiedHealth + (healthModifier * dC.currentDungeonLevel);
        health = maxHealth;

        slider.value = dC.CalculateHealth(health,maxHealth);
    }
    public void Update()
    {
        slider.value = dC.CalculateHealth(health, maxHealth);

        //show health bars if damaged
        if (health < maxHealth)
        {
            sliderObj.SetActive(true);
        }
        else if (health >= maxHealth)
        {
            sliderObj.SetActive(false);
        }
        //check death
        if (health <= 0)
        {
            //initialise destroy/death procedure
            dC.OnDeath(gameObject);
        }
    }
    //private void OnDeath()
    //{
    //    //enemy on death
    //    Destroy(gameObject);
    //}
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Player":
                //deals damage to player when hit with body
                dC.CalculateDamage(collision.gameObject,heldWeapon,damageModifiers);
                break;
            default:
                break;
        }
    }
}
