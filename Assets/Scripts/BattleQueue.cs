using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

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
    public bool ExecutePostTurn { get; private set; }
    public Queue<BaseAction> Queue { get; private set; }

    /// <summary>
    /// Called when the queue becomes active.
    /// </summary>
    public Action<BattleSystem> OnActivation { get; set; }

    public BattleQueueWrapper(BattleQueueType type, bool executePostTurn = true)
    {
        Type = type;
        Queue = new Queue<BaseAction>();
        ExecutePostTurn = executePostTurn;
    }
}

public class BattleQueue : MonoBehaviour
{
    public event Action<BattleQueue> Empty;

    [SerializeField] private bool _logRegistrations;

    private BattleQueueWrapper _playerCommands;
    private BattleQueueWrapper _statusUpdates;
    private BattleQueueWrapper _weatherUpdates;
    private BattleQueueWrapper _backLog;

    private Queue<BattleQueueWrapper> _queues;

    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.ActionRegistered += OnActionRegistered;

        _queues = new Queue<BattleQueueWrapper>();

        _playerCommands = new BattleQueueWrapper(BattleQueueType.PlayerCommand);
        _statusUpdates = new BattleQueueWrapper(BattleQueueType.StatusUpdate, false);
        _weatherUpdates = new BattleQueueWrapper(BattleQueueType.Weather, false);
        _backLog = new BattleQueueWrapper(BattleQueueType.Normal);

        _statusUpdates.OnActivation = OnEnterStatusUpdate;
        _weatherUpdates.OnActivation = OnEnterWeather;

        Reset();
    }

    void OnActionRegistered(BaseAction action, BattleQueueType type)
    {
        switch (type)
        {
            case BattleQueueType.Normal:
                _backLog.Queue.Enqueue(action);
                break;
            case BattleQueueType.PlayerCommand:
                _playerCommands.Queue.Enqueue(action);
                //LogEx.Log<BattleQueue>("Registered player command '" + action.GetType() + "' targeting '" + (action as TargetedAction).Reciever.name + "'");
                break;
            case BattleQueueType.StatusUpdate:
                _statusUpdates.Queue.Enqueue(action);
                break;
            case BattleQueueType.Weather:
                _weatherUpdates.Queue.Clear();
                _weatherUpdates.Queue.Enqueue(action);
                break;
        }
    }

    public void Reset()
    {
        // REMOVE.
        GenerateQueue(ExecutionType.Normal);
    }

    void GenerateQueue(ExecutionType executionType)
    {
        _queues.Clear();

        if (executionType == ExecutionType.Normal)
        {
            // Specify order here.
            _queues.Enqueue(_playerCommands);
            _queues.Enqueue(_statusUpdates);
            _queues.Enqueue(_weatherUpdates);
        }
        else
        {
            _queues.Enqueue(_playerCommands);
        }
    }

    BaseAction Dequeue(ExecutionType executionType)
    {
        GenerateQueue(executionType);

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

    public void Execute(ExecutionType executionType)
    {
        var next = Dequeue(executionType);

        if (next != null)
        {
            next.Completed += OnActionExecutionComplete;
            next.Execute(_battleSystem);
        }
        else
        {
            LogEx.Log<BattleSystem>("No more battle actions left to execute.");

            // TODO: Move Turn Count somewhere else.
            //TurnCount++;

            Reset();

            Empty.InvokeSafe(this);
        }
    }

    void OnActionExecutionComplete(BaseAction action)
    {
        action.Completed -= OnActionExecutionComplete;

        if (action.IsGarbage)
            Destroy(action.gameObject);
    }

    #region State Callbacks
    void OnEnterStatusUpdate(BattleSystem battleSystem)
    {
        // Find existing status effects on each player and register each one.
        battleSystem.ActiveCharacters.ForEach(x =>
        {
            if (x.Status.Effect != null)
            {
                Debug.LogFormat("Registering status update for '{0}'", x.name);
                RegisterStatusUpdate(x.Status.Effect);
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
                _weatherUpdates.Queue.Clear();
                _weatherUpdates.Queue.Enqueue(action);
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

    public void RegisterPlayerCommand(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }
}
