using Engine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuController : ICleanable, IController
{
    public Action OnCloseMenu;
    public Action OnOpenMenu;
    public Action OnExitGame;

    private GameMenuView _gameMenuView;
    private OptionsPanelView _optionsPanelView;
    private SoundManager _soundManager;
    private bool _isClosed;
    private bool _isFirstClick;
    public GameMenuController(GameData gameData, UserInput input, SoundManager soundManager)
    {
        _gameMenuView = gameData.PlayerGameMenuView;
        _gameMenuView.Init();
        _soundManager = soundManager;
        _optionsPanelView = _gameMenuView.OptionsPanelView;
        _optionsPanelView.Init();

        _gameMenuView.StartBattleButton.onClick.AddListener(OpenCloseBox);
        _gameMenuView.OptionsButton.onClick.AddListener(() => _optionsPanelView.gameObject.SetActive(true));
        _gameMenuView.ExitButton.onClick.AddListener(ExitGame);
        //_optionsPanelView.ConfirmButton.onClick.AddListener(SetSoundOptions);
        //_optionsPanelView.BackButton.onClick.AddListener(ResetSoundOptions);
        _optionsPanelView.MusicSlider.onValueChanged.AddListener(_soundManager.SetMusicValue);
        _optionsPanelView.EffectsSlider.onValueChanged.AddListener(_soundManager.SetEffectsValue);

        input.Player.Escape.performed += (context) => OpenCloseBox();

        _isClosed = true;
        _isFirstClick = true;

    }

    private void ExitGame()
    {
        OnExitGame?.Invoke();
    }

    //private void SetSoundOptions()
    //{
    //    _optionsPanelView.SetSaundOptions;
    //    _optionsPanelView.gameObject.SetActive(false);
    //}

    //private void ResetSoundOptions()
    //{

    //    _optionsPanelView.gameObject.SetActive(false);
    //}

    private void OpenCloseBox()
    {
        if (_isFirstClick)
        {
            _gameMenuView.SetStartButtonText("Continue");
        }

        if (_isClosed)
        {
            _gameMenuView.OpenBox();
            OnCloseMenu?.Invoke();
        }
        else
        {
            _gameMenuView.CloseBox();
            OnOpenMenu?.Invoke();
        }

        _isClosed = !_isClosed;
    }

    public void CleanUp()
    {
        _gameMenuView.UnsubscribeButtons();
        _optionsPanelView.UnsubscribeButtons();
    }
}
