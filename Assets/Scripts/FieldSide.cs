using UnityEngine;
using System.Collections.Generic;
using System;

public class FieldSide : MonoBehaviour
{
    public int ID { get; private set; }
    public int MaxSlots { get; private set; }
    public List<FieldSlot> Slots { get; private set; }

    private BattleSystem _battleSystem;

    private Dictionary<FieldEffectType, FieldEffect> _fieldEffects;

    public void Initialize(BattleSystem battleSystem, int id, int maxSlots)
    {
        _battleSystem = battleSystem;

        ID = id;
        MaxSlots = maxSlots;

        battleSystem.PostCharacterAddedToSlot += OnCharacterAdded;

        _fieldEffects = new Dictionary<FieldEffectType, FieldEffect>();

        _fieldEffects.Add(FieldEffectType.Spikes, new FieldEffectSpikes(battleSystem.Helper.SpikesParameters));
        // TODO: Add more effects here.

        Slots = new List<FieldSlot>();
        for (int i = 0; i < maxSlots; i++)
        {
            var slot = gameObject.AddComponent<FieldSlot>();
            slot.Initialize(this);
            Slots.Add(slot);
        }
    }

    void OnCharacterAdded(Character character)
    {
        if (character.Slot.FieldSide != this)
            return;

        bool logShown = false;

        foreach (var fieldEffect in _fieldEffects)
        {
            if (!logShown)
            {
                LogEx.Log<FieldSide>("Apply effect to {0}", character.name);
                logShown = true;
            }

            if (fieldEffect.Value.Active)
            {
                _battleSystem.Log(fieldEffect.Value.Message);
                fieldEffect.Value.ApplyEffect(character);
            }
        }
    }

    public bool Register(Character character)
    {
        // TODO: Assign character to first non-empty slot. Return false if no slots available.
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].Character == null)
            {
                Slots[i].Assign(character);
                return true;
            }
        }

        return false;
    }

    public void AddEffect(FieldEffectType fieldEffectType)
    {
        if (_fieldEffects.ContainsKey(fieldEffectType))
        {
            var fieldEffect = _fieldEffects[fieldEffectType];

            if (fieldEffect.CanApply)
            {
                LogEx.Log<FieldSide>("Added effect: " + fieldEffectType);
                _fieldEffects[fieldEffectType].IncreaseEffect();
            }
            else
            {
                LogEx.Log<FieldSide>("Field effect of type '{0}' cannot be applied.", fieldEffectType);
            }
        }
    }

    public void RemoveEffect(FieldEffectType fieldEffectType)
    {
        // TODO.
        LogEx.Log<FieldSide>("Removed effect: " + fieldEffectType);

        _fieldEffects[fieldEffectType].Reset();
    }

    public void RemoveAllEffects()
    {
        // TODO.
        LogEx.Log<FieldSide>("Removed all effects.");

        foreach (var fieldEffect in _fieldEffects)
        {
            fieldEffect.Value.Reset();
        }
    }
}