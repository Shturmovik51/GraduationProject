using System.Collections.Generic;

namespace Engine
{
    public sealed class ControllersManager : IInitializable, IUpdatable, ILateUpdatable, IFixedUpdatable, ICleanable
    {
        private readonly List<IInitializable> _initialisibleControllers;
        private readonly List<IUpdatable> _updatableControllers;
        private readonly List<ILateUpdatable> _lateUpdatableControllers;
        private readonly List<IFixedUpdatable> _fixedUpdatableControllers;
        private readonly List<ICleanable> _cleanableControllers;
        private readonly List<IGUIUpdatable> _gUIUpdatableControllers;

        public ControllersManager()
        {
            _initialisibleControllers = new List<IInitializable>();
            _updatableControllers = new List<IUpdatable>();
            _lateUpdatableControllers = new List<ILateUpdatable>();
            _fixedUpdatableControllers = new List<IFixedUpdatable>();
            _cleanableControllers = new List<ICleanable>();
            _gUIUpdatableControllers = new List<IGUIUpdatable>();
        }

        public ControllersManager Add(IController controller)
        {
            if (controller is IInitializable initializeController)
            {
                _initialisibleControllers.Add(initializeController);
            }

            if (controller is IUpdatable executeController)
            {
                _updatableControllers.Add(executeController);
            }

            if (controller is ILateUpdatable lateUpdatableControllers)
            {
                _lateUpdatableControllers.Add(lateUpdatableControllers);
            }

            if (controller is IFixedUpdatable fixedUpdatableControllers)
            {
                _fixedUpdatableControllers.Add(fixedUpdatableControllers);
            }

            if (controller is IGUIUpdatable gUIUpdatableControllers)
            {
                _gUIUpdatableControllers.Add(gUIUpdatableControllers);
            }

            if (controller is ICleanable cleanupController)
            {
                _cleanableControllers.Add(cleanupController);
            }

            return this;
        }

        public void Initialization()
        {
            for (var index = 0; index < _initialisibleControllers.Count; ++index)
            {
                _initialisibleControllers[index].Initialization();
            }
        }

        public void LocalUpdate(float deltaTime)
        {
            for (var index = 0; index < _updatableControllers.Count; ++index)
            {
                _updatableControllers[index].LocalUpdate(deltaTime);
            }
        }

        public void LocalLateUpdate(float deltaTime)
        {
            for (var index = 0; index < _lateUpdatableControllers.Count; ++index)
            {
                _lateUpdatableControllers[index].LocalLateUpdate(deltaTime);
            }
        }

        public void LocalFixedUpdate(float fixedDeltaTime)
        {
            for (var index = 0; index < _fixedUpdatableControllers.Count; ++index)
            {
                _fixedUpdatableControllers[index].LocalFixedUpdate(fixedDeltaTime);
            }
        }

        public void LocalOnGUI()
        {
            for (var index = 0; index < _gUIUpdatableControllers.Count; ++index)
            {
                _gUIUpdatableControllers[index].LocalOnGUI();
            }
        }

        public void CleanUp()
        {
            for (var index = 0; index < _cleanableControllers.Count; ++index)
            {
                _cleanableControllers[index].CleanUp();
            }
        }
    }
}