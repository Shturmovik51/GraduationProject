using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomWindow : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _changeAccessStateButton;
    [SerializeField] private Text _changeAccessStateButtonText;
    [SerializeField] private Button _changeVisibilityStateButton;
    [SerializeField] private Text _changeVisibilityStateButtonText;
    [SerializeField] private Button _StartGameButton;

    [SerializeField] private Transform _scrollViewContent;
    [SerializeField] private Canvas _roomVindowCanvas;

    [SerializeField] private TMP_Text _accessStatusText;
    [SerializeField] private TMP_Text _visibilityStatusText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private TMP_Text _roomOwnerText;

    [SerializeField] private AwaiterWindow _awaiterWindow;

    private const string CLIENT_NAME_KEY = "cn";
    private const string CLIENT_ID_KEY = "cid";

    //private Room _room;
    private Dictionary<Player, TextMeshProUGUI> _players;

    private void Start()
    {
        _players = new Dictionary<Player, TextMeshProUGUI>();
        _changeAccessStateButton.onClick.AddListener(ChangeRoomAccessStatus);
        _changeVisibilityStateButton.onClick.AddListener(ChangeRoomVisibilityStatus);
        _StartGameButton.onClick.AddListener(StartGame);
    }

    public void OpenRoomWindow()
    {    
        _roomVindowCanvas.enabled = true;
    }

    private void InitRoom()
    {
        var room = PhotonNetwork.CurrentRoom;
        _roomNameText.text = $"Room: {room.Name}";

        if (room.CustomProperties.TryGetValue("on", out object name))
        {
            _roomOwnerText.text = $"Owner: {name}";
        }  
    }

    private void ChangeRoomAccessStatus()
    {
        PhotonNetwork.CurrentRoom.IsOpen = !PhotonNetwork.CurrentRoom.IsOpen;    
    }

    private void ChangeRoomVisibilityStatus()
    {
        PhotonNetwork.CurrentRoom.IsVisible = !PhotonNetwork.CurrentRoom.IsVisible;
    }    

    private void OnDestroy()
    {
        _changeAccessStateButton.onClick.RemoveAllListeners();
        _changeVisibilityStateButton.onClick.RemoveAllListeners();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        var textObject = new GameObject("UserName");
        textObject.transform.SetParent(_scrollViewContent);
        var textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = newPlayer.NickName;

        _players.Add(newPlayer, textComponent);

        PhotonNetwork.CurrentRoom.CustomProperties.Add(CLIENT_NAME_KEY, newPlayer.NickName);
        PhotonNetwork.CurrentRoom.CustomProperties.Add(CLIENT_ID_KEY, newPlayer.UserId);

        _awaiterWindow.SetOpponentDetectionState();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();       
    }

    public override void OnJoinedRoom()    
    {
        base.OnJoinedRoom();

        if(PhotonNetwork.IsMasterClient  /*PhotonNetwork.LocalPlayer.UserId == PhotonNetwork.MasterClient.UserId*/)
        {
            _changeAccessStateButton.interactable = true;
            _changeVisibilityStateButton.interactable = true;
            _StartGameButton.interactable = true;
        }
        else
        {
            _changeAccessStateButton.interactable = false;
            _changeVisibilityStateButton.interactable = false;
            _StartGameButton.interactable = false;

            var textObject = new GameObject("UserName");
            textObject.transform.SetParent(_scrollViewContent);
            var textComponent = textObject.AddComponent<TextMeshProUGUI>();
            textComponent.text = PhotonNetwork.LocalPlayer.NickName;

            _awaiterWindow.SetOpponentDetectionState();
        }

        InitRoom();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);               

        if (propertiesThatChanged.ContainsKey(GamePropertyKey.IsOpen))
        {
            var accessValue = ((bool)propertiesThatChanged[GamePropertyKey.IsOpen]);
            _accessStatusText.text = (accessValue) ? "Open" : "Close";
            _changeAccessStateButtonText.text = (accessValue) ? "CLOSE ROOM" : "OPEN ROOM";
        }

        if (propertiesThatChanged.ContainsKey(GamePropertyKey.IsVisible))
        {
            var visibilityValue = ((bool)propertiesThatChanged[GamePropertyKey.IsVisible]);
            _visibilityStatusText.text = (visibilityValue) ? "Visible" : "Hidden";
            _changeVisibilityStateButtonText.text = (visibilityValue) ? "HIDE ROOM" : "UNHIDE ROOM";
        }
    }

    private void StartGame()
    {



        PhotonNetwork.LoadLevel(1);
    }    
}
