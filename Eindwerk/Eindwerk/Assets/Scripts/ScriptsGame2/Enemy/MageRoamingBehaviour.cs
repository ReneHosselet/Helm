using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageRoamingBehaviour : StateMachineBehaviour
{
    public float attackDistance;
    private float distance;
    private Enemy enemy;
    private Transform playerPos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<Enemy>();
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 direction = playerPos.position - animator.transform.position;
        Ray ray = new Ray(animator.transform.position, direction.normalized);
        RaycastHit hit;
        distance = Vector3.Distance(playerPos.position, animator.transform.position);
        if (distance <= attackDistance) // in attack range
        {
            if (Physics.Raycast(ray, out hit, direction.magnitude))
            {
                if (hit.collider.gameObject.tag.Equals(playerPos.tag))
                {
                    animator.SetTrigger("Shoot");
                    return;
                }
            }
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
