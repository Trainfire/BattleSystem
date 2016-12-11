using System;
using UnityEngine;

class ModifyStatus : TargetedAction
{
    public Reciever Affector;
    public Status Status;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Affector != global::Reciever.All)
        {
            var player = Affector == global::Reciever.Source ? Source : Reciever;
            battleSystem.Helper.SetPlayerStatus(Status, player);
        }
        else
        {
            battleSystem.Registry.ActiveCharacters.ForEach(x => battleSystem.Helper.SetPlayerStatus(Status, x));
        }

        TriggerCompletion();
    }
}
