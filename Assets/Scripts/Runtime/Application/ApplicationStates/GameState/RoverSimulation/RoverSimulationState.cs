using System.Threading;
using Application.GameState.Menu;
using Application.UI;
using Core;
using Core.Factory;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ILogger = Core.ILogger;

namespace Application.GameState.RoverSimulation
{
    public class RoverSimulationState : StateController
    {
        private readonly ISettingProvider _settingProvider;
        private readonly GameObjectFactory _factory;
        private readonly IUiService _uiService;
        private readonly BaseRoverInputController _roverInputController;

        private GameObject _terrain;
        private RoverView _roverView;
        private CancellationTokenSource _cancellationTokenSource;

        public RoverSimulationState(ILogger logger,
            ISettingProvider settingProvider,
            GameObjectFactory factory,
            IUiService uiService,
            BaseRoverInputController roverInputController) : base(logger)
        {
            _settingProvider = settingProvider;
            _factory = factory;
            _uiService = uiService;
            _roverInputController = roverInputController;
        }

        public override UniTask Enter(CancellationToken cancellationToken = default)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var hudScreen = _uiService.GetScreen<HUDScreen>(ConstUI.HUDScreen);
            hudScreen.BackToMenuButtonPressEvent += BackToMenu;
            hudScreen.ShowImmediately(cancellationToken).Forget();

            var levelConfig = _settingProvider.Get<LevelConfig>($"LevelConfig_{GetLevelIndex()}");

            _terrain = _factory.Create(levelConfig.TerrainPrefab);
            _roverView = _factory.Create<RoverView>(levelConfig.RoverPrefab, levelConfig.StartRoverPosition, Quaternion.identity, null);
            _roverView.Show(levelConfig.RoverConfig);

            _roverInputController.Run(_cancellationTokenSource.Token).Forget();

            return UniTask.CompletedTask;
        }

        public override UniTask Exit()
        {
            _roverInputController.Stop().Forget();

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            UnityEngine.Object.Destroy(_terrain);
            UnityEngine.Object.Destroy(_roverView.gameObject);
            _uiService.HideScreenImmediately(ConstUI.HUDScreen, true);

            return base.Exit();
        }

        private void BackToMenu()
        {
            GoTo<MenuState>().Forget();
        }

        private int GetLevelIndex()
        {
            return 0;
        }
    }
}
