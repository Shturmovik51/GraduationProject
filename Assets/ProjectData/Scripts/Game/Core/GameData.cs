using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    [field: SerializeField] public GameObject UserViewField;
    [field: SerializeField] public GameObject MasterCellsHolderLeft;
    [field: SerializeField] public GameObject MasterCellsHolderRight;
    [field: SerializeField] public GameObject OpponentCellsHolderLeft;
    [field: SerializeField] public GameObject OpponentCellsHolderRight;
    [field: SerializeField] public GameObject MasterShipsHolder;
    [field: SerializeField] public GameObject OpponentShipsHolder;

    [field: SerializeField] public ActionsView ActionsView;
    [field: SerializeField] public PlayerInfoView PlayerView;
    [field: SerializeField] public PlayerInfoView OpponentView;
    [field: SerializeField] public EndBattleView EndBattleView;
    [field: SerializeField] public AvatarsConfig AvatarsConfig;
    [field: SerializeField] public AutoBattleView AutoBattleView;
    [field: SerializeField] public MutePanelView MutePanelView; 

    [field: SerializeField] public Transform MasterTransform;
    [field: SerializeField] public Transform OpponentTransform;


    [SerializeField] private GameMenuView _masterGameMenuView;
    [SerializeField] private GameMenuView _opponentGameMenuView;

    public GameMenuView PlayerGameMenuView 
    { get 
        {
            return (PhotonNetwork.IsMasterClient? _masterGameMenuView: _opponentGameMenuView);                
        } 
    }
}

