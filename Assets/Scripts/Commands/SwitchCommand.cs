using UnityEngine;
using System.Collections;
using System;

public class SwitchCommand : BaseAction
{
    public Player Player { get; private set; }
    public Character SwitchTarget { get; private set; }

    public void Initialize(Player player, Character switchTarget)
    {
        Player = player;
        SwitchTarget = switchTarget;
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Player.Party.InBattle != null)
        {
            if (Player.Party.InBattle.Status.HasCondition(ConditionType.SwitchLock))
            {
                battleSystem.Log("{0} cannot be switched out!", Player.Party.InBattle.name);
                TriggerCompletion();
                return;
            }

            battleSystem.Log("{0} called back {1}!", Player.name, Player.Party.InBattle.name);
            battleSystem.RegisterAction(Player.Party.InBattle.SwitchOut, "Switch Out");
        }

        battleSystem.Log("{0} sent in {1}!", Player.name, SwitchTarget.name);
        battleSystem.RegisterAction(SwitchTarget.SwitchIn, "Switch In");

        TriggerCompletion();
    }

    public static SwitchCommand Create(Player player, Character switchTarget)
    {
        var switchAction = new GameObject("Switch").AddComponent<SwitchCommand>();
        switchAction.Initialize(player, switchTarget);
        return switchAction;
    }
}
