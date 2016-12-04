using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class StatusChangeEvent
{
    public Player Player { get; private set; }
    public Status Status { get; private set; }

    public StatusChangeEvent(Player player, Status status)
    {
        Player = player;
        Status = status;
    }
}

public class ConditionChangeEvent
{
    public Player Player { get; private set; }
    public ConditionType Condition { get; private set; }
    public AddRemoveType Type { get; private set; }

    public ConditionChangeEvent(Player player, ConditionType condition, AddRemoveType type)
    {
        Player = player;
        Condition = condition;
        Type = type;
    }
}

public class PlayerStatus : MonoBehaviour
{
    public event Action<StatusChangeEvent> StatusChanged;
    public event Action<ConditionChangeEvent> ConditionChanged;

    private TargetedAction _statusEffect;
    public TargetedAction Effect { get { return _statusEffect; } }
    public Status Current { get; private set; }

    private List<Condition> _conditions;
    private Player _player;

    public void Initialize(Player player)
    {
        _player = player;

        _conditions = new List<Condition>();
    }

    public void SetStatus(Status status, TargetedAction action)
    {
        Current = status;

        if (Current == Status.None && _statusEffect != null)
        {
            Destroy(_statusEffect.gameObject);
        }
        else if (action != null)
        {
            _statusEffect = action;
            _statusEffect.SetReciever(_player);
        }

        if (StatusChanged != null)
            StatusChanged.Invoke(new StatusChangeEvent(_player, status));
    }

    public void AddCondition(Condition condition)
    {
        if (_conditions.Contains(condition))
        {
            // Error here.
        }
        else
        {
            _conditions.Add(condition);

            condition.Removed += RemoveCondition;

            if (ConditionChanged != null)
                ConditionChanged.Invoke(new ConditionChangeEvent(_player, condition.Type, AddRemoveType.Added));
        }
    }

    public void RemoveCondition(Condition condition)
    {
        // TODO: BUG - Condition is somehow removed before being able to check if it exists???
        if (!_conditions.Contains(condition))
        {
            // Error here.
        }
        else
        {
            _conditions.Remove(condition);

            condition.Removed -= RemoveCondition;

            if (ConditionChanged != null)
                ConditionChanged.Invoke(new ConditionChangeEvent(_player, condition.Type, AddRemoveType.Removed));
        }
    }

    public bool HasCondition(ConditionType condition)
    {
        return _conditions.Any(x => x.Type == condition);

        //switch (condition)
        //{
        //    case ConditionType.Paralysis: return GetComponent<ConditionParalysis>();
        //    case ConditionType.Confusion: return GetComponent<ConditionConfusion>();
        //    case ConditionType.Sleep: return GetComponent<ConditionSleep>();
        //}

        //return false;
    }
}
