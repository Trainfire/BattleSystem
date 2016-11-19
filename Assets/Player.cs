using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public event Action<Player> ReadyStateChanged;
    public bool IsReady { get; private set; }

    void Awake()
    {
        gameObject.AddComponent<Health>();
    }

    public void ToggleReady()
    {
        IsReady = !IsReady;

        if (ReadyStateChanged != null)
            ReadyStateChanged.Invoke(this);
    }

    public void ResetReady()
    {
        IsReady = false;
    }
}
