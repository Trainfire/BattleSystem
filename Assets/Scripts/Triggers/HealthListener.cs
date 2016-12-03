using System;
using UnityEngine;

public abstract class HealthListener : PlayerListener
{
    private bool _ignoreSelfDamage = true;

    private Health _health;

    protected override void OnSetPlayer()
    {
        base.OnSetPlayer();

        _health = Player.Health;
        _health.Changed += CheckSource;
    }

    void CheckSource(HealthChangeEvent healthChange)
    {
        if (_ignoreSelfDamage && healthChange.Source == healthChange.Reciever)
            return;

        OnHealthChanged(healthChange);
    }

    protected virtual void OnHealthChanged(HealthChangeEvent healthChange) { }

    protected void Relay(HealthChangeEvent healthChange)
    {
        foreach (var action in GetComponents<TargetedAction>())
        {
            //action.Initialize(BattleSystem);
            action.SetSource(healthChange.Source);
            action.SetReciever(healthChange.Reciever);

            BattleSystem.Registry.RegisterAction(action);
        }
    }

    void OnDestroy()
    {
        if (_health != null)
            _health.Changed -= CheckSource;
    }
}
