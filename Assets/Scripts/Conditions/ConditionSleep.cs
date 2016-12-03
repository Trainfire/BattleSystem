using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SleepParameters : ConditionParameters
{
    public int MinTurns;
    public int MaxTurns;
    public float WakeUpChance;
}

public class ConditionSleep : Condition
{
    public SleepParameters Parameters;

    protected override ConditionResult OnEvaluate()
    {
        if (EvaluationCount >= Parameters.MinTurns && EvaluationCount <= Parameters.MaxTurns)
        {
            var roll = UnityEngine.Random.Range(0f, 1f);
            
            if (roll <= Parameters.WakeUpChance)
                return new ConditionResult(this, Parameters, ConditionResultType.Removed);
        }
        else if (EvaluationCount >= Parameters.MaxTurns)
        {
            return new ConditionResult(this, Parameters, ConditionResultType.Removed);
        }

        return new ConditionResult(this, Parameters, ConditionResultType.Failed);
    }

}
