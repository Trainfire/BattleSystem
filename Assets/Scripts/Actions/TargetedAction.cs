using UnityEngine;
using System;
using System.Collections.Generic;

public class TargetedAction : BaseAction
{
    public Player Source { get; private set; }
    public Player Reciever { get; private set; }

    private List<TargetedAction> _attachedComponents;

    public override bool IsGarbage
    {
        get
        {
            var lifetime = GetComponent<Lifetime>();
            if (lifetime == null)
            {
                return _attachedComponents.TrueForAll(x => x.Executed);
            }
            else
            {
                // ???
                return lifetime.Expired;
            }
        }
    }

    void Awake()
    {
        _attachedComponents = new List<TargetedAction>();

        foreach (var action in GetComponents<TargetedAction>())
        {
            if (action != this)
                _attachedComponents.Add(action);
        }
    }

    protected override void OnExecute(BattleSystem battleSystem) { }

    public void SetSource(Player source)
    {
        Source = source;
        OnSourceSet();
    }

    public void SetReciever(Player reciever)
    {
        Reciever = reciever;
        OnRecieverSet();
    }

    protected virtual void OnSourceSet() { }
    protected virtual void OnRecieverSet() { }

    public static T Create<T>(Player source, Player reciever) where T : TargetedAction
    {
        var targetedAction = new GameObject("TargetedAction").AddComponent<T>();
        targetedAction.SetSource(source);
        targetedAction.SetReciever(reciever);
        return targetedAction;
    }

    public static T Create<T>(Player reciever) where T : TargetedAction
    {
        var targetedAction = new GameObject("TargetedAction").AddComponent<T>();
        targetedAction.SetReciever(reciever);
        return targetedAction;
    }

    protected void Relay(BattleSystem battleSystem)
    {
        foreach (var action in GetComponents<TargetedAction>())
        {
            if (action != this)
            {
                action.SetSource(Source);
                action.SetReciever(Reciever);
                battleSystem.Registry.RegisterAction(action);
            }
        }
    }
}
