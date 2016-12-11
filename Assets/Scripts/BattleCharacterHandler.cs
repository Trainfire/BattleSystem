using System;
using UnityEngine;

class BattleCharacterHandler : MonoBehaviour
{
    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.Registry.CharacterAdded += OnPlayerAdded;
        // TODO: Support removal.
    }

    void OnPlayerAdded(Character player)
    {
        player.Health.Changed += OnHealthChanged;
        player.Status.StatusChanged += OnStatusChanged;
        player.Status.ConditionChanged += OnConditionChanged;
    }

    void OnPlayerRemoved(Character player)
    {
        player.Health.Changed -= OnHealthChanged;
        player.Status.StatusChanged -= OnStatusChanged;
        player.Status.ConditionChanged -= OnConditionChanged;
    }

    void OnStatusChanged(StatusChangeEvent arg)
    {
        var str = _battleSystem.Helper.GetStatusAddedMessage(arg.Character, arg.Status);
        _battleSystem.Log(str);
    }

    void OnConditionChanged(ConditionChangeEvent arg)
    {
        string str = "";

        if (arg.Type == AddRemoveType.Added)
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
        _battleSystem.Registry.RegisterAction(UpdateHealth.Create(arg));
    }
}
