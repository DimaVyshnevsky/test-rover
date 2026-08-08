using System.Threading;
using Application.GameState.Menu.UI;
using Application.GameState.RoverSimulation;
using Core.StateMachine;
using Application.UI;
using Core.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;

namespace Application.GameState.Menu
{
    public class MenuState : StateController
    {
        private readonly IUiService _uiService;
        private readonly StartSettingsController _startSettingsController;
        private readonly RoverLevelModel _roverLevelModel;
        private readonly LevelsService _levelsService;

        private MenuScreen _menuScreen;

        public MenuState(ILogger logger,
            IUiService uiService,
            StartSettingsController startSettingsController,
            RoverLevelModel roverLevelModel,
            LevelsService levelsService) : base(logger)
        {
            _uiService = uiService;
            _startSettingsController = startSettingsController;
            _roverLevelModel = roverLevelModel;
            _levelsService = levelsService;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            _uiService.ShowScreenImmediately(ConstUI.LoadingScreen, default).Forget();

            _menuScreen = _uiService.GetScreen<MenuScreen>(ConstUI.MenuScreen);
            _menuScreen.SettingsButtonPressEvent += ShowSettings;
            _menuScreen.PlayLevelButtonPressEvent += StartSimulation;
            _menuScreen.StartLevelEditorButtonPressEvent += StartLevelEditor;
            _menuScreen.Initialize(await _levelsService.GetAllLevels());
            _menuScreen.ShowAsync(cancellationToken).Forget();

            _uiService.HideScreen(ConstUI.LoadingScreen, true, default).Forget();
        }

        public override async UniTask Exit()
        {
            await _uiService.HideScreen(ConstUI.MenuScreen, true);
        }

        private void ShowSettings()
        {
            _startSettingsController.Run(default).Forget();
        }

        private void StartSimulation(LevelConfig levelConfig)
        {
            _roverLevelModel.LevelConfig = levelConfig;
            GoTo<RoverSimulationState>().Forget();
        }

        private void StartLevelEditor()
        {
            //todo:
        }
    }
}