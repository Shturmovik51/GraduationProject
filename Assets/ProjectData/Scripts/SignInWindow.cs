using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignInWindow : AccountDataWindowBase
{
    [SerializeField] private Button _signInButton;

    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();

        _signInButton.onClick.AddListener(SignIn);
    }

    private void SignIn()
    {
        _awaiterWindow.SetAwaitState();

        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _password,
        },
        request =>
        {
            _awaiterWindow.SetSuccessState();
            Debug.Log($"Success: {_username}");
            SceneManager.LoadScene(1);
        },
        error =>
        {
            _awaiterWindow.SetErrorState();
            Debug.Log($"Fail: {error.ErrorMessage}");
        });   
    }
}
