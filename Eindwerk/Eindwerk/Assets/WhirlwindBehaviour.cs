using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlwindBehaviour : StateMachineBehaviour
{
    public float timer;
    public float minTime;
    public float maxTime;

    private Transform playerPos;
    private Enemy enemy;
    public float speed;


    public float attackDistance;
    private float distance;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemy = animator.gameObject.GetComponent<Enemy>();
        enemy.heldWeapon.ActivateAttack();
        timer = Random.Range(minTime, maxTime);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 target = new Vector3(playerPos.position.x, animator.transform.position.y, playerPos.position.z);
        Vector3 dir = playerPos.position - animator.transform.position;

        distance = Vector3.Distance(playerPos.position, animator.transform.position);
        if (timer <= 0)
        {
            animator.SetTrigger("Tired");
        }
        else if (distance <= attackDistance)
        {
            return;
        }
        else
        {
            timer -= Time.deltaTime;
            animator.transform.position = Vector3.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);
        }        

    }

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
