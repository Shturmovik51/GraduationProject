using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycaster
{
    public event Action OnMissShoot;

    private Camera _camera;
    private Mouse _mouse;
    private LayerMask _layerMaskForClickAction;
    private ShipMoveController _shipMoveController;

    private bool _isCanHitCell;

    public MouseRaycaster(UserInput input, ShipMoveController shipMoveController)
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _shipMoveController = shipMoveController;
        _layerMaskForClickAction = LayerMask.GetMask("CellsLayer", "ShipsLayer");

        input.Player.Mouse_L.started += (context) => LeftClickDown();
        input.Player.Mouse_L.canceled += (context) => LeftClickUp();

        _isCanHitCell = false;
    }    

    public void SetUnableToHitCell(bool isCanHitCell)
    {
        _isCanHitCell = isCanHitCell;
    }
   
    private void LeftClickDown()
    {
        Ray ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, 400, _layerMaskForClickAction))
        {
            if (hit.collider.TryGetComponent<Ship>(out var shipComponent))
            {
                if (shipComponent.IsLocked)
                {
                    return;
                }

                var offset = hit.point - hit.transform.position;

                _shipMoveController.SetShipToMove(shipComponent, offset);
            }

            if (hit.collider.TryGetComponent<FieldCell>(out var fieldSell))
            {
                if (!_isCanHitCell)
                {
                    return;
                }

                if (fieldSell.IsBattleFieldCell && !fieldSell.IsUsed && fieldSell.IsUnableToClick)
                {
                    fieldSell.SendClickEvent();
                    fieldSell.InitAction();

                    if (!fieldSell.IsShipTarget)
                    {
                        _isCanHitCell = false;
                        OnMissShoot?.Invoke();
                    }
                }
            }
        }
    }

    private void LeftClickUp()
    {
        _shipMoveController.ClearData();
    }
}
