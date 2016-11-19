using UnityEngine;
using System;

public abstract class TargetedAction : BaseAction
{
    public BattleSystem BattleSystem { get; private set; }
    public Player Source { get; private set; }
    public Player Target { get; private set; }

    public void Initialize(BattleSystem battleSystem, Player source, Player target)
    {
        BattleSystem = battleSystem;
        Source = source;
        Target = target;
    }

    public static T Create<T>(BattleSystem battleSystem, Player source, Player target) where T : TargetedAction
    {
        var targetedAction = new GameObject("TargetedAction").AddComponent<T>();
        targetedAction.Initialize(battleSystem, source, target);
        return targetedAction;
    }

    public static T Create<T>(BattleSystem battleSystem, Player target) where T : TargetedAction
    {
        var targetedAction = new GameObject("TargetedAction").AddComponent<T>();
        targetedAction.Initialize(battleSystem, null, target);
        return targetedAction;
    }
}
