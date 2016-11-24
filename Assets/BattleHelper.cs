using UnityEngine;
using System;

public class BattleHelper : MonoBehaviour
{
    public GameObject Poison;
    public GameObject Sleep;

    public void SetPlayerStatus(Status status, Player target)
    {
        TargetedAction instance = null;

        switch (status)
        {
            case Status.None: break;
            case Status.Poisoned: instance = Create(Poison); break;
            case Status.Asleep: break;
        }

        target.SetStatus(status, instance);
    }

    TargetedAction Create(GameObject prototype)
    {
        return Instantiate(prototype).GetComponent<TargetedAction>();
    }
}
