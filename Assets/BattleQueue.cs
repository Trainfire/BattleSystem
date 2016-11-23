using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleQueue : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    public Queue<BaseAction> PlayerCommands { get; private set; }
    public Queue<BaseAction> BackLog { get; private set; }

    private Queue<Queue<BaseAction>> _master;
    private Queue<BaseAction> _current;

    public BattleQueue()
    {
        _master = new Queue<Queue<BaseAction>>();

        PlayerCommands = new Queue<BaseAction>();
        BackLog = new Queue<BaseAction>();

        Reset();
    }

    public void Reset()
    {
        _master.Clear();

        // Specify order here.
        _master.Enqueue(PlayerCommands);
        // TODO: Status Updates
        // TODO: Weather Effect Update

        _current = _master.Dequeue();
    }

    public BaseAction Dequeue()
    {
        if (BackLog.Count != 0)
            return BackLog.Dequeue();

        if (_current.Count == 0)
        {
            if (_master.Count != 0)
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

    public void RegisterAction(Action action, string name)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: " + name);

        BackLog.Enqueue(AnonAction.Create(action, name));
    }

    public void RegisterAction(BaseAction action)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: " + action.GetType().Name);

        BackLog.Enqueue(action);
    }

    public void RegisterPlayerCommand(TargetedAction action)
    {
        LogEx.Log<BattleQueue>("Registered player command '" + action.GetType() + "' targeting '" + action.Reciever.name + "'");
        PlayerCommands.Enqueue(action);
    }

    public bool Empty { get { return _current.Count == 0 && _master.Count == 0 && BackLog.Count == 0; } }
}
