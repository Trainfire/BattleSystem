using UnityEngine;

public class Attack : TargetedAction
{
    protected override void OnExecute(BattleSystem battleSystem)
    {
        battleSystem.Log(Source.name + " used {0}!", name.Replace("(Clone)", ""));
        Relay(battleSystem);
        TriggerCompletion();
    }
}
