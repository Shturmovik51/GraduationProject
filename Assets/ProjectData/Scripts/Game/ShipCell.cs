using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipCell : MonoBehaviour
{
    [SerializeField] private GameObject _hitEffect;
    public bool IsSubscribed { get; private set; }
    public bool IsDestroyed { get; private set; }
    public int FieldCellArrayIndex { get; private set; }

    private event Action _damageCheck;
    private FieldCell _fieldSell;
    private LayerMask _layerMask;

    private void Awake()
    {
        _layerMask = LayerMask.GetMask("CellsLayer");
    }

    public void InitDamageCheckCallback(Action damageCheck)
    {
        _damageCheck = damageCheck;
    }

    public void FindCell()
    {
        if (Physics.Raycast(transform.position, -transform.up, out var hit, 400, _layerMask))
        {
            if (hit.collider.TryGetComponent<FieldCell>(out var fieldCell))
            {
                if (!fieldCell.IsShipTarget)
                {
                    _fieldSell = fieldCell;
                    FieldCellArrayIndex = fieldCell.Arrayindex;
                }
            }
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
            //_fieldSell.OnCellClick += damageCheck;
            _fieldSell.SetAsShipTarget(true);
            IsSubscribed = true;
        }        
    }

    public void UnsubscribeShipSell()
    {
        if (_fieldSell != null)
        {
            _fieldSell.OnCellClick -= OnCellClick;
            //_fieldSell.OnCellClick -= damageCheck;
            _fieldSell.SetAsShipTarget(false);
            _fieldSell = null;
            IsSubscribed = false;
        }       
    }    

    private void OnCellClick()
    {
        _hitEffect.SetActive(true);
        IsDestroyed = true;
        _damageCheck?.Invoke();
    }
}
