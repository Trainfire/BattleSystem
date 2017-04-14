using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SetupParams
{
    public class PlayerData
    {
        public Player Player { get; private set; }
        public int FieldSide { get; private set; }

        public PlayerData(Player player, int fieldSide)
        {
            Player = player;
            FieldSide = fieldSide;
        }
    }

    public List<PlayerData> Players { get; private set; }

    public SetupParams()
    {
        Players = new List<PlayerData>();
    }

    public void AddPlayer(Player player, int fieldSide)
    {
        if (Players.Any(x => x.Player == player))
        {
            LogEx.LogError<SetupParams>("Cannot add player as player {0} has already been registered.", player.name);
        }
        else
        {
            Players.Add(new PlayerData(player, fieldSide));
        }
    }
}
