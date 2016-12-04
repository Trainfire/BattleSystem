using UnityEngine;
using System;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public event Action<StatusChangeEvent> StatusChanged;
    public event Action<ConditionChangeEvent> ConditionChanged;

    public event Action<Player> ReadyStateChanged;

    public bool IsReady { get; private set; }

    public TargetedAction Attack;
    public GameObject HeldItem;
    public GameObject Ability;

    public Health Health { get; private set; }
    public PlayerStatus Status { get; private set; }

    private List<PlayerListener> _battleEntities;
    public List<PlayerListener> BattleEntities { get { return _battleEntities; } }

    void Awake()
    {
        Health = gameObject.AddComponent<Health>();

        Status = gameObject.AddComponent<PlayerStatus>();
        Status.Initialize(this);

        _battleEntities = new List<PlayerListener>();
        RegisterListener(HeldItem);
        RegisterListener(Ability);
    }

    void RegisterListener(GameObject prototype)
    {
        if (prototype == null)
            return;

        var instance = GameObject.Instantiate(prototype);

        var battleEntity = instance.GetComponent<PlayerListener>();
        if (battleEntity == null)
        {
            Debug.LogWarningFormat("Tried to register object '{0}' but no Event Listener is attached.", prototype.name);
        }
        else
        {
            battleEntity.SetPlayer(this);
            _battleEntities.Add(battleEntity);
        }
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
