using System;
using UnityEngine;

class ModifyHealth : TargetedAction
{
    public int Amount;

    protected override void OnExecute()
    {
        Target.GetComponent<Health>().ChangeHealth(Source, Amount);
        TriggerCompletion();
    }
}
