using System;
using UnityEngine;

class ModifyStatus : TargetedAction
{
    public Context Affector;
    public Status Status;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Affector != Context.All)
        {
            var player = Affector == Context.Source ? Source : Reciever;
            battleSystem.Helper.SetPlayerStatus(Status, player);
        }
        else
        {
            battleSystem.Players.ForEach(x => battleSystem.Helper.SetPlayerStatus(Status, x));
        }

        TriggerCompletion();
    }
}
