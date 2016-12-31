using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

public class Character : MonoBehaviour
{
    public event Action<Character> SwitchedOut;
    public event Action<Character> SwitchedIn;

    public event Action<StatusChangeEvent> StatusChanged;
    public event Action<ConditionChangeEvent> ConditionChanged;

    public Attack Attack;
    public GameObject HeldItem;
    public GameObject Ability;

    public Player Owner { get; private set; }
    public Health Health { get; private set; }
    public CharacterStatus Status { get; private set; }
    public ActiveState ActiveState { get; private set; } 

    private List<CharacterListener> _battleEntities;
    public List<CharacterListener> BattleEntities { get { return _battleEntities; } }

    void Awake()
    {
        Owner = gameObject.GetComponentInParent<Player>();
        Health = gameObject.AddComponent<Health>();
        Health.Changed += OnHealthChanged;

        Status = gameObject.AddComponent<CharacterStatus>();
        Status.Initialize(this);

        _battleEntities = new List<CharacterListener>();
        RegisterListener(HeldItem);
        RegisterListener(Ability);
    }

    void OnHealthChanged(HealthChangeEvent obj)
    {
        if (obj.NewValue <= 0)
        {
            ActiveState = ActiveState.Fainted;
        }
    }

    void RegisterListener(GameObject prototype)
    {
        if (prototype == null)
            return;

        var instance = GameObject.Instantiate(prototype);

        var battleEntity = instance.GetComponent<CharacterListener>();
        if (battleEntity == null)
        {
            Debug.LogWarningFormat("Tried to register object '{0}' but no Event Listener is attached.", prototype.name);
        }
        else
        {
            battleEntity.SetCharacter(this);
            _battleEntities.Add(battleEntity);
        }
    }

    public void SwitchOut()
    {
        if (CanSwitch())
        {
            ActiveState = ActiveState.Inactive;
            SwitchedOut.InvokeSafe(this);
        }
    }

    public void SwitchIn()
    {
        if (CanSwitch())
        {
            ActiveState = ActiveState.InBattle;
            SwitchedIn.InvokeSafe(this);
        }
    }

    public bool CanSwitch()
    {
        if (ActiveState == ActiveState.Fainted)
            Debug.LogWarningFormat("Cannot switch in character '{0}' as their state is currently Fainted.", name);

        // TODO: Check if character is locked into battle.

        return ActiveState != ActiveState.Fainted;
    }
}
