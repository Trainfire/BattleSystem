using System;
using UnityEngine;

class Lifetime : TargetedAction
{
    public int MaxExecutions;
    public string ExpiryMessage;

    private int _executionsTotal;

    void Awake()
    {
        MaxExecutions = Mathf.Clamp(MaxExecutions, 1, MaxExecutions);
    }

    protected override void OnExecute(BattleSystem battleSystem)
    {
        _executionsTotal++;

        TriggerCompletion();

        if (_executionsTotal >= MaxExecutions)
        {
            LogEx.Log<Lifetime>(name + "'s life expired.");
            battleSystem.Log(ExpiryMessage);
            Destroy(gameObject);
        }
    }
}
