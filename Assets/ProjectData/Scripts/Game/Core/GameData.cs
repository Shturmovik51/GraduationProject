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

    [field: SerializeField] public List<Ship> MasterShips;
    [field: SerializeField] public List<Ship> OpponentShips;

    [field: SerializeField] public PlayerView PlayerView;

    [field: SerializeField] public Transform MasterTransform;
    [field: SerializeField] public Transform OpponentTransform;
}

