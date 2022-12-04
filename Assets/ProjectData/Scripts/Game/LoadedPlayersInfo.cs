using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedPlayersInfo
{
    public string PlayerID { get; private set; }
    public string PlayerName { get; private set; }
    public string PlayerCharacterName { get; private set; }
    public string OpponentID { get; private set; }
    public string OpponentName { get; private set; }
    public string OpponentCharacterName { get; private set; }

    public int PlayerExp { get; private set; }
    public int PlayerLVL { get; private set; }
    public int OpponentExp { get; private set; }
    public int OpponentLVL { get; private set; }

    public Sprite PlayerCharacterSprite { get; private set; }
    public Sprite OpponentCharacterSprite { get; private set; }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
    }

    public void SetPlayerID(string id)
    {
        PlayerID = id;
    }

    public void SetPlayerCharacterInfo(string name, int lvl, int exp, Sprite sprite)
    {
        PlayerCharacterName = name;
        PlayerExp = exp;
        PlayerLVL = lvl;
        PlayerCharacterSprite = sprite;
    }
    public void SetOpponentName(string name)
    {
        OpponentName = name;
    }

    public void SetOpponentID(string id)
    {
        OpponentID = id;
    }

    public void SetOpponentCharacterInfo(string name, int lvl, int exp, Sprite sprite)
    {
        OpponentCharacterName = name;
        OpponentExp = exp;
        OpponentLVL = lvl;
        OpponentCharacterSprite = sprite;
    }
}
