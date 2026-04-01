using System.Threading;
using Application.BootstrapState;
using Application.Game.Menu;
using Application.Game.TicTacToy;
using Cysharp.Threading.Tasks;
using Core.StateMachine;
using Zenject;
using ILogger = Core.ILogger;

namespace Application.GameState
{
    public class GameState : StateController
    {
        private readonly StateMachine _stateMachine;
        private readonly MenuStateController _menuStateController;
        private readonly BotBotTicTacToyStateController _botBotTicTacToyStateController;
        private readonly PlayerBotTicTacToyStateController _playerBotTicTacToyStateController;
        private readonly PlayerPlayerTicTacToyStateController _playerPlayerTicTacToyStateController;
        private readonly UserDataStateChangeController _userDataStateChangeController;

        public GameState(ILogger logger,
            [Inject(Id = BindingConst.GameStateMachine)] StateMachine stateMachine,
            MenuStateController menuStateController,
            BotBotTicTacToyStateController botBotTicTacToyStateController,
            PlayerBotTicTacToyStateController playerBotTicTacToyStateController,
            PlayerPlayerTicTacToyStateController playerPlayerTicTacToyStateController,
            UserDataStateChangeController userDataStateChangeController) : base(logger)
        {
            _stateMachine = stateMachine;
            _menuStateController = menuStateController;
            _botBotTicTacToyStateController = botBotTicTacToyStateController;
            _playerBotTicTacToyStateController = playerBotTicTacToyStateController;
            _playerPlayerTicTacToyStateController = playerPlayerTicTacToyStateController;
            _userDataStateChangeController = userDataStateChangeController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _userDataStateChangeController.Run(default);

            _stateMachine.Initialize(_menuStateController, _botBotTicTacToyStateController, _playerBotTicTacToyStateController, _playerPlayerTicTacToyStateController);
            _stateMachine.GoTo<MenuStateController>(default).Forget();
        }
    }
}