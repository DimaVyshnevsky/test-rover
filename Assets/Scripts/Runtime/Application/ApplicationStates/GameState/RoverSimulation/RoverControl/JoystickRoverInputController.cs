using Core;
using UnityEngine;
using Zenject;

namespace Application.GameState.RoverSimulation
{
    public class JoystickRoverInputController : BaseController, ITickable
    {
        private readonly RoverInputModel _roverInputModel;

        public JoystickRoverInputController(RoverInputModel roverInputModel)
        {
            _roverInputModel = roverInputModel;
        }

        public void Tick()
        {
            if (CurrentState != ControllerState.Run)
                return;

            float move = Input.GetAxis("Vertical");
            float turn = Input.GetAxis("Horizontal");

            move = ApplyDeadZone(move, 0.1f) * -1f;
            turn = ApplyDeadZone(turn, 0.1f) * -1f;

            _roverInputModel.Move = Mathf.Clamp(move, -1f, 1f);
            _roverInputModel.Turn = Mathf.Clamp(turn, -1f, 1f);
        }

        private float ApplyDeadZone(float value, float deadZone)
        {
            if (Mathf.Abs(value) < deadZone)
                return 0f;

            return value;
        }
    }
}