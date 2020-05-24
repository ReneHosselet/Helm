using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InAttackRangeCondition", menuName = "HELM_AI/Condition/InAttackRangeCondition")]
public class InAttackRangeCondition : Condition
{
    public override bool Test(Agent agent)
    {
        return agent.target == null;
    }
}
