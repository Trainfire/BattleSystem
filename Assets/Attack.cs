public class Attack : TargetedAction
{
    protected override void OnExecute()
    {
        BattleSystem.Log(Source.name + " used Flail!");

        // BattleSystem.RegisterAction(() => Target.GetComponent<Health>().ChangeHealth(Source, -10));
        foreach (var action in GetComponents<TargetedAction>())
        {
            if (action != this)
            {
                action.Initialize(BattleSystem, Source, Target);
                BattleSystem.RegisterAction(action);
            }
        }

        TriggerCompletion();

        Destroy(gameObject);
    }
}
