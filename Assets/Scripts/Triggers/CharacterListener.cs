using UnityEngine;

public class CharacterListener : MonoBehaviour
{
    public BattleSystem BattleSystem { get; private set; }
    public Character Character { get; private set; }

    public void Initialize(BattleSystem battleSystem)
    {
        BattleSystem = battleSystem;
        OnInitialize();
    }

    protected virtual void OnInitialize() { }

    public void SetCharacter(Character character)
    {
        Character = character;
        OnSetCharacter();
    }

    protected virtual void OnSetCharacter() { }
}
