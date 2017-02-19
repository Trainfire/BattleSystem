using System;
using System.Linq;
using UnityEngine;

class BattleCharacterHandler : MonoBehaviour
{
    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.PlayerAdded += OnPlayerAdded;
        _battleSystem.CharacterAddedToSlot += OnCharacterAdded;
        _battleSystem.CharacterRemovedFromSlot += OnCharacterRemoved;
    }

    void OnPlayerAdded(Player player)
    {
        player.AttackSelected += OnPlayerAttackSelected;
        player.SwitchedCharacter += OnPlayerSwitchedCharacter;
    }

    void OnPlayerAttackSelected(AttackCommand attack)
    {
        _battleSystem.RegisterPlayerCommand(attack);
    }

    void OnPlayerSwitchedCharacter(SwitchCommand arg)
    {
        Debug.LogFormat("{0} wants to switch to {1}.", arg.Player.name, arg.SwitchTarget.name);
        _battleSystem.RegisterPlayerCommand(arg);
    }

    void OnCharacterAdded(Character character)
    {
        character.Health.Changed += OnHealthChanged;
        character.Status.StatusChanged += OnStatusChanged;
        character.Status.ConditionChanged += OnConditionChanged;
    }

    void OnCharacterRemoved(Character character)
    {
        character.Health.Changed -= OnHealthChanged;
        character.Status.StatusChanged -= OnStatusChanged;
        character.Status.ConditionChanged -= OnConditionChanged;
    }

    void OnStatusChanged(StatusChangeEvent arg)
    {
        var str = _battleSystem.Helper.GetStatusAddedMessage(arg.Character, arg.Status);
        _battleSystem.Log(str);
    }

    void OnConditionChanged(ConditionChangeEvent arg)
    {
        string str = "";

        if (arg.Type == AddRemoveType.Add)
        {
            str = _battleSystem.Helper.GetConditionAddedMessage(arg.Character, arg.Condition);
        }
        else
        {
            str = _battleSystem.Helper.GetConditionRemovedMessage(arg.Character, arg.Condition);
        }
         
        _battleSystem.Log(str);
    }

    void OnHealthChanged(HealthChangeEvent arg)
    {
        _battleSystem.RegisterAction(UpdateHealth.Create(arg));
    }
}
