using System;
using System.Collections.Generic;
using System.Threading;
using Application.Game.Menu;
using Application.Services.Audio;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Extensions;
using Core.Services.Audio;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.Game.TicTacToy
{
    public abstract class BaseTicTacToyStateController : StateController
    {
        protected readonly IUiService _uiService;
        protected readonly IAudioService _audioService;
        protected readonly UserDataProvider _userDataProvider;
        protected readonly TicTacToyGameController _ticTacToyGameController;
        protected readonly BoardModel _boardModel;
        protected readonly TimerController _timerController;

        protected TicTacToyScreen _screen;
        protected GameSessionData _gameSessionData;
        protected CancellationTokenSource _cancellationTokenSource;

        public BaseTicTacToyStateController(ILogger logger,
            TicTacToyGameController ticTacToyGameController,
            IUiService uiService,
            BoardModel boardModel,
            UserDataProvider userDataProvider,
            IAudioService audioService,
            TimerController timerController) : base(logger)
        {
            _ticTacToyGameController = ticTacToyGameController;
            _uiService = uiService;
            _audioService = audioService;
            _boardModel = boardModel;
            _userDataProvider = userDataProvider;
            _timerController = timerController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            Initialization();

            if(_cancellationTokenSource.IsCancellationRequested)
                return;

            var request = PlayerInitialization();

            await StartGame(request);

            if(_cancellationTokenSource.IsCancellationRequested)
                return;

            await FinishGame();

            if(_cancellationTokenSource.IsCancellationRequested)
                return;

            RestartGame();
        }

        public override async UniTask Exit()
        {
            if (_gameSessionData.Result != GameResultType.None)
                _userDataProvider.GetUserData().GameSessionData.Add(_gameSessionData);

            if(_ticTacToyGameController.CurrentControllerState == ControllerState.Run)
                _ticTacToyGameController.Stop();

            if(_timerController.CurrentControllerState == ControllerState.Run)
                _timerController.Stop();

            _boardModel.BoardUpdateEvent -= UpdateGameSession;

            UnsubscribeScreen(_screen);

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            await _uiService.HideScreen(ConstUI.TikTacGameScreen, true);
        }

        protected abstract TicTacToyGameRequest PlayerInitialization();

        protected abstract void RestartGame();

        protected void Initialization()
        {
            _boardModel.ResetBoard();
            _boardModel.BoardUpdateEvent += UpdateGameSession;

            _gameSessionData = new GameSessionData();
            _gameSessionData.StartTime = DateTime.Now.ToInvariantShortDateString();

            _cancellationTokenSource = new CancellationTokenSource();

            _screen = CreateScreen();
            _screen.ShowAsync(default).Forget();
        }

        protected virtual TicTacToyScreen CreateScreen()
        {
            var screen = _uiService.GetScreen<TicTacToyScreen>(ConstUI.TikTacGameScreen);
            SubscribeScreen(screen);
            screen.Initialize();
            return screen;
        }

        protected virtual void SubscribeScreen(TicTacToyScreen screen)
        {
            screen.BackButtonPressEvent += ReturnToMenu; 
            screen.RestartPressEvent += RestartGame;
        }
        
        protected virtual void UnsubscribeScreen(TicTacToyScreen screen)
        {
            screen.BackButtonPressEvent -= ReturnToMenu;
            screen.RestartPressEvent -= RestartGame;
        }

        protected virtual async UniTask StartGame(TicTacToyGameRequest request)
        {
            _timerController.Run(new TimerControllerRequest()
            {
                Screen = _screen
            }, _cancellationTokenSource.Token).Forget();

            await _ticTacToyGameController.Run(request, _cancellationTokenSource.Token);
        }

        protected virtual async UniTask FinishGame()
        {
            _screen.EnableInput(false);
            _timerController.Stop();

            await TryPlayWinAnimation(_cancellationTokenSource.Token);
            if(_cancellationTokenSource.IsCancellationRequested)
                return;

            _gameSessionData.Result = _boardModel.GameResult;
            await _uiService.ShowPopup(ConstUI.MessagePopup, new MessagePopupData()
            {
                Message = GetGameResultMessage(_boardModel.GameResult),
                ShowButton = true
            });
        }

        protected void ReturnToMenu()
        {
            _audioService.PlaySound(ConstAudio.CloseScreenSound);
            GoTo<MenuStateController>().Forget();
        }

        private async UniTask TryPlayWinAnimation(CancellationToken cancellationToken)
        {
            if (_boardModel.GameResult == GameResultType.None || _boardModel.GameResult == GameResultType.Draw)
                return;
            var pattern = _boardModel.GetWinPattern();
            await _screen.PlayWinAnimation(pattern, cancellationToken);
        }

        protected void UpdateGameSession(Slot slot)
        {
            _gameSessionData.SequenceCommands.Add(new StepCommand()
            {
                SlotIndex = slot.Index,
                SlotType = slot.Type
            });
        }

        protected string GetGameResultMessage(GameResultType gameResultType)
        {
            switch (gameResultType)
            {
                case GameResultType.Draw:
                    return ConstTicTacToyGame.Draw;
                case GameResultType.Cross:
                    return ConstTicTacToyGame.CrossWinKey;
                case GameResultType.Circle:
                    return ConstTicTacToyGame.CircleWinKey;

                default:
                    throw new KeyNotFoundException($"No handler found for this type: {_boardModel.GameResult}");
            }
        }
    }
}