using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AutoBattleView : MonoBehaviour
{
    [SerializeField] private Button _autoBattleButton;
    [SerializeField] private TMP_Text _autoBattleText;

    public void SubscribeButton(UnityAction action)
    {
        _autoBattleButton.onClick.AddListener(action);
    }

    public void RotateButton()
    {
        _autoBattleButton.transform.Rotate(0, 0, 1);
    }

    public void UnsubscribeButton()
    {
        _autoBattleButton.onClick.RemoveAllListeners();
    }

    public void SetTextVisibility(bool status)
    {
        _autoBattleText.enabled = status;
    }

    public void SetButtonVisibility(bool status)
    {
        _autoBattleButton.gameObject.SetActive(status);
    }
}
