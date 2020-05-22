using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public float speed = 1;
    private Rigidbody rb;

    private GameObject weaponAnchorPoint;
    public GameObject weapon;
    //private float angle = 90;

    private Camera cam;
    private Animator anim;
    private TrailRenderer tR;
    private Collider col;

    //force
    public float force;
    //coroutine vars
    private bool running;

    private void Start()
    {
        weaponAnchorPoint = transform.Find("bodyparts/right arm/hand/WeaponAnchorPoint").gameObject;
        rb = this.GetComponent<Rigidbody>();
        cam = GameObject.Find("Camera Player").GetComponent<Camera>();
        anim = this.GetComponent<Animator>();

        //instantiate current weapon and trail renderer
        weapon = Instantiate(weapon,weaponAnchorPoint.transform);
        tR = weapon.transform.Find("trail").GetComponent<TrailRenderer>();
        col = weapon.transform.Find("collider").GetComponent<Collider>();
    }
    // Update is called once per frame
    void Update()
    {
        Attack();
    }
    void FixedUpdate()
    {
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        //basic movement
        float hor = Input.GetAxisRaw("Horizontal");
        float ver = Input.GetAxisRaw("Vertical");

        //rb.MovePosition(transform.position + new Vector3(hor, 0f, ver).normalized * speed * Time.deltaTime);
        rb.velocity = new Vector3(hor, 0f, ver).normalized * speed * Time.deltaTime;

        //set run animation
        if (rb.velocity.magnitude > 0)
        {
            anim.SetBool("running",true);
        }
        else
        {
            anim.SetBool("running", false);
        }

        
        //looks at mouse pointer
        Ray mouseRay = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(mouseRay,out hit, 100))
        {
            transform.LookAt(new Vector3(hit.point.x,this.transform.position.y, hit.point.z));
        }
    }
    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //attack1
            if (!running)
            {
                running = true;
                StartCoroutine(Attack1(weapon));
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
        }
    }
    private void DealDamage()
    {
        if (weapon.name.Substring(0, 5) == "Sword")
        {
            //set animations to sword
        }
    }
    private IEnumerator Attack1(GameObject obj)
    {
        float time = 0f;
            switch (obj.name.Substring(0, 5))
            {
                case "Sword":
                  //current attack1 anim = 0.15f
                    time = 0.15f;
                    col.enabled = true;
                    tR.enabled = true;
                    anim.SetBool("attack1", true);
                    yield return new WaitForSeconds(time);
                    col.enabled = false;
                    anim.SetBool("attack1", false);
                    col.enabled = false;
                    yield return new WaitForSeconds(time);
                    tR.enabled = false;
                break;
                default:
                    break;
            }
        running = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Enemy":
                other.attachedRigidbody.AddForce((other.transform.position - transform.position).normalized * force);
                Debug.Log("HIT");
                break;
            default:
                break;
        }
    }
}
