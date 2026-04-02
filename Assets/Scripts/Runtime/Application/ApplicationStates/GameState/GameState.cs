using System.Threading;
using Application.BootstrapState;
using Application.GameState.Menu;
using Cysharp.Threading.Tasks;
using Core.StateMachine;
using Zenject;
using ILogger = Core.ILogger;

namespace Application.GameState
{
    public class GameState : StateController
    {
        private readonly StateMachine _stateMachine;

        private readonly MenuState _menuState;
        private readonly UserDataStateChangeController _userDataStateChangeController;

        public GameState([Inject(Id = BindingConst.GameStateMachine)] StateMachine stateMachine,
            ILogger logger,
            MenuState menuState,
            UserDataStateChangeController userDataStateChangeController) : base(logger)
        {
            _stateMachine = stateMachine;
            _menuState = menuState;
            _userDataStateChangeController = userDataStateChangeController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _userDataStateChangeController.Run(default);

            _stateMachine.Initialize(_menuState);
            _stateMachine.GoTo<MenuState>(default).Forget();
        }
    }
}