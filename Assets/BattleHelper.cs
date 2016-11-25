using UnityEngine;
using System;

public class BattleHelper : MonoBehaviour
{
    public GameObject Poison;
    public GameObject Sleep;

    private BattleSystem _battleSystem;

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
    }

    public void SetPlayerStatus(Status status, Player target)
    {
        TargetedAction instance = null;

        switch (status)
        {
            case Status.Poisoned: instance = Create(Poison); break;
            case Status.Asleep: break;
        }

        target.SetStatus(status, instance);

        _battleSystem.Queue.RegisterStatusUpdate(instance, status.ToString());
    }

    TargetedAction Create(GameObject prototype)
    {
        return Instantiate(prototype).GetComponent<TargetedAction>();
    }
}
