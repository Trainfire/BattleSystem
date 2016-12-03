using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BattleQueue), typeof(BattleWeather))]
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private bool _autoReadyPlayers;
    [SerializeField] private bool _enableManualStepping;

    public event Action<BattleSystem> CommandsDepleted;

    public BattleHelper Helper { get; private set; }
    public BattleQueue Queue { get; private set; }
    public BattleWeather Weather { get; private set; }
    public List<Player> Players { get; private set; }
    public int TurnCount { get; private set; }

    public bool AutoReadyPlayers { get { return _autoReadyPlayers; } }
    public bool EnableManualStepping { get { return _enableManualStepping; } set { _enableManualStepping = true; } }

    void Awake()
    {
        Players = new List<Player>();

        Queue = gameObject.GetComponent<BattleQueue>();
        Queue.Initialize(this);

        Helper = gameObject.GetComponent<BattleHelper>();

        Weather = gameObject.GetComponent<BattleWeather>();
    }

    public void RegisterPlayer(Player player)
    {
        LogEx.Log<BattleSystem>("Registered player: '{0}'", player.name);

        Players.Add(player);

        player.GetComponent<Health>().Changed += OnPlayerHealthChanged;

        player.BattleEntities.ForEach(x => x.Initialize(this));
    }

    public void Log(string message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message))
            Queue.RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message), "BattleLog");
    }

    public void Continue()
    {
        var next = Queue.Dequeue();

        if (next != null)
        {
            next.Completed += OnActionExecutionComplete;
            next.Execute(this);
        }
        else
        {
            LogEx.Log<BattleSystem>("No more battle actions left to execute.");

            TurnCount++;

            Queue.Reset();
            //Queue.RegisterWeather(Weather.Current);

            if (CommandsDepleted != null)
                CommandsDepleted.Invoke(this);
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
        Queue.RegisterAction(UpdateHealth.Create(obj));
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) && _enableManualStepping)
            Continue();
    }
}
