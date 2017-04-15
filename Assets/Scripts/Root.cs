using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour
{
    public Battle BattlePrototype;

    public void Start()
    {
        if (BattlePrototype != null)
        {
            var setupParams = new SetupParams();
            var players = FindObjectsOfType<Player>();

            // Register each player to a side.
            for (int i = 0; i < players.Length; i++)
            {
                int sideID = i % 2;
                setupParams.AddPlayer(players[i], sideID);
            }

            var battleInstance = Instantiate(BattlePrototype);
            battleInstance.Initialize(setupParams);
        }
        else
        {
            LogEx.LogError<Root>("The Battle reference is missing or null. Make sure that the Battle field is referencing a prefab.");
        }
    }
}
