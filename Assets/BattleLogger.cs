using System;
using UnityEngine;

class BattleLogger : TargetedAction
{
    public string Message;

    protected override void OnExecute()
    {
        Message = Message.Replace("{SOURCE}", Source.name);
        Message = Message.Replace("{TARGET}", Target.name);

        BattleSystem.Log(Message);
        TriggerCompletion();
    }
}
