using System;
using UnityEngine;

class ModifyStatus : EffectBase
{
    public Status Status;

    protected override void OnApplyEffect(BattleSystem battleSystem, Character character)
    {
        battleSystem.Helper.SetPlayerStatus(Status, character);
    }
}
