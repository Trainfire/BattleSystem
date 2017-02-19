using UnityEngine;
using System;

public class BattleHelper : MonoBehaviour
{
    public GameObject Poison;
    public GameObject Spikes;

    [Header("Status Effects")]
    public ConfusionParameters ConfusionParameters;
    public ParalysisParameters ParalysisParameters;
    public SleepParameters SleepParameters;

    [Header("Field Effects")]
    public FieldEffectSpikesParameters SpikesParameters;

    public bool SetPlayerStatus(Status status, Character target)
    {
        if (target.Status.Current == Status.None)
        {
            TargetedAction instance = null;

            switch (status)
            {
                case Status.Poisoned: instance = Create(Poison); break;
            }

            target.Status.SetStatus(status, instance);

            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetConditionAddedMessage(Character player, ConditionType condition)
    {
        // TODO: Remove hard-coded strings.
        switch (condition)
        {
            case ConditionType.Confusion: return string.Format("{0} became confused!", player.name);
        }

        return "";
    }

    public string GetConditionRemovedMessage(Character player, ConditionType condition)
    {
        // TODO: Remove hard-coded strings.
        switch (condition)
        {
            case ConditionType.Confusion: return string.Format("{0} snapped out of their confusion!", player.name);
            case ConditionType.Sleep: return string.Format("{0} woke up!", player.name);
        }

        return "";
    }

    public string GetStatusAddedMessage(Character player, Status status)
    {
        // TODO: Remove hard-coded strings.
        switch (status)
        {
            case Status.Poisoned: return string.Format("{0} was poisoned!", player.name);
            case Status.Asleep: return string.Format("{0} fell asleep!", player.name);
            case Status.Paralyzed: return string.Format("{0} became paralyzed! They may be unable to move!", player.name);
        }

        return "";
    }

    public bool SetCondition(ConditionType condition, Character target)
    {
        bool isStatus = condition == ConditionType.Paralysis || condition == ConditionType.Sleep;

        if (isStatus && target.Status.Current == Status.None)
        {
            switch (condition)
            {
                case ConditionType.Paralysis:

                    var paralysis = target.gameObject.AddComponent<ConditionParalysis>();
                    paralysis.Parameters = ParalysisParameters;

                    target.Status.SetStatus(Status.Paralyzed, null);
                    target.Status.AddCondition(paralysis);

                    break;

                case ConditionType.Sleep:

                    var sleep = target.gameObject.AddComponent<ConditionSleep>();
                    sleep.Parameters = SleepParameters;

                    target.Status.SetStatus(Status.Asleep, null);
                    target.Status.AddCondition(sleep);

                    break;
            }

            return true;
        }
        else if (!target.Status.HasCondition(condition))
        {
            Condition conditionComp = null;

            // Concurrent conditions.
            switch (condition)
            {
                case ConditionType.Confusion:
                    var confusion = target.gameObject.AddComponent<ConditionConfusion>();
                    confusion.Parameters = ConfusionParameters;
                    conditionComp = confusion;
                    break;
            }

            target.Status.AddCondition(conditionComp);

            return true;
        }

        LogEx.Log<BattleHelper>("Failed to set condition to: " + condition);

        return false;
    }

    TargetedAction Create(GameObject prototype)
    {
        return Instantiate(prototype).GetComponent<TargetedAction>();
    }
}
