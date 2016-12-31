using UnityEngine;

public class Attack : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        // Evaluate pre attack conditions.
        ConditionResult result = null;
        foreach (var condition in Source.GetComponents<Condition>())
        {
            result = condition.Evaluate();

            if (result.Type == ConditionResultType.Removed)
            {
                //Source.Status.RemoveCondition(result.Condition.Type);
                result.Condition.Remove();

                // TODO: Do this later?
                Destroy(result.Condition);
            }
            else
            {
                // Show OnEvaluate message.
                if (result.Parameters.OnEvaluateMessage != string.Empty)
                    battleSystem.Log(BattleLogger.Format(result.Parameters.OnEvaluateMessage, Source, Source));

                if (result.Type == ConditionResultType.Failed)
                    break;
            }
        }

        if (result != null && result.Type == ConditionResultType.Failed)
        {
            // Show OnFail message.
            if (result.Parameters.OnFailMessage != string.Empty)
                battleSystem.Log(BattleLogger.Format(result.Parameters.OnFailMessage, Source, Source));

            // Instantiate OnFailAction and register action.
            if (result.Parameters.OnFailAction != null)
            {
                var action = Instantiate(result.Parameters.OnFailAction);
                action.SetReciever(Source);
                battleSystem.RegisterAction(action);
            }
        }
        else
        {
            battleSystem.Log(Source.name + " used {0}!", name.Replace("(Clone)", ""));

            // Relay to other attached components.
            Relay(battleSystem);
        }

        TriggerCompletion();
    }
}
