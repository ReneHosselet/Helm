using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
     private Rigidbody rb;
     public float speed = 5f;
    private DungeonCreator dC;
    private GameObject parentObject;
    private Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
        WhoIsThisWielder();
        parentObject = this.transform.parent.gameObject;
        dir = parentObject.transform.forward.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(dir * speed, ForceMode.VelocityChange);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (parentObject.CompareTag("Player") && other.CompareTag("Enemy"))
        {
            //enemy gets damaged
            DoDamage(other, dC.playerAttackUp);
            Destroy(this.gameObject);
        }
        else if (parentObject.CompareTag("Enemy") && other.CompareTag("Player"))
        {
            //player gets damaged
            DoDamage(other, parentObject.GetComponentInParent<Enemy>().damageModifiers);
            Destroy(this.gameObject);
        }
        else if (parentObject.CompareTag("Player") && other.CompareTag("Breakable"))
        {
            //deals damage to breakable
            DoDamage(other, dC.playerAttackUp);
            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Wall"))
        {
            Instantiate(dC.emitterList[1], other.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        else
        {
            return;
        }
    }
    private void DoDamage(Collider obj, float damagemodifier)
    {
        //1 = firexplosion
        if (obj.tag != "Breakable")
        {
            //pushback
            obj.attachedRigidbody.AddForce((obj.transform.position - transform.position).normalized * parentObject.GetComponent<Enemy>().heldWeapon.force);
        }
        if (parentObject.tag == "Player")
        {
            Instantiate(dC.emitterList[1], obj.transform.position, Quaternion.identity);
            dC.CalculateDamage(obj.gameObject, parentObject.GetComponent<ThirdPersonCharacterController>().heldWeapon, damagemodifier);
        }
        else if (parentObject.tag == "Enemy")
        {
            Instantiate(dC.emitterList[1], obj.transform.position, Quaternion.identity);
            dC.CalculateDamage(obj.gameObject, parentObject.GetComponent<Enemy>().heldWeapon, damagemodifier);
        }
        
    }
    private void WhoIsThisWielder()
    {
        if (DungeonCreator.FindParentWithTag(gameObject, "Player") != null)
        {
            parentObject = DungeonCreator.FindParentWithTag(gameObject, "Player");
        }
        else if (DungeonCreator.FindParentWithTag(gameObject, "Enemy") != null)
        {
            parentObject = DungeonCreator.FindParentWithTag(gameObject, "Enemy");
        }
    }
}
