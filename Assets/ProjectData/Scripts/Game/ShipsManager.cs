using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipsManager : IOnEventCallback
{
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsRight;
    private List<Ship> _masterShips;
    private List<Ship> _opponentShips;
    private string _playerID;
    public ShipsManager(GameData gameData, LoadedPlayerInfo playerinfo, List<FieldCell> masterCellsRight, 
            List<FieldCell> opponentCellsRight)
    {
        _masterShips = gameData.MasterShips;
        _opponentShips = gameData.OpponentShips;
        _masterCellsRight = masterCellsRight;
        _opponentCellsRight = opponentCellsRight;

        _playerID = playerinfo.PlayerID;

        foreach (var ship in _masterShips)
        {
            ship.InitOwner(_playerID);
            PhotonNetwork.AddCallbackTarget(ship);
            Debug.Log(_playerID);
        }

        foreach (var ship in _opponentShips)
        {
            ship.InitOwner(_playerID);
            PhotonNetwork.AddCallbackTarget(ship);
            Debug.Log(_playerID);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.DestroyShip:

                object[] resultShipDestroyData = (object[])photonEvent.CustomData;

                if (_playerID != (string)resultShipDestroyData[0])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        var shipType = (ShipType)resultShipDestroyData[1];
                        var ship = _opponentShips.Find(ship => ship.ShipType == shipType && !ship.IsDestroyed);

                        var cellIndex = (int)resultShipDestroyData[2];
                        //var rotation = new Vector3((float)resultShipDestroyData[3],
                        //                           (float)resultShipDestroyData[4],
                        //                           (float)resultShipDestroyData[5]);

                        var rotation = (Quaternion)resultShipDestroyData[3];

                        //ship.transform.parent = null;
                        ship.transform.position = _masterCellsRight[cellIndex].transform.position;
                        ship.transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
                        ship.SetShipIsLocked(true);
                        ship.SetShipIsDestroyed(true);
                        ship.SetExplosionRadius();

                        Debug.Log("SetOnMaster");
                        Debug.Log(rotation);
                    }

                    if (!PhotonNetwork.IsMasterClient)
                    {
                        var shipType = (ShipType)resultShipDestroyData[1];
                        var ship = _masterShips.Find(ship => ship.ShipType == shipType && !ship.IsDestroyed);

                        var cellIndex = (int)resultShipDestroyData[2];
                        //var rotation = new Vector3((float)resultShipDestroyData[3],
                        //                           (float)resultShipDestroyData[4],
                        //                           (float)resultShipDestroyData[5]);

                        var rotation = (Quaternion)resultShipDestroyData[3];

                        //ship.transform.parent = null;
                        ship.transform.position = _opponentCellsRight[cellIndex].transform.position;
                        ship.transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
                        ship.SetShipIsLocked(true);
                        ship.SetShipIsDestroyed(true);
                        ship.SetExplosionRadius();

                        Debug.Log("SetOnClient");
                        Debug.Log(rotation);
                    }
                }

                break;
        }
    }
}