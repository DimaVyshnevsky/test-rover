using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public class PlayerPlayerTicTacToyStateController : BaseTicTacToyStateController
    {
        private readonly HintController _hintController;

        private RealPlayerUnit _crossPlayer;
        private RealPlayerUnit _circlePlayer;

        public PlayerPlayerTicTacToyStateController(ILogger logger,
            TicTacToyGameController ticTacToyGameController,
            IUiService uiService,
            BoardModel boardModel,
            UserDataProvider userDataProvider,
            IAudioService audioService,
            HintController hintController,
            TimerController timerController) : base(logger, ticTacToyGameController, uiService, boardModel, userDataProvider, audioService, timerController)
        {
            _hintController = hintController;
        }

        protected override void SubscribeScreen(TicTacToyScreen screen)
        {
            screen.BackButtonPressEvent += ReturnToMenu;
            screen.RestartPressEvent += RestartGame;
            screen.UndoPressEvent += UndoMove;
            screen.HintPressEvent += ShowHint;
        }

        protected override void UnsubscribeScreen(TicTacToyScreen screen)
        {
            screen.BackButtonPressEvent -= ReturnToMenu;
            screen.RestartPressEvent -= RestartGame;
            screen.UndoPressEvent -= UndoMove;
            screen.HintPressEvent -= ShowHint;
        }

        protected override TicTacToyGameRequest PlayerInitialization()
        {
            _crossPlayer = new RealPlayerUnit(SlotType.Cross);
            _circlePlayer = new RealPlayerUnit(SlotType.Circle);

            _screen.SlotPressEvent += _crossPlayer.OnSlotPress;
            _screen.SlotPressEvent += _circlePlayer.OnSlotPress;

            var players = new BasePlayerUnit[]
            {
                _crossPlayer,
                _circlePlayer
            };

            var request = new TicTacToyGameRequest
            {
                Players = players
            };

            return request;
        }

        protected override void RestartGame()
        {
            GoTo<PlayerPlayerTicTacToyStateController>().Forget();
        }

        private void ShowHint()
        {
            if(_hintController.CurrentControllerState == ControllerState.Run)
                return;

            _hintController.Run(new HintControllerRequest()
            {
                Screen = _screen
            }, _cancellationTokenSource.Token).Forget();
        }
        
        private void UndoMove()
        {
            if(_gameSessionData.SequenceCommands.Count == 0)
                return;

            var lastCommandIndex = _gameSessionData.SequenceCommands.Count - 1;
            var command = _gameSessionData.SequenceCommands[lastCommandIndex];
            if(_boardModel.GetSlotType(command.SlotIndex) == SlotType.None)
                return;

            _boardModel.UpdateSlot(command.SlotIndex, SlotType.None);
        }
    }
}