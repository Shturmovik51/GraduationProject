using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AvatarView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _image;
    [SerializeField] private Image _border;
    
    public int Index { get; private set; }
    public Button Button => _button;   

    public void Init(Sprite sprite, int index)
    {
        _image.sprite = sprite;
        Index = index;
    }

    public void ResetSelection()
    {
        _border.color = Color.white;
    }

    public void SetSelection()
    {
        _border.color = Color.green;
    }

    public void SubscribeButton(UnityAction<AvatarView> action)
    {
        _button.onClick.AddListener(() => action(this));
    }
}
