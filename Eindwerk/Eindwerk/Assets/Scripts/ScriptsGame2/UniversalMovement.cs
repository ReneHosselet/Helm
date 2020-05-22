using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class UniversalMovement : MonoBehaviour
{
    public CharacterController controller;
    protected Animator animator;
    public float speed = 10f;
    public float rotationTime = 0.5f;
    //rigidbody
    public Rigidbody rb;
    
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void Move(Vector3 localDirection)
    {
        if (localDirection.Equals(Vector3.zero))
        {
            return;
        }
        Vector3 direction = transform.right * localDirection.x + transform.forward * localDirection.z;
        //controller.Move(direction * speed * Time.deltaTime);
        rb.velocity = direction.normalized * speed * Time.deltaTime;
        //rb.AddForce(direction * speed);
        if (localDirection.z == -1 && localDirection.x == 0)
        {
            return;
        }
        Rotate(direction.normalized);
    }

    public virtual void Rotate(Vector3 direction)
    {
        if (direction.magnitude != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(direction.normalized),rotationTime);
        }
    }
}
