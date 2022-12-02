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

    [field: SerializeField] public GameObject PlayerShipsHolder;
    [field: SerializeField] public GameObject OpponentShipsHolder;

    [field: SerializeField] public ActionsView ActionsView;
    [field: SerializeField] public PlayerInfoView PlayerView;
    [field: SerializeField] public PlayerInfoView OpponentView;
    [field: SerializeField] public EndBattleView EndBattleView;

    [field: SerializeField] public Transform PlayerTransform;
    [field: SerializeField] public Transform OpponentTransform;
}

