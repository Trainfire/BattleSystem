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
    public event Action<SwitchCommand> SwitchedCharacter;
    public event Action<AttackCommand> AttackSelected;
    public event Action<ReplaceCommand> ReplacementCharacterSelected;

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

        // TODO: 
        // * FieldSlot is null here but set inside the AttackCommand. This is temporary until there's a UI.
        // * Attack is just the first attack. This is temporary until the player can select from a list of attacks.
        var attackCommand = AttackCommand.Create(this, ActiveCharacter, null, ActiveCharacter.Attack);
        AttackSelected.InvokeSafe(attackCommand);
        SetReady();
    }

    public void Switch(Character switchTarget)
    {
        SwitchedCharacter.InvokeSafe(SwitchCommand.Create(this, switchTarget));
        SetReady();
    }

    /// <summary>
    /// TEMP!!!
    /// </summary>
    /// <param name="switchTarget"></param>
    public void Switch()
    {
        var switchTarget = Party.Characters.Where(x => x != Party.InBattle).FirstOrDefault();
        SwitchedCharacter.InvokeSafe(SwitchCommand.Create(this, switchTarget));
        SetReady();
    }

    /// <summary>
    /// TEMP!!! Needs to include Character as an argument.
    /// </summary>
    public void SelectReplacement()
    {
        // TEMP: Just select whichever character hasn't fainted.
        var replacement = Party.Characters.FirstOrDefault(x => x.ActiveState != ActiveState.Fainted);
        ReplacementCharacterSelected.InvokeSafe(ReplaceCommand.Create(this, replacement));
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
