using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldController
{
    PlayerFieldView _view;

    public PlayerFieldController(UserInput input, PlayerFieldView view)
    {
        _view = view;

        input.Player.A_Button.performed += (contex) => _view.SetCameraLeftPosition();
        input.Player.D_Button.performed += (contex) => _view.SetCameraRightPosition();
        input.Player.W_Button.performed += (contex) => _view.SetCameraLeftOrRightPosition();
        input.Player.S_Button.performed += (contex) => _view.SetCameraCenterPosition();
    }
}
