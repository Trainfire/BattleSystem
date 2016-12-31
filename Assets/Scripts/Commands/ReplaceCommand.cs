using UnityEngine;
using System.Collections;
using System;

public class ReplaceCommand : BaseAction
{
    public Player Player { get; private set; }
    public Character Replacement { get; private set; }

    public void Initialize(Player player, Character replacement)
    {
        Player = player;
        Replacement = replacement;
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        battleSystem.Log("{0} sent in {1}!", Player.name, Replacement.name);
        battleSystem.Registry.RegisterAction(Replacement.SwitchIn, "Switch In");
    }

    public static ReplaceCommand Create(Player player, Character replacement)
    {
        var replaceAction = new GameObject("Switch").AddComponent<ReplaceCommand>();
        replaceAction.Initialize(player, replacement);
        return replaceAction;
    }
}
