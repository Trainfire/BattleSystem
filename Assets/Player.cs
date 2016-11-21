using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public event Action<Player> ReadyStateChanged;
    public bool IsReady { get; private set; }

    public TargetedAction Attack;
    public TargetedAction HeldItem;

    void Awake()
    {
        gameObject.AddComponent<Health>();

        if (HeldItem != null)
            HeldItem = Instantiate(HeldItem);
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
