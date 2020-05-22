using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public float baseDamage;
    public float baseSpeed;

    public List<GameObject> weaponList;
    public List<ParticleSystem> emitterList;

    private TrailRenderer tR;
    private Collider col;

    //force
    public float force;

    [HideInInspector]
    public ParticleSystem hitEmitter;
    // Start is called before the first frame update
    void Start()
    {
        CheckWeapon();
        tR = transform.Find("trail").GetComponent<TrailRenderer>();
        col = transform.Find("collider").GetComponent<Collider>();
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
                hitEmitter = emitterList[0].gameObject.GetComponent<ParticleSystem>();
                baseDamage = 3;
                baseSpeed = 0.15f;
                force = 1000f;
                weap = "s";
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
}
