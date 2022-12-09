using DG.Tweening;
using Engine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{   
    [SerializeField] private GameData _gameData;

    private List<FieldCell> _masterCellsLeft;
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsLeft;
    private List<FieldCell> _opponentCellsRight;

    private ControllersManager _controllersManager;
    private PlayerFieldView _playerFieldView;

    private LoadedPlayersInfo _playersInfo;

    private bool _isInitialised;

    private void Awake()
    {
        _playersInfo = new LoadedPlayersInfo();

        if (PhotonNetwork.IsMasterClient)
        {
            var playerObject = Instantiate(_gameData.UserViewField, _gameData.MasterTransform);
            _playerFieldView = playerObject.GetComponent<PlayerFieldView>();

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("on", out object userName))
            {
                _playersInfo.SetPlayerName(userName.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("oid", out object userID))
            {
                _playersInfo.SetPlayerID(userID.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("cn", out object clientName))
            {
                _playersInfo.SetOpponentName(clientName.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("cid", out object clientID))
            {
                _playersInfo.SetOpponentID(clientID.ToString());
            }
        }
        else
        {
            var playerObject = Instantiate(_gameData.UserViewField, _gameData.OpponentTransform);
            _playerFieldView = playerObject.GetComponent<PlayerFieldView>();

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("on", out object userName))
            {
                _playersInfo.SetOpponentName(userName.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("oid", out object userID))
            {
                _playersInfo.SetOpponentID(userID.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("cn", out object clientName))
            {
                _playersInfo.SetPlayerName(clientName.ToString());
            }
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("cid", out object clientID))
            {
                _playersInfo.SetPlayerID(clientID.ToString());
            }
        }        
    }  

    private void Start()
    {   
        _masterCellsLeft = _gameData.MasterCellsHolderLeft.GetComponentsInChildren<FieldCell>().ToList();
        _masterCellsRight = _gameData.MasterCellsHolderRight.GetComponentsInChildren<FieldCell>().ToList();
        _opponentCellsLeft = _gameData.OpponentCellsHolderLeft.GetComponentsInChildren<FieldCell>().ToList();
        _opponentCellsRight = _gameData.OpponentCellsHolderRight.GetComponentsInChildren<FieldCell>().ToList();

        if (PhotonNetwork.IsConnected)
        {      
            PhotonNetwork.NickName = _playersInfo.PlayerName;
            PhotonNetwork.AutomaticallySyncScene = true;
            //PhotonNetwork.AuthValues = new AuthenticationValues(UserID);

            GetUserData(_playersInfo.PlayerID);
        }
        else
        {
            InitCells();
            InitGame();
            _isInitialised = true;
        }    
    }

    private void GetUserData(string myPlayFabId)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myPlayFabId
        }, result => 
        {
            Debug.Log(myPlayFabId);

            var characterID =  result.Data["chid"].Value;
            var characterName = result.Data["chn"].Value;            

            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest()
            {
                CharacterId = characterID
            }, OnGetCharactersSuccess, OnError);

            void OnGetCharactersSuccess(GetCharacterStatisticsResult result)
            {
                var level = 0;
                var experience = 0;
                var spriteIndex = 0;

                if (int.TryParse(result.CharacterStatistics["LVL"].ToString(), out var lvlResult))
                {
                    level = lvlResult;
                }
                if (int.TryParse(result.CharacterStatistics["EXP"].ToString(), out var expResult))
                {
                    experience = expResult;
                }
                if (int.TryParse(result.CharacterStatistics["AvatarID"].ToString(), out var indexResult))
                {
                    spriteIndex = indexResult;
                }
                                
                var sprite = _gameData.AvatarsConfig.GetAvatarByIndex(spriteIndex);
                
                _playersInfo.SetPlayerCharacterInfo(characterName, level, experience, sprite, characterID);
                _gameData.PlayerView.InitInfoView(characterName, level, experience, sprite);

                SendCharacterInfoEvent(level, experience, spriteIndex, characterName);

                StartInitialization();   
            }
        }, error => 
        {
            Debug.Log("Got error retrieving user data:");
            Debug.Log(error.GenerateErrorReport());
        });
    }    

    private void StartInitialization()
    {       
        InitCells();
        InitGame();
        _isInitialised = true;        
    }


    private void InitGame()
    {
        _controllersManager = new ControllersManager();
        new GameInitializator(_controllersManager, _playerFieldView, _masterCellsLeft, _masterCellsRight,
                _opponentCellsLeft, _opponentCellsRight, _playersInfo, _gameData);

        _controllersManager.Initialization();
    }

    private void Update()
    {
        if (!_isInitialised) return;

        var deltaTime = Time.deltaTime;
        _controllersManager.LocalUpdate(deltaTime);
    }

    private void LateUpdate()
    {
        if (!_isInitialised) return;

        var deltaTime = Time.deltaTime;
        _controllersManager.LocalLateUpdate(deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_isInitialised) return;

        var fixedDeltaTime = Time.fixedDeltaTime;
        _controllersManager.LocalFixedUpdate(fixedDeltaTime);
    }

    private void OnGUI()
    {
        if (!_isInitialised) return;

        _controllersManager.LocalOnGUI();
    }

    private void OnDestroy()
    {
        if (!_isInitialised) return;

        _controllersManager.CleanUp();
    }

    private void InitCells()
    {
        if (!PhotonNetwork.IsConnected)
            return;
        for (int i = 0; i < _masterCellsLeft.Count; i++)
        {
            _masterCellsLeft[i].InitCell(i, _playersInfo.PlayerID);
            _masterCellsRight[i].InitCell(i, _playersInfo.PlayerID);
            _opponentCellsLeft[i].InitCell(i, _playersInfo.PlayerID);
            _opponentCellsRight[i].InitCell(i, _playersInfo.PlayerID);

            PhotonNetwork.AddCallbackTarget(_masterCellsLeft[i]);
            PhotonNetwork.AddCallbackTarget(_masterCellsRight[i]);
            PhotonNetwork.AddCallbackTarget(_opponentCellsLeft[i]);
            PhotonNetwork.AddCallbackTarget(_opponentCellsRight[i]);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventType)photonEvent.Code)
        {
            case EventType.CellClick:

                object[] cellClickResult = (object[])photonEvent.CustomData;

                if (_playersInfo.PlayerID != (string)cellClickResult[1])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _masterCellsLeft[(int)cellClickResult[0]].InitAction();
                    }
                    else if(!PhotonNetwork.IsMasterClient)
                    {
                        _opponentCellsLeft[(int)cellClickResult[0]].InitAction();
                    }
                }
                else if(_playersInfo.PlayerID == (string)cellClickResult[1])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        //_opponentCellsLeft[(int)cellClickResult[0]].InitAction();
                    }
                    else if (!PhotonNetwork.IsMasterClient)
                    {
                        //_masterCellsLeft[(int)cellClickResult[0]].InitAction();
                    }
                }
                break;

            case EventType.CharacterInfo:

                object[] characterInfoResult = (object[])photonEvent.CustomData;

                if(_playersInfo.PlayerID != (string)characterInfoResult[4])
                {
                    var level = (int)characterInfoResult[0];
                    var experience = (int)characterInfoResult[1];               
                    var spriteIndex = (int)characterInfoResult[2];
                    var characterName = (string)characterInfoResult[3];

                    var sprite = _gameData.AvatarsConfig.GetAvatarByIndex(spriteIndex);

                    _playersInfo.SetOpponentCharacterInfo(characterName, level, experience, sprite);
                    _gameData.OpponentView.InitInfoView(characterName, level, experience, sprite);
                }

                break;
        }
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    public void SendCharacterInfoEvent(int level, int experience, int spriteIndex, string characterName)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] sendData = new object[]
        {
            level,
            experience,
            spriteIndex,
            characterName,
            _playersInfo.PlayerID
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.CharacterInfo, sendData, options, sendOptions);
    }
}
