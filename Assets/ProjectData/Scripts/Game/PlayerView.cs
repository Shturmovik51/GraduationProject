using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private TMP_Text _actionTitleText;
    [SerializeField] private TMP_Text _actionButtonText;
    [SerializeField] private TMP_Text _opponentNameText;
    [SerializeField] private TMP_Text _opponentInfoText;
    [SerializeField] private TMP_Text _opponentActionText;

    [SerializeField] private Button _actionButton;

    public void SetPlacementStage(UnityAction action)
    {
        _actionButton.onClick.RemoveAllListeners();

        _actionTitleText.text = "Place Your Ships";
        _actionButtonText.text = "Ready To Battle";

        _actionButton.onClick.AddListener(action);
    }

    public void SetRollStage(UnityAction action)
    {
        _actionButton.onClick.RemoveAllListeners();

        _actionTitleText.text = "Rolling For Turn";
        _actionButtonText.text = "Roll The Dice";

        _actionButton.interactable = true;
        _actionButton.onClick.AddListener(action);
    }

    public void SetOpponentActionText(string text)
    {
        _opponentActionText.text = text;
    }

    public void SetOpponentRollValue(int value)
    {
        _opponentActionText.text = value.ToString();
    }

    public void SetWaitingForRollStage()
    {
        _actionTitleText.text = "Waiting Opponent";
        _actionButton.interactable = false;
    }

}
