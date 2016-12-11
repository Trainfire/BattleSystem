using System;
using UnityEngine;

class ModifyCondition : TargetedAction
{
    public Reciever Affector;
    public ConditionType Condition;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Affector != global::Reciever.All)
        {
            var player = Affector == global::Reciever.Source ? Source : Reciever;
            battleSystem.Helper.SetCondition(Condition, player);
        }
        else
        {
            battleSystem.Registry.ActiveCharacters.ForEach(x => battleSystem.Helper.SetCondition(Condition, x));
        }

        TriggerCompletion();
    }
}
