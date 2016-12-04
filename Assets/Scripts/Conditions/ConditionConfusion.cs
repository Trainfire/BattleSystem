using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ConfusionParameters : ConditionParameters
{
    public int MinTurns;
    public int MaxTurns;
    public float ChanceRemoval;
    public float ChanceSelfDamage;
}

public class ConditionConfusion : Condition
{
    public ConfusionParameters Parameters;

    public override ConditionType Type { get { return ConditionType.Confusion; } }

    protected override ConditionResult OnEvaluate()
    {
        if (EvaluationCount >= Parameters.MinTurns && EvaluationCount <= Parameters.MaxTurns)
        {
            var removalRoll = UnityEngine.Random.Range(0f, 1f);

            if (removalRoll <= Parameters.ChanceRemoval)
                return new ConditionResult(this, Parameters, ConditionResultType.Removed);
        }
        else if (EvaluationCount >= Parameters.MaxTurns)
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Removed);
        }

        var selfDamageRoll = UnityEngine.Random.Range(0f, 1f);
        if (selfDamageRoll <= Parameters.ChanceSelfDamage)
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Failed);
        }
        else
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Passed);
        }
    }
}
