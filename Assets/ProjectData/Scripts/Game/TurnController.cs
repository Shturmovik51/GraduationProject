using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
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
    private bool _isRolling;
    private bool _isCompleteRolling;

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

    private void SendRollEvent(int diceIndex)
    {
        _isCompleteRolling = true;

        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] data = new object[]
        {
            _playerID,
            diceIndex,
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.SendRollData, data, options, sendOptions);
    }

    private void SendStartBattleEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.StartBattle, _playerID, options, sendOptions);
    }
    private void SendSincFieldsEvent(int intData)
    {
        _isCompleteRolling = true;

        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] eventContent = new object[]
        {
            _playerID,
            intData,
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.SincFields, eventContent, options, sendOptions);
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
                        SendStartRollingEvent();
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

                    if (_isCompleteRolling)
                    {
                        AwaitForBattleStage();
                    }
                }

                break;

            case EventType.SincFields:

                object[] resultSincData = (object[])photonEvent.CustomData;

                if (_playerID != (string)resultSincData[0])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _masterCellsRight[(int)resultSincData[1]].SetAsShipTarget(true);
                    }
                    else if(!PhotonNetwork.IsMasterClient)
                    {
                        _opponentCellsRight[(int)resultSincData[1]].SetAsShipTarget(true);
                    }
                }

                break;

            case EventType.StartBattle:
                
                StartBattleStage();                

                break;

            default:
                break;
        }
    }

    private void StartPlacementStage()
    {
        _playerView.SetPlacementStage(SendReadinessEvent);
    }
   
    private void StartRollingStage()
    {        
        _playerView.SetRollStage(RollingSystem);
    }

    private void AwaitForBattleStage()
    {
        var awaitSequence = DOTween.Sequence();
        awaitSequence.AppendInterval(2);
        awaitSequence.AppendCallback(SinchronizeFields);
        awaitSequence.AppendInterval(1);
        awaitSequence.OnComplete(SendStartBattleEvent);
    }   

    private void RollingSystem()
    {
        _isRolling = true;

        var timer = DOTween.Sequence();
        timer.AppendInterval(4);
        timer.OnComplete(() => _isRolling = false);

        GetRandomValue();

        void GetRandomValue()
        {
            var roll = DOTween.Sequence();
            roll.AppendInterval(0.2f);
            roll.OnComplete(() =>
            {     
                var (value, index) = _playerView.GetRandomDiceValue();               

                if (_isRolling)
                {
                    GetRandomValue();
                }
                else
                {
                    SendRollEvent(index);
                }
            });
        }
    }

    private void SinchronizeFields()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < _masterCellsLeft.Count; i++)
            {
                if (_masterCellsLeft[i].IsShipTarget)
                {
                    SendSincFieldsEvent(i);
                }
            }
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < _opponentCellsLeft.Count; i++)
            {
                if (_opponentCellsLeft[i].IsShipTarget)
                {
                    SendSincFieldsEvent(i);
                }
            }
        }
    }

    private void StartBattleStage()
    {
        _playerView.SetBattleStage(_playerView.PlayerRolledValue > _playerView.OpponentRolledValue);
    }
}
