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
            var sounrManager = Object.FindObjectOfType<SoundManager>();
            sounrManager.SubscribeGameButtons();
            sounrManager.PlayGameMainTheme();

            var inputSystemController = new InputSystemController();
            var userInput = inputSystemController.GetInputSystem();

            var shipMoveController = new ShipMoveController(userInput);
            var mouseRaycaster = new MouseRaycaster(userInput, shipMoveController);

            var playerFieldController = new PlayerFieldController(userInput, playerFieldView);

            var endBattleController = new EndBattleController(gameData, playerInfo);

            var shipsManager = new ShipsManager(gameData, playerInfo, masterCellsRight, opponentCellsRight, endBattleController);

            var turnController = new TurnController(masterCellsLeft, masterCellsRight, opponentCellsLeft, 
                    opponentCellsRight, gameData, playerInfo, shipsManager, mouseRaycaster);


            controllersManager.Add(inputSystemController);
            controllersManager.Add(shipMoveController);
            controllersManager.Add(turnController);

            PhotonNetwork.AddCallbackTarget(turnController);
            PhotonNetwork.AddCallbackTarget(shipsManager);
            PhotonNetwork.AddCallbackTarget(endBattleController);
        }
    }
}