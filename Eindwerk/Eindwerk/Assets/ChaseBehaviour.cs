using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseBehaviour : StateMachineBehaviour
{
    public float timer;
    private int rand;
    public float minTime;
    public float maxTime;
    private Enemy enemy;
    private Transform playerPos;
    public float speed;
    public float rotationTime;

    
    public float attackDistance;
    private float distance;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<Enemy>();
        enemy.heldWeapon.DeactivateAttack();
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        timer = Random.Range(minTime, maxTime);
        rand = Random.Range(0, 3);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 target = new Vector3(playerPos.position.x, animator.transform.position.y, playerPos.position.z);
        Vector3 dir = playerPos.position - animator.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir.normalized);
        distance = Vector3.Distance(playerPos.position, animator.transform.position);
        if (timer <= 0)
        {
            if (rand == 0)
            {
                animator.SetTrigger("PowerUp");
            }
            else if (rand == 1 && enemy.isEnraged)
            {
                animator.SetTrigger("whirlwind");
            }
            else if (rand == 1)
            {
                animator.SetTrigger("Tired");
            }         
        }
        else if (distance <= attackDistance) // in attack range
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            timer -= Time.deltaTime;
        }
        
        animator.transform.rotation = Quaternion.Lerp(animator.transform.rotation, lookRot,Time.deltaTime*rotationTime);
        animator.transform.position = Vector3.MoveTowards(animator.transform.position, target, speed * Time.deltaTime);

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
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
