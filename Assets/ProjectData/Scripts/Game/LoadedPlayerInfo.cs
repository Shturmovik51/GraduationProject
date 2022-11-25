using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedPlayerInfo
{
    public string PlayerID { get; private set; }
    public string PlayerName { get; private set; }

    public void SetPlayerID(string id)
    {
        PlayerID = id;
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }
}
