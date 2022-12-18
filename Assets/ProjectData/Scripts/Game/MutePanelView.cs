using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MutePanelView : MonoBehaviour
{
    [SerializeField] private Button _muteButton;
    [SerializeField] private Image _mutedImage;
    [SerializeField] private Image _unmutedImage;

    public void Init()
    {
        _muteButton.onClick.AddListener(ChangeMuteState);
    }

    public void SubscribeButton(UnityAction action)
    {
        _muteButton.onClick.AddListener(action);
    }

    public void SetMuteState(bool isMute)
    {
        _mutedImage.enabled = isMute;
        _unmutedImage.enabled = !isMute;
    }

    private void ChangeMuteState()
    {
        _mutedImage.enabled = !_mutedImage.enabled;
        _unmutedImage.enabled = !_unmutedImage.enabled;
    }

    public void UnsubscribeButton()
    {
        _muteButton.onClick.RemoveAllListeners();
    }    
}
