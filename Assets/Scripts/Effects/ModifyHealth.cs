using System;
using UnityEngine;

class ModifyHealth : EffectBase
{
    // TODO: Support percentage changes and absolute values.
    public int Amount;

    protected override void OnApplyEffect(BattleSystem battleSystem, Character character)
    {
        character.Health.ChangeHealth(Source, Amount);
    }
}
