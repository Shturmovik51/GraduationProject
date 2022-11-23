using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMoveController : IUpdatable, IController
{
    private Camera _camera;
    private Mouse _mouse;
    private Ship _movedShip;
    private Vector3 _offset;
    private LayerMask _layerMaskForMoveShip;

    public ShipMoveController()
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _layerMaskForMoveShip = LayerMask.GetMask("RaycasterSurface");
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
        _offset = offset;
    }

    public void ClearData()
    {
        if( _movedShip != null)
        {
            _movedShip.SetShipPosition();
            _movedShip = null;
        }
    }
}
