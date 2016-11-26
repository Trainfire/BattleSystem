using UnityEngine;
using System.Collections;

public class ConditionResult
{
    public Condition Condition { get; private set; }
    public bool Passed { get; private set; }
    public TargetedAction Action { get; private set; }

    public ConditionResult(Condition condition, bool passed, TargetedAction action)
    {
        Condition = condition;
        Passed = passed;
        Action = action;
    }
}

public abstract class Condition : MonoBehaviour
{
    public virtual string EvaluationMessage { get { return string.Empty; } }
    public abstract ConditionResult Evaluate();
}
