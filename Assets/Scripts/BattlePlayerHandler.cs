using System;
using UnityEngine;

class BattlePlayerHandler : MonoBehaviour
{
    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.Registry.PlayerAdded += OnPlayerAdded;
    }

    void OnPlayerAdded(Player player)
    {
        player.Health.Changed += OnPlayerHealthChanged;
        // TODO: Status change here.
    }

    void OnPlayerHealthChanged(HealthChangeEvent arg)
    {
        _battleSystem.Registry.RegisterAction(UpdateHealth.Create(arg));
    }
}
