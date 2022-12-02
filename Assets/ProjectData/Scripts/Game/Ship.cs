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

    private List<ShipCell> _shipCells;
    private Vector3 _droppedPosition;
    private Quaternion _droppedRotation;
    private LayerMask _shipsLayer;
    private LayerMask _cellsLayer;
    private string _playerID;

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
            SetShipIsInPosition(true);
            return;
        }

        if (CheckOnCollisions())
        {
            ClearShipPosition();
            transform.position = _droppedPosition;
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
}
