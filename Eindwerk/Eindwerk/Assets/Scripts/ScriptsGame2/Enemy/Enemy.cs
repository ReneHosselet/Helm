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
    public WeaponScript heldWeapon;
    private GameObject weaponAnchorPoint;
    //damage modifier
    public float damageModifiers = 0;
    //animator
    private Animator anim;
    private bool isAttackRunning;

    public bool isBoss;
    private bool isRange;

    private GameObject canvas;
    private GameObject sliderObj;
    private Slider slider;

    private DungeonCreator dC;
    public bool isEnraged;

    public GameObject projectile;

    public GameObject dirPointer;

    //protrets for bosses
    public Image bossPortret;
    public List<Sprite> bossAvatars;
    
    private float time;
    private void Start()
    {
        CheckMob();
        //animator
        anim = gameObject.GetComponent<Animator>();
        //healthbar on start
        canvas = gameObject.transform.Find("Canvas").gameObject;
        sliderObj = canvas.transform.GetChild(0).GetComponentInChildren<Slider>().gameObject;
        slider = sliderObj.GetComponentInChildren<Slider>();
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();

        //give enemy weapon
        heldWeapon = Instantiate(weapon, weaponAnchorPoint.transform).GetComponent<WeaponScript>();

        //start health
        maxHealth = unmodifiedHealth + (healthModifier * (dC.currentDungeonLevel/2));
        health = maxHealth;

        slider.value = dC.CalculateHealth(health,maxHealth);

        time = heldWeapon.baseSpeed;
    }
    public void Update()
    {
        if (weaponAnchorPoint = null)
        {
            CheckMob();
        }
        slider.value = dC.CalculateHealth(health, maxHealth);

        if (!isBoss)
        {
            //show health bars if damaged
            if (health < maxHealth)
            {
                sliderObj.SetActive(true);
            }
            else if (health >= maxHealth)
            {
                sliderObj.SetActive(false);
            }
        }
        //check boss is over healthtreshold
        
        if (isBoss)
        {
            if (health <= maxHealth / 2)
            {
                if (!isEnraged)
                {                    
                    anim.SetTrigger("Enraged");
                    isEnraged = true;
                }
            }
            changeBossAvatar();
        }
        //check death
        if (health <= 0)
        {
            //initialise destroy/death procedure
            dC.OnDeath(gameObject);
        }
    }
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
    public void Attack()
    {
        if (!isBoss && !isRange)
        {
            if (!isAttackRunning)
            {
                StartCoroutine(AttackSequence());
            }
        }
    }
    private IEnumerator AttackSequence()
    {
        switch (heldWeapon.CheckWeapon())
        {
            case "s":
                //current swordattack anim 
                time = heldWeapon.baseSpeed;
                heldWeapon.ActivateAttack();
                anim.SetBool("IsInAttackRange", true);
                //0 = sword wooshsound
                dC.PlaySound(dC.audioEffectList[0]);
                yield return new WaitForSeconds(time);
                anim.SetBool("IsInAttackRange", false);
                heldWeapon.DeactivateAttack();
                yield return new WaitForSeconds(time);
                break;
            default:
                break;
        }
        isAttackRunning = false;
    }
    private void CheckMob()
    {
        if (this.name.Substring(0, 4) == "Boss")
        {
            projectile = null;
            isBoss = true;
            isRange = false;
            weaponAnchorPoint = transform.Find("RightArm/hand/weaponAnchorPoint").gameObject;
        }
        else if (this.name.Substring(0, 4) == "Mage")
        {
            isBoss = false;
            isRange = true;
            weaponAnchorPoint = transform.Find("body/RightArm/hand/weaponAnchorPoint").gameObject;
        }
        else
        {
            projectile = null;
            isBoss = false;
            isRange = false;
            weaponAnchorPoint = transform.Find("body/hand/weaponAnchorPoint").gameObject;
        }
    }
    private void changeBossAvatar()
    {
        float currhealth = health / maxHealth;

        if (currhealth >= 0.75)
        {
            bossPortret.sprite = bossAvatars[0];
        }
        else if (currhealth < 0.75 && currhealth > 0.25)
        {
            bossPortret.sprite = bossAvatars[1];
        }
        else if (currhealth <= 0.25)
        {
            bossPortret.sprite = bossAvatars[2];
        }
    }
}
