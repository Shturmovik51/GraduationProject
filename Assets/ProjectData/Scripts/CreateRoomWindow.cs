using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
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

    private bool _isVisibleOption;
    private bool _isOpenOption;

    private void Start()
    {
        _createRoomButton.onClick.AddListener(CreateRoom);

        _visibilityDropDown.ClearOptions();
        _visibilityDropDown.AddOptions(new List<string>() { "Visible", "Hide"});
        _visibilityDropDown.onValueChanged.AddListener((value) => SetVisibilityOption(value));

        _accessDropDown.ClearOptions();
        _accessDropDown.AddOptions(new List<string>() { "Opne", "Close" });
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

        var roomOptions = new RoomOptions
        {
            MaxPlayers = 12,
            
            //CustomRoomProperties = new Hashtable { { MONEY_PROP_KEY, 400 }, { MAP_PROP_KEY, "Map_3" } },
            //CustomRoomPropertiesForLobby = new[] { MONEY_PROP_KEY, MAP_PROP_KEY },
            IsVisible = _isVisibleOption,
            IsOpen = _isOpenOption,
            PublishUserId = true
        };

        PhotonNetwork.CreateRoom($"{_roomName.text}", roomOptions);

        _roomWindow.OpenRoomWindow();

        Debug.Log("CreateRoom");
    }
}
