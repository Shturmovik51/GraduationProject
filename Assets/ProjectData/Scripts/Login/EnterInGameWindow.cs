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
    [SerializeField] private MutePanelView _mutePanelView;

    private SoundManager _soundManager;
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = FindObjectOfType<SceneLoader>();
        _soundManager = FindObjectOfType<SoundManager>();
    }

    private void Start()
    {
        _sceneLoader.CompleteLoadScene();
        _signInButton.onClick.AddListener(OpenSignInWindow);
        _createAcciuntButton.onClick.AddListener(OpenCreateAccountWindow);
        _soundManager.SubscribeMenuButtons();
        _mutePanelView.Init();
        _mutePanelView.SetMuteState(_soundManager.IsMuted);
        _mutePanelView.SubscribeButton(_soundManager.MuteOrUnmuteSound);
        //_soundManager.PlayMenuSound();

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
