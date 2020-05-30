using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public float baseDamage;
    public float baseSpeed;
    
    private TrailRenderer tR;
    private Collider col;
    //wielder
    private GameObject wielder;
    //force
    public float force;
    //dungeoncreator
    private DungeonCreator dC;

    [HideInInspector]
    public ParticleSystem hitEmitter;
    // Start is called before the first frame update
    void Start()
    {
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
        CheckWeapon();
        WhoIsThisWielder();
        tR = transform.Find("trail").GetComponent<TrailRenderer>();
        col = this.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public string CheckWeapon()
    {
        string weap = "";
        switch (this.name.Substring(0, 5))
        {
            //0 = sword effect
            case "Sword":
                hitEmitter = dC.emitterList[0];
                force = 1000f;
                weap = "s";
                break;
            case "Spear":
                //spear effects en param
                hitEmitter = dC.emitterList[0];
                force = 500f;
                break;
            default:
                break;
        }
        return weap;
    }
    public void ActivateAttack()
    {
        col.enabled = true;
        tR.enabled = true;
    }
    public void DeactivateAttack()
    {
        col.enabled = false;
        tR.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (wielder.CompareTag("Player") && other.CompareTag("Enemy"))
        {
            //enemy gets damaged
            DoDamage(other,dC.playerAttackUp);
        }
        else if (wielder.CompareTag("Enemy") && other.CompareTag("Player"))
        {
            //player gets damaged
            DoDamage(other, wielder.GetComponentInParent<Enemy>().damageModifiers);
        }
        else if (wielder.CompareTag("Player") && other.CompareTag("Breakable"))
        {
            //deals damage to breakable
            DoDamage(other, dC.playerAttackUp);
        }
        else
        {
            return;
        }
    }
    private void DoDamage(Collider obj,float damagemodifier)
    {
        if (obj.tag != "Breakable")
        {
            //pushback
            obj.attachedRigidbody.AddForce((obj.transform.position - transform.position).normalized * force);
        }        
        Instantiate(hitEmitter, obj.transform.position, Quaternion.identity);
        dC.CalculateDamage(obj.gameObject, this, damagemodifier);
    }
    private void WhoIsThisWielder()
    {
        if (DungeonCreator.FindParentWithTag(gameObject,"Player") != null)
        {
            wielder = DungeonCreator.FindParentWithTag(gameObject, "Player");
        }
        else if (DungeonCreator.FindParentWithTag(gameObject, "Enemy") != null)
        {
            wielder = DungeonCreator.FindParentWithTag(gameObject, "Enemy");
        }
    }

}
