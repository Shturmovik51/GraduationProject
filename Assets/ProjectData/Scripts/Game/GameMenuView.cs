using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuView : MonoBehaviour
{  
    [SerializeField] private Transform _gameBoxConnector;
    [SerializeField] private Button _startBattleButton;
    [SerializeField] private TMP_Text _startBattleButtonText;
    [SerializeField] private Canvas _gameMenuCanvas;

    private Sequence _openSequence;
    private Sequence _closeSequence;

    public Button StartBattleButton => _startBattleButton;

    public void Init()
    {
        var camera = Camera.main;      
        _gameMenuCanvas.worldCamera = camera;
    }

    public void OpenBox()
    {        
        DOTween.Kill($"CloseBox");
        _openSequence = DOTween.Sequence();
        _openSequence.SetId($"OpenBox");
        _openSequence.Append(_gameBoxConnector.DOLocalRotate(new Vector3 (0, 0, 0), 1));
    }

    public void CloseBox()
    {
        DOTween.Kill($"OpenBox");
        _closeSequence = DOTween.Sequence();
        _closeSequence.SetId($"CloseBox");
        _closeSequence.Append(_gameBoxConnector.DOLocalRotate(new Vector3(0, 0, 180), 1));
    }

    public void UnsubscribeButton()
    {
        _startBattleButton.onClick.RemoveAllListeners();
    }
}
