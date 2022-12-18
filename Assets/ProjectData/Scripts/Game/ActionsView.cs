using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionsView : HidablePanel
{
    [SerializeField] private TMP_Text _actionTitleText;
    [SerializeField] private TMP_Text _actionButtonText;
    [SerializeField] private TMP_Text _opponentActionText;
    [SerializeField] private Button _actionButton;
    [SerializeField] private Image _plyerDiceImage;
    [SerializeField] private Image _opponentDiceImage;
    [SerializeField] private Button _hideButton;
    [SerializeField] private Image _toHideImage;
    [SerializeField] private Image _toUnhideImage;
    [SerializeField] private Image _clickHelpImage;
    [SerializeField] private Image _moveHelpImage;

    [SerializeField] private List<DiceValue> _rollSystem;

    public Button HideButton => _hideButton;
    public int PlayerRolledValue { get; private set; }
    public int OpponentRolledValue { get; private set; }

    public void SetHideOrUnhideImage(bool isHidden)
    {
        _toHideImage.enabled = !isHidden;
        _toUnhideImage.enabled = isHidden;
    }

    public void SetPlacementStage(UnityAction action)
    {
        _actionButton.onClick.RemoveAllListeners();

        _actionTitleText.text = "Place Your Ships";
        _actionButtonText.text = "Ready To Battle";

        _actionButton.onClick.AddListener(action);
        _clickHelpImage.enabled = true;
    }    

    public void SetFinishedPlasemenStage(bool isFinished)
    {
        _clickHelpImage.enabled = !isFinished;
        _actionButton.gameObject.SetActive(isFinished);
    }

    public void SetRollStage(UnityAction action)
    {
        _actionButton.onClick.RemoveAllListeners();

        _actionTitleText.text = "Is Rolling For Turn";
        _actionButtonText.text = "Roll The Dice";

        _actionButton.interactable = true;
        _actionButton.onClick.AddListener(action);
        _actionButton.onClick.AddListener(ShowDices);
    }

    public void SetOpponentActionText(string text)
    {
        _opponentActionText.text = text;
    }

    public void SetOpponentRollValue(int index)
    {
        _opponentActionText.enabled = false;
        _opponentDiceImage.enabled = true;
        var dice = _rollSystem[index];
        _opponentDiceImage.sprite = dice.Sprite;
        OpponentRolledValue = dice.Value;
    }

    public void SetWaitingForRollStage()
    {
        _actionTitleText.text = "Waiting Opponent";
        _actionButton.interactable = false;
    }

    public void SetSynchronizationStage()
    {
        _actionTitleText.text = "Syncronizing Fields";
    }

    public (int value, int index) GetRandomDiceValue()
    {
        var index = Random.Range(0, _rollSystem.Count);
        var dice = _rollSystem[index];
        _plyerDiceImage.sprite = dice.Sprite;
        PlayerRolledValue = dice.Value;
        return (dice.Value, index);
    }

    public void SetBattleStage(bool isPlayerTurn)
    {
        _plyerDiceImage.enabled = false;
        _opponentDiceImage.enabled = false;
        //_actionButton.gameObject.SetActive(true);
        //_actionButtonText.text = "Surrender";
        _clickHelpImage.enabled = false;
        _moveHelpImage.enabled = true;
        _actionButton.onClick.RemoveAllListeners();
        RefreshBattleStageUI(isPlayerTurn);
    }

    public void RefreshBattleStageUI(bool isPlayerTurn)
    {
        _actionTitleText.text = isPlayerTurn ? "It's Your Turn" : "Wait, Fear and Pray";
    }

    public void ClearSubscribes()
    {
        _actionButton.onClick.RemoveAllListeners();
    }    

    private void ShowDices()
    {
        _actionButton.gameObject.SetActive(false);
        _plyerDiceImage.enabled = true;
    }   
}
