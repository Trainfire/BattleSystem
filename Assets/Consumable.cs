using System;
using UnityEngine;

class Consumable : TargetedAction
{
    public int MaxUses;

    private int _uses;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        _uses++;

        TriggerCompletion();

        if (_uses == MaxUses)
        {
            LogEx.Log<Consumable>(name + " was consumed and destroyed.");
            Destroy(gameObject);
        }
    }
}
