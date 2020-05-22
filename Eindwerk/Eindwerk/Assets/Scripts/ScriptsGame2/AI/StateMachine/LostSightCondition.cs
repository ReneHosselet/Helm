﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LostSightCondition", menuName = "HELM_AI/Condition/LostSightCondition")]
public class LostSightCondition : Condition
{
    public override bool Test(Agent agent)
    {
        return agent.target==null;
    }
}
