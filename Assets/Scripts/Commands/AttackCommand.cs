using System.Linq;
using UnityEngine;

/// <summary>
/// A player-issued command that stores a reference to the player's chosen target and the attack they want to use.
/// </summary>
public class AttackCommand : BaseAction
{
    public static AttackCommand Create(Player player, Character actor, FieldSlot targetSlot, Attack attack)
    {
        var attackCommand = new GameObject("AttackCommand").AddComponent<AttackCommand>();
        attackCommand.Initialize(player, actor, targetSlot, attack);
        return attackCommand;
    }

    private Player _player;
    private Character _actor;
    private FieldSlot _targetSlot; // Unused.
    private Attack _attack;

    void Initialize(Player player, Character actor, FieldSlot targetSlot, Attack attack)
    {
        _player = player;
        _actor = actor;
        _targetSlot = targetSlot;
        _attack = attack;
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        // TEMP!
        var target = battleSystem.Players.Where(x => x != _player).First().ActiveCharacter;

        var attackInstance = GameObject.Instantiate(_attack.gameObject).GetComponent<Attack>();
        attackInstance.name = "Attack";
        attackInstance.SetSource(_actor);
        attackInstance.SetReciever(target);

        battleSystem.RegisterAction(attackInstance);

        TriggerCompletion();
    }
}
