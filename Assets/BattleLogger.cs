using System;
using UnityEngine;

class BattleLogger : TargetedAction
{
    public string Message;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        if (Source != null)
            Message = Message.Replace("{SOURCE}", Source.name);

        if (Reciever != null)
            Message = Message.Replace("{TARGET}", Reciever.name);

        battleSystem.Log(Message);
        TriggerCompletion();
    }
}
