using UnityEngine;
using Framework;

public class Root : MonoBehaviour
{
    void Start()
    {
        var battleSystem = gameObject.GetOrAddComponent<BattleSystem>();

        var queue = gameObject.GetOrAddComponent<BattleQueue>();
        queue.Initialize(battleSystem);

        var handler = gameObject.GetOrAddComponent<BattleCharacterHandler>();
        handler.Initialize(battleSystem);

        var coordinator = gameObject.GetOrAddComponent<BattleCoordinator>();
        coordinator.Initialize(battleSystem, queue);
    }
}
