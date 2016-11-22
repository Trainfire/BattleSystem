using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BattleSystem))]
public class BattleSystemEditor : Editor
{
    private bool _manualSteppingWasOn;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var battleSystem = target as BattleSystem;

        if (battleSystem.EnableManualStepping)
        {
            if (GUILayout.Button("Continue"))
                battleSystem.Continue();
        }
        else if (_manualSteppingWasOn)
        {
            battleSystem.Continue();
        }

        _manualSteppingWasOn = battleSystem.EnableManualStepping;
    }
}
