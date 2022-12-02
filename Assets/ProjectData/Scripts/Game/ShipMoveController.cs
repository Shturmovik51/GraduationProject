using DG.Tweening;
using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMoveController : IUpdatable, IController
{
    //public event Action<bool> OnChangeShipPosition;

    private Camera _camera;
    private Mouse _mouse;
    private Ship _movedShip;
    private Vector3 _offset;
    private LayerMask _layerMaskForMoveShip;
    private bool _isRotating;
    private Sequence _rotationSequence;

    public ShipMoveController(UserInput input)
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _layerMaskForMoveShip = LayerMask.GetMask("RaycasterSurface");

        input.Player.Mouse_R.performed += (context) => RotateShip();
    }

    public void LocalUpdate(float deltaTime)
    {
        if (_movedShip != null)
        {
            Ray ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());
            if (Physics.Raycast(ray, out var hit, 400, _layerMaskForMoveShip))
            {
                if (hit.collider)
                {
                    _movedShip.transform.position = hit.point - _offset;
                }
            }
        }
    }    

    public void SetShipToMove(Ship movedShip, Vector3 offset)
    {
        _movedShip = movedShip;
        _movedShip.ClearShipPosition();
        _movedShip.SetShipIsInPosition(false);
        _offset = offset;

        //if (_movedShip.IsNotInStartPosition)
        //{
        //    OnChangeShipPosition?.Invoke(_movedShip.IsPositioned);
        //}
    }

    public void ClearData()
    {
        if( _movedShip != null)
        {
            DOTween.Kill($"Rotate");
            _movedShip.SetShipPosition(_isRotating);            
            _movedShip = null;
            _isRotating = false;

            //if (_movedShip.IsNotInStartPosition)
            //{
            //    OnChangeShipPosition?.Invoke(_movedShip.IsPositioned);
            //}
        }
    }

    public void RotateShip()
    {
        if(_movedShip != null && !_isRotating)
        {
            _isRotating = true;
            var step = _movedShip.transform.rotation * Quaternion.Euler(0, 90, 0);
           
            _rotationSequence = DOTween.Sequence();
            _rotationSequence.SetId($"Rotate");
            _rotationSequence.Append(_movedShip.transform.DORotateQuaternion(step, 0.5f));
            _rotationSequence.OnComplete(() => _isRotating = false);
        }
    }
}
