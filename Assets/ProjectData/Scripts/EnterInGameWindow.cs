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

    private void Start()
    {
        _signInButton.onClick.AddListener(OpenSignInWindow);
        _createAcciuntButton.onClick.AddListener(OpenCreateAccountWindow);
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
