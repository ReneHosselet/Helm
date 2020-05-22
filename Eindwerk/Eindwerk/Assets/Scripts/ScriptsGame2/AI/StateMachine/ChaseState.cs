using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public GameObject indicator;

    public override void OnEnable()
    {
        base.OnEnable();
        if (indicator)
        {
            indicator.SetActive(true);
        }
    }
    public override void OnDisable()
    {
        base.OnDisable();
        if (indicator)
        {
            indicator.SetActive(false);
        }
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
