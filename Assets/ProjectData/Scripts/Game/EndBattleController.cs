using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBattleController : IOnEventCallback
{
    private EndBattleView _endBattleView;
    private PlayerInfoView _playerInfoView;
    private string _playerID;

    public EndBattleController(GameData gameData, LoadedPlayersInfo playerInfo)
    {
        _endBattleView = gameData.EndBattleView;
        _playerInfoView = gameData.PlayerView;
        _playerID = playerInfo.PlayerID;
    }

    public void CheckForRemainingShips()
    {
        if(_playerInfoView.ShipsCount == 0)
        {
            SendEndBattleEvent();
            _endBattleView.SetLoseScreen();

        }
    }

    public void SendEndBattleEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.EndBattle, _playerID, options, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.EndBattle:

                if (_playerID != (string)photonEvent.CustomData)
                {
                    _endBattleView.SetWinScreen();
                }
                break;
        }
    }
}
