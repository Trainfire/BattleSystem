using System;
using UnityEngine;

class TriggerDamage : TargetedAction
{
    protected override void OnInitialize()
    {
        base.OnInitialize();
        Source.GetComponent<Health>().Changed += OnHealthChanged;
    }

    void OnHealthChanged(HealthChangeEvent obj)
    {
        TriggerComponents();
    }

    void OnDestroy()
    {
        Source.GetComponent<Health>().Changed -= OnHealthChanged;
    }
}
