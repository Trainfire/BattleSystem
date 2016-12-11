using UnityEngine;
using System;

[RequireComponent(typeof(PlayerParty))]
public class Player : MonoBehaviour
{
    public event Action<Player> ReadyStateChanged;

    public PlayerParty Party { get; private set; }
    public Character ActiveCharacter { get { return Party.InBattle; } }

    public bool IsReady { get; private set; }

    void Awake()
    {
        Party = GetComponent<PlayerParty>();
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
