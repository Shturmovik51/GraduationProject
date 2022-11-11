using UnityEngine;
using UnityEngine.UI;

public class AccountDataWindowBase : MonoBehaviour
{
    [SerializeField] private InputField _usernameField;
    [SerializeField] private InputField _passwordField;
    [SerializeField] protected AwaiterWindow _awaiterWindow;

    protected string _username;
    protected string _password;

    private void Start()
    {
        SubscriptionsElementsUI();
    }

    protected virtual void SubscriptionsElementsUI()
    {
        _usernameField.onValueChanged.AddListener(UpdateUsername);
        _passwordField.onValueChanged.AddListener(UpdatePassword);
    }

    private void UpdatePassword(string password)
    {
        _password = password;
    }

    private void UpdateUsername(string username)
    {
        _username = username;
    }
}
