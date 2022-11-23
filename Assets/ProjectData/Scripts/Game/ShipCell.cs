using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCell : MonoBehaviour
{
    private FieldCell _fieldSell;
    private LayerMask _layerMask;

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("CellsLayer");
    }

    public void FindCell()
    {
        if (Physics.Raycast(transform.position, -transform.up, out var hit, 400, _layerMask))
        {
            if (hit.collider.TryGetComponent<FieldCell>(out var fieldCell))
            {
                _fieldSell = fieldCell;               
                Debug.Log("hit");
            }            
        }
        else
        {
            _fieldSell = null;
        }
    }

    public Transform GetPositionTransform()
    {
        if(_fieldSell != null)
        {
            return _fieldSell.transform;
        }
        else
        {
            return null;
        }
    }

    public void SubscribeShipSell()
    {
        if (_fieldSell != null)
        {
            _fieldSell.OnCellClick += OnCellClick;
            _fieldSell.SetAsShipTarget(true);
        }        
    }

    public void  UnsubscribeShipSell()
    {
        if (_fieldSell != null)
        {
            _fieldSell.OnCellClick -= OnCellClick;
            _fieldSell.SetAsShipTarget(false);
            _fieldSell = null;
        }       
    }

    private void OnCellClick()
    {

    }
}
