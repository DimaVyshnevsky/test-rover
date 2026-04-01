using System.Text;
using System.Threading;
using Application.UI;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Application.Game.TicTacToy
{
    public class TimerController : BaseController<TimerControllerRequest>, ITickable
    {
        private StringBuilder _builder = new StringBuilder();
        private TicTacToyScreen _screen;
        private int _gameSessionTime = -1;
        private int _difference;

        public override UniTask Run(TimerControllerRequest request, CancellationToken cancellationToken)
        {
            base.Run(request, cancellationToken);

            _difference = (int)Time.time;
            _screen = request.Screen;

            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            if(CurrentState != ControllerState.Run || _screen == null)
                return;

            var currentSessionTime = (int)Time.time - _difference;
            if (_gameSessionTime != currentSessionTime)
            {
                _gameSessionTime = currentSessionTime;
                _screen.UpdateTimerText(ConvertSecondsToTimeString(_gameSessionTime));
            }
        }

        private string ConvertSecondsToTimeString(int totalSeconds)
        {
            _builder.Clear();

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            return _builder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds).ToString();
        }
    }
}