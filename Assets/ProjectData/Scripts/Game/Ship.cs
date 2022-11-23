using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private List<ShipCell> _shipCells;

    private void Awake()
    {
        _shipCells = GetComponentsInChildren<ShipCell>().ToList();
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

        var pointTransform = _shipCells[0].GetPositionTransform();

        if (pointTransform != null)
        {            
            transform.position = new Vector3 (pointTransform.position.x, transform.position.y, pointTransform.position.z);
        }
    }
    
}
