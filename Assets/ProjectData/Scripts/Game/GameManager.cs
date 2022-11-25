using Engine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
{   
    [SerializeField] private GameData _gameData;

    private List<FieldCell> _masterCellsLeft;
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsLeft;
    private List<FieldCell> _opponentCellsRight;

    private ControllersManager _controllersManager;
    private PlayerFieldView _playerFieldView;

    private LoadedPlayerInfo _playerInfo;

    private bool _isInitialised;

    private void Awake()
    {     
        if (PhotonNetwork.IsMasterClient)
        {
            var playerObject = Instantiate(_gameData.UserViewField, _gameData.MasterTransform);
            _playerFieldView = playerObject.GetComponent<PlayerFieldView>();
        }
        else
        {
            var playerObject = Instantiate(_gameData.UserViewField, _gameData.OpponentTransform);
            _playerFieldView = playerObject.GetComponent<PlayerFieldView>();
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
            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { }, OnGetInfo, OnError);
        }
        else
        {
            InitCells();
            InitGame();
            _isInitialised = true;
        }

        void OnGetInfo(GetAccountInfoResult result)
        {
            _playerInfo = new LoadedPlayerInfo();

            _playerInfo.SetPlayerID(result.AccountInfo.PlayFabId);
            _playerInfo.SetPlayerName(result.AccountInfo.Username);

            //PhotonNetwork.AuthValues = new AuthenticationValues(UserID);
            PhotonNetwork.NickName = result.AccountInfo.Username;
            //PhotonNetwork.AutomaticallySyncScene = true;
            InitCells();
            InitGame();

            _isInitialised = true;
        }

        void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }
    }   

    private void InitGame()
    {
        _controllersManager = new ControllersManager();
        new GameInitializator(_controllersManager, _playerFieldView, _masterCellsLeft, _masterCellsRight,
                _opponentCellsLeft, _opponentCellsRight, _playerInfo, _gameData);

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
        for (int i = 0; i < _masterCellsLeft.Count; i++)
        {
            _masterCellsLeft[i].InitCell(i, _playerInfo.PlayerID);
            _masterCellsRight[i].InitCell(i, _playerInfo.PlayerID);
            _opponentCellsLeft[i].InitCell(i, _playerInfo.PlayerID);
            _opponentCellsRight[i].InitCell(i, _playerInfo.PlayerID);

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

                object[] result = (object[])photonEvent.CustomData;

                if (_playerInfo.PlayerID != (string)result[1])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _masterCellsLeft[(int)result[0]].InitAction();
                    }
                    else if(!PhotonNetwork.IsMasterClient)
                    {
                        _opponentCellsLeft[(int)result[0]].InitAction();
                    }
                }
                else if(_playerInfo.PlayerID == (string)result[1])
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        _opponentCellsLeft[(int)result[0]].InitAction();
                    }
                    else if (!PhotonNetwork.IsMasterClient)
                    {
                        _masterCellsLeft[(int)result[0]].InitAction();
                    }
                }
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

}
