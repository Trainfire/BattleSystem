using UnityEngine;
using System;

public class HealthChangeEvent
{
    public Health Health { get; set; }
    public Player Source { get; set; }
    public Player Target { get; set; }
    public int OldValue { get; set; }
    public int NewValue { get; set; }
}

public class Health : MonoBehaviour
{
    public event Action<HealthChangeEvent> Changed;

    public int Current { get; private set; }

    void Awake()
    {
        Current = 100;
    }

    public void ChangeHealth(Player source, int amount, bool silent = false)
    {
        if (Changed != null && !silent)
        {
            Changed.Invoke(new HealthChangeEvent()
            {
                Health = this,
                Source = source,
                Target = GetComponent<Player>(),
                OldValue = Current,
                NewValue = Current + amount,
            });
        }
    }

    public void ChangeHealth(int amount)
    {
        Current += amount;
        Debug.Log(name + " received " + amount + " damage.");
    }

    public void Set(int value)
    {
        Current = value;
    }
}
