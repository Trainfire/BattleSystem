using UnityEngine;
using System;

public class HealthChangeEvent
{
    public Health Health { get; set; }
    public Character Source { get; set; }
    public Character Reciever { get; set; }
    public int OldValue { get; set; }
    public int NewValue { get; set; }
}

public class Health : MonoBehaviour
{
    public event Action<HealthChangeEvent> Changed;

    public int Current;

    void Awake()
    {
        Current = 100;
    }

    public void ChangeHealth(Character source, int amount)
    {
        if (Changed != null)
        {
            Changed.Invoke(new HealthChangeEvent()
            {
                Health = this,
                Source = source,
                Reciever = GetComponent<Character>(),
                OldValue = Current,
                NewValue = Current + amount,
            });
        }
    }

    public void ChangeHealth(int amount)
    {
        var old = Current;
        Current += amount;
        Debug.LogFormat("{0} HP changed from {1} to {2}", name, old, Current);
    }

    public void Set(int value)
    {
        Current = value;
        Debug.Log(name + " now has " + Current + " HP.");
    }
}
