using Core;
using UnityEngine;
using Zenject;

namespace Application.GameState.RoverSimulation
{
    public class KeyboardRoverInputController : BaseController, ITickable
    {
        private readonly RoverInputModel _roverInputModel;

        public KeyboardRoverInputController(RoverInputModel roverInputModel)
        {
            _roverInputModel = roverInputModel;
        }

        public void Tick()
        {
            if(CurrentState != ControllerState.Run)
                return;

            _roverInputModel.Move = 0f;
            _roverInputModel.Turn = 0f;

            if (Input.GetKey(KeyCode.W))
                _roverInputModel.Move += 1f;

            if (Input.GetKey(KeyCode.S))
                _roverInputModel.Move -= 1f;

            if (Input.GetKey(KeyCode.D))
                _roverInputModel.Turn += 1f;

            if (Input.GetKey(KeyCode.A))
                _roverInputModel.Turn -= 1f;

            _roverInputModel.Move = Mathf.Clamp(_roverInputModel.Move, -1f, 1f);
            _roverInputModel.Turn = Mathf.Clamp(_roverInputModel.Turn, -1f, 1f);
        }
    }
}