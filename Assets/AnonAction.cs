using UnityEngine;
using System;

public class AnonAction : BaseAction
{
    private Action _action;

    public void Initialize(Action action)
    {
        _action = action;
    }

    protected override void OnExecute()
    {
        _action.Invoke();
        _action = null;

        TriggerCompletion();

        Destroy(gameObject);
    }

    public static AnonAction Create(Action action)
    {
        var go = new GameObject("AnonAction");
        var anonAction = go.AddComponent<AnonAction>();
        anonAction.Initialize(action);
        return anonAction;
    }
}
