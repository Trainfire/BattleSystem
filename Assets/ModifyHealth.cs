using System;
using UnityEngine;

class ModifyHealth : TargetedAction
{
    public Context Affector;
    public int Amount;

    protected override void OnExecute()
    {
        var health = Affector == Context.Source ? Source.GetComponent<Health>() : Target.GetComponent<Health>();
        health.ChangeHealth(Source, Amount);
        TriggerCompletion();
    }
}
