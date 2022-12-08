using Engine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldController : ICleanable, IController
{
    private PlayerFieldView _view;
    private GameMenuController _gameMenuController;

    public PlayerFieldController(UserInput input, PlayerFieldView view, GameMenuController gameMenuController)
    {
        _view = view;
        _gameMenuController = gameMenuController;

        input.Player.A_Button.performed += (contex) => _view.SetCameraLeftPosition();
        input.Player.D_Button.performed += (contex) => _view.SetCameraRightPosition();
        input.Player.W_Button.performed += (contex) => _view.SetLastPosition();
        input.Player.S_Button.performed += (contex) => _view.SetCameraCenterPosition();

        _gameMenuController.OnOpenMenu += _view.SetCameraMenuPosition;
        _gameMenuController.OnCloseMenu += _view.SetLastPositionAfterMenu;
    }

    public void CleanUp()
    {
        _gameMenuController.OnOpenMenu -= _view.SetCameraMenuPosition;
        _gameMenuController.OnCloseMenu -= _view.SetLastPositionAfterMenu;
    }
}
