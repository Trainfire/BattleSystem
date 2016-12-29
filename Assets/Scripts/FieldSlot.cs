using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

public class FieldSlot : MonoBehaviour
{
    public Character Character { get; private set; }

    public void SetCharacter(Character character)
    {
        Character = character;
    }
}
