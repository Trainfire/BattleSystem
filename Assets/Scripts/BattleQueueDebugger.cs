using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BattleQueue), typeof(BattleSystem), typeof(BattleCoordinator))]
class BattleQueueDebugger : MonoBehaviour
{
    private BattleSystem _battleSystem;
    private BattleQueue _battleQueue;
    private BattleCoordinator _battleCoordinator;

    private Queue<string> _logs;
    private GUIStyle _fontStyle;

    void Awake()
    {
        _battleSystem = GetComponent<BattleSystem>();
        _battleQueue = GetComponent<BattleQueue>();
        _battleCoordinator = GetComponent<BattleCoordinator>();

        if (_battleSystem == null)
            Debug.LogError("BattleSystem is missing!", gameObject);

        if (_battleQueue == null)
            Debug.LogError("BattleQueue is missing!", gameObject);

        if (_battleCoordinator == null)
            Debug.LogError("BattleCoordinator is missing!", gameObject);

        if (_battleSystem != null)
            _battleSystem.LogPosted += OnLogPosted;

        _logs = new Queue<string>();

        // Setup style
        _fontStyle = new GUIStyle();
        _fontStyle.fontSize = 10;

        var normal = new GUIStyleState();
        normal.textColor = Color.white;

        _fontStyle.normal = normal;
    }

    void OnLogPosted(string log)
    {
        const int maxLogs = 10;

        while (_logs.Count > maxLogs)
        {
            _logs.Dequeue();
        }

        _logs.Enqueue(log);
    }

    void OnGUI()
    {
        DrawQueues();
        DrawPlayers();
        DrawLog();
        DrawMisc();
    }

    void DrawQueues()
    {
        GUILayout.BeginVertical();

        GUILayout.Label("<b>Queued Actions</b>");

        if (_battleQueue.Empty)
        {
            GUILayout.Label("Queue is empty.", _fontStyle);
        }
        else
        {
            DrawQueue(_battleQueue.GenericUpdates);
            DrawQueue(_battleQueue.PlayerCommands);
            DrawQueue(_battleQueue.StatusUpdates);
            DrawQueue(_battleQueue.WeatherUpdates);
        }

        GUILayout.EndVertical();
    }

    void DrawQueue(List<BaseAction> actions)
    {
        foreach (var action in actions)
        {
            string log = string.Format("{0} ({1}) ({2})", action.name, action.GetType().ToString(), action.LogInfo);
            GUILayout.Label(log, _fontStyle);
        }

        GUILayout.Space(5);
    }

    void DrawPlayers()
    {
        const float width = 200f;
        GUILayout.BeginArea(new Rect(Screen.width - width, 0f, width, Screen.height));

        GUILayout.Label("<b>Players</b>");

        GUILayout.BeginVertical();

        foreach (var player in _battleSystem.Players)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(player.name, _fontStyle, GUILayout.Width(100f));
            GUILayout.Label(player.ActiveCharacter == null ? "N/A" : player.ActiveCharacter.name, _fontStyle, GUILayout.Width(75f));
            GUILayout.Label(player.ActiveCharacter == null ? "N/A" : player.ActiveCharacter.Health.Current.ToString() + "HP", _fontStyle, GUILayout.Width(25f));

            GUILayout.EndHorizontal();
        }

        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    void DrawLog()
    {
        const float height = 150f;

        GUILayout.BeginArea(new Rect(0f, Screen.height - height, 200f, height));

        GUILayout.Label("<b>Battlelog</b>");

        GUILayout.BeginVertical();

        foreach (var log in _logs)
        {
            GUILayout.Label(log, _fontStyle);
        }

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    void DrawMisc()
    {
        const float width = 200f;
        const float height = 150f;

        GUILayout.BeginArea(new Rect(Screen.width - width, Screen.height - height, width, height));

        GUILayout.Label("<b>Misc</b>");

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Battle State: " + _battleCoordinator.State, _fontStyle);
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.EndArea();
    }
}