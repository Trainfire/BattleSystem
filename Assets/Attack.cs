public class Attack : PlayerCommand
{
    protected override void OnExecute()
    {
        BattleSystem.Log(Target.name + " used Flail!");
        BattleSystem.RegisterAction(() => Target.GetComponent<Health>().ChangeHealth(-10));
        TriggerCompletion();
        Destroy(gameObject);
    }
}
