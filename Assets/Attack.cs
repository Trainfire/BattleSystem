public class Attack : TargetedAction
{
    protected override void OnExecute()
    {
        BattleSystem.Log(Target.name + " used Flail!");
        BattleSystem.RegisterAction(() => Target.GetComponent<Health>().ChangeHealth(Source, -10));
        TriggerCompletion();
        Destroy(gameObject);
    }
}
