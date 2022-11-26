using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScreen : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _createNewButton;
    [SerializeField] private Button _joinSelectedRoomButton;
    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private GameObject _roomPanelPref;
    [SerializeField] private Canvas _lobbyCanvas;
    [SerializeField] private CreateRoomWindow _createRoomWindow;
    [SerializeField] private RoomWindow _roomWindow;

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

        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { }, OnGetInfo, OnError);

        void OnGetInfo(GetAccountInfoResult result)
        {
            _userID = result.AccountInfo.PlayFabId;
            _playerName = result.AccountInfo.Username;

            PhotonNetwork.AuthValues = new AuthenticationValues(_userID);
            PhotonNetwork.NickName = _playerName;
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }
        
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
        PhotonNetwork.JoinRoom(_currentSelectedRoom.RoomName);
        _roomWindow.OpenRoomWindow();
    }

    private void CheckJoinButtonVisibility()
    {
        _joinSelectedRoomButton.interactable = _currentSelectedRoom != null;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        for (int i = 0; i < roomList.Count; i++)        
        {
            var currentRoom = _rooms.Find(room => room.RoomName == roomList[i].Name);
            if (currentRoom != null)
            {
                currentRoom.InitRoomMiniView(roomList[i], i);
                CheckRoomVisibility(roomList[i], currentRoom);
                CheckRoomPresence(roomList[i], currentRoom);

                continue;
            }

            var roomObject = Instantiate(_roomPanelPref, _scrollViewContent);
            var roomMiniView = roomObject.GetComponent<RoomMiniView>();
            roomMiniView.InitRoomMiniView(roomList[i], i);
            CheckRoomVisibility(roomList[i], roomMiniView);
            _rooms.Add(roomMiniView);
             
            roomMiniView.OnClickRoomMiniView += SelectCurrentRoom;
        }        
    }

    private void CheckRoomVisibility(RoomInfo roomInfo, RoomMiniView room)
    {
        if (!roomInfo.IsVisible || !roomInfo.IsOpen)
        {
            if(room.gameObject.activeInHierarchy)
                room.gameObject.SetActive(false);
        }
        else
        {
            if (!room.gameObject.activeInHierarchy)
                room.gameObject.SetActive(true);
        }
    }
    private void CheckRoomPresence(RoomInfo roomInfo, RoomMiniView room)
    {
        if (roomInfo.RemovedFromList)
        {
            room.OnClickRoomMiniView -= SelectCurrentRoom;
            _rooms.Remove(room);    
            Destroy(room.gameObject);
        }
    }

    private void StartRoomCreationProcess()
    {
        _createRoomWindow.OpenCreateRoomWindow();
    }

    private void OnDestroy()
    {
        foreach (var room in _rooms)
        {
            room.OnClickRoomMiniView -= SelectCurrentRoom;
        }
    }
}
