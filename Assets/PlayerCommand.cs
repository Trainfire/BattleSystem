using UnityEngine;
using System;

public abstract class PlayerCommand : BaseAction
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
}
