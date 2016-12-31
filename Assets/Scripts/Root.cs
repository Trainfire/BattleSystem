using UnityEngine;
using System.Collections.Generic;

public class Root : MonoBehaviour
{
    void Start()
    {
        var battleSystem = gameObject.GetComponent<BattleSystem>();

        foreach (var player in FindObjectsOfType<Player>())
        {
            battleSystem.Registry.RegisterPlayer(player);
        }

        var queue = gameObject.GetComponent<BattleQueue>();
        queue.Initialize(battleSystem);

        var coordinator = gameObject.GetComponent<BattleCoordinator>();
        coordinator.Initialize(battleSystem, queue);
    }
}
