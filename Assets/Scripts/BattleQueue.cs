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

public class BattleQueue : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    private Queue<BaseAction> _playerCommands;
    private Queue<BaseAction> _statusUpdates;
    private Queue<BaseAction> _weather;
    private Queue<BaseAction> _backLog;

    private List<Queue<BaseAction>> _master;
    private Queue<BaseAction> _current;

    public BattleQueue()
    {
        _master = new List<Queue<BaseAction>>();

        _playerCommands = new Queue<BaseAction>();
        _statusUpdates = new Queue<BaseAction>();
        _weather = new Queue<BaseAction>();
        _backLog = new Queue<BaseAction>();

        // Specify order here.
        _master.Add(_playerCommands);
        _master.Add(_statusUpdates);
        _master.Add(_weather);

        Reset();
    }

    public void Reset()
    {
        _current = _master[0];
    }

    public BaseAction Dequeue()
    {
        if (_backLog.Count != 0)
            return _backLog.Dequeue();

        if (_current.Count == 0)
            _current = _master.FirstOrDefault(x => x.Count != 0);

        return _current != null && _current.Count != 0 ? _current.Dequeue() : null;
    }

    public void ClearWeather()
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Weather cleared.");

        _weather.Clear();
    }

    void RegisterAction(BaseAction action, BattleQueueType type)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: {0} of type '{1}'", action.GetType().Name, type.ToString());

        switch (type)
        {
            case BattleQueueType.Normal:
                _backLog.Enqueue(action);
                break;
            case BattleQueueType.PlayerCommand:
                _playerCommands.Enqueue(action);
                LogEx.Log<BattleQueue>("Registered player command '" + action.GetType() + "' targeting '" + (action as TargetedAction).Reciever.name + "'");
                break;
            case BattleQueueType.StatusUpdate:
                _statusUpdates.Enqueue(action);
                break;
            case BattleQueueType.Weather:
                _weather.Clear();
                _weather.Enqueue(action);
                break;
        }
    }

    public void RegisterWeather(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.Weather);
    }

    public void RegisterStatusUpdate(BaseAction action, string name)
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

    public bool Empty { get { return _current.Count == 0 && _master.Count == 0 && _backLog.Count == 0; } }
}
