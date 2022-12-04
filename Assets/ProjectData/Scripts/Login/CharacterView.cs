using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Button _button;

    [SerializeField] private Image _borderImage;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TMP_Text _emptyButtonText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private TMP_Text _characterLevelText;
    [SerializeField] private TMP_Text _characterExperienceText;

    public Button Button => _button;
    public bool IsFilled { get; private set; }
    public bool IsSelected { get; private set; }
    public string CharacterName { get { return _characterNameText.text; } }
    public string CharacterID { get; private set; }

    public void SetCharacterInfo(string name, string level, string experience)
    {
        _emptyButtonText.enabled = false;
        _characterNameText.text = name;
        _characterLevelText.text = $"LvL {level}";
        _characterExperienceText.text = $"Exp {experience}";

        _characterNameText.enabled = true;
        _characterLevelText.enabled = true;
        _characterExperienceText.enabled = true;
    }

    public void SetCharacterID(string id)
    {
        CharacterID = id;
    }

    public void SubscribeAction(UnityAction<CharacterView> action)
    {
        Button.onClick.AddListener(() => action(this));
    }

    public void SetCharacterIsSelected(bool isSelected)
    { 
        _borderImage.color = isSelected ? Color.green : Color.white;
        IsSelected = isSelected;
    }

    public void SetFilledState(bool isFilled)
    {
        IsFilled = isFilled;
    }

    public void SetCharacterSprite(Sprite sprite)
    {
        _characterImage.sprite = sprite;
        _characterImage.enabled = true;
    }   
}
