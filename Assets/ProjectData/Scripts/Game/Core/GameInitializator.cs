using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Input;
using System.Runtime.CompilerServices;

namespace Engine
{
    public class GameInitializator
    {
        public GameInitializator(ControllersManager controllersManager, PlayerFieldView playerFieldView)
        {


            var inputSystemController = new InputSystemController();
            var userInput = inputSystemController.GetInputSystem();

            var shipMoveController = new ShipMoveController(userInput);
            var mouseRaycaster = new MouseRaycaster(userInput, shipMoveController);

            var playerFieldController = new PlayerFieldController(userInput, playerFieldView);

            controllersManager.Add(inputSystemController);
            controllersManager.Add(shipMoveController);
        }
    }
}