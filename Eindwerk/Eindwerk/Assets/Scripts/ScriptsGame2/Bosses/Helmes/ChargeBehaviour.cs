using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeBehaviour : StateMachineBehaviour
{
    public float speed;
    private Enemy enemy;
    public float chargeDistance;
    private bool didCharge;
    private Vector3 newPos;
    public float timer;
    private Vector3 dir;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponent<Enemy>();
        enemy.GetComponent<TrailRenderer>().enabled = true;
        enemy.heldWeapon.ActivateAttack();
        didCharge = false;        
        timer = 1f;
        enemy.dirPointer.SetActive(false);

        //RaycastHit hit;
        //if (Physics.Raycast(enemy.transform.position, enemy.transform.TransformDirection(Vector3.forward)*chargeDistance, out hit))
        //{
        //    newPos = hit.transform.position;
        //    newPos.y = 0;
        //}
        newPos = animator.transform.position + animator.transform.forward;
        dir =newPos - animator.transform.position;
        dir.y = 0;
        //newPos = animator.transform.forward * Time.deltaTime * chargeDistance;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //float distance = Vector3.Distance(enemy.transform.position, newPos);

        //Debug.DrawRay(animator.transform.position, enemy.transform.TransformDirection(Vector3.forward) * chargeDistance,Color.red);
        if (timer <= 0)
        {
            enemy.heldWeapon.DeactivateAttack();
            Debug.Log("test");  
            //RaycastHit hit;
            //if (Physics.Raycast(animator.transform.position, Vector3.forward, out hit))
            //{
            
            //enemy.GetComponent<Rigidbody>().AddForce(chargeDistance);
            //animator.transform.position += animator.transform.forward * Time.deltaTime * chargeDistance;
            //if (timer <= 0)
            //   {
            //    didCharge = true;
            //  }                        
            //}
            animator.SetTrigger("Tired");
        }
        else
        {
            timer -= Time.deltaTime;
            //enemy.GetComponent<Rigidbody>().velocity =dir.normalized *chargeDistance;
            enemy.GetComponent<Rigidbody>().AddForce(dir.normalized*chargeDistance,ForceMode.VelocityChange);
            //enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, newPos - new Vector3(2, 0, 2), speed * Time.deltaTime);            
        }
        //else if (didCharge)
        //{
        //    animator.SetTrigger("Tired");
        //}
        //else
        //{
        //    timer -= Time.deltaTime;
        //}
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
