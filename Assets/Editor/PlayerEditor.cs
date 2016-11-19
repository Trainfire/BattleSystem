using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Ready"))
            (target as Player).ToggleReady();
    }
}
