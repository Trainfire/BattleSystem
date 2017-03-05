using UnityEngine;
using System.Collections;
using System;

public class ConditionMoveLock : Condition
{
    public override ConditionType Type { get { return ConditionType.MoveLock; } }

    protected override ConditionResult OnEvaluate()
    {
        return new ConditionResult(this, new ConditionParameters(), ConditionResultType.Passed);
    }
}
