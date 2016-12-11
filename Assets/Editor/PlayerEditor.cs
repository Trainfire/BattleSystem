using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Character))]
public class PlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Ready"))
            (target as Character).ToggleReady();
    }
}
