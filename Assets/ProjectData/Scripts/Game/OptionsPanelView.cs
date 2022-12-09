using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelView : MonoBehaviour
{
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _effectsSlider;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _backButton;

    public Button ConfirmButton => _confirmButton;
    public Button BackButton => _backButton;
    public Slider MusicSlider => _musicSlider;
    public Slider EffectsSlider => _effectsSlider;

    public void Init()
    {
        _confirmButton.onClick.AddListener(SetSoundOptions);
        _backButton.onClick.AddListener(ResetSoundOptions);

        ResetSoundOptions();
        _musicSlider.onValueChanged.Invoke(_musicSlider.value);
        _effectsSlider.onValueChanged.Invoke(_effectsSlider.value);
    }

    public void UnsubscribeButtons()
    {
        _confirmButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _musicSlider.onValueChanged.RemoveAllListeners();
        _effectsSlider.onValueChanged.RemoveAllListeners();
    }

    private void SetSoundOptions()
    {
        PlayerPrefs.SetFloat("MusicSoundOptions", _musicSlider.value);
        PlayerPrefs.SetFloat("EffectsSoundOptions", _effectsSlider.value);

        gameObject.SetActive(false);
    }

    private void ResetSoundOptions()
    {
        if (PlayerPrefs.HasKey("MusicSoundOptions"))
        {
            _musicSlider.value = PlayerPrefs.GetFloat("MusicSoundOptions");
        }
        else
        {
            _musicSlider.value = _musicSlider.maxValue;
            PlayerPrefs.SetFloat("MusicSoundOptions", _musicSlider.value);
        }

        if (PlayerPrefs.HasKey("EffectsSoundOptions"))
        {
            _effectsSlider.value = PlayerPrefs.GetFloat("EffectsSoundOptions");
        }
        else
        {
            _effectsSlider.value = _effectsSlider.maxValue;
            PlayerPrefs.SetFloat("EffectsSoundOptions", _effectsSlider.value);
        }

        gameObject.SetActive(false);
    }
}
