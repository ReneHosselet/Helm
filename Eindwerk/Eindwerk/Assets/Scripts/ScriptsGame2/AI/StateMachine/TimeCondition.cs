using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeCondition", menuName = "HELM_AI/Condition/TimeCondition")]
public class TimeCondition : Condition
{
    public float timeToWait = 2f, timePassed = 0;

    public override bool Test(Agent agent)
    {
        timePassed += Time.deltaTime;
        if (timePassed >= timeToWait)
        {
            timePassed = 0;
            return true;
        }
        return false;
    }
}
