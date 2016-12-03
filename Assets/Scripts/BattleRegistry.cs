using UnityEngine;
using System;

public class BattleRegistry : MonoBehaviour
{
    [SerializeField] private bool _logRegistrations;

    public event Action<BaseAction, BattleQueueType> ActionRegistered;

    void RegisterAction(BaseAction action, BattleQueueType type)
    {
        if (_logRegistrations)
            LogEx.Log<BattleQueue>("Registered action: {0} of type '{1}'", action.GetType().Name, type.ToString());

        if (ActionRegistered != null)
            ActionRegistered.Invoke(action, type);
    }

    public void RegisterWeather(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.Weather);
    }

    public void RegisterStatusUpdate(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.StatusUpdate);
    }

    public void RegisterAction(Action action, string name)
    {
        RegisterAction(AnonAction.Create(action, name), BattleQueueType.Normal);
    }

    public void RegisterAction(BaseAction action)
    {
        RegisterAction(action, BattleQueueType.Normal);
    }

    public void RegisterPlayerCommand(TargetedAction action)
    {
        RegisterAction(action, BattleQueueType.PlayerCommand);
    }
}
