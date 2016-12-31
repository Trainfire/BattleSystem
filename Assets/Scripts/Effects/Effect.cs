using System;
using UnityEngine;
using System.Collections.Generic;

abstract class EffectBase : TargetedAction
{
    public Reciever Affector;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        var affectors = new List<Character>();

        if (Affector != global::Reciever.All)
        {
            if (Affector == global::Reciever.Source)
            {
                affectors.Add(Source);
            }
            else
            {
                affectors.Add(Reciever);
            }
        }
        else
        {
            affectors = battleSystem.ActiveCharacters;
        }

        affectors.ForEach(x => OnApplyEffect(battleSystem, x));

        TriggerCompletion();
    }

    protected abstract void OnApplyEffect(BattleSystem battleSystem, Character character);
}
