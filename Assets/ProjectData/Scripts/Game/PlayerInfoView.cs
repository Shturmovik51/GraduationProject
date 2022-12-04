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
    public int ShipsCount { get; private set; }

    private List<Image> _shipMarkers;
    
    private void Awake()
    {
        _shipMarkers = _shipMarkersHolder.GetComponentsInChildren<Image>().ToList();
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
                var offMarker = _shipMarkers.Find(marker => marker.enabled == false);
                if (offMarker != null)
                {
                    offMarker.enabled = true;
                }
                break;

            case ChangeCountType.Remove:
                ShipsCount--;
                var onMarker = _shipMarkers.Find(marker => marker.enabled == true);
                if (onMarker != null)
                {
                    onMarker.enabled = false;
                }
                break;

            default:
                break;
        }  
    }
    public void InitInfoView(string charName, int level, int experience)
    {
        _playerTitleText.text = charName;
        _playerInfoText.text = $"Level: {level} | Experience: {experience}";
    }
}
