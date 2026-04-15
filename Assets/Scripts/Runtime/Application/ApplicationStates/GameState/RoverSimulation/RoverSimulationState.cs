using System.Threading;
using Application.GameState.Menu;
using Application.UI;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;

namespace Application.GameState.RoverSimulation
{
    public class RoverSimulationState : StateController
    {
        private readonly IUiService _uiService;
        private readonly RoverInputController _roverInputController;
        private readonly RoverSpawnController _roverSpawnController;
        private readonly TerrainSpawnController _terrainSpawnController;

        private CancellationTokenSource _cancellationTokenSource;

        public RoverSimulationState(ILogger logger,
            IUiService uiService,
            RoverInputController roverInputController,
            TerrainSpawnController terrainSpawnController,
            RoverSpawnController roverSpawnController) : base(logger)
        {
            _uiService = uiService;
            _roverInputController = roverInputController;
            _terrainSpawnController = terrainSpawnController;
            _roverSpawnController = roverSpawnController;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var hudScreen = _uiService.GetScreen<HUDScreen>(ConstUI.HUDScreen);
            hudScreen.BackToMenuButtonPressEvent += BackToMenu;
            hudScreen.ShowImmediately(_cancellationTokenSource.Token).Forget();

            await _terrainSpawnController.Run(_cancellationTokenSource.Token);
            await _roverSpawnController.Run(_cancellationTokenSource.Token);

            _roverInputController.Run(_cancellationTokenSource.Token).Forget();
        }

        public override UniTask Exit()
        {
            _roverInputController.Stop().Forget();
            _terrainSpawnController.Stop().Forget();
            _roverSpawnController.Stop().Forget();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            _uiService.HideScreenImmediately(ConstUI.HUDScreen, true);

            return base.Exit();
        }

        private void BackToMenu()
        {
            GoTo<MenuState>().Forget();
        }
    }
}
