using UnityEngine;
using System.Collections;
using System;

public class ConditionSwitchLock : Condition
{
    public override ConditionType Type { get { return ConditionType.SwitchLock; } }

    protected override ConditionResult OnEvaluate()
    {
        return new ConditionResult(this, new ConditionParameters(), ConditionResultType.Passed);
    }
}