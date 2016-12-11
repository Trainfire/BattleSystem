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

    public static string Format(string message, Character source, Character reciever)
    {
        if (source != null)
            message = message.Replace("{SOURCE}", source.name);

        if (reciever != null)
            message = message.Replace("{TARGET}", reciever.name);

        return message;
    }
}
