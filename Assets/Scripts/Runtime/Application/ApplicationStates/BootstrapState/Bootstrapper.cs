using Core.StateMachine;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Application.BootstrapState
{
    public class Bootstrapper : IInitializable
    {
        private readonly StateMachine _stateMachine;
        private readonly BootstrapState _bootstrapState;
        private readonly GameState.GameState _gameState;

        public Bootstrapper([Inject(Id = BindingConst.ApplicationStateMachine)] StateMachine stateMachine,
            BootstrapState bootstrapState,
            GameState.GameState gameState)
        {
            _stateMachine = stateMachine;
            _bootstrapState = bootstrapState;
            _gameState = gameState;
        }

        //Initial point
        public void Initialize()
        {
            _stateMachine.Initialize(_bootstrapState, _gameState);
            _stateMachine.GoTo<BootstrapState>().Forget();
        }
    }
}