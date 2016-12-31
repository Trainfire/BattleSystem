using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleRegistry : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    public event Action<Player> PlayerAdded;
    public event Action<Player> PlayerRemoved;
    public event Action<Character> CharacterAdded;
    public event Action<Character> CharacterRemoved;
    public event Action<BaseAction, BattleQueueType> ActionRegistered;

    public List<Player> Players { get; private set; }
    public List<Character> ActiveCharacters { get; private set; }

    void Awake()
    {
        Players = new List<Player>();
        ActiveCharacters = new List<Character>();
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

    /// <summary>
    /// TODO: Use Typing to only allow actions of type PlayerCommnd.
    /// </summary>
    /// <param name="action"></param>
    public void RegisterPlayerCommand(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }

    public void RegisterPlayer(Player player)
    {
        if (!Players.Contains(player))
        {
            LogEx.Log<BattleRegistry>("Registered player: '{0}'", player.name);

            Players.Add(player);

            // TEMP!!!
            RegisterCharacter(player.ActiveCharacter);

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

            // TEMP!!!
            UnregisterCharacter(player.ActiveCharacter);

            if (PlayerRemoved != null)
                PlayerRemoved.Invoke(player);
        }
        else
        {
            LogEx.LogError<BattleRegistry>("Cannot unregister player '{0}' as they were never registered.", player.name);
        }
    }

    public void RegisterCharacter(Character character)
    {
        if (!ActiveCharacters.Contains(character))
        {
            LogEx.Log<BattleRegistry>("Registered character: '{0}'", character.name);

            ActiveCharacters.Add(character);

            if (CharacterAdded != null)
                CharacterAdded.Invoke(character);
        }
        else
        {
            LogEx.LogError<BattleRegistry>("Cannot register character '{0}' as they are already registered.", character.name);
        }
    }

    public void UnregisterCharacter(Character character)
    {
        if (ActiveCharacters.Contains(character))
        {
            LogEx.Log<BattleRegistry>("Unregistered character: '{0}'", character.name);

            ActiveCharacters.Remove(character);

            if (CharacterRemoved != null)
                CharacterRemoved.Invoke(character);
        }
        else
        {
            LogEx.LogError<BattleRegistry>("Cannot unregister character '{0}' as they were never registered.", character.name);
        }
    }
}
