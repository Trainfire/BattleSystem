using System;
using UnityEngine;

class TriggerOnDestroy : TargetedAction
{
    public GameObject Reference;

    public void RelayToReference(BattleSystem battleSystem)
    {
        if (Log)
            LogEx.Log<TriggerOnDestroy>("Relaying...");

        foreach (var action in Reference.GetComponents<TargetedAction>())
        {
            action.SetSource(Source);
            action.SetReciever(Reciever);
            battleSystem.Queue.RegisterAction(action);
        }
    }
}
