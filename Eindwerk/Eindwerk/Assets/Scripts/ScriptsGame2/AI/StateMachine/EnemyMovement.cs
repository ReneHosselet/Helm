using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMovement : UniversalMovement
{
    public override void Move(Vector3 localDirection)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        if (rb.velocity.magnitude != 0)
        {
            animator.SetBool("running",true);
        }
        else
        {
            animator.SetBool("running", false);
        }
        base.Move(localDirection);
    }
}
