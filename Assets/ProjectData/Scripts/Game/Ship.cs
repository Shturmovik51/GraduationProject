using DG.Tweening;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public event Action<ShipType> OnDestroyShip;
    public event Action<bool> OnSetShipOnField;

    [SerializeField] private List<Transform> _collisionControllers;
    [SerializeField] private GameObject _body;
    [SerializeField] private ShipType _shipType;
    [SerializeField] private AudioSource _shipSetInPositionAudioSource;
    [SerializeField] private List<ParticleSystem> _explosionEffects;
    [SerializeField] private Collider _shipCollider;
 
    private List<ShipCell> _shipCells;
    private Vector3 _droppedPosition;
    private Quaternion _droppedRotation;
    private LayerMask _shipsLayer;
    private LayerMask _cellsLayer;
    private string _playerID;
    private List<Collider> _colliders;
    private Collider _mainCollider;
    private Rigidbody _mainRigidbody;
    private int _decsCount;
    private int _currentExplodedDeck;

    public bool IsNotInStartPosition { get; private set; }
    public bool IsDestroyed { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsPositioned { get; private set; }

    public ShipType ShipType => _shipType; 

    private void Awake()
    {
        _shipCells = GetComponentsInChildren<ShipCell>().ToList();

        _droppedPosition = transform.position;
        _droppedRotation = transform.rotation;
        _shipsLayer = LayerMask.GetMask("ShipsLayer");
        _cellsLayer = LayerMask.GetMask("CellsLayer");

        _colliders = GetComponentsInChildren<Collider>().ToList();
        _mainCollider = GetComponent<Collider>();
        _mainRigidbody = GetComponent<Rigidbody>();

        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }
        _mainCollider.enabled = true;
        _decsCount = _explosionEffects.Count;
    }

    public void ClearShipPosition()
    {
        foreach (var cell in _shipCells)
        {
            cell.UnsubscribeShipSell();
            cell.InitDamageCheckCallback(CheckForAliveDeck);
        }
    }

    public void SetShipPosition(bool isRotating)
    {
        if (isRotating)
        {
            ClearShipPosition();
            transform.position = _droppedPosition;
            transform.rotation = _droppedRotation;
            SetShipIsInPosition(true);

            return;
        }

        foreach (var cell in _shipCells)
        {
            cell.FindCell();
            cell.SubscribeShipSell();
        }

        var unsubscribedCell = _shipCells.Find(cell => !cell.IsSubscribed);

        if (unsubscribedCell != null)
        {
            ClearShipPosition();
            transform.position = _droppedPosition;
            transform.rotation = _droppedRotation;
            SetShipIsInPosition(true);
            return;
        }

        if (CheckOnCollisions())
        {
            ClearShipPosition();
            transform.position = _droppedPosition;
            transform.rotation = _droppedRotation;
            SetShipIsInPosition(true);
            return;
        }

        var pointTransform = _shipCells[0].GetPositionTransform();

        if (pointTransform != null)
        {            
            transform.position = new Vector3 (pointTransform.position.x, transform.position.y, pointTransform.position.z);
            _droppedPosition = transform.position;
            _droppedRotation = transform.rotation;
            IsNotInStartPosition = true;
            SetShipIsInPosition(true);
            //transform.parent = null;
        }
    }

    private bool CheckOnCollisions()
    {
        var isHaveCollision = false;

        foreach(var controller in _collisionControllers)
        {
            if(Physics.Raycast(controller.position, -Vector3.up, 100, _shipsLayer))
            {
                isHaveCollision = true;
            }
        }

        return isHaveCollision;
    }    

    public void CheckForAliveDeck()
    {
        var aliveDeck = _shipCells.Find(cell => !cell.IsDestroyed);

        Debug.Log(_shipCells.Find(cell => !cell.IsDestroyed));
        Debug.Log(aliveDeck);

        if(aliveDeck == null)
        {
            SendDestroyShipEvent();
            SetShipIsDestroyed(true);
            OnDestroyShip?.Invoke(_shipType);
            StartExplosionProcess();
        }

    }

    public void SetShipIsDestroyed(bool isDestroyed)
    {
        IsDestroyed = isDestroyed;        
    }

    public void SetShipIsLocked(bool isLocked)
    {
        IsLocked = isLocked;
    }

    public void SetShipIsInPosition(bool isPositioned)
    {
        if (IsNotInStartPosition)
        {
            IsPositioned = isPositioned;
            SendSetShipOnFieldEvent(isPositioned);
            OnSetShipOnField?.Invoke(isPositioned);

            if (isPositioned)
            {
                _shipSetInPositionAudioSource.Play();
            }
        }
    }

    public void SetExplosionRadius()
    {
        _body.SetActive(false);

        foreach (var controller in _collisionControllers)
        {
            if (Physics.Raycast(controller.position, -Vector3.up, out var hit, 100, _cellsLayer))
            {
                if(hit.collider.TryGetComponent<FieldCell>(out var cell))
                {
                    cell.InitAction();
                    cell.SetUnableToClick(true);
                }
            }
        }
    }

    public void InitOwner(string ID)
    {
        _playerID = ID;
    }

    public void SendDestroyShipEvent()
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };        

        Quaternion rotation = transform.rotation;

        object[] sendData = new object[]
        {
            _playerID,
            (int)ShipType,
            _shipCells[0].FieldCellArrayIndex,
            rotation
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.DestroyShip, sendData, options, sendOptions);
    }

    public void SendSetShipOnFieldEvent(bool isSet)
    {
        ReceiverGroup receiverGroup = ReceiverGroup.All;
        RaiseEventOptions options = new RaiseEventOptions { Receivers = receiverGroup };
        SendOptions sendOptions = new SendOptions { Reliability = true };

        object[] sendData = new object[]
        {
            _playerID,
            isSet
        };

        PhotonNetwork.RaiseEvent((byte)(int)EventType.SetShipOnField, sendData, options, sendOptions);
    }

    private void StartExplosionProcess()
    {
        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }

        ExploideDeck();
    }

    private void ExploideDeck()
    {
        var hits = Physics.SphereCastAll(_explosionEffects[_currentExplodedDeck].transform.position, 1f, Vector3.up).ToList();

        foreach (var hit in hits)
        {
            if (hit.collider.TryGetComponent<Rigidbody>(out var rigidBody))
            {
                if (rigidBody != _mainRigidbody)
                {
                    //hit.collider.gameObject.transform.parent = null;
                    rigidBody.isKinematic = false;
                    rigidBody.AddExplosionForce(400, _explosionEffects[_currentExplodedDeck].transform.position, 1f);
                    _explosionEffects[_currentExplodedDeck].gameObject.SetActive(true);
                    //hit.collider.enabled = false;
                }
            }
        }

        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.OnComplete(() =>
        {
            if(_currentExplodedDeck < _decsCount - 1)
            {
                _currentExplodedDeck++;
                ExploideDeck();
            }
        });
    }

    public void DeactivateCollider()
    {
        _shipCollider.enabled = false;
    }
}
