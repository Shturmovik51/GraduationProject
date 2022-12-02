using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndBattleView : MonoBehaviour
{
    [SerializeField] private Canvas _screenCanvas;
    [SerializeField] private Image _winImage;
    [SerializeField] private Image _loseImage;

    [SerializeField] private TMP_Text _victoryTitleText;
    [SerializeField] private TMP_Text _victoryPraiseText;
    [SerializeField] private TMP_Text _revardTitleText;
    [SerializeField] private TMP_Text _revardText;
    [SerializeField] private TMP_Text _loseTitleText;
    [SerializeField] private TMP_Text _loseConsolationText;

    [SerializeField] private GameObject _playAgainPanel;
    [SerializeField] private GameObject _opponentDisconnectPanel;

    [SerializeField] private Button _playerYesButton;
    [SerializeField] private Button _playerNoButton;
    [SerializeField] private Button _opponentYesButton;
    [SerializeField] private Button _opponentNoButton;
    [SerializeField] private Image _playerYesButtonImage;
    [SerializeField] private Image _playerNoButtonImage;
    [SerializeField] private Image _opponentYesButtonImage;
    [SerializeField] private Image _opponentNoButtonImage;

    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _restartButton;

    public void SetLoseScreen()
    {
        _loseImage.enabled = true;
        _loseTitleText.enabled = true;
        _loseConsolationText.enabled = true;
        _screenCanvas.enabled = true;
    }

    public void SetWinScreen()
    {
        _winImage.enabled = true;
        _victoryTitleText.enabled = true;
        _victoryPraiseText.enabled = true;
        _revardTitleText.enabled = true;
        _revardText.enabled = true;
        _screenCanvas.enabled = true;
    }
}
