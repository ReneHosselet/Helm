using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : UniversalMovement 
{
    
    Animator animator;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (vertical != 0)
            animator.SetFloat("velocity", vertical);
        //else if (vertical < 0)
        //    animator.SetFloat("velocity", -1);
        else
            animator.SetFloat("velocity", 0);
        Move(new Vector3(horizontal,0, vertical));

    }

}
