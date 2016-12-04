using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleRegistry : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    public event Action<Player> PlayerAdded;
    public event Action<Player> PlayerRemoved;
    public event Action<BaseAction, BattleQueueType> ActionRegistered;

    public List<Player> Players { get; private set; }

    void Awake()
    {
        Players = new List<Player>();
    }

    void RegisterAction(BaseAction action, BattleQueueType type)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: {0} of type '{1}'", action.GetType().Name, type.ToString());

        if (ActionRegistered != null)
            ActionRegistered.Invoke(action, type);
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

    public void RegisterPlayer(Player player)
    {
        if (!Players.Contains(player))
        {
            LogEx.Log<BattleRegistry>("Registered player: '{0}'", player.name);

            Players.Add(player);

            if (PlayerAdded != null)
                PlayerAdded.Invoke(player);
        }
        else
        {
            LogEx.LogError<BattleRegistry>("Cannot register player '{0}' as they are already registered.", player.name);
        }
    }

    public void UnregisterPlayer(Player player)
    {
        if (Players.Contains(player))
        {
            LogEx.Log<BattleRegistry>("Unregistered player: '{0}'", player.name);

            Players.Remove(player);

            if (PlayerRemoved != null)
                PlayerRemoved.Invoke(player);
        }
        else
        {
            LogEx.LogError<BattleRegistry>("Cannot unregister player '{0}' as they were never registered.", player.name);
        }
    }
}
