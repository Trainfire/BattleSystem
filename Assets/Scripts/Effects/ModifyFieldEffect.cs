using UnityEngine;
using System.Collections;
using System;

class ModifyFieldEffect : EffectBase
{
    [SerializeField] private AddRemoveType _actionType;
    [SerializeField] private FieldEffectType _fieldEffectType;

    protected override void OnApplyEffect(BattleSystem battleSystem, Character character)
    {
        switch (_actionType)
        {
            case AddRemoveType.Add: character.Slot.FieldSide.AddEffect(_fieldEffectType); break;
            case AddRemoveType.Remove: character.Slot.FieldSide.RemoveEffect(_fieldEffectType); break;
            case AddRemoveType.RemoveAll: character.Slot.FieldSide.RemoveAllEffects(); break;
        }
    }
}
