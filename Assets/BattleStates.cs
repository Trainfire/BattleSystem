using UnityEngine;
using System;
using System.Collections.Generic;

public enum BattleStateID
{
    WaitingForInput,
    Executing,
}

public class BattleStates : MonoBehaviour
{
    private BattleSystem _battleSystem;
    private BattleState _state;
    private BattleStateInput _stateInput;
    private BattleStateExecute _stateExecute;

    T RegisterState<T>() where T : BattleState
    {
        var instance = Activator.CreateInstance<T>();
        instance.Ended += OnStateEnd;
        return instance;
    }

    public void Initialize(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;

        _stateInput = RegisterState<BattleStateInput>();
        _stateExecute = RegisterState<BattleStateExecute>();

        SetState(BattleStateID.WaitingForInput);
    }

    void OnStateEnd(BattleState state)
    {
        if (state.ID == BattleStateID.WaitingForInput)
        {
            SetState(BattleStateID.Executing);
        }
        else
        {
            // TODO: Evaluate win condition.
            SetState(BattleStateID.WaitingForInput);
        }
    }

    void SetState(BattleStateID stateID)
    {
        switch (stateID)
        {
            case BattleStateID.WaitingForInput: _state = _stateInput; break;
            case BattleStateID.Executing: _state = _stateExecute; break;
        }

        LogEx.Log<BattleStates>("State is now: " + _state.ID);

        _state.Start(_battleSystem);
    }
}

public abstract class BattleState
{
    public event Action<BattleState> Ended;

    public abstract BattleStateID ID { get; }
    public BattleSystem BattleSystem { get; private set; }

    public void Start(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
        OnStart();
    }

    public void End()
    {
        OnEnd();

        if (Ended != null)
            Ended.Invoke(this);
    }

    public virtual void OnStart() { }
    public virtual void OnEnd() { }
}

public class BattleStateInput : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.WaitingForInput; } }

    public override void OnStart()
    {
        BattleSystem.Players.ForEach(x => x.ReadyStateChanged += OnPlayerReadyStateChanged);

        LogEx.Log<BattleStates>("Waiting for {0} players.", BattleSystem.Players.Count);
    }

    void OnPlayerReadyStateChanged(Player player)
    {
        LogEx.Log<BattleStates>("{0} ready state changed to: {1}", player.name, player.IsReady);

        if (BattleSystem.Players.TrueForAll(x => x.IsReady))
            End();
    }

    public override void OnEnd()
    {
        LogEx.Log<BattleStates>("#-- Begin Turn " + (BattleSystem.TurnCount + 1) + " ---#");

        BattleSystem.Players.ForEach(player =>
        {
            player.ReadyStateChanged -= OnPlayerReadyStateChanged;
            player.ResetReady();

            // TEMP.
            var attack = GameObject.Instantiate(player.Attack);
            attack.Initialize(BattleSystem, player, player);

            BattleSystem.RegisterPlayerCommand(attack);
        });
    }
}

public class BattleStateExecute : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.Executing; } }

    public override void OnStart()
    {
        base.OnStart();
        BattleSystem.CommandsDepleted += OnCommandsDepleted;
        BattleSystem.Execute();
    }

    void OnCommandsDepleted(BattleSystem battleSystem)
    {
        battleSystem.CommandsDepleted -= OnCommandsDepleted;
        End();
    }
}

