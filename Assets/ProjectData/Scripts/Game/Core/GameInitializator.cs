using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using System.Runtime.CompilerServices;
using Photon.Pun;
using UnityEngine.Playables;

namespace Engine
{
    public class GameInitializator
    {
        public GameInitializator(ControllersManager controllersManager, PlayerFieldView playerFieldView,
            List<FieldCell> masterCellsLeft, List<FieldCell> masterCellsRight, List<FieldCell> opponentCellsLeft,
                List<FieldCell> opponentCellsRight, LoadedPlayerInfo playerInfo, GameData gameData)
        {


            var inputSystemController = new InputSystemController();
            var userInput = inputSystemController.GetInputSystem();

            var shipMoveController = new ShipMoveController(userInput);
            var mouseRaycaster = new MouseRaycaster(userInput, shipMoveController);

            var playerFieldController = new PlayerFieldController(userInput, playerFieldView);

            var turnController = new TurnController(masterCellsLeft, masterCellsRight, opponentCellsLeft, 
                    opponentCellsRight, gameData, playerInfo);

            var shipsManager = new ShipsManager(gameData, playerInfo, masterCellsRight, opponentCellsRight);

            controllersManager.Add(inputSystemController);
            controllersManager.Add(shipMoveController);

            PhotonNetwork.AddCallbackTarget(turnController);
            PhotonNetwork.AddCallbackTarget(shipsManager);
        }
    }
}