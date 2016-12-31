using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BattleCoordinator))]
public class BattleCoordinatorEditor : Editor
{
    private bool _manualSteppingWasOn;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var coordinator = target as BattleCoordinator;

        if (coordinator.EnableManualStepping)
        {
            if (GUILayout.Button("Continue"))
                coordinator.Continue();
        }
        else if (_manualSteppingWasOn)
        {
            coordinator.Continue();
        }

        _manualSteppingWasOn = coordinator.EnableManualStepping;
    }
}
