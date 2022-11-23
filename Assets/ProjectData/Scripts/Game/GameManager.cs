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
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform _masterTransform;
    [SerializeField] private Transform _opponentTransform;

    [SerializeField] private GameObject _masterCellsHolderLeft;
    [SerializeField] private GameObject _masterCellsHolderRight;
    [SerializeField] private GameObject _opponentCellsHolderLeft;
    [SerializeField] private GameObject _opponentCellsHolderRight;

    private List<FieldCell> _masterCellsLeft;
    private List<FieldCell> _masterCellsRight;
    private List<FieldCell> _opponentCellsLeft;
    private List<FieldCell> _opponentCellsRight;

    private ControllersManager _controllersManager;

    public string UserID { get; private set; }
    public string PlayerName { get; private set; }

    private void Awake()
    {     
        if (PhotonNetwork.IsMasterClient)
        {
            Instantiate(_playerPrefab, _masterTransform);
        }
        else
        {
            Instantiate(_playerPrefab, _opponentTransform);
        }
    }  

    private void Start()
    {    
        _masterCellsLeft = _masterCellsHolderLeft.GetComponentsInChildren<FieldCell>().ToList();
        _masterCellsRight = _masterCellsHolderRight.GetComponentsInChildren<FieldCell>().ToList();
        _opponentCellsLeft = _opponentCellsHolderLeft.GetComponentsInChildren<FieldCell>().ToList();
        _opponentCellsRight = _opponentCellsHolderRight.GetComponentsInChildren<FieldCell>().ToList();
        
        _controllersManager = new ControllersManager();
        new GameInitializator(_controllersManager);
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
