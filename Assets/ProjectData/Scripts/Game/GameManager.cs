using Engine;
using ExitGames.Client.Photon;
using Photon.Pun;
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
    public string UserID { get; private set; }
    public string PlayerName { get; private set; }

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
        
        _controllersManager = new ControllersManager();
        new GameInitializator(_controllersManager, _playerFieldView);
        _controllersManager.Initialization();

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { }, OnGetInfo, OnError);

        void OnGetInfo(GetAccountInfoResult result)
        {
            UserID = result.AccountInfo.PlayFabId;
            PlayerName = result.AccountInfo.Username;

            //PhotonNetwork.AuthValues = new AuthenticationValues(UserID);
            PhotonNetwork.NickName = PlayerName;
            //PhotonNetwork.AutomaticallySyncScene = true;

            InitCells();
        }

        void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }

    }   

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        _controllersManager.LocalUpdate(deltaTime);
    }

    private void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        _controllersManager.LocalLateUpdate(deltaTime);
    }

    private void FixedUpdate()
    {
        var fixedDeltaTime = Time.fixedDeltaTime;
        _controllersManager.LocalFixedUpdate(fixedDeltaTime);
    }

    private void OnGUI()
    {
        _controllersManager.LocalOnGUI();
    }

    private void OnDestroy()
    {
        _controllersManager.CleanUp();
    }

    private void InitCells()
    {
        for (int i = 0; i < _masterCellsLeft.Count; i++)
        {
            _masterCellsLeft[i].InitCell(i, UserID);
            _masterCellsRight[i].InitCell(i, UserID);
            _opponentCellsLeft[i].InitCell(i, UserID);
            _opponentCellsRight[i].InitCell(i, UserID);

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

                if (UserID != (string)result[1])
                {
                    if (PhotonNetwork.IsMasterClient)
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
