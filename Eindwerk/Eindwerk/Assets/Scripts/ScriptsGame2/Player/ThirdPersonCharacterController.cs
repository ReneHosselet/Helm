﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonCharacterController : MonoBehaviour
{
    public float speed = 1;
    private Rigidbody rb;

    private GameObject weaponAnchorPoint;
    public GameObject weapon;
    private WeaponScript heldWeapon;
    //private float angle = 90;

    private Camera cam;
    private Animator anim;

    //weapon prefab list
    public List<GameObject> weaponList;
    public List<GameObject> emitterList;
    
    //coroutine vars
    private bool isRunning;
    //dungeoncreator
    private DungeonCreator dC;

    private void Start()
    {
        weaponAnchorPoint = transform.Find("bodyparts/right arm/hand/WeaponAnchorPoint").gameObject;
        rb = this.GetComponent<Rigidbody>();
        cam = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Camera>();
        anim = this.GetComponent<Animator>();
        dC = GameObject.FindGameObjectWithTag("DungeonCreator").GetComponent<DungeonCreator>();
        //instantiate current weapon and trail renderer
        heldWeapon = Instantiate(weapon,weaponAnchorPoint.transform).GetComponent<WeaponScript>();
    }
    // Update is called once per frame
    void Update()
    {
        Attack();

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) //scroll mousewheel
        {
            Zoom(Input.GetAxis("Mouse ScrollWheel"));
        }
    }
    void FixedUpdate()
    {
        PlayerMovement();
    }
    
    private void Zoom(float val)
    {
        if (val > 0f) // zoom out
        {
            if (cam.orthographicSize > 2)
            {
                cam.orthographicSize--;
            }
        }
        else if (val < 0f) // zoom in
        {
            if (cam.orthographicSize < 10)
            {
                cam.orthographicSize++;
            }
        }
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
                transform.LookAt(new Vector3(hit.point.x, this.transform.position.y, hit.point.z));
        }
    }
    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            //attack1
            if (!isRunning)
            {
                isRunning = true;
                StartCoroutine(Attack1());
            }
        }
        else if (Input.GetButtonUp("Fire1"))
        {
        }
    }
    private IEnumerator Attack1()
    {
        float time = 0f;
            switch (heldWeapon.CheckWeapon())
            {
                case "s":
                  //current attack1 anim = 0.15f
                    time = heldWeapon.baseSpeed;
                    heldWeapon.ActivateAttack();
                    anim.SetBool("attack1", true);
                    yield return new WaitForSeconds(time);
                    anim.SetBool("attack1", false);
                    heldWeapon.DeactivateAttack();
                    yield return new WaitForSeconds(time);
                break;
                default:
                    break;
            }
        isRunning = false;
    }
}
