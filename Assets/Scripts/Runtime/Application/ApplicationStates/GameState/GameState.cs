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

        private readonly MenuStateController _menuStateController;
        private readonly UserDataStateChangeController _userDataStateChangeController;

        public GameState([Inject(Id = BindingConst.GameStateMachine)] StateMachine stateMachine,
            ILogger logger,
            MenuStateController menuStateController,
            UserDataStateChangeController userDataStateChangeController) : base(logger)
        {
            _stateMachine = stateMachine;
            _menuStateController = menuStateController;
            _userDataStateChangeController = userDataStateChangeController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _userDataStateChangeController.Run(default);

            _stateMachine.Initialize(_menuStateController);
            _stateMachine.GoTo<MenuStateController>(default).Forget();
        }
    }
}