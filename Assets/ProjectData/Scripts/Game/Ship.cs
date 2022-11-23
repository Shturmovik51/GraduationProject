using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private List<Transform> _collisionControllers;

    private List<ShipCell> _shipCells;
    private Vector3 _droppedPosition;
    private LayerMask _shipsLayer;

    private void Awake()
    {
        _shipCells = GetComponentsInChildren<ShipCell>().ToList();
        _droppedPosition = transform.position;
        _shipsLayer = LayerMask.GetMask("ShipsLayer");
    }

    public void ClearShipPosition()
    {
        foreach (var cell in _shipCells)
        {
            cell.UnsubscribeShipSell();
        }
    }

    public void SetShipPosition()
    {
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
            return;
        }

        if (CheckOnCollisions())
        {
            ClearShipPosition();
            transform.position = _droppedPosition;
            return;
        }

        var pointTransform = _shipCells[0].GetPositionTransform();

        if (pointTransform != null)
        {            
            transform.position = new Vector3 (pointTransform.position.x, transform.position.y, pointTransform.position.z);
            _droppedPosition = transform.position;            
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
}
