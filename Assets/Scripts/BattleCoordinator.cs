using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum BattleStateID
{
    WaitingForInput,
    Executing,
    PostTurn,
}

public class BattleCoordinator : MonoBehaviour
{
    [SerializeField] private bool _autoReadyPlayers;
    [SerializeField] private bool _enableManualStepping;

    public bool AutoReadyPlayers { get { return _autoReadyPlayers; } }
    public bool EnableManualStepping { get { return _enableManualStepping; } set { _enableManualStepping = true; } }

    private BattleSystem _system;
    private BattleQueue _queue;
    private BattleState _state;
    private BattleStateInput _stateInput;
    private BattleStateExecute _stateExecute;
    private BattleStatePostTurn _statePostTurn;

    public BattleSystem System { get { return _system; } }
    public BattleQueue Queue { get { return _queue; } }

    T RegisterState<T>() where T : BattleState
    {
        var instance = Activator.CreateInstance<T>();
        instance.Ended += OnStateEnd;
        return instance;
    }

    public void Initialize(BattleSystem system, BattleQueue queue)
    {
        _system = system;
        _queue = queue;

        _stateInput = RegisterState<BattleStateInput>();
        _stateExecute = RegisterState<BattleStateExecute>();
        _statePostTurn = RegisterState<BattleStatePostTurn>();

        SetState(BattleStateID.WaitingForInput);
    }

    public void Continue()
    {
        _state.OnContinue();
    }

    void OnStateEnd(BattleState state)
    {
        switch (state.ID)
        {
            case BattleStateID.WaitingForInput: SetState(BattleStateID.Executing); break;

            case BattleStateID.Executing:
                // TODO: Evaluate win condition.
                bool gameOver = false;

                if (gameOver)
                {
                    
                }
                else
                {
                    SetState(BattleStateID.PostTurn);
                }
                break;

            case BattleStateID.PostTurn: SetState(BattleStateID.WaitingForInput); break;
        }
    }

    void SetState(BattleStateID stateID)
    {
        switch (stateID)
        {
            case BattleStateID.WaitingForInput: _state = _stateInput; break;
            case BattleStateID.Executing: _state = _stateExecute; break;
            case BattleStateID.PostTurn: _state = _statePostTurn; break;
        }

        LogEx.Log<BattleCoordinator>("State is now: " + _state.ID);

        _state.Start(this);
    }

    void LateUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space) && EnableManualStepping)
            _state.OnContinue();
    }
}

public abstract class BattleState
{
    public event Action<BattleState> Ended;

    public abstract BattleStateID ID { get; }
    protected BattleCoordinator Coordinator { get; private set; }

    public void Start(BattleCoordinator coordinator)
    {
        Coordinator = coordinator;
        OnStart();
    }

    public void End()
    {
        OnEnd();

        if (Ended != null)
            Ended.Invoke(this);
    }

    public virtual void OnStart() { }
    public virtual void OnContinue() { }
    public virtual void OnEnd() { }
}

public class BattleStateInput : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.WaitingForInput; } }

    public override void OnStart()
    {
        Coordinator.System.Registry.Players.ForEach(x => x.ReadyStateChanged += OnPlayerReadyStateChanged);

        LogEx.Log<BattleCoordinator>("Waiting for {0} players.", Coordinator.System.Registry.Players.Count);

        if (Coordinator.AutoReadyPlayers)
            Coordinator.System.Registry.Players.ForEach(x => x.Attack());
    }

    void OnPlayerReadyStateChanged(Player player)
    {
        LogEx.Log<BattleCoordinator>("{0} ready state changed to: {1}", player.name, player.IsReady);

        if (Coordinator.System.Registry.Players.TrueForAll(x => x.IsReady))
            End();
    }

    public override void OnEnd()
    {
        Coordinator.System.Registry.Players.ForEach(player =>
        {
            player.ReadyStateChanged -= OnPlayerReadyStateChanged;
            player.ResetReady();
        });
    }
}

public class BattleStateExecute : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.Executing; } }

    public override void OnStart()
    {
        base.OnStart();

        //LogEx.Log<BattleController>("#-- Begin Turn " + (Controller.TurnCount + 1) + " ---#");

        Coordinator.Queue.Empty += OnCommandsDepleted;

        OnContinue();
    }

    public override void OnContinue()
    {
        base.OnContinue();
        Coordinator.Queue.Execute(ExecutionType.Normal);
    }

    void OnCommandsDepleted(BattleQueue queue)
    {
        //LogEx.Log<BattleController>("#-- End Turn " + (Controller.TurnCount + 1) + " ---#");

        queue.Empty -= OnCommandsDepleted;
        End();
    }
}

public class BattleStatePostTurn : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.PostTurn; } }

    private List<Player> _replacingPlayers;

    public override void OnStart()
    {
        base.OnStart();

        // Get all players whose character has fainted.
        _replacingPlayers = new List<Player>();
        _replacingPlayers.AddRange(Coordinator.System.Registry.Players
            .Where(x => x.ActiveCharacter.ActiveState == ActiveState.Fainted)
            .ToList());

        if (_replacingPlayers.Count != 0)
        {
            _replacingPlayers.ForEach(x => x.ReplacementCharacterSelected += OnPlayerReplacementCharacterSelected);
        }
        else
        {
            End();
        }
    }

    void OnPlayerReplacementCharacterSelected(ReplaceCommand replaceCommand)
    {
        Coordinator.System.Registry.RegisterPlayerCommand(replaceCommand);

        if (_replacingPlayers.Contains(replaceCommand.Player))
        {
            _replacingPlayers.Remove(replaceCommand.Player);
        }
        else
        {
            LogEx.LogError<BattleStatePostTurn>("Player {0} cannot be removed as they were never registered.", replaceCommand.Player.name);
        }

        if (_replacingPlayers.Count == 0)
        {
            LogEx.Log<BattleStatePostTurn>("All players have chosen their replacements. Executing...");
            Coordinator.Queue.Empty += OnCommandsDepleted;
            Coordinator.Continue();
        }
    }

    void OnCommandsDepleted(BattleQueue obj)
    {
        LogEx.Log<BattleStatePostTurn>("All players did a thing.");
        //End();
    }

    //void Execute()
    //{
    //    var next = _replacementActions.Dequeue();
    //    next.Completed += OnActionCompleted;
    //    next.Execute(BattleSystem);
    //}

    //void OnActionCompleted(BaseAction action)
    //{
    //    if (_replacementActions.Count == 0)
    //        End();
    //}
}
