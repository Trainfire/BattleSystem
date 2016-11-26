using UnityEngine;

public class Attack : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        // Evaluate pre attack conditions.
        TargetedAction preAttackAction = null;
        foreach (var condition in Source.GetComponents<Condition>())
        {
            battleSystem.Log(BattleLogger.Format(condition.EvaluationMessage, Source, Source));

            var result = condition.Evaluate();
            if (!result.Passed)
            {
                result.Action.SetReciever(Source);
                preAttackAction = result.Action;
                break;
            }
        }

        if (preAttackAction != null)
        {
            battleSystem.Queue.RegisterAction(preAttackAction);
        }
        else
        {
            battleSystem.Log(Source.name + " used {0}!", name.Replace("(Clone)", ""));
            Relay(battleSystem);
        }

        TriggerCompletion();
    }
}
