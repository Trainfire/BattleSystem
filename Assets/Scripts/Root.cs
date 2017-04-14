using UnityEngine;
using System.Collections;

public class Root : MonoBehaviour
{
    public void Start()
    {
        var battle = GetComponentInChildren<Battle>();

        if (battle != null)
        {
            var setupParams = new SetupParams();
            var players = FindObjectsOfType<Player>();

            // Register each player to a side.
            for (int i = 0; i < players.Length; i++)
            {
                int sideID = i % 2;
                setupParams.AddPlayer(players[i], sideID);
            }

            battle.Initialize(setupParams);
        }
        else
        {
            LogEx.LogError<Root>("Failed to find Battle as child component of Root. Make sure there is a child GameObject with a Battle component attached.");
        }
    }
}
