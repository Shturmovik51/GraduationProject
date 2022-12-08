using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuController : ICleanable, IController
{
    public Action OnCloseMenu;
    public Action OnOpenMenu;

    private GameMenuView _view;
    private bool _isClosed;
    public GameMenuController(GameData gameData, UserInput input)
    {
        _view = gameData.PlayerGameMenuView;
        _view.Init();

        _view.StartBattleButton.onClick.AddListener(OpenCloseBox);
        input.Player.Escape.performed += (context) => OpenCloseBox();

        _isClosed = true;
    }

    private void OpenCloseBox()
    {
        if (_isClosed)
        {
            _view.OpenBox();
            OnCloseMenu?.Invoke();
        }
        else
        {
            _view.CloseBox();
            OnOpenMenu?.Invoke();
        }

        _isClosed = !_isClosed;
    }

    public void CleanUp()
    {
        _view.UnsubscribeButton();
    }
}
