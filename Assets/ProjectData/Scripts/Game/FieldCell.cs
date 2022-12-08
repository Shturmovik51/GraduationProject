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
    public Action<FieldCell> OnCellClick;

    [SerializeField] private bool _isBattleFieldCell;
    [SerializeField] private MeshRenderer _cellBody;
    [SerializeField] private GameObject _missMarker;
    [SerializeField] private GameObject _hitMarker;
    [SerializeField] private Transform _pointedBodyTransform;
    [SerializeField] private AudioSource _missClickAudioSource;
    [SerializeField] private AudioSource _hitAudioSource;
    [SerializeField] private Collider _bodyCollider;
    [SerializeField] private Collider _markerCollider;

    [SerializeField] private Material _waterMaterial;
    [SerializeField] private Material _taggedMaterial;

    private Sequence _pointerEnterSequence;
    private Sequence _pointerExitSequence;
    private string _nameID;
    private string _user;
    private Transform _startBodyTransform;

    public bool IsBattleFieldCell => _isBattleFieldCell;
    [field: SerializeField] public bool IsShipTarget { get; private set; }
    public bool IsUnableToClick { get; private set; }
    public bool IsUsed { get; private set; }
    public int Arrayindex { get; private set; }

    private void Start()
    {
        _nameID = gameObject.name;
        _startBodyTransform = transform;
        IsUnableToClick = true;  // убрать
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsShipTarget)
        {
            _cellBody.material = _taggedMaterial;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsShipTarget)
        {
            _cellBody.material = _waterMaterial;
        }
    }  

    public void InitAction()
    {
        OnCellClick?.Invoke(this);
        IsUsed = true;

        if (IsBattleFieldCell)
        {
            if (!IsShipTarget)
            {
                _missMarker.SetActive(true);
                _missClickAudioSource.Play();
            }
            else
            {
                _hitMarker.SetActive(true);
                _hitAudioSource.Play();
            }
        }
        else if(!IsBattleFieldCell)
        {
            if (!IsShipTarget)
            {
                _missMarker.SetActive(true);
                _missClickAudioSource.Play();
            }            
        }
    }       

    public void SetWaterMaterial()
    {
        if (_cellBody.material != _waterMaterial)
        {
            _cellBody.material = _waterMaterial;
        }
    }

    public void SetAsShipTarget(bool isTarget)
    {
        IsShipTarget = isTarget;
    }

    private void SetPointedState()
    {
        if (!IsUnableToClick) return;

        DOTween.Kill($"Down {_nameID}");
        _pointerEnterSequence = DOTween.Sequence();
        _pointerEnterSequence.SetId($"Up {_nameID}");
        _pointerEnterSequence.Append(gameObject.transform.DOMove(_pointedBodyTransform.position, 1));
    }   

    private void SetUnpointedState()
    {
        if (!IsUnableToClick) return;

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

    public void SetUnableToClick(bool status)
    {
        IsUnableToClick = status;
    }

    public void SendClickEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };
        //int send = value;
        object[] sendData = new object[]
        {
            Arrayindex,
            _user
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.CellClick, sendData, options, sendOptions);        
    }

    public void DeactivateCollider()
    {
        _bodyCollider.enabled = false;
        _markerCollider.enabled = false;
    }
}
