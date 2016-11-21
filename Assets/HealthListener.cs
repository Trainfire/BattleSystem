using System;
using UnityEngine;

public abstract class HealthListener : PlayerListener
{
    private Health _health;

    protected override void OnSetPlayer()
    {
        base.OnSetPlayer();

        _health = Player.Health;
        _health.Changed += OnHealthChanged;
    }

    protected virtual void OnHealthChanged(HealthChangeEvent healthChange) { }

    protected void Relay(HealthChangeEvent healthChange)
    {
        foreach (var action in GetComponents<TargetedAction>())
        {
            //action.Initialize(BattleSystem);
            action.SetSource(healthChange.Source);
            action.SetReciever(healthChange.Reciever);

            BattleSystem.RegisterAction(action);
        }
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.Changed -= OnHealthChanged;
    }
}
