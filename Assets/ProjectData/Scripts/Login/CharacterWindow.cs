using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;
using TMPro;

public class CharacterWindow : MonoBehaviourPunCallbacks
{
    [SerializeField] private Canvas _characterScreenCanvas;
    [SerializeField] private List<CharacterView> _characterViews;
    [SerializeField] private Button _confirmMainButton;
    [SerializeField] private Button _backMainButton;
    [SerializeField] private Button _confirmCreatePanelButton;
    [SerializeField] private Button _backCreatePanelButton;
    [SerializeField] private GameObject _creationPanel;
    [SerializeField] private InputField _inputCharacterNameField;
    [SerializeField] private LobbyScreen _lobbiScreen;

    private int _selectedViewIndex;

    public void OpenCharacterScreen()
    {
        _characterScreenCanvas.enabled = true;

        for (int i = 0; i < _characterViews.Count; i++)        
        {
            _characterViews[i].SubscribeAction(SelectCharacterView);
        }

        _confirmCreatePanelButton.onClick.AddListener(CreateNewCharacter);
        _backCreatePanelButton.onClick.AddListener(() => _creationPanel.SetActive(false));
        _confirmMainButton.onClick.AddListener(_lobbiScreen.OpenLobbiScreen);
        _confirmMainButton.interactable = false;

        GetCharacter();
    }  
    
    private void GetCharacter()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(), OnGetCharactersSuccess, OnError);  
    }

    private void OnGetCharactersSuccess(ListUsersCharactersResult result)
    {        
        ShowCharactersInfo(result.Characters);
    }

    private void ShowCharactersInfo(List<CharacterResult> characters)
    {        
        if (characters.Count > 0)         // todo переделать условие
        {
            for (int i = 0; i < characters.Count; i++)            
            {
                DisplayCharacter(characters[i]);
            }
        }
    }

    private void DisplayCharacter(CharacterResult characterResult)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = characterResult.CharacterId
        },
        result =>
        {
            var level = result.CharacterStatistics["LVL"].ToString();
            var experience = result.CharacterStatistics["EXP"].ToString();
            var viewID = result.CharacterStatistics["ViewID"];

            _characterViews[viewID].SetCharacterInfo(characterResult.CharacterName, level, experience);
            _characterViews[viewID].SetFilledState(true);
        }, OnError);
    }

    private void SelectCharacterView(CharacterView view)
    {
        if (!view.IsFilled)
        {
            _creationPanel.SetActive(true);
            _selectedViewIndex = _characterViews.FindInstanceID(view);
        }
        else
        {
            foreach (var characterView in _characterViews)
            {
                characterView.SetCharacterIsSelected(false);
            }

            view.SetCharacterIsSelected(true);

            _confirmMainButton.interactable = true;
        }
    }

    private void CreateNewCharacter()
    {
        PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest
        {
            CatalogVersion = "FirstCatalog",
            StoreId = "TokensStore"
        }, 
        result =>
        {
            var token = result.Store.Find(item => item.ItemId == "Character_token");

            if (token != null)
            {
                PurchaseCharacterToken(token);
            }
            
        }, OnError);

    }

    private void PurchaseCharacterToken(StoreItem storeItem)
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            ItemId = storeItem.ItemId,
            Price = (int)storeItem.VirtualCurrencyPrices["SC"],
            VirtualCurrency = "SC"
        }, result => CreateCharacterWhithToken(storeItem.ItemId), OnError);
    }

    private void CreateCharacterWhithToken(string itemID)
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _inputCharacterNameField.text,
            ItemId = itemID
        }, result => SetNewCharacterStatistics(result.CharacterId), OnError);
    }

    private void SetNewCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"LVL", 1 },
                {"EXP", 0 },
                {"ViewID", _selectedViewIndex }
            }
        }, result =>
        {

            foreach (var characterView in _characterViews)
            {
                characterView.SetCharacterIsSelected(false);
            }

            _confirmMainButton.interactable = false;

            _creationPanel.SetActive(false);

            _selectedViewIndex = 0;
            GetCharacter();

        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }
}
