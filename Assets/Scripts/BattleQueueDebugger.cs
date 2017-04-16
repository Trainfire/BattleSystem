using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BattleQueue))]
class BattleQueueDebugger : MonoBehaviour
{
    private BattleQueue _battleQueue;
    private GUIStyle _fontStyle;

    void Awake()
    {
        _battleQueue = GetComponent<BattleQueue>();

        if (_battleQueue == null)
            Debug.LogError("BattleQueue is missing!", gameObject);

        _fontStyle = new GUIStyle();
        _fontStyle.fontSize = 10;

        var styleState = new GUIStyleState();
        styleState.textColor = Color.white;

        _fontStyle.normal = styleState;
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        if (_battleQueue.Empty)
        {
            GUILayout.Label("Queue is empty.", _fontStyle);
        }
        else
        {
            DrawQueue(_battleQueue.HealthUpdates);
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
}