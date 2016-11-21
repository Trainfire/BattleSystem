using System;
using UnityEngine;

class ModifyHealth : TargetedAction
{
    public Context Affector;
    public int Amount;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        var health = Affector == Context.Source ? Source.GetComponent<Health>() : Reciever.GetComponent<Health>();
        health.ChangeHealth(Source, Amount);
        TriggerCompletion();
    }
}
