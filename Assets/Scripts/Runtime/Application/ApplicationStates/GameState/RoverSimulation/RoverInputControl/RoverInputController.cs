using System.Threading;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.GameState.RoverSimulation
{
    public class RoverInputController : BaseController
    {
        private readonly JoystickRoverInputController _joystickRoverInputController;
        private readonly KeyboardRoverInputController _keyboardRoverInputController;
        private readonly RoverInputModel _roverInputModel;

        private CancellationTokenRegistration _cancellationTokenRegistration;

        public RoverInputController(JoystickRoverInputController joystickRoverInputController,
            KeyboardRoverInputController keyboardRoverInputController,
            RoverInputModel roverInputModel)
        {
            _joystickRoverInputController = joystickRoverInputController;
            _keyboardRoverInputController = keyboardRoverInputController;
            _roverInputModel = roverInputModel;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            _cancellationTokenRegistration = cancellationToken.Register(ForceStop);

            switch (_roverInputModel.RoverControlType)
            {
                case RoverControlType.Keyboard:
                    _keyboardRoverInputController.Run(cancellationToken).Forget();
                    break;

                case RoverControlType.Joystick:
                    _joystickRoverInputController.Run(cancellationToken).Forget();
                    break;
            }

            return UniTask.CompletedTask;
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            if(_joystickRoverInputController.CurrentControllerState == ControllerState.Run)
                _joystickRoverInputController.Stop().Forget();

            if(_keyboardRoverInputController.CurrentControllerState == ControllerState.Run)
                _keyboardRoverInputController.Stop().Forget();
            
            _cancellationTokenRegistration.Dispose();
        }

        private void ForceStop()
        {
            if(CurrentState == ControllerState.Run)
                Stop().Forget();
        }
    }
}