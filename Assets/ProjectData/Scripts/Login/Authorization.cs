using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Authorization : MonoBehaviourPunCallbacks, System.IDisposable
{
    [SerializeField] private string _playFabTitle;
    [SerializeField] private ConnectionView _connectionView;

    private string _customID;

    void Start()
    {
        _connectionView.Init();

        _connectionView.SetOfflineConnectionStatus();

        _connectionView.ConnectButton.interactable = false;
        _connectionView.DisconnectButton.interactable = false;

        _connectionView.LoginButton.onClick.AddListener(Login);
        _connectionView.ConnectButton.onClick.AddListener(Connect);
        _connectionView.DisconnectButton.onClick.AddListener(Disconnect);
    }

    private void Login()
    {
        if (string.IsNullOrEmpty(_connectionView.GetLogin()))
        {
            _connectionView.SetLoginWarning(true);
            return;
        }
        else
        {
            _customID = _connectionView.GetLogin();
        }

        if(string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = _customID,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log(result.PlayFabId);
                PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
                PhotonNetwork.NickName = result.PlayFabId;
                                
                _connectionView.ConnectButton.interactable = true;
                _connectionView.DisconnectButton.interactable = true;

                //Connect();
            },
            error =>
            {
                Debug.LogError(error);
            });
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = PhotonNetwork.AppVersion;
        }        
    }

    private void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{Random.Range(0, 9999)}");

        _connectionView.SetOnlineConnectionStatus();
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");
        _connectionView.SetOfflineConnectionStatus();
    }

    public void Dispose()
    {
        _connectionView.Clear();

        _connectionView.LoginButton.onClick.RemoveAllListeners();
        _connectionView.ConnectButton.onClick.RemoveAllListeners();
        _connectionView.DisconnectButton.onClick.RemoveAllListeners();
    }
}
