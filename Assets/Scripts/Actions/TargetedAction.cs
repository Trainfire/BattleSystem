using UnityEngine;
using System;
using System.Collections.Generic;

public class TargetedAction : BaseAction
{
    public Character Source { get; private set; }
    public Character Reciever { get; private set; }

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

    public void SetSource(Character source)
    {
        Source = source;
        OnSourceSet();
    }

    public void SetReciever(Character reciever)
    {
        Reciever = reciever;
        OnRecieverSet();
    }

    protected virtual void OnSourceSet() { }
    protected virtual void OnRecieverSet() { }

    public static T Create<T>(Character source, Character reciever) where T : TargetedAction
    {
        var targetedAction = new GameObject("TargetedAction").AddComponent<T>();
        targetedAction.SetSource(source);
        targetedAction.SetReciever(reciever);
        return targetedAction;
    }

    public static T Create<T>(Character reciever) where T : TargetedAction
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
                battleSystem.RegisterAction(action);
            }
        }
    }
}
