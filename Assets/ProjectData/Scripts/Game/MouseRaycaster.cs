using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycaster : ICleanable, IController
{
    public event Action OnMissShoot;

    private Camera _camera;
    private Mouse _mouse;
    private LayerMask _layerMaskForClickAction;
    private ShipMoveController _shipMoveController;
    private GameMenuController _gameMenuController;

    private bool _isCanHitCell;
    private bool _isRayCasterBlocked;

    public MouseRaycaster(UserInput input, ShipMoveController shipMoveController, GameMenuController gameMenuController)
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _shipMoveController = shipMoveController;
        _layerMaskForClickAction = LayerMask.GetMask("CellsLayer", "ShipsLayer");
        _gameMenuController = gameMenuController;

        input.Player.Mouse_L.started += (context) => LeftClickDown();
        input.Player.Mouse_L.canceled += (context) => LeftClickUp();
        _gameMenuController.OnOpenMenu += BlockRayCaster;
        _gameMenuController.OnCloseMenu += UnblockRayCaster;

        _isCanHitCell = false;
        _isRayCasterBlocked = true;
    }    

    public void SetHitCellAvaliability(bool isCanHitCell)
    {
        _isCanHitCell = isCanHitCell;
    }
   
    public void BlockRayCaster()
    {
        _isRayCasterBlocked = true;
    }

    public void UnblockRayCaster()
    {
        _isRayCasterBlocked = false;
    }

    private void LeftClickDown()
    {
        if (_isRayCasterBlocked) return;

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
        if (_isRayCasterBlocked) return;

        _shipMoveController.ClearData();
    }

    public void CleanUp()
    {
        _gameMenuController.OnOpenMenu -= BlockRayCaster;
        _gameMenuController.OnCloseMenu -= UnblockRayCaster;
    }
}
