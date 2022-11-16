using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomMiniView : MonoBehaviourPunCallbacks, IPointerClickHandler
{
    public event Action<RoomMiniView> OnClickRoomMiniView;
    public bool IsSelected { get; private set; }
    public RoomInfo RoomInfo { get; private set; }

    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _roomOwner;
    [SerializeField] private Image _backGroundImage;
    [SerializeField] private Color _selectedColor;

    private Color _idleColor;

    public void InitRoomMiniView(RoomInfo roomInfo)
    {
        RoomInfo = roomInfo;
        _roomName.text = roomInfo.Name;
        _idleColor = _backGroundImage.color;
        //var ownerID = roomInfo.masterClientId; 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickRoomMiniView?.Invoke(this);
        Debug.Log("OnPointerClick");
    }

    public void SelectView()
    {
        IsSelected = true;
        _backGroundImage.color = _selectedColor;
    }

    public void DeselectView()
    {
        IsSelected = false;        
        _backGroundImage.color = _idleColor;
    }

    public void ClearRoomMiniView()
    {
        RoomInfo = null;
        _roomName.text = "";
    }
}
