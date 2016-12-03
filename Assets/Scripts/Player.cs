using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public event Action<Player> ReadyStateChanged;
    public bool IsReady { get; private set; }

    public TargetedAction Attack;
    public GameObject HeldItem;
    public GameObject Ability;

    private Health _health;
    public Health Health { get { return _health; } }

    private TargetedAction _statusEffect;
    public TargetedAction StatusEffect { get { return _statusEffect; } }
    public Status Status { get; private set; }

    private List<PlayerListener> _battleEntities;
    public List<PlayerListener> BattleEntities { get { return _battleEntities; } }

    void Awake()
    {
        _health = gameObject.AddComponent<Health>();

        _battleEntities = new List<PlayerListener>();
        RegisterListener(HeldItem);
        RegisterListener(Ability);
    }

    void RegisterListener(GameObject prototype)
    {
        if (prototype == null)
            return;

        var instance = GameObject.Instantiate(prototype);

        var battleEntity = instance.GetComponent<PlayerListener>();
        if (battleEntity == null)
        {
            Debug.LogWarningFormat("Tried to register object '{0}' but no Event Listener is attached.", prototype.name);
        }
        else
        {
            battleEntity.SetPlayer(this);
            _battleEntities.Add(battleEntity);
        }
    }

    public void SetStatus(Status status, TargetedAction action)
    {
        Status = status;

        // TODO: Prevent status effects from being stacked.
        if (Status == Status.None && _statusEffect != null)
        {
            Destroy(_statusEffect.gameObject);
        }
        else if (action != null)
        {
            _statusEffect = action;
            _statusEffect.SetReciever(this);
        }
    }

    public bool HasCondition(ConditionType condition)
    {
        switch (condition)
        {
            case ConditionType.Paralysis: return GetComponent<ConditionParalysis>();
            case ConditionType.Confusion: return GetComponent<ConditionConfusion>();
            case ConditionType.Sleep: return GetComponent<ConditionSleep>();
        }

        return false;
    }

    public void ToggleReady()
    {
        IsReady = !IsReady;

        if (ReadyStateChanged != null)
            ReadyStateChanged.Invoke(this);
    }

    public void ResetReady()
    {
        IsReady = false;
    }
}
