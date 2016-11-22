using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] private bool _autoReadyPlayers;
    [SerializeField] private bool _enableManualStepping;
    [SerializeField] private bool _logRegistrations;

    public event Action<BattleSystem> CommandsDepleted;

    public List<Player> Players { get; private set; }
    public int TurnCount { get; private set; }
    public bool AutoReadyPlayers { get { return _autoReadyPlayers; } }
    public bool EnableManualStepping { get { return _enableManualStepping; } set { _enableManualStepping = true; } }

    private Queue<TargetedAction> _playerCommands;
    private Queue<BaseAction> _actions;

    void Awake()
    {
        Players = new List<Player>();

        _playerCommands = new Queue<TargetedAction>();
        _actions = new Queue<BaseAction>();
    }

    public void RegisterPlayer(Player player)
    {
        LogEx.Log<BattleSystem>("Registered player: '{0}'", player.name);

        Players.Add(player);

        player.GetComponent<Health>().Changed += OnPlayerHealthChanged;

        player.BattleEntities.ForEach(x => x.Initialize(this));
    }

    public void RegisterAction(Action action, string name)
    {
        if (_logRegistrations)
            LogEx.Log<BattleSystem>("Registered action: " + name);

        _actions.Enqueue(AnonAction.Create(action, name));
    }

    public void RegisterAction(BaseAction action)
    {
        if (_logRegistrations)
            LogEx.Log<BattleSystem>("Registered action: " + action.GetType().Name);

        _actions.Enqueue(action);
    }

    public void RegisterPlayerCommand(TargetedAction action)
    {
        LogEx.Log<BattleSystem>("Registered player command '" + action.GetType() + "' targeting '" + action.Reciever.name + "'");
        _playerCommands.Enqueue(action);
    }

    public void Log(string message)
    {
        RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message), "BattleLog");
    }

    public void Execute()
    {
        if (_playerCommands.Count != 0)
        {
            LogEx.Log<BattleSystem>("Executing next player's command...");

            // Execute next player command.
            var next = _playerCommands.Dequeue();
            next.Completed += OnActionExecutionComplete;
            next.Execute(this);
        }
        else
        {
            LogEx.Log<BattleSystem>("No more player commands left to execute.");

            TurnCount++;

            if (CommandsDepleted != null)
                CommandsDepleted.Invoke(this);
        }
    }

    public void Continue()
    {
        if (_actions.Count != 0)
        {
            var next = _actions.Dequeue();
            next.Completed += OnActionExecutionComplete;
            next.Execute(this);
        }
        else
        {
            Execute();
        }
    }

    void OnActionExecutionComplete(BaseAction action)
    {
        action.Completed -= OnActionExecutionComplete;

        if (!_enableManualStepping)
            Continue();
    }

    void OnPlayerHealthChanged(HealthChangeEvent obj)
    {
        RegisterAction(UpdateHealth.Create(obj));
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) && _enableManualStepping)
            Continue();
    }
}
