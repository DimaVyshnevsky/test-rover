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
        private readonly RoverLevelModel _roverLevelModel;

        public RoverSimulationState(ILogger logger,
            IUiService uiService,
            RoverInputController roverInputController,
            TerrainSpawnController terrainSpawnController,
            RoverSpawnController roverSpawnController,
            RoverLevelModel roverLevelModel) : base(logger)
        {
            _uiService = uiService;
            _roverInputController = roverInputController;
            _terrainSpawnController = terrainSpawnController;
            _roverSpawnController = roverSpawnController;
            _roverLevelModel = roverLevelModel;
        }

        public override async UniTask Enter(CancellationToken cancellationToken = default)
        {
            _uiService.ShowScreenImmediately(ConstUI.LoadingScreen, default).Forget();

            var levelConfig = _roverLevelModel.LevelConfig;

            var spawnTerrainRequest = new SpawnTerrainRequest()
            {
                LevelConfig = levelConfig
            };
            await _terrainSpawnController.Run(spawnTerrainRequest, cancellationToken);

            var spawnRoverRequest = new SpawnRoverRequest()
            {
                LevelConfig = levelConfig,
                TerrainTransform = spawnTerrainRequest.TerrainTransformResponse
            };
            await _roverSpawnController.Run(spawnRoverRequest, cancellationToken);

            _roverInputController.Run(cancellationToken).Forget();

            _uiService.HideScreenImmediately(ConstUI.LoadingScreen, true);

            var hudScreen = _uiService.GetScreen<HUDScreen>(ConstUI.HUDScreen);
            hudScreen.BackToMenuButtonPressEvent += BackToMenu;
            hudScreen.ShowImmediately(cancellationToken).Forget();
        }

        public override UniTask Exit()
        {
            _roverInputController.Stop().Forget();
            _terrainSpawnController.Stop().Forget();
            _roverSpawnController.Stop().Forget();

            _uiService.HideScreenImmediately(ConstUI.HUDScreen, true);

            return base.Exit();
        }

        private void BackToMenu()
        {
            GoTo<MenuState>().Forget();
        }
    }
}
