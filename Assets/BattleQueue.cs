using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleQueue : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    private Queue<BaseAction> _playerCommands;
    private Queue<BaseAction> _weather;
    private Queue<BaseAction> _backLog;

    private Queue<Queue<BaseAction>> _master;
    private Queue<BaseAction> _current;

    public BattleQueue()
    {
        _master = new Queue<Queue<BaseAction>>();

        _playerCommands = new Queue<BaseAction>();
        _weather = new Queue<BaseAction>();
        _backLog = new Queue<BaseAction>();

        Reset();
    }

    public void Reset()
    {
        _master.Clear();

        // Specify order here.
        _master.Enqueue(_playerCommands);
        // TODO: Status Updates
        _master.Enqueue(_weather);

        _current = _master.Dequeue();
    }

    public BaseAction Dequeue()
    {
        if (_backLog.Count != 0)
            return _backLog.Dequeue();

        if (_current.Count == 0)
        {
            if (_master.Count != 0 && _current.Count != 0)
            {
                _current = _master.Dequeue();
            }
            else
            {
                return null;
            }
        }

        return _current.Dequeue();
    }

    public void ClearWeather()
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Weather cleared.");

        _weather.Clear();
    }

    public void RegisterWeather(BaseAction action)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered weather: " + action.name);

        _weather.Clear();
        _weather.Enqueue(action);
    }

    public void RegisterAction(Action action, string name)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: " + name);

        _backLog.Enqueue(AnonAction.Create(action, name));
    }

    public void RegisterAction(BaseAction action)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: " + action.GetType().Name);

        _backLog.Enqueue(action);
    }

    public void RegisterPlayerCommand(TargetedAction action)
    {
        LogEx.Log<BattleQueue>("Registered player command '" + action.GetType() + "' targeting '" + action.Reciever.name + "'");
        _playerCommands.Enqueue(action);
    }

    public bool Empty { get { return _current.Count == 0 && _master.Count == 0 && _backLog.Count == 0; } }
}
