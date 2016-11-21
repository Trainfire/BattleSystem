using System;
using UnityEngine;

class TriggerHealth : HealthListener
{
    public Equality Equality;
    public float Threshold;

    protected override void OnHealthChanged(HealthChangeEvent healthChange)
    {
        base.OnHealthChanged(healthChange);

        if (!EqualityHelper.IsEqual(Equality, healthChange.NewValue, Threshold))
            return;

        Relay(healthChange);
    }
}
