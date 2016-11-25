using System;
using UnityEngine;

class ModifyHealth : TargetedAction
{
    public Reciever Affector;
    public int Amount;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Affector != global::Reciever.All)
        {
            var health = Affector == global::Reciever.Source ? Source.GetComponent<Health>() : Reciever.GetComponent<Health>();
            health.ChangeHealth(Source, Amount);
        }
        else
        {
            battleSystem.Players.ForEach(x => x.Health.ChangeHealth(Source, Amount));
        }

        TriggerCompletion();
    }
}
