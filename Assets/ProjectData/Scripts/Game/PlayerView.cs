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
    [SerializeField] private Image _plyerDiceImage;
    [SerializeField] private Image _opponentDiceImage;
    [SerializeField] private List<DiceValue> _rollSystem;
    [SerializeField] private TMP_Text _allShipsCounterText;

    private int _shipsCount;

    public int PlayerRolledValue { get; private set; }
    public int OpponentRolledValue { get; private set; }

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
        _actionButton.onClick.AddListener(ShowDices);
    }

    public void SetOpponentActionText(string text)
    {
        _opponentActionText.text = text;
    }

    public void SetOpponentRollValue(int value)
    {
        _opponentActionText.enabled = false;
        _opponentInfoText.enabled = false;
        _opponentDiceImage.enabled = true;
        var dice = _rollSystem[value];
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
        _opponentInfoText.enabled = true;
        _actionButton.gameObject.SetActive(true);
        _actionButtonText.text = "Surrender";
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

    public void ChangeShipsCount(ChangeCountType type)
    {
        switch (type)
        {            
            case ChangeCountType.Add:
                _shipsCount = _shipsCount++;
                break;

            case ChangeCountType.Remove:
                _shipsCount = _shipsCount--;
                break;

            default:
                break;
        }
        
        _allShipsCounterText.text = _shipsCount.ToString();
    }

    private void ShowDices()
    {
        _actionButton.gameObject.SetActive(false);
        _plyerDiceImage.enabled = true;
    }

}
