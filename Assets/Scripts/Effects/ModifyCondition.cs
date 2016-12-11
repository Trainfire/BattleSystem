using System;
using System.Collections.Generic;
using UnityEngine;

class ModifyCondition : EffectBase
{
    public ConditionType Condition;

    protected override void OnApplyEffect(BattleSystem battleSystem, Character character)
    {
        battleSystem.Helper.SetCondition(Condition, character);
    }
}
