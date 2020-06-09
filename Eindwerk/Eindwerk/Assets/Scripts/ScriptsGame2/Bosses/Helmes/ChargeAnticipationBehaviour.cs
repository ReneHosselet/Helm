using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAnticipationBehaviour : StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;
    private Enemy enemy;

    public GameObject directionPointer;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<Enemy>();
        timer = Random.Range(minTime, maxTime);
        enemy.GetComponent<TrailRenderer>().enabled = true;
        animator.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform.position);
        enemy.dirPointer.SetActive(true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timer <= 0)
        {
            animator.SetTrigger("Charge");
        }
        else
        {
            timer -= Time.deltaTime;
        }
        //Debug.DrawRay(enemy.transform.position, enemy.transform.TransformDirection(Vector3.forward) * 10,Color.red);
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
