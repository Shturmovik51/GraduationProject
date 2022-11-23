using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class GameInitializator
    {
        public GameInitializator(ControllersManager _controllersManager)
        {
            var userInput = new UserInput();
            userInput.Enable();

            var shipMoveController = new ShipMoveController();
            var mouseRaycaster = new MouseRaycaster(userInput, shipMoveController);

            _controllersManager.Add(shipMoveController);
        }
    }
}