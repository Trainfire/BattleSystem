using UnityEngine;
using System.Collections;
using System;

public class Switch : TargetedAction
{
    private PlayerSwitchEvent _switchEvent;

    public void Initialize(PlayerSwitchEvent switchEvent)
    {
        _switchEvent = switchEvent;
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        battleSystem.Log("{0} called back {1}!", _switchEvent.Player.name, _switchEvent.Player.Party.InBattle.name);
        battleSystem.Registry.RegisterAction(_switchEvent.Player.Party.InBattle.SwitchOut, "Switch Out");

        battleSystem.Log("{0} sent in {1}!", _switchEvent.Player.name, _switchEvent.SwitchTarget.name);
        battleSystem.Registry.RegisterAction(_switchEvent.SwitchTarget.SwitchIn, "Switch In");
    }

    public static Switch Create(PlayerSwitchEvent arg)
    {
        var switchAction = new GameObject("Switch").AddComponent<Switch>();
        switchAction.Initialize(arg);
        switchAction.SetReciever(arg.SwitchTarget);
        return switchAction;
    }
}
