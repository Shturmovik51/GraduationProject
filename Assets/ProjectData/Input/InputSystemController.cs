using Engine;

namespace Input
{
    public class InputSystemController : IInitializable, ICleanable, IController
    {
        private UserInput _userInputSystem;

        public InputSystemController()
        {
            _userInputSystem = new UserInput();
            _userInputSystem.Enable();
        }

        public void Initialization()
        {
        }

        public void CleanUp()
        {
            _userInputSystem.Disable();
        }

        public UserInput GetInputSystem()
        {
            return _userInputSystem;
        }
    }
}