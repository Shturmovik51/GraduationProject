using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLable;
    [SerializeField] private TMP_Text _catalogLable;

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
    }


    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.Log(errorMessage);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _titleLable.text = $"Welcome {accountInfo.Username} \n" +
                           $"{accountInfo.PlayFabId} \n";
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        ShowCatalog(result.Catalog);
        Debug.Log("Complete load catalog!");
    }

    private void ShowCatalog(List<CatalogItem> catalog)
    {
        _catalogLable.text = "Inventory Info:";

        foreach (var item in catalog)
        {
            _catalogLable.text += $"\n{item.DisplayName}, cost {item.VirtualCurrencyPrices["SC"]} SC ";                                 
        }
    }
}
