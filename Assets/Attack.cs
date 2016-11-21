public class Attack : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        battleSystem.Log(Source.name + " used Flail!");
        Relay(battleSystem);
        TriggerCompletion();
        Destroy(gameObject);
    }
}
