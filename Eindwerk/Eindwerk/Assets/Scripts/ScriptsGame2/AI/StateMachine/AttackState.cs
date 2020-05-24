using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void OnEnable()
    {
        base.OnEnable();
    }
    public override void OnDisable()
    {
        base.OnDisable();
    }
    public override void Update()
    {
        if (agent.target == null)
        {
            return;
        }
        var direction = agent.target.transform.position - gameObject.transform.position;
        direction.Normalize();
        movementController.Move(gameObject.transform.InverseTransformDirection(direction));
    }
}
