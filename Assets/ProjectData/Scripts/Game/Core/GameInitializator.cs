using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using Photon.Pun;

namespace Engine
{
    public class GameInitializator
    {
        public GameInitializator(ControllersManager controllersManager, PlayerFieldView playerFieldView,
            List<FieldCell> masterCellsLeft, List<FieldCell> masterCellsRight, List<FieldCell> opponentCellsLeft,
                List<FieldCell> opponentCellsRight, LoadedPlayersInfo playerInfo, GameData gameData)
        {
            var sounrManager = Object.FindObjectOfType<SoundManager>();
            var sceneLoader = Object.FindObjectOfType<SceneLoader>();

            sceneLoader.CompleteLoadScene();
            sounrManager.AddShipsAudioSources();
            sounrManager.AddCellsAudioSources();
            sounrManager.SubscribeGameButtons();
            sounrManager.PlayGameMainTheme();

            var inputSystemController = new InputSystemController();
            var userInput = inputSystemController.GetInputSystem();

            var shipMoveController = new ShipMoveController(userInput);
            var gameMenuController = new GameMenuController(gameData, userInput, sounrManager, playerInfo);
            var mouseRaycaster = new MouseRaycaster(userInput, shipMoveController, gameMenuController);

            var playerFieldController = new PlayerFieldController(userInput, playerFieldView, gameMenuController);

            var endBattleController = new EndBattleController(gameData, playerInfo, gameMenuController, 
                    sounrManager, sceneLoader, mouseRaycaster);

            var shipsManager = new ShipsManager(gameData, playerInfo, masterCellsRight, opponentCellsRight, endBattleController);

            var turnController = new TurnController(masterCellsLeft, masterCellsRight, opponentCellsLeft, 
                    opponentCellsRight, gameData, playerInfo, shipsManager, mouseRaycaster);

            var autoBattleController = new AutoBattleController(gameData, turnController, masterCellsRight, opponentCellsRight,
                    mouseRaycaster, endBattleController);

            controllersManager.Add(inputSystemController);
            controllersManager.Add(mouseRaycaster);
            controllersManager.Add(shipMoveController);
            controllersManager.Add(turnController);
            controllersManager.Add(endBattleController);
            controllersManager.Add(autoBattleController);
            controllersManager.Add(gameMenuController);
            controllersManager.Add(playerFieldController);

            PhotonNetwork.AddCallbackTarget(turnController);
            PhotonNetwork.AddCallbackTarget(shipsManager);
            PhotonNetwork.AddCallbackTarget(endBattleController);
        }
    }
}