using System;
using UnityEngine;

class ModifyHealth : TargetedAction
{
    public Context Affector;
    public int Amount;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Affector != Context.All)
        {
            var health = Affector == Context.Source ? Source.GetComponent<Health>() : Reciever.GetComponent<Health>();
            health.ChangeHealth(Source, Amount);
        }
        else
        {
            battleSystem.Players.ForEach(x => x.Health.ChangeHealth(Source, Amount));
        }

        TriggerCompletion();
    }
}
