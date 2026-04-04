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

        public RoverInputController(JoystickRoverInputController joystickRoverInputController,
            KeyboardRoverInputController keyboardRoverInputController,
            RoverInputModel roverInputModel)
        {
            _joystickRoverInputController = joystickRoverInputController;
            _keyboardRoverInputController = keyboardRoverInputController;
            _roverInputModel = roverInputModel;
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            switch (_roverInputModel.RoverControlType)
            {
                case RoverControlType.Keyboard:
                    _keyboardRoverInputController.Run(cancellationToken).Forget();
                    break;

                case RoverControlType.Joystick:
                    _joystickRoverInputController.Run(cancellationToken).Forget();
                    break;
            }
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            if(_joystickRoverInputController.CurrentControllerState == ControllerState.Run)
                _joystickRoverInputController.Stop().Forget();

            if(_keyboardRoverInputController.CurrentControllerState == ControllerState.Run)
                _keyboardRoverInputController.Stop().Forget();
        }
    }
}