using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum BattleQueueType
{
    Normal,
    PlayerCommand,
    StatusUpdate,
    Weather,
}

public class BattleQueueWrapper
{
    public BattleQueueType Type { get; private set; }
    public Queue<BaseAction> Queue { get; private set; }

    /// <summary>
    /// Called when the queue becomes active.
    /// </summary>
    public Action<BattleSystem> OnActivation { get; set; }

    public BattleQueueWrapper(BattleQueueType type)
    {
        Type = type;
        Queue = new Queue<BaseAction>();
    }
}

public class BattleQueue : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    private BattleQueueWrapper _playerCommands;
    private BattleQueueWrapper _statusUpdates;
    private BattleQueueWrapper _weather;
    private BattleQueueWrapper _backLog;

    private Queue<BattleQueueWrapper> _queues;

    private int _index;

    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;

        _queues = new Queue<BattleQueueWrapper>();

        _playerCommands = new BattleQueueWrapper(BattleQueueType.PlayerCommand);
        _statusUpdates = new BattleQueueWrapper(BattleQueueType.StatusUpdate);
        _weather = new BattleQueueWrapper(BattleQueueType.Weather);
        _backLog = new BattleQueueWrapper(BattleQueueType.Normal);

        _statusUpdates.OnActivation = OnEnterStatusUpdate;
        _weather.OnActivation = OnEnterWeather;

        Reset();
    }

    public void Reset()
    {
        _queues.Clear();

        // Specify order here.
        _queues.Enqueue(_playerCommands);
        _queues.Enqueue(_statusUpdates);
        _queues.Enqueue(_weather);
    }

    public BaseAction Dequeue()
    {
        if (_backLog.Queue.Count != 0)
            return _backLog.Queue.Dequeue();

        if (_queues.Count == 0)
        {
            return null;
        }
        else
        {
            if (_queues.Peek().Queue.Count != 0)
            {
                return _queues.Peek().Queue.Dequeue();
            }
            else
            {
                // Find the next non-empty queue.
                BattleQueueWrapper next = null;

                while (next == null || next.Queue.Count == 0 && _queues.Count != 0)
                {
                    next = _queues.Dequeue();

                    // Activate the next queue.
                    if (next.OnActivation != null)
                        next.OnActivation(_battleSystem);
                }

                if (next != null)
                    LogEx.Log<BattleQueue>("Moving to next non-empty queue: " + next.Type);

                return next != null && next.Queue.Count != 0 ? next.Queue.Dequeue() : null;
            }
        }
    }

    public void ClearWeather()
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Weather cleared.");

        _weather.Queue.Clear();
    }

    #region State Callbacks
    void OnEnterStatusUpdate(BattleSystem battleSystem)
    {
        // Find existing status effects on each player and register each one.
        battleSystem.Players.ForEach(x =>
        {
            if (x.StatusEffect != null)
            {
                Debug.LogFormat("Registering status update for '{0}'", x.name);
                RegisterStatusUpdate(x.StatusEffect);
            }
        });
    }

    void OnEnterWeather(BattleSystem battleSystem)
    {
        if (battleSystem.Weather.Current != null)
        {
            LogEx.Log<BattleQueue>("Found a weather effect.");
            RegisterWeather(battleSystem.Weather.Current);
        }
    }
    #endregion

    void RegisterAction(BaseAction action, BattleQueueType type)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: {0} of type '{1}'", action.GetType().Name, type.ToString());

        switch (type)
        {
            case BattleQueueType.Normal:
                _backLog.Queue.Enqueue(action);
                break;
            case BattleQueueType.PlayerCommand:
                _playerCommands.Queue.Enqueue(action);
                LogEx.Log<BattleQueue>("Registered player command '" + action.GetType() + "' targeting '" + (action as TargetedAction).Reciever.name + "'");
                break;
            case BattleQueueType.StatusUpdate:
                _statusUpdates.Queue.Enqueue(action);
                break;
            case BattleQueueType.Weather:
                _weather.Queue.Clear();
                _weather.Queue.Enqueue(action);
                break;
        }
    }

    public void RegisterWeather(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.Weather);
    }

    public void RegisterStatusUpdate(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.StatusUpdate);
    }

    public void RegisterAction(Action action, string name)
    {
        RegisterAction(AnonAction.Create(action, name), BattleQueueType.Normal);
    }

    public void RegisterAction(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.Normal);
    }

    public void RegisterPlayerCommand(TargetedAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }
}
