using Photon.Pun;
using Photon.Realtime;
using System.Collections;
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

    private Room _room;
    private Dictionary<Player, TMP_Text> _players;

    private void Start()
    {   
        _changeAccessStateButton.onClick.AddListener(ChangeRoomAccessStatus);
        _changeVisibilityStateButton.onClick.AddListener(ChangeRoomVisibilityStatus);
    }

    public void OpenRoomWindow()
    {    
        _roomVindowCanvas.enabled = true;
    }

    private void InitRoom()
    {
        _room = PhotonNetwork.CurrentRoom;
        _roomNameText.text = _room.Name;
        SetRoomAccessStatusText();
        SetRoomVisibilityStatusText();
    }

    private void ChangeRoomAccessStatus()
    {
        _room.IsOpen = !_room.IsOpen;
        SetRoomAccessStatusText();
    }

    private void ChangeRoomVisibilityStatus()
    {
        _room.IsVisible = !_room.IsVisible;
        SetRoomVisibilityStatusText();
    }

    private void SetRoomAccessStatusText()
    {
        _accessStatusText.text = _room.IsOpen ? "Open" : "Close";
        _changeAccessStateButtonText.text = _room.IsOpen ? "CLOSE ROOM" : "OPEN ROOM";
    }

    private void SetRoomVisibilityStatusText()
    {
        _visibilityStatusText.text = _room.IsVisible ? "Visible" : "Hidden";
        _changeVisibilityStateButtonText.text = _room.IsVisible ? "HIDE ROOM" : "UNHIDE ROOM";
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
        var textComponent = textObject.AddComponent<TMP_Text>();
        textComponent.text = newPlayer.NickName;

        _players.Add(newPlayer, textComponent);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        InitRoom();
    }
}
