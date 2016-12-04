using System;
using UnityEngine;

class Lifetime : TargetedAction
{
    public bool NeverExpires;
    public int MaxExecutions;
    public string ExpiryMessage;

    public bool Expired
    {
        get { return NeverExpires ? false : _executionsTotal >= MaxExecutions; }
    }

    private int _executionsTotal;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        _executionsTotal++;

        TriggerCompletion();

        if (Expired)
        {
            LogEx.Log<Lifetime>(name + "'s life expired.");
            battleSystem.Log(ExpiryMessage);
        }
    }
}
