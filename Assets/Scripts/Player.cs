using UnityEngine;
using System;
using System.Linq;
using Framework;

public class PlayerSwitchEvent
{
    public Player Player { get; private set; }
    public Character SwitchTarget { get; private set; } // Maybe replace with index?

    public PlayerSwitchEvent(Player player, Character switchTarget)
    {
        Player = player;
        SwitchTarget = switchTarget;
    }
}

[RequireComponent(typeof(PlayerParty))]
public class Player : MonoBehaviour
{
    public event Action<Player> ReadyStateChanged;
    public event Action<PlayerSwitchEvent> SwitchedCharacter;
    public event Action<Player> AttackSelected;

    public PlayerParty Party { get; private set; }
    public Character ActiveCharacter { get { return Party.InBattle; } }

    public bool IsReady { get; private set; }

    void Awake()
    {
        Party = GetComponent<PlayerParty>();
    }

    public void Attack()
    {
        // TODO: 
        // * Allow target to be passed in.
        // * Allow move to be passed in.
        AttackSelected.InvokeSafe(this);
        SetReady();
    }

    public void Switch(Character switchTarget)
    {
        SwitchedCharacter.InvokeSafe(new PlayerSwitchEvent(this, switchTarget));
        SetReady();
    }

    /// <summary>
    /// TEMP!!!
    /// </summary>
    /// <param name="switchTarget"></param>
    public void Switch()
    {
        var switchTarget = Party.Characters.Where(x => x != Party.InBattle).FirstOrDefault();
        SwitchedCharacter.InvokeSafe(new PlayerSwitchEvent(this, switchTarget));
        SetReady();
    }

    public void SetReady()
    {
        IsReady = true;
        ReadyStateChanged.InvokeSafe(this);
    }

    public void ToggleReady()
    {
        IsReady = !IsReady;
        ReadyStateChanged.InvokeSafe(this);
    }

    public void ResetReady()
    {
        IsReady = false;
    }
}
