using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum BattleStateID
{
    Start,
    WaitingForInput,
    Executing,
    PostTurn,
    PostTurnExecute,
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
    private BattleStateStart _stateStart;
    private BattleStateInput _stateInput;
    private BattleStateExecute _stateExecute;
    private BattleStatePostTurn _statePostTurn;
    private BattleStatePostTurnExecute _statePostTurnExecute;

    public BattleSystem System { get { return _system; } }
    public BattleQueue Queue { get { return _queue; } }
    public int TurnCount { get; private set; }
    public BattleStateID State { get { return _state.ID; } }

    T RegisterState<T>() where T : BattleState
    {
        var instance = Activator.CreateInstance<T>();
        instance.Ended += OnStateEnd;
        return instance;
    }

    public void Initialize(BattleSystem system, BattleQueue queue, SetupParams parameters)
    {
        _system = system;
        _queue = queue;

        _stateStart = RegisterState<BattleStateStart>();
        _stateInput = RegisterState<BattleStateInput>();
        _stateExecute = RegisterState<BattleStateExecute>();
        _statePostTurn = RegisterState<BattleStatePostTurn>();
        _statePostTurnExecute = RegisterState<BattleStatePostTurnExecute>();

        // Register players into battle.
        parameters.Players.ForEach(x => system.RegisterPlayer(x.Player, x.FieldSide));

        SetState(BattleStateID.Start);
    }

    public void Continue()
    {
        _state.OnContinue();
    }

    public void AddTurn()
    {
        TurnCount++;
    }

    void OnStateEnd(BattleState endingState)
    {
        switch (endingState.ID)
        {
            case BattleStateID.Start: SetState(BattleStateID.WaitingForInput); break;
            case BattleStateID.WaitingForInput: SetState(BattleStateID.Executing); break;

            case BattleStateID.Executing:
                // TODO: Evaluate win condition.
                bool gameOver = false;

                if (gameOver)
                {
                    LogEx.Log<BattleCoordinator>("Game Over!");
                }
                else
                {
                    SetState(BattleStateID.PostTurn);
                }
                break;

            case BattleStateID.PostTurn: SetState(BattleStateID.PostTurnExecute); break;
            case BattleStateID.PostTurnExecute: SetState(BattleStateID.WaitingForInput); break;
        }
    }

    void SetState(BattleStateID stateID)
    {
        switch (stateID)
        {
            case BattleStateID.Start: _state = _stateStart; break;
            case BattleStateID.WaitingForInput: _state = _stateInput; break;
            case BattleStateID.Executing: _state = _stateExecute; break;
            case BattleStateID.PostTurn: _state = _statePostTurn; break;
            case BattleStateID.PostTurnExecute: _state = _statePostTurnExecute; break;
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

public class BattleStateStart : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.Start; } }

    public override void OnStart()
    {
        // Insert any battle initialization logic here.

        // Send in each player's first character.
        foreach (var player in Coordinator.System.Players)
        {
            Coordinator.Queue.RegisterPlayerCommand(SwitchCommand.Create(player, player.Party.Characters[0]));
        }

        Coordinator.Queue.Emptied += OnQueueEmpty;
        OnContinue();
    }

    public override void OnContinue()
    {
        base.OnContinue();
        Coordinator.Queue.Execute(ExecutionType.PostTurn);
    }

    void OnQueueEmpty(BattleQueue obj)
    {
        Coordinator.Queue.Emptied -= OnQueueEmpty;
        End();
    }
}

public class BattleStateInput : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.WaitingForInput; } }

    public override void OnStart()
    {
        Coordinator.System.Players.ForEach(x => x.ReadyStateChanged += OnPlayerReadyStateChanged);

        LogEx.Log<BattleCoordinator>("Waiting for {0} players.", Coordinator.System.Players.Count);

        if (Coordinator.AutoReadyPlayers)
            Coordinator.System.Players.ForEach(x => x.Attack());
    }

    void OnPlayerReadyStateChanged(Player player)
    {
        LogEx.Log<BattleCoordinator>("{0} ready state changed to: {1}", player.name, player.IsReady);

        if (Coordinator.System.Players.TrueForAll(x => x.IsReady))
            End();
    }

    public override void OnEnd()
    {
        Coordinator.System.Players.ForEach(player =>
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

        Coordinator.AddTurn();

        LogEx.Log<BattleStateExecute>("#--- Begin Turn " + (Coordinator.TurnCount) + " ---#");

        Coordinator.Queue.Emptied += OnCommandsDepleted;

        OnContinue();
    }

    public override void OnContinue()
    {
        base.OnContinue();
        Coordinator.Queue.Execute(ExecutionType.Normal);
    }

    void OnCommandsDepleted(BattleQueue queue)
    {
        LogEx.Log<BattleStateExecute>("#--- End Turn " + (Coordinator.TurnCount) + " ---#");

        queue.Emptied -= OnCommandsDepleted;
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
        _replacingPlayers.AddRange(Coordinator.System.Players
            .Where(x => x.ActiveCharacter.ActiveState == ActiveState.Fainted)
            .ToList());

        if (_replacingPlayers.Count != 0)
        {
            LogEx.Log<BattleStatePostTurn>("Waiting for {0} players to choose a replacement...", _replacingPlayers.Count);
            _replacingPlayers.ForEach(x => x.ReplacementCharacterSelected += OnPlayerReplacementCharacterSelected);
        }
        else
        {
            End();
        }
    }

    void OnPlayerReplacementCharacterSelected(ReplaceCommand replaceCommand)
    {
        Coordinator.System.RegisterPlayerCommand(replaceCommand);

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
            End();
        }
    }
}

public class BattleStatePostTurnExecute : BattleState
{
    public override BattleStateID ID { get { return BattleStateID.PostTurnExecute; } }

    public override void OnStart()
    {
        base.OnStart();

        Coordinator.Queue.Emptied += OnCommandsDepleted;

        OnContinue();
    }

    public override void OnContinue()
    {
        base.OnContinue();

        Coordinator.Queue.Execute(ExecutionType.PostTurn);
    }

    void OnCommandsDepleted(BattleQueue queue)
    {
        queue.Emptied -= OnCommandsDepleted;
        End();
    }
}
