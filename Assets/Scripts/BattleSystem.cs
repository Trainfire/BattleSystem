using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework;

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private bool _logRegistrations;

    public event Action<Player> PlayerAdded;
    public event Action<Player> PlayerRemoved;

    public event Action<Character> CharacterAddedToSlot;
    public event Action<Character> PostCharacterAddedToSlot;
    public event Action<Character> CharacterRemovedFromSlot;
    public event Action<Character> PostCharacterRemovedFromSlot;

    public event Action<BaseAction, BattleQueueType> ActionRegistered;
    public event Action<string> LogPosted;

    private List<Player> _players;
    private List<Character> _activeCharacters;
    private List<FieldSide> _fieldSides;

    /// <summary>
    /// Returns a copy of the list of Players. Use (Un)registerPlayer instead if you want to modify the collection.
    /// </summary>
    public List<Player> Players { get { return _players.ToList(); } }

    /// <summary>
    /// Returns a copy of the list of Active Characters. Use (Un)registerCharacter instead if you want to modify the collection.
    /// </summary>
    public List<Character> ActiveCharacters { get { return _activeCharacters.ToList(); } }

    public BattleHelper Helper { get; private set; }
    public BattleWeather Weather { get; private set; }

    public int TurnCount { get; private set; }

    void Awake()
    {
        _players = new List<Player>();
        _activeCharacters = new List<Character>();
        _fieldSides = new List<FieldSide>();

        Helper = gameObject.GetOrAddComponent<BattleHelper>();
        Weather = gameObject.GetOrAddComponent<BattleWeather>();

        // 2 sides. 1 slot each.
        for (int i = 0; i < 2; i++)
        {
            var fieldSide = new GameObject("Field Side").AddComponent<FieldSide>();
            fieldSide.Initialize(this, i, 1);

            foreach (var slot in fieldSide.Slots)
            {
                slot.CharacterAdded += RegisterCharacter;
                slot.CharacterRemoved += UnregisterCharacter;
            }

            _fieldSides.Add(fieldSide);
        }
    }

    public void Log(string message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message))
        {
            RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message), "BattleLog");

            RegisterAction(() =>
            {
                LogEx.Log<BattleSystem>("Battle Log: " + message);
                LogPosted.InvokeSafe(message);

            }, "BattleLog");
        }
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
        RegisterAction(AnonAction.Create(action, name), BattleQueueType.GenericUpdate);
    }

    public void RegisterAction(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.GenericUpdate);
    }

    /// <summary>
    /// TODO: Use Typing to only allow actions of type PlayerCommnd.
    /// </summary>
    /// <param name="action"></param>
    public void RegisterPlayerCommand(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }

    public void RegisterPlayer(Player player, int fieldSideID)
    {
        if (!_players.Contains(player))
        {
            LogEx.Log<BattleSystem>("Registered player: '{0}'", player.name);

            _players.Add(player);

            fieldSideID = Mathf.Clamp(fieldSideID, 0, _fieldSides.Count - 1);

            player.AssignFieldSide(_fieldSides[fieldSideID]);

            // TEMP!!!
            //RegisterCharacter(player.ActiveCharacter);

            // TEMP!!!
            //player.ActiveCharacter.BattleEntities.ForEach(x => x.Initialize(this));

            if (PlayerAdded != null)
                PlayerAdded.Invoke(player);
        }
        else
        {
            LogEx.LogError<BattleSystem>("Cannot register player '{0}' as they are already registered.", player.name);
        }
    }

    public void UnregisterPlayer(Player player)
    {
        if (_players.Contains(player))
        {
            LogEx.Log<BattleSystem>("Unregistered player: '{0}'", player.name);

            _players.Remove(player);

            // TEMP!!!
            UnregisterCharacter(player.ActiveCharacter);

            if (PlayerRemoved != null)
                PlayerRemoved.Invoke(player);
        }
        else
        {
            LogEx.LogError<BattleSystem>("Cannot unregister player '{0}' as they were never registered.", player.name);
        }
    }

    public void RegisterCharacter(Character character)
    {
        if (!_activeCharacters.Contains(character))
        {
            LogEx.Log<BattleSystem>("Registered character: '{0}'", character.name);

            _activeCharacters.Add(character);

            character.BattleEntities.ForEach(x => x.Initialize(this));

            // Assign character into player slot.
            //character.Owner.FieldSide.Register(character);

            if (CharacterAddedToSlot != null)
                CharacterAddedToSlot.Invoke(character);

            if (PostCharacterAddedToSlot != null)
                PostCharacterAddedToSlot.Invoke(character);
        }
        else
        {
            LogEx.LogError<BattleSystem>("Cannot register character '{0}' as they are already registered.", character.name);
        }
    }

    public void UnregisterCharacter(Character character)
    {
        if (_activeCharacters.Contains(character))
        {
            LogEx.Log<BattleSystem>("Unregistered character: '{0}'", character.name);

            _activeCharacters.Remove(character);

            if (CharacterRemovedFromSlot != null)
                CharacterRemovedFromSlot.Invoke(character);

            if (PostCharacterRemovedFromSlot != null)
                PostCharacterRemovedFromSlot.Invoke(character);
        }
        else
        {
            LogEx.LogError<BattleSystem>("Cannot unregister character '{0}' as they were never registered.", character.name);
        }
    }
}
