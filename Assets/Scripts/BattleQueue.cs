using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework;

public enum BattleQueueType
{
    GenericUpdate,
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
    public Action<BattleSystem> OnActivation { get; private set; }

    public BattleQueueWrapper(BattleQueueType type, Action<BattleSystem> onActivation, bool executePostTurn = true)
    {
        Type = type;
        OnActivation = onActivation;
        Queue = new Queue<BaseAction>();
        ExecutePostTurn = executePostTurn;
    }
}

public class BattleQueue : MonoBehaviour
{
    public event Action<BattleQueue> Emptied;

    public bool Empty
    {
        get
        {
            return _linearQueues.ToList().TrueForAll(x => x.Queue.Count == 0)
                && _genericUpdates.Queue.Count == 0;
        }
    }

    [SerializeField] private bool _logRegistrations;

    public List<BaseAction> GenericUpdates { get { return _genericUpdates.Queue.ToList(); } }
    public List<BaseAction> PlayerCommands { get { return _playerCommands.Queue.ToList(); } }
    public List<BaseAction> StatusUpdates { get { return _statusUpdates.Queue.ToList(); } }
    public List<BaseAction> WeatherUpdates { get { return _weatherUpdates.Queue.ToList(); } }

    private BattleQueueWrapper _genericUpdates;
    private BattleQueueWrapper _playerCommands;
    private BattleQueueWrapper _statusUpdates;
    private BattleQueueWrapper _weatherUpdates;

    private Queue<BattleQueueWrapper> _linearQueues;

    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        _battleSystem.ActionRegistered += OnActionRegistered;

        _linearQueues = new Queue<BattleQueueWrapper>();

        // This order maps to the order in which each queue is evaluated.
        // Generic updates are always checked first, then player commands, etc...
        _genericUpdates = new BattleQueueWrapper(BattleQueueType.GenericUpdate, null);

        _playerCommands = new BattleQueueWrapper(BattleQueueType.PlayerCommand, null);
        _statusUpdates = new BattleQueueWrapper(BattleQueueType.StatusUpdate, OnEnterStatusUpdate, false);
        _weatherUpdates = new BattleQueueWrapper(BattleQueueType.Weather, OnEnterWeather, false);
        
        Reset();
    }

    void OnActionRegistered(BaseAction action, BattleQueueType type)
    {
        switch (type)
        {
            case BattleQueueType.GenericUpdate:
                _genericUpdates.Queue.Enqueue(action);
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
        _linearQueues.Clear();

        if (executionType == ExecutionType.Normal)
        {
            // Specify order here.
            _linearQueues.Enqueue(_playerCommands);
            _linearQueues.Enqueue(_statusUpdates);
            _linearQueues.Enqueue(_weatherUpdates);
        }
        else
        {
            _linearQueues.Enqueue(_playerCommands);
        }
    }

    BaseAction Dequeue(ExecutionType executionType)
    {
        GenerateQueue(executionType);

        if (_genericUpdates.Queue.Count != 0)
            return _genericUpdates.Queue.Dequeue();

        if (_linearQueues.Count == 0)
        {
            return null;
        }
        else
        {
            if (_linearQueues.Peek().Queue.Count != 0)
            {
                return _linearQueues.Peek().Queue.Dequeue();
            }
            else
            {
                // Find the next non-empty queue.
                BattleQueueWrapper next = null;

                while (next == null || next.Queue.Count == 0 && _linearQueues.Count != 0)
                {
                    next = _linearQueues.Dequeue();

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

            Emptied.InvokeSafe(this);
        }
    }

    void OnActionExecutionComplete(BaseAction action)
    {
        action.Completed -= OnActionExecutionComplete;

        if (action.FlaggedForRemoval)
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
            case BattleQueueType.GenericUpdate:
                _genericUpdates.Queue.Enqueue(action);
                break;
            case BattleQueueType.PlayerCommand:
                _playerCommands.Queue.Enqueue(action);
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
        RegisterAction(AnonAction.Create(action, name), BattleQueueType.GenericUpdate);
    }

    public void RegisterAction(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.GenericUpdate);
    }

    public void RegisterPlayerCommand(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }
}
