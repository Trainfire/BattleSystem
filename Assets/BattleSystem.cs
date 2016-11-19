using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleSystem
{
    public event Action<BattleSystem> CommandsDepleted;

    public List<Player> Players { get; private set; }
    public int TurnCount { get; private set; }

    private Queue<PlayerCommand> _playerCommands;
    private Queue<AnonAction> _actions;

    public BattleSystem()
    {
        Players = new List<Player>();

        _playerCommands = new Queue<PlayerCommand>();
        _actions = new Queue<AnonAction>();
    }

    public void RegisterPlayer(Player player)
    {
        LogEx.Log<BattleSystem>("Registered player: '{0}'", player.name);
        Players.Add(player);
    }

    public void RegisterAction(Action action)
    {
        _actions.Enqueue(AnonAction.Create(action));
    }

    public void RegisterPlayerCommand(PlayerCommand action)
    {
        LogEx.Log<BattleSystem>("Registered player command: " + action.GetType());
        _playerCommands.Enqueue(action);
    }

    public void Log(string message)
    {
        RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message));
    }

    public void Execute()
    {
        if (_playerCommands.Count != 0)
        {
            // Execute next player command.
            var next = _playerCommands.Dequeue();
            next.Completed += OnActionExecutionComplete;
            next.Execute();
        }
        else
        {
            LogEx.Log<BattleSystem>("No more player commands left to execute.");

            TurnCount++;

            if (CommandsDepleted != null)
                CommandsDepleted.Invoke(this);
        }
    }

    void UpdateState()
    {
        if (_actions.Count != 0)
        {
            var next = _actions.Dequeue();
            next.Completed += OnActionExecutionComplete;
            next.Execute();
        }
        else
        {
            Execute();
        }
    }

    void OnActionExecutionComplete(BaseAction action)
    {
        action.Completed -= OnActionExecutionComplete;
        UpdateState();
    }
}
