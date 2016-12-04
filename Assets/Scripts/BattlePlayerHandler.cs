using System;
using UnityEngine;

class BattlePlayerHandler : MonoBehaviour
{
    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.Registry.PlayerAdded += OnPlayerAdded;
        // TODO: Support removal.
    }

    void OnPlayerAdded(Player player)
    {
        player.Health.Changed += OnHealthChanged;
        player.Status.StatusChanged += OnStatusChanged;
        player.Status.ConditionChanged += OnConditionChanged;
    }

    void OnPlayerRemoved(Player player)
    {
        player.Health.Changed -= OnHealthChanged;
        player.Status.StatusChanged -= OnStatusChanged;
        player.Status.ConditionChanged -= OnConditionChanged;
    }

    void OnStatusChanged(StatusChangeEvent arg)
    {
        var str = _battleSystem.Helper.GetStatusAddedMessage(arg.Player, arg.Status);
        _battleSystem.Log(str);
    }

    void OnConditionChanged(ConditionChangeEvent arg)
    {
        string str = "";

        if (arg.Type == AddRemoveType.Added)
        {
            str = _battleSystem.Helper.GetConditionAddedMessage(arg.Player, arg.Condition);
        }
        else
        {
            str = _battleSystem.Helper.GetConditionRemovedMessage(arg.Player, arg.Condition);
        }
         
        _battleSystem.Log(str);
    }

    void OnHealthChanged(HealthChangeEvent arg)
    {
        _battleSystem.Registry.RegisterAction(UpdateHealth.Create(arg));
    }
}
