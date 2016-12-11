using UnityEngine;
using System.Collections.Generic;

public class PlayerParty : MonoBehaviour
{
    [SerializeField]
    private List<Character> _characters;
    public List<Character> Characters { get { return _characters; } }

    void Awake()
    {
        if (_characters == null)
            _characters = new List<Character>();
    }

    // TEMP!!!
    public Character InBattle { get { return _characters[0]; } }
}
