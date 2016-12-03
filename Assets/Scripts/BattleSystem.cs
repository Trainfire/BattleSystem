using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(BattleQueue), typeof(BattleWeather))]
public class BattleSystem : MonoBehaviour
{
    [SerializeField] private bool _autoReadyPlayers;
    [SerializeField] private bool _enableManualStepping;

    public event Action<BattleSystem> CommandsDepleted;

    public BattleRegistry Registry { get; private set; }
    public BattleHelper Helper { get; private set; }
    public BattleWeather Weather { get; private set; }
    public List<Player> Players { get; private set; }

    private BattleQueue _queue;

    public int TurnCount { get; private set; }

    public bool AutoReadyPlayers { get { return _autoReadyPlayers; } }
    public bool EnableManualStepping { get { return _enableManualStepping; } set { _enableManualStepping = true; } }

    void Awake()
    {
        Players = new List<Player>();

        Registry = gameObject.GetComponent<BattleRegistry>();

        _queue = gameObject.GetComponent<BattleQueue>();
        _queue.Initialize(this);

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
            Registry.RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message), "BattleLog");
    }

    /// <summary>
    /// Do not call directly except for Editor purposes.
    /// </summary>
    public void Continue()
    {
        var next = _queue.Dequeue();

        if (next != null)
        {
            next.Completed += OnActionExecutionComplete;
            next.Execute(this);
        }
        else
        {
            LogEx.Log<BattleSystem>("No more battle actions left to execute.");

            _queue.Reset();

            if (CommandsDepleted != null)
                CommandsDepleted.Invoke(this);

            TurnCount++;
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
        Registry.RegisterAction(UpdateHealth.Create(obj));
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) && _enableManualStepping)
            Continue();
    }
}
