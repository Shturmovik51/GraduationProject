using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoView : MonoBehaviour
{
    [SerializeField] private PlayerInfoViewType _viewType;
    [SerializeField] private TMP_Text _playerTitleText;
    [SerializeField] private TMP_Text _playerInfoText;
    [SerializeField] private GameObject _shipMarkersHolder;
    [SerializeField] private Image _plaerImage;
    [SerializeField] private Transform _hidePosition;

    public int ShipsCount { get; private set; }

    private List<Image> _shipMarkers;
    
    private void Start()
    {
        _shipMarkers = _shipMarkersHolder.GetComponentsInChildren<Image>().ToList();
        foreach (var shipMarker in _shipMarkers)
        {
            shipMarker.gameObject.SetActive(false);
        }
    }

    public void SetInfoTextVisibility(bool isVisible)
    {
        _playerInfoText.enabled = isVisible;
    }

    public void ChangeShipsCount(ChangeCountType type)
    {
        switch (type)
        {
            case ChangeCountType.Add:
                ShipsCount++;
                var offMarker = _shipMarkers.Find(marker => !marker.gameObject.activeInHierarchy);
                if (offMarker != null)
                {
                    offMarker.gameObject.SetActive(true);
                }
                break;

            case ChangeCountType.Remove:
                ShipsCount--;
                var onMarker = _shipMarkers.Find(marker => marker.gameObject.activeInHierarchy);
                if (onMarker != null)
                {
                    onMarker.gameObject.SetActive(false);
                }
                break;

            default:
                break;
        }  
    }
    public void InitInfoView(string charName, int level, int experience, Sprite sprite)
    {
        _playerTitleText.text = charName;
        _playerInfoText.text = $"Level: {level} | Experience: {experience}";
        _plaerImage.sprite = sprite;
    }
}
