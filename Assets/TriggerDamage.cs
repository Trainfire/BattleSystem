using System;
using UnityEngine;

class TriggerDamage : HealthListener
{
    protected override void OnHealthChanged(HealthChangeEvent healthChange)
    {
        base.OnHealthChanged(healthChange);
        Relay(healthChange);
    }
}
