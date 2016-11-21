using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    public event Action<BaseAction> Completed;

    public virtual void Execute(BattleSystem battleSystem)
    {
        OnExecute(battleSystem);
    }

    protected abstract void OnExecute(BattleSystem battleSystem);

    protected void TriggerCompletion()
    {
        if (Completed != null)
            Completed.Invoke(this);

        if (tag != "DontDestroyOnTrigger")
            Destroy(gameObject);
    }

    protected virtual void OnFinish() { }
}

