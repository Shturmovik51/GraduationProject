using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbiScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _createNewButton;
    [SerializeField] private Button _joinSelectedRoomButton;
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private GameObject _roomPanelPref;
    [SerializeField] private Canvas _lobbyCanvas;
    [SerializeField] private CreateRoomWindow _createRoomWindow;

    private List<RoomMiniView> _rooms;
    private string _userID;
    private string _playerName;
    private RoomMiniView _currentSelectedRoom;

    private void Start()
    {
        _rooms = new List<RoomMiniView>();
        _createNewButton.onClick.AddListener(StartRoomCreationProcess);
        _joinSelectedRoomButton.onClick.AddListener(JointSpecificRoom);
    }

    public void OpenLobbiScreen()
    {
        _lobbyCanvas.enabled = true;

        PlayFabClientAPI.GetAccountInfo( new GetAccountInfoRequest { }, 
        request =>
        {
            _userID = request.AccountInfo.PlayFabId;
            _playerName = request.AccountInfo.Username;
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });

        PhotonNetwork.AuthValues = new AuthenticationValues(_userID);
        PhotonNetwork.NickName = _playerName;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        //PhotonNetwork.AutomaticallySyncScene = true;
        CheckJoinButtonVisibility();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();

        Debug.Log("OnConnectedToMaster");
    }


    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("OnJoinedLobby");
    }
       

    private void SelectCurrentRoom(RoomMiniView currentRoom)
    {        
        _currentSelectedRoom = currentRoom;

        foreach (var room in _rooms)
        {
            if(room == currentRoom)
            {
                if (!room.IsSelected)
                {
                    room.SelectView();                    
                    CheckJoinButtonVisibility();
                }
                else
                {
                    room.DeselectView();
                    _currentSelectedRoom = null;
                    CheckJoinButtonVisibility();
                }
            }
            else
            {
                if (room.IsSelected)
                {
                    room.DeselectView();
                }
            }
        }       
    }

    private void JointSpecificRoom()
    {
        PhotonNetwork.JoinRoom(_currentSelectedRoom.RoomInfo.Name);
    }

    private void CheckJoinButtonVisibility()
    {
        _joinSelectedRoomButton.interactable = _currentSelectedRoom != null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        foreach (var roomInfo in roomList)
        {
            if (_rooms.Find(room => room.RoomInfo == roomInfo))
            {
                continue;
            }

            var roomObject = Instantiate(_roomPanelPref, _scrollViewContent);
            var roomMiniView = roomObject.GetComponent<RoomMiniView>();
            roomMiniView.InitRoomMiniView(roomInfo);
            _rooms.Add(roomMiniView);
             
            roomMiniView.OnClickRoomMiniView += SelectCurrentRoom;
        }

        Debug.Log("OnRoomListUpdate");
    }

    private void StartRoomCreationProcess()
    {
        _createRoomWindow.OpenCreateRoomWindow();
    }    
}
