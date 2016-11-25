using UnityEngine;
using System;

public class TargetedRelayAction : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        Relay(battleSystem);
    }
}
