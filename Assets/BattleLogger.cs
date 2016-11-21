using System;
using UnityEngine;

class BattleLogger : TargetedAction
{
    public string Message;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        Message = Message.Replace("{SOURCE}", Source.name);
        Message = Message.Replace("{TARGET}", Reciever.name);

        battleSystem.Log(Message);
        TriggerCompletion();
    }
}
