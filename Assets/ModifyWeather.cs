using System;
using UnityEngine;

class ModifyWeather : TargetedAction
{
    [SerializeField] private Weather _weather;

    protected override void OnExecute(BattleSystem battleSystem)
    {
        battleSystem.Weather.Set(_weather);
        TriggerCompletion();
    }
}
