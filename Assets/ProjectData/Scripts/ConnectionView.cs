using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _connectionStatusText;
    [SerializeField] private Image _connectionStatusImage;
    [SerializeField] private Image _noLoginImage;
    [SerializeField] private InputField _inputField;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;

    public Button LoginButton => _loginButton;
    public Button ConnectButton => _connectButton;
    public Button DisconnectButton => _disconnectButton;

    public void SetLoginWarning(bool status)
    {
        _noLoginImage.gameObject.SetActive(status);
    }

    public void SetOfflineConnectionStatus()
    {
        _connectionStatusImage.color = Color.red;
        _connectionStatusText.text = "Connection status: <color=red>Offline</color>";
    }
    public void SetOnlineConnectionStatus()
    {
        _connectionStatusImage.color = Color.green;
        _connectionStatusText.text = "Connection status: <color=green>Online</color>";
    }

}
