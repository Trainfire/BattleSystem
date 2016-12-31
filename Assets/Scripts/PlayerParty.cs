using UnityEngine;
using System.Collections.Generic;

public class PlayerParty : MonoBehaviour
{
    [SerializeField]
    private List<Character> _characters;
    public List<Character> Characters { get { return _characters; } }

    public Character InBattle { get; private set; }

    void Awake()
    {
        if (_characters == null)
            _characters = new List<Character>();

        //// TEMP!!!
        //InBattle = _characters[0];
        //InBattle.SwitchIn();

        _characters.ForEach(x => x.SwitchedIn += OnCharacterSwitchedIn);
    }

    void OnCharacterSwitchedIn(Character character)
    {
        InBattle = character;
    }
}
