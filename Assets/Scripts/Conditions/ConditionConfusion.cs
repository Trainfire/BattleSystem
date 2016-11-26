using UnityEngine;
using System.Collections;
using System;

public class ConditionConfusion : Condition
{
    public float Chance;
    public TargetedAction OnFailAction;

    public override string EvaluationMessage { get { return "{TARGET} is confused!"; } }

    public override ConditionResult Evaluate()
    {
        var roll = UnityEngine.Random.Range(0f, 1f);
        var passed = roll <= Chance;

        TargetedAction action = null;
        if (!passed)
        {
            if (OnFailAction != null)
            {
                action = Instantiate(OnFailAction.gameObject).GetComponent<TargetedAction>();
            }
            else
            {
                Debug.LogError("OnFailAction is missing.");
            }
        }

        return new ConditionResult(this, passed, action);
    }
}
