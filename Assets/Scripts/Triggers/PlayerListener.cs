using UnityEngine;

public class PlayerListener : MonoBehaviour
{
    public BattleSystem BattleSystem { get; private set; }
    public Player Player { get; private set; }

    public void Initialize(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    public void SetPlayer(Player player)
    {
        Player = player;
        OnSetPlayer();
    }

    protected virtual void OnSetPlayer() { }
}
