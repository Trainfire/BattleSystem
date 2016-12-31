using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

public class FieldSlot : MonoBehaviour
{
    public event Action<Character> CharacterAdded;
    public event Action<Character> CharacterRemoved;

    public Character Character { get; private set; }

    public void Assign(Character character)
    {
        LogEx.Log<FieldSlot>("Assigned character '{0}'", character.name);
        Character = character;
        Character.Fainted += OnCharacterFainted;
        Character.AssignSlot(this);
        CharacterAdded.InvokeSafe(Character);
    }

    void OnCharacterFainted(Character character)
    {
        Character.Fainted -= OnCharacterFainted;
        Clear();
    }

    public void Clear()
    {
        LogEx.Log<FieldSlot>("Slot is now empty.");
        CharacterRemoved.InvokeSafe(Character);
        Character = null;
    }
}
