using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateAccountWindow : AccountDataWindowBase
{
    [SerializeField] private InputField _emailField;
    [SerializeField] private Button _createAccountButton;

    private string _email;

    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();

        _emailField.onValueChanged.AddListener(UpdateEmail);
        _createAccountButton.onClick.AddListener(CreateAccount);
    }

    private void UpdateEmail(string email)
    {
        _email = email;
    }

    private void CreateAccount()
    {
        _awaiterWindow.SetAwaitState();

        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _email,
            Password = _password
        }, request =>
        {
            _awaiterWindow.SetSuccessState();

            Debug.Log($"Success: {_username}");

            _lobbiScreen.OpenLobbiScreen();

            //SceneManager.LoadScene(1);
        }, error =>
        {
            _awaiterWindow.SetErrorState();
            Debug.Log($"Fail: {error.ErrorMessage}");
        });
    }
}
