using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using PlayFab.EconomyModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldCell : MonoBehaviour
{
    public Action OnCellClick;

    [SerializeField] private bool _isBattleFieldCell;
    [SerializeField] private GameObject _cellBody;
    [SerializeField] private GameObject _missMarker;
    [SerializeField] private GameObject _hitMarker;
    [SerializeField] private Transform _pointedBodyTransform;

    private Sequence _pointerEnterSequence;
    private Sequence _pointerExitSequence;
    private string _nameID;
    private string _user;
    private Transform _startBodyTransform;

    public bool IsBattleFieldCell => _isBattleFieldCell;
    public bool IsShipTarget { get; private set; }
    public bool IsActionable { get; private set; }
    public bool IsUsed { get; private set; }
    public int Arrayindex { get; private set; }

    private void Start()
    {
        _nameID = gameObject.name;
        _startBodyTransform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        _cellBody.SetActive(false);
    }

    private void OnTriggerExit(Collider other)
    {
        _cellBody.SetActive(true);
    }           

    public void InitAction()
    {
        SendClickEvent(EventType.CellClick, Arrayindex);

        OnCellClick?.Invoke();
        IsUsed = true;              

        if (!IsShipTarget)
        {
            _missMarker.SetActive(true);
        }
        else
        {
            _hitMarker.SetActive(true);
        }
    }
    
    public void SetAsShipTarget(bool isTarget)
    {
        IsShipTarget = isTarget;
    }

    private void SetPointedState()
    {
        if (!IsActionable) return;

        DOTween.Kill($"Down {_nameID}");
        _pointerEnterSequence = DOTween.Sequence();
        _pointerEnterSequence.SetId($"Up {_nameID}");
        _pointerEnterSequence.Append(gameObject.transform.DOMove(_pointedBodyTransform.position, 1));
    }   

    private void SetUnpointedState()
    {
        if (!IsActionable) return;

        DOTween.Kill($"Up {_nameID}");
        _pointerExitSequence = DOTween.Sequence();
        _pointerExitSequence.SetId($"Down {_nameID}");
        _pointerExitSequence.Append(gameObject.transform.DOMove(_startBodyTransform.position, 1));
    }

    public void InitCell(int index, string user)
    {
        Arrayindex = index;
        _user = user;
    }

    public void SendClickEvent(EventType eventType, int value, ReceiverGroup receiverGroup = ReceiverGroup.All)
    {        
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        //int send = value;
        object[] sendData = new object[]
        {
            value,
            _user
        };

        PhotonNetwork.RaiseEvent((byte)(int)eventType, sendData, options, sendOptions);        
    }
}
