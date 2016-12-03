using UnityEngine;
using System;

[Serializable]
public class ConditionParameters
{
    public string OnAddMessage;
    public string OnEvaluateMessage;
    public string OnFailMessage;
    public string OnRemoveMessage;

    public TargetedAction OnFailAction;
    public TargetedAction OnRemoveAction;
}

public enum ConditionResultType
{
    Failed,
    Passed,
    Removed,
}

public class ConditionResult
{
    public Condition Condition { get; private set; }
    public ConditionParameters Parameters { get; private set; }
    public ConditionResultType Type { get; private set; }

    public ConditionResult(Condition condition, ConditionParameters parameters, ConditionResultType type)
    {
        Condition = condition;
        Parameters = parameters;
        Type = type;
    }
}

public abstract class Condition : MonoBehaviour
{
    public bool IsNew { get { return EvaluationCount <= 1; } }

    protected int EvaluationCount { get; private set; }

    public ConditionResult Evaluate()
    {
        EvaluationCount++;
        return OnEvaluate();
    }

    protected abstract ConditionResult OnEvaluate();
}
