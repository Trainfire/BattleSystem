using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    public bool Log;

    public event Action<BaseAction> Completed;

    public virtual bool FlaggedForRemoval { get { return false; } }

    public bool Executed { get; private set; }

    public virtual string LogInfo { get { return string.Empty; } }

    private BattleSystem _battleSystem;

    public virtual void Execute(BattleSystem battleSystem)
    {
        Executed = true;

        if (Log)
            LogEx.Log<BaseAction>(name + " executed.");

        _battleSystem = battleSystem;

        OnExecute(battleSystem);
    }

    protected abstract void OnExecute(BattleSystem battleSystem);

    protected void TriggerCompletion()
    {
        if (Completed != null)
            Completed.Invoke(this);
    }

    protected virtual void OnFinish() { }

    void OnDestroy()
    {
        var triggerOnDestroy = GetComponent<TriggerOnDestroy>();
        if (triggerOnDestroy != null && _battleSystem != null)
            triggerOnDestroy.RelayToReference(_battleSystem);
    }
}

