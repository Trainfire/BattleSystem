using UnityEngine;
using System;

public class AnonAction : BaseAction
{
    private Action _action;

    public void Initialize(Action action)
    {
        _action = action;
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        _action.Invoke();
        _action = null;

        TriggerCompletion();
    }

    public static AnonAction Create(Action action, string name = "AnonAction")
    {
        var anonAction = new GameObject(name).AddComponent<AnonAction>();
        anonAction.Initialize(action);
        return anonAction;
    }
}
