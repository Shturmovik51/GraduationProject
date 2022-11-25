using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class TurnController : IOnEventCallback
{
    private List<FieldCell> _masterCellsLeft;
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsLeft;
    private List<FieldCell> _opponentCellsRight;
    private PlayerView _playerView;
    private string _playerName;
    private string _playerID;
    private bool _isReadyForRoll;

    public TurnController(List<FieldCell> masterCellsLeft, List<FieldCell> masterCellsRight, 
            List<FieldCell> opponentCellsLeft, List<FieldCell> opponentCellsRight, GameData gameData,
                LoadedPlayerInfo playerinfo)
    {
        _masterCellsLeft = masterCellsLeft;
        _masterCellsRight = masterCellsRight;
        _opponentCellsLeft = opponentCellsLeft;
        _opponentCellsRight = opponentCellsRight;
        _playerView = gameData.PlayerView;

        if(playerinfo != null)
        {
            _playerName = playerinfo.PlayerName;
            _playerID = playerinfo.PlayerID;
        }

        StartPlacementStage();

    }

    private void SendReadinessEvent()
    {
        _isReadyForRoll = true;
        _playerView.SetWaitingForRollStage();

        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };                

        PhotonNetwork.RaiseEvent((byte)(int)EventType.Readiness, _playerID, options, sendOptions);
    }

    private void SendStartRollingEvent()
    {  
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };        

        PhotonNetwork.RaiseEvent((byte)(int)EventType.StartRolling, _playerID, options, sendOptions);
    }

    private void SendRollValueEvent(int value)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] data = new object[]
        {
            _playerID,
            value,
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.SendRollData, data, options, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.Readiness:

                if (_playerID != (string)photonEvent.CustomData)
                {
                    _playerView.SetOpponentActionText("Ready For Rolling");

                    if (_isReadyForRoll)
                    {
                        Df();
                    }
                }
                break;

            case EventType.StartRolling:

                StartRollingStage();

                break;

            case EventType.SendRollData:

                object[] resultSendRollData = (object[])photonEvent.CustomData;

                if (_playerID != (string)resultSendRollData[0])                    
                {
                    _playerView.SetOpponentRollValue((int)resultSendRollData[1]);
                }

                break;
        }
    }

    private void StartPlacementStage()
    {
        _playerView.SetPlacementStage(SendReadinessEvent);
    }

    private void Df()
    {
        SendStartRollingEvent();
    }

    private void StartRollingStage()
    {
        //SendStartRollingEvent();
        _playerView.SetRollStage(RollingSystem);
    }

    private void RollingSystem()
    {
        SendRollValueEvent(15);
    }


}
