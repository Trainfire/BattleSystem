using UnityEngine;
using System;

public class BattleHelper : MonoBehaviour
{
    public GameObject Poison;

    public ConfusionParameters ConfusionParameters;
    public ParalysisParameters ParalysisParameters;
    public SleepParameters SleepParameters;

    public void SetPlayerStatus(Status status, Player target)
    {
        TargetedAction instance = null;

        switch (status)
        {
            case Status.Poisoned: instance = Create(Poison); break;
        }

        target.SetStatus(status, instance);
    }

    public bool SetCondition(ConditionType condition, Player target)
    {
        // TODO: Test.

        bool isStatus = condition == ConditionType.Paralysis || condition == ConditionType.Sleep;

        if (isStatus && target.Status == Status.None)
        {
            switch (condition)
            {
                case ConditionType.Paralysis:
                    var paralysis = target.gameObject.AddComponent<ConditionParalysis>();
                    paralysis.Parameters = ParalysisParameters;
                    target.SetStatus(Status.Paralyzed, null);
                    break;
                case ConditionType.Sleep:
                    var sleep = target.gameObject.AddComponent<ConditionSleep>();
                    sleep.Parameters = SleepParameters;
                    target.SetStatus(Status.Asleep, null);
                    break;
            }

            return true;
        }
        else if (!target.HasCondition(condition))
        {
            // Concurrent conditions.
            switch (condition)
            {
                case ConditionType.Confusion:
                    var confusion = target.gameObject.AddComponent<ConditionConfusion>();
                    confusion.Parameters = ConfusionParameters;
                    break;
            }

            return true;
        }

        return false;
    }

    TargetedAction Create(GameObject prototype)
    {
        return Instantiate(prototype).GetComponent<TargetedAction>();
    }
}
