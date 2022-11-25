using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseRaycaster
{    
    private Camera _camera;
    private Mouse _mouse;
    private LayerMask _layerMaskForClickAction;
    private ShipMoveController _shipMoveController;

    public MouseRaycaster(UserInput input, ShipMoveController shipMoveController)
    {
        _mouse = Mouse.current;
        _camera = Camera.main;
        _shipMoveController = shipMoveController;
        _layerMaskForClickAction = LayerMask.GetMask("CellsLayer", "ShipsLayer");

        input.Player.Mouse_L.started += (context) => LeftClickDown();
        input.Player.Mouse_L.canceled += (context) => LeftClickUp();
    }    
   
    private void LeftClickDown()
    {
        Ray ray = _camera.ScreenPointToRay(_mouse.position.ReadValue());
        if (Physics.Raycast(ray, out var hit, 400, _layerMaskForClickAction))
        {
            if (hit.collider.TryGetComponent<Ship>(out var shipComponent))
            {                
                var offset = hit.point - hit.transform.position;

                _shipMoveController.SetShipToMove(shipComponent, offset);
            }

            if (hit.collider.TryGetComponent<FieldCell>(out var fieldSell))
            {
                if (fieldSell.IsBattleFieldCell && !fieldSell.IsUsed && fieldSell.IsUnableToClick)
                {
                    fieldSell.SendClickEvent();
                    fieldSell.InitAction();
                }
            }
        }
    }

    private void LeftClickUp()
    {
        _shipMoveController.ClearData();
    }
}
