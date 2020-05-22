using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SightCondition",menuName = "HELM_AI/Condition/SightCondition")]
public class SightCondition : Condition
{
    public override bool Test(Agent agent)
    {
        return agent.target;
    }
}
