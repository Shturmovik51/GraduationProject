using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedPlayersInfo
{
    public string PlayerID { get; private set; }
    public string PlayerName { get; private set; }
    public string PlayerCharacterName { get; private set; }
    public string PlayerCharacterID {get; private set; }
    public string OpponentID { get; private set; }
    public string OpponentName { get; private set; }
    public string OpponentCharacterName { get; private set; }
    public string OpponentCharacterID { get; private set; }

    public int PlayerCharacterExp { get; private set; }
    public int PlayerCharacterLVL { get; private set; }
    public int OpponentCharacterExp { get; private set; }
    public int OpponentCharacterLVL { get; private set; }

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

    public void SetPlayerCharacterInfo(string name, int lvl, int exp, Sprite sprite, string id)
    {
        PlayerCharacterName = name;
        PlayerCharacterExp = exp;
        PlayerCharacterLVL = lvl;
        PlayerCharacterSprite = sprite;
        PlayerCharacterID = id;
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
        OpponentCharacterExp = exp;
        OpponentCharacterLVL = lvl;
        OpponentCharacterSprite = sprite;
    }
}
