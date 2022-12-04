using Engine;
using ExitGames.Client.Photon;
using Newtonsoft.Json.Serialization;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.SceneManagement;

public class EndBattleController : IOnEventCallback, ICleanable, IController
{
    private EndBattleView _endBattleView;
    private PlayerInfoView _playerInfoView;
    private string _playerID;
    private LoadedPlayersInfo _loadedPlayersInfo;
    private const int BATTLE_REVARD = 300;
    private VoteType _playerVoteResult;
    private VoteType _opponentVoteResult;
    public EndBattleController(GameData gameData, LoadedPlayersInfo playerInfo)
    {
        _endBattleView = gameData.EndBattleView;
        _playerInfoView = gameData.PlayerView;
        _playerID = playerInfo.PlayerID;
        _loadedPlayersInfo = playerInfo;

        _playerVoteResult = VoteType.None;
        _opponentVoteResult = VoteType.None;

        _endBattleView.PlayerYesButton.onClick.AddListener(OnClickYesButton);
        _endBattleView.PlayerNoButton.onClick.AddListener(OnClickNoButton);
        _endBattleView.RestartButton.onClick.AddListener(RestartGame);
        _endBattleView.ExitButton.onClick.AddListener(OutGame);

        _endBattleView.RestartButton.interactable = false;
    }

    public void CheckForRemainingShips()
    {
        if(_playerInfoView.ShipsCount == 0)
        {
            _endBattleView.SetLoseScreen();
            SendEndBattleEvent();
        }
    }

    public void SendEndBattleEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.EndBattle, _playerID, options, sendOptions);
    }
    public void SendYesOrNoEvent(VoteType voteType)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] sendData = new object[]
        {
            _loadedPlayersInfo.PlayerID,
            (int)voteType
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.YesOrNo, sendData, options, sendOptions);
    }

    public void SendOutGameEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.LeaveTheGame, _playerID, options, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.EndBattle:

                if (_playerID != (string)photonEvent.CustomData)
                {
                    _endBattleView.SetWinScreen();
                    UpdateCharacterStatistics();
                }
                break;

            case EventType.YesOrNo:

                object[] yesOrNoResult = (object[])photonEvent.CustomData;

                if (_playerID != (string)yesOrNoResult[0])
                {
                    _endBattleView.SetOpponentVoteResult((VoteType)yesOrNoResult[1]);
                    _opponentVoteResult = (VoteType)yesOrNoResult[1];
                    CheckVoteResults();                          
                }
                break;

            case EventType.LeaveTheGame:

                if (_playerID != (string)photonEvent.CustomData)
                {
                    _endBattleView.RestartButton.interactable = false;
                    _endBattleView.SetOpponentLeftGameState();
                }
                break;
        }
    }

    private void UpdateCharacterStatistics()
    {
        var lvl = _loadedPlayersInfo.PlayerCharacterLVL;
        var exp = _loadedPlayersInfo.PlayerCharacterExp + BATTLE_REVARD;

        if(exp > 500)
        {
            lvl++;
            exp -= 500;
        }

        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = _loadedPlayersInfo.PlayerCharacterID,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"LVL", lvl },
                {"EXP", exp },
            }
        }, result =>
        {

        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnClickYesButton()
    {
        _endBattleView.SetPlayeVoteResult(VoteType.Yes);
        _playerVoteResult = VoteType.Yes;
        SendYesOrNoEvent(VoteType.Yes);
        CheckVoteResults();
    }

    private void OnClickNoButton()
    {
        _endBattleView.SetPlayeVoteResult(VoteType.No);
        _playerVoteResult = VoteType.No;
        SendYesOrNoEvent(VoteType.No);
        CheckVoteResults();
    }

    private void CheckVoteResults()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(_playerVoteResult != VoteType.None && _opponentVoteResult != VoteType.None)
        {
            if(_playerVoteResult == VoteType.No || _opponentVoteResult == VoteType.No)
            {
                _endBattleView.RestartButton.interactable = false;
            }
            else
            {
                _endBattleView.RestartButton.interactable = true;
            }
        }
    }

    private void OutGame()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        SendOutGameEvent();
        PlayerPrefs.SetInt("LoggedIn", 1);
        SceneManager.LoadScene(0);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void CleanUp()
    {
        
    }
}
