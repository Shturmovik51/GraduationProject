using Engine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ShipsManager : IOnEventCallback, ICleanable, IController
{
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsRight;
    private List<Ship> _playerShips;
    private List<Ship> _opponentShips;
    private List<Ship> _allShips;
    private PlayerInfoView _playerView;
    private PlayerInfoView _opponentView;
    private string _playerID;
    private Action<ShipType> _onDestroyShipAction;
    private Action<bool> _onSetShipOnFieldAction;
    public List<Ship> PlayerShips => _playerShips;
    public List<Ship> OpponentShips => _opponentShips;

    public ShipsManager(GameData gameData, LoadedPlayerInfo playerinfo, List<FieldCell> masterCellsRight, 
            List<FieldCell> opponentCellsRight, EndBattleController endBattleController)
    {
        _playerShips = gameData.PlayerShipsHolder.GetComponentsInChildren<Ship>().ToList();
        _opponentShips = gameData.OpponentShipsHolder.GetComponentsInChildren<Ship>().ToList();
        _masterCellsRight = masterCellsRight;
        _opponentCellsRight = opponentCellsRight;
        _playerView = gameData.PlayerView;
        _opponentView = gameData.OpponentView;

        _allShips = new List<Ship>();
        _allShips.AddRange(_playerShips);
        _allShips.AddRange(_opponentShips);

        _playerID = playerinfo.PlayerID;

        _onDestroyShipAction = (_) => 
            {
                _playerView.ChangeShipsCount(ChangeCountType.Remove);
                endBattleController.CheckForRemainingShips();
            };
        _onSetShipOnFieldAction = (isSet) => _playerView.ChangeShipsCount(isSet? ChangeCountType.Add : ChangeCountType.Remove);

        foreach (var ship in _allShips)
        {
            ship.InitOwner(_playerID);
            ship.OnDestroyShip += _onDestroyShipAction;
            ship.OnSetShipOnField += _onSetShipOnFieldAction;
            PhotonNetwork.AddCallbackTarget(ship);
        }       
    }

    public void SetAllShipsLocked()
    {
        foreach (var ship in _allShips)
        {
            ship.SetShipIsLocked(true);
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

                        var rotation = (Quaternion)resultShipDestroyData[3];
                                              
                        ship.transform.position = _masterCellsRight[cellIndex].transform.position;
                        ship.transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
                        ship.SetShipIsLocked(true);
                        ship.SetShipIsDestroyed(true);
                        ship.SetExplosionRadius();

                        Debug.Log(rotation);
                    }

                    if (!PhotonNetwork.IsMasterClient)
                    {
                        var shipType = (ShipType)resultShipDestroyData[1];
                        var ship = _playerShips.Find(ship => ship.ShipType == shipType && !ship.IsDestroyed);

                        var cellIndex = (int)resultShipDestroyData[2];                        

                        var rotation = (Quaternion)resultShipDestroyData[3];
                       
                        ship.transform.position = _opponentCellsRight[cellIndex].transform.position;
                        ship.transform.rotation = rotation * Quaternion.Euler(0, 180, 0);
                        ship.SetShipIsLocked(true);
                        ship.SetShipIsDestroyed(true);
                        ship.SetExplosionRadius();

                        Debug.Log(rotation);
                    }

                    _opponentView.ChangeShipsCount(ChangeCountType.Remove);
                }

                break;

            case EventType.SetShipOnField:

                object[] resultSetShipData = (object[])photonEvent.CustomData;

                if (_playerID != (string)resultSetShipData[0])
                {
                    var isSet = (bool)resultSetShipData[1];
                    _opponentView.ChangeShipsCount(isSet? ChangeCountType.Add : ChangeCountType.Remove);
                }
                
                break;
        }
    }

    public void CleanUp()
    {
        foreach (var ship in _allShips)
        {
            ship.OnDestroyShip -= _onDestroyShipAction;
            ship.OnSetShipOnField -= _onSetShipOnFieldAction;
        }
    }
}


// сделать окончание игры при count = 0
// протащить чарактера до игровой сцены