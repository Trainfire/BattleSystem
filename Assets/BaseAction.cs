using UnityEngine;
using System;

public abstract class BaseAction : MonoBehaviour
{
    public event Action<BaseAction> Completed;

    public virtual void Execute()
    {
        OnExecute();
    }

    protected abstract void OnExecute();

    protected void TriggerCompletion()
    {
        if (Completed != null)
            Completed.Invoke(this);

        if (tag != "DontDestroyOnTrigger")
            Destroy(gameObject);
    }
}

