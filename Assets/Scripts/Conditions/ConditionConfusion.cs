using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ConfusionParameters : ConditionParameters
{
    public float Chance;
}

public class ConditionConfusion : Condition
{
    public ConfusionParameters Parameters;

    protected override ConditionResult OnEvaluate()
    {
        var roll = UnityEngine.Random.Range(0f, 1f);
        var passed = roll <= Parameters.Chance;

        if (passed)
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Passed);
        }
        else
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Failed);
        }
    }
}
