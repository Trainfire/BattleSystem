using UnityEngine;
using System;

public class RelayAction : BaseAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        foreach (var action in GetComponents<BaseAction>())
        {
            if (action != this)
                battleSystem.Registry.RegisterAction(action);
        }
    }
}
