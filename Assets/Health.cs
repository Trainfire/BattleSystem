using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int Current { get; private set; }

    void Awake()
    {
        Current = 100;
    }

    public void ChangeHealth(int amount)
    {
        Current += amount;

        Debug.Log(name + " received " + amount + " damage.");
    }
}
