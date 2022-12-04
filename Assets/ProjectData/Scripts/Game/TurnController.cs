using DG.Tweening;
using Engine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using System.Collections.Generic;

public class TurnController : IOnEventCallback, ICleanable, IController
{
    private List<FieldCell> _masterCellsLeft;
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsLeft;
    private List<FieldCell> _opponentCellsRight;
    private ActionsView _actionsView;
    private PlayerInfoView _playerInfoView;
    private PlayerInfoView _opponentInfoView;
    private ShipsManager _shipsManager;
    private MouseRaycaster _mouseRaycaster;
    private string _playerName;
    private string _playerID;
    private bool _isReadyForRoll;
    private bool _isRolling;
    private bool _isCompleteRolling;

    public TurnController(List<FieldCell> masterCellsLeft, List<FieldCell> masterCellsRight, 
            List<FieldCell> opponentCellsLeft, List<FieldCell> opponentCellsRight, GameData gameData,
                LoadedPlayersInfo playerinfo, ShipsManager shipsManager, MouseRaycaster mouseRaycaster)
    {
        _masterCellsLeft = masterCellsLeft;
        _masterCellsRight = masterCellsRight;
        _opponentCellsLeft = opponentCellsLeft;
        _opponentCellsRight = opponentCellsRight;

        _actionsView = gameData.ActionsView;

        _playerInfoView = gameData.PlayerView;
        _opponentInfoView = gameData.OpponentView;

        _shipsManager = shipsManager;
        _mouseRaycaster = mouseRaycaster;

        if (playerinfo != null)
        {
            _playerName = playerinfo.PlayerName;
            _playerID = playerinfo.PlayerID;
        }

        _mouseRaycaster.OnMissShoot += SendChangeTurnEvent;

        StartPlacementStage();
    }

    private void SendReadinessEvent()
    {
        _isReadyForRoll = true;
        _actionsView.SetWaitingForRollStage();
        _shipsManager.SetAllShipsLocked();

        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };                

        PhotonNetwork.RaiseEvent((byte)(int)EventType.ReadyForRoll, _playerID, options, sendOptions);
    }

    private void SendStartRollingEvent()
    {  
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };        

        PhotonNetwork.RaiseEvent((byte)(int)EventType.StartRolling, _playerID, options, sendOptions);
    }

    private void SendStartSynchronizingFieldsEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.StartSync, _playerID, options, sendOptions);
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

        PhotonNetwork.RaiseEvent((byte)(int)EventType.SyncFields, eventContent, options, sendOptions);
    }

    private void SendChangeTurnEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.ChangeTurn, _playerID, options, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.ReadyForRoll:

                if (_playerID != (string)photonEvent.CustomData)
                {
                    _actionsView.SetOpponentActionText("Ready For Rolling");                    

                    if (_isReadyForRoll)
                    {
                        SendStartSynchronizingFieldsEvent();
                    }
                }
                break;

            case EventType.StartSync:

                AwaitForSynchronizationFields();

                break;

            case EventType.StartRolling:

                StartRollingStage();

                break;

            case EventType.SendRollData:

                object[] resultSendRollData = (object[])photonEvent.CustomData;

                if (_playerID != (string)resultSendRollData[0])                    
                {
                    _actionsView.SetOpponentRollValue((int)resultSendRollData[1]);
                    _opponentInfoView.SetInfoTextVisibility(false);

                    if (_isCompleteRolling)
                    {
                        AwaitForBattleStage();
                    }
                }

                break;

            case EventType.SyncFields:

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

            case EventType.ChangeTurn:

                var playerID = (string)photonEvent.CustomData;

                if (_playerID != playerID)
                {
                    _mouseRaycaster.SetUnableToHitCell(true);
                    _actionsView.RefreshBattleStageUI(true);
                }
                else if(_playerID == playerID)
                {
                    _actionsView.RefreshBattleStageUI(false);
                }

                break;


            default:
                break;
        }
    }

    private void StartPlacementStage()
    {
        _actionsView.SetPlacementStage(SendReadinessEvent);
    }
   
    private void AwaitForSynchronizationFields()
    {
        var awaitSequence = DOTween.Sequence();
        awaitSequence.AppendCallback(SinchronizeFields);
        awaitSequence.AppendInterval(1);
        awaitSequence.OnComplete(SendStartRollingEvent);
    }

    private void StartRollingStage()
    {        
        _actionsView.SetRollStage(RollingSystem);
    }

    private void AwaitForBattleStage()
    {
        var awaitSequence = DOTween.Sequence();
        awaitSequence.AppendInterval(2);
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
                var (value, index) = _actionsView.GetRandomDiceValue();               

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
        _actionsView.SetSynchronizationStage();

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
        else
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
        var rollResult = _actionsView.PlayerRolledValue > _actionsView.OpponentRolledValue;             //todo need rerol
        _actionsView.SetBattleStage(rollResult);
        _opponentInfoView.SetInfoTextVisibility(true);
        _mouseRaycaster.SetUnableToHitCell(rollResult);
    }

    public void CleanUp()
    {
        _mouseRaycaster.OnMissShoot -= SendChangeTurnEvent;
        _actionsView.ClearSubscribes();
    }
}
