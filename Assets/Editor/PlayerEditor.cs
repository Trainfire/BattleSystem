using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Player))]
public class PlayerEditor : Editor
{
    private Player _player;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _player = target as Player;

        if (!_player.IsReady)
        {
            if (GUILayout.Button("Attack"))
                _player.Attack();

            if (GUILayout.Button("Switch"))
                _player.Switch();
        }

        if (GUILayout.Button("Replace"))
            _player.SelectReplacement();
    }
}
