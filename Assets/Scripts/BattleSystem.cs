using UnityEngine;
using System;
using Framework;

public enum ExecutionType
{
    Normal,
    PostTurn,
}

public class BattleSystem : MonoBehaviour
{
    public BattleRegistry Registry { get; private set; }
    public BattleHelper Helper { get; private set; }
    public BattleWeather Weather { get; private set; }

    private BattleCharacterHandler _characterHandler;

    public int TurnCount { get; private set; }

    void Awake()
    {
        Registry = gameObject.GetComponent<BattleRegistry>();
        Registry.PlayerAdded += OnPlayerAdded;

        _characterHandler = gameObject.GetComponent<BattleCharacterHandler>();
        _characterHandler.Initialize(this);

        Helper = gameObject.GetComponent<BattleHelper>();

        Weather = gameObject.GetComponent<BattleWeather>();
    }

    void OnPlayerAdded(Player player)
    {
        // TEMP!!!
        player.ActiveCharacter.BattleEntities.ForEach(x => x.Initialize(this));
    }

    public void Log(string message, params object[] args)
    {
        if (!string.IsNullOrEmpty(message))
            Registry.RegisterAction(() => LogEx.Log<BattleSystem>("Battle Log: " + message), "BattleLog");
    }

    //public void Execute(ExecutionType executionType)
    //{
    //    var next = _queue.Dequeue(_executionType);

    //    if (next != null)
    //    {
    //        next.Completed += OnActionExecutionComplete;
    //        next.Execute(this);
    //    }
    //    else
    //    {
    //        LogEx.Log<BattleSystem>("No more battle actions left to execute.");

    //        // TODO: Move Turn Count somewhere else.
    //        //TurnCount++;

    //        _queue.Reset();

    //        if (CommandsDepleted != null)
    //            CommandsDepleted.Invoke(this);
    //    }
    //}

    //void OnActionExecutionComplete(BaseAction action)
    //{
    //    action.Completed -= OnActionExecutionComplete;

    //    if (action.IsGarbage)
    //        Destroy(action.gameObject);
    //}
}
