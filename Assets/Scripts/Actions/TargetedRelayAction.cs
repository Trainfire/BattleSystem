using UnityEngine;
using System;
using System.Collections.Generic;

public class TargetedRelayAction : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        Relay(battleSystem);
    }
}
