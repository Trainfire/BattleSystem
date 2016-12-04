using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ParalysisParameters : ConditionParameters
{
    public float Chance;
}

public class ConditionParalysis : Condition
{
    public ParalysisParameters Parameters;

    public override ConditionType Type { get { return ConditionType.Paralysis; } }

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
