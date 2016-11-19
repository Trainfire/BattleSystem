using UnityEngine;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    void Awake()
    {
        var battleSystem = new BattleSystem();

        foreach (var player in FindObjectsOfType<Player>())
        {
            battleSystem.RegisterPlayer(player);
        }

        var battleStates = gameObject.AddComponent<BattleStates>();
        battleStates.Initialize(battleSystem);
    }
}
