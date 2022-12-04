using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterInGameWindow : MonoBehaviour
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _createAcciuntButton;

    [SerializeField] private Canvas _enterInGameCanvsa;
    [SerializeField] private Canvas _createAccountCanvas;
    [SerializeField] private Canvas _signInCanvas;
    [SerializeField] private AwaiterWindow _awaiterWindow;
    [SerializeField] private LobbyScreen _lobbyScreen;

    private SoundManager _soundManager;

    private void Start()
    {
        _signInButton.onClick.AddListener(OpenSignInWindow);
        _createAcciuntButton.onClick.AddListener(OpenCreateAccountWindow);
        _soundManager = FindObjectOfType<SoundManager>();
        _soundManager.SubscribeMenuButtons();
        _soundManager.PlayMenuSound();

        if (PlayerPrefs.HasKey("LoggedIn"))
        {
            if (PlayerPrefs.GetInt("LoggedIn") == 1)
            {
                PlayerPrefs.SetInt("LoggedIn", 0);
                _lobbyScreen.OpenLobbiScreen();
            }
        }
        else
        {
            PlayerPrefs.SetInt("LoggedIn", 0);
        }

    }

    private void OpenSignInWindow()
    {
        _createAccountCanvas.enabled = false;
        _signInCanvas.enabled = true;
        _awaiterWindow.SetHelloState();
    }

    private void OpenCreateAccountWindow()
    {
        _createAccountCanvas.enabled = true;
        _signInCanvas.enabled = false;
        _awaiterWindow.SetHelloState();
    }

    private void OnDestroy()
    {
        _signInButton.onClick.RemoveAllListeners();
        _createAcciuntButton.onClick.RemoveAllListeners();
    }
}
