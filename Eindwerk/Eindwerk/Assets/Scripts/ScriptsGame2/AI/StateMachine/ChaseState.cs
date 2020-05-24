using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State
{
    public GameObject indicator;
    private Enemy enemy;
    public float attackDistance = 3.0f;

    private void Start()
    {
        enemy = gameObject.GetComponent<Enemy>();
    }
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
        var distance = Vector3.Distance(agent.target.transform.position, gameObject.transform.position);
        var direction = agent.target.transform.position - gameObject.transform.position;
        direction.Normalize();
        if (distance <= attackDistance)
        {
            enemy.Attack();
        }
        else
        {
            movementController.Move(gameObject.transform.InverseTransformDirection(direction));
        }
    }
}
