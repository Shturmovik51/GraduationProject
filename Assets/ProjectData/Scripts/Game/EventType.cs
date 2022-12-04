using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventType
{
    None = 0,
    CellClick = 1,
    ReadyForRoll = 2,
    StartRolling = 3,
    SendRollData = 4,
    SyncFields = 5,
    StartSync = 6,
    StartBattle = 7,
    DestroyShip = 8,
    ChangeTurn = 9,
    SetShipOnField = 10,
    EndBattle = 11,
    OpponentDisconnected = 12,
    CharacterInfo = 13,
}
