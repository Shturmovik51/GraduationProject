using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class CreateRoomWindow : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField _roomName;
    [SerializeField] private TMP_Dropdown _visibilityDropDown;
    [SerializeField] private TMP_Dropdown _accessDropDown;
    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Canvas _createRoomCanvas;
    [SerializeField] private RoomWindow _roomWindow;

    private const string OWNER_NAME_KEY = "on";
    private const string OWNER_ID_KEY = "oid";
    private const string OWNER_CHARACTER_NAME_KEY = "ocn";
    private const string CLIENT_NAME_KEY = "cn";
    private const string CLIENT_ID_KEY = "cid";
    private const string CLIENT_CHARACTER_NAME_KEY = "ccn";
    private const string FRIEND_1_KEY = "f1";
    private const string FRIEND_2_KEY = "f2";
    private const string FRIEND_3_KEY = "f3";
    private const string FRIEND_4_KEY = "f4";

    private bool _isVisibleOption;
    private bool _isOpenOption;

    private void Start()
    {
        _createRoomButton.onClick.AddListener(CreateRoom);

        _visibilityDropDown.ClearOptions();
        _visibilityDropDown.AddOptions(new List<string>() { "Visible", "Hide"});
        _visibilityDropDown.onValueChanged.AddListener((value) => SetVisibilityOption(value));

        _accessDropDown.ClearOptions();
        _accessDropDown.AddOptions(new List<string>() { "Open", "Close" });
        _accessDropDown.onValueChanged.AddListener((value) => SetAccessOption(value));

        SetVisibilityOption(_visibilityDropDown.value);
        SetAccessOption(_accessDropDown.value);
    }

    public void OpenCreateRoomWindow()
    {
        _createRoomCanvas.enabled = true;  
    }

    private void SetVisibilityOption(int value)
    {
        switch (value)
        {
            case 0: _isVisibleOption = true;
                break;
            case 1: _isVisibleOption = false;
                break;

            default:
                break;
        }
    }

    private void SetAccessOption(int value)
    {
        switch (value)
        {
            case 0:
                _isOpenOption = true;
                break;
            case 1:
                _isOpenOption = false;
                break;

            default:
                break;
        }
    }

    private void CreateRoom()
    {
        if (_roomName.text.IsNullOrEmpty())
        {
            return;
        }

        
        //PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        //{
        //    PlayFabId = myPlayFabId,
        //    Keys = null
        //}, result => {
        //    Debug.Log("Got user data:");
        //    if (result.Data == null || !result.Data.ContainsKey("Ancestor")) Debug.Log("No Ancestor");
        //    else Debug.Log("Ancestor: " + result.Data["Ancestor"].Value);
        //}, (error) => {
        //    Debug.Log("Got error retrieving user data:");
        //    Debug.Log(error.GenerateErrorReport());
        //});
        



        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { }, OnGetInfo, OnError);

        void OnGetInfo(GetAccountInfoResult result)
        {
            var ownerName = result.AccountInfo.Username;
            var ownerID = result.AccountInfo.PlayFabId;

            var roomOptions = new RoomOptions
            {
                MaxPlayers = 2,

                CustomRoomProperties = new Hashtable
                {
                    { OWNER_NAME_KEY, ownerName },
                    { OWNER_ID_KEY, ownerID }
                },

                CustomRoomPropertiesForLobby = new[] { OWNER_NAME_KEY, OWNER_ID_KEY },

                IsVisible = _isVisibleOption,
                IsOpen = _isOpenOption,
                PublishUserId = true
            };

            PhotonNetwork.CreateRoom($"{_roomName.text}", roomOptions);

            _roomWindow.OpenRoomWindow();

            Debug.Log("CreateRoom");
        }

        void OnError(PlayFabError error)
        {
            Debug.Log(error.GenerateErrorReport());
        }        
    }
}
