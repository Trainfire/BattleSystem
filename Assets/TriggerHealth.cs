using System;
using UnityEngine;

class TriggerHealth : TargetedAction
{
    public Equality Equality;
    public float Threshold;

    protected override void OnInitialize()
    {
        base.OnInitialize();
        Source.GetComponent<Health>().Changed += OnHealthChanged;
    }

    void OnHealthChanged(HealthChangeEvent obj)
    {
        if (!EqualityHelper.IsEqual(Equality, obj.NewValue, Threshold))
            return;

        foreach (var action in GetComponents<TargetedAction>())
        {
            if (action != this)
            {
                action.Initialize(BattleSystem, Source, Target);
                BattleSystem.RegisterAction(action);
            }
        }
    }

    void OnDestroy()
    {
        if (Source != null)
            Source.GetComponent<Health>().Changed -= OnHealthChanged;
    }
}
