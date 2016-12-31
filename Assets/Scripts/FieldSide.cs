using UnityEngine;
using System.Collections.Generic;

public class FieldSide : MonoBehaviour
{
    public int ID { get; private set; }
    public int MaxSlots { get; private set; }
    public List<FieldSlot> Slots { get; private set; }

    public void Initialize(int id, int maxSlots)
    {
        ID = id;
        MaxSlots = maxSlots;

        Slots = new List<FieldSlot>();
        for (int i = 0; i < maxSlots; i++)
        {
            var slot = gameObject.AddComponent<FieldSlot>();
            Slots.Add(slot);
        }
    }

    public bool Register(Character character)
    {
        // TODO: Assign character to first non-empty slot. Return false if no slots available.
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].Character == null)
            {
                Slots[i].Assign(character);
                return true;
            }
        }

        return false;
    }
}
