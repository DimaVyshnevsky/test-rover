using System.Threading;
using Application.GameState.Menu.UI;
using Core.StateMachine;
using Application.UI;
using Core.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;

namespace Application.GameState.Menu
{
    public class MenuStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly StartSettingsController _startSettingsController;

        private SimpleMenuScreen _menuScreen;

        public MenuStateController(ILogger logger, IUiService uiService, StartSettingsController startSettingsController) : base(logger)
        {
            _uiService = uiService;
            _startSettingsController = startSettingsController;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            _menuScreen = _uiService.GetScreen<SimpleMenuScreen>(ConstUI.MenuScreen);
            _menuScreen.SettingsButtonPressEvent += ShowSettingsButtonPopup;
            _menuScreen.Initialize();
            _menuScreen.ShowAsync(cancellationToken).Forget();

            _uiService.HideScreen(ConstUI.LoadingScreen, true, default).Forget();

            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            _menuScreen.SettingsButtonPressEvent -= ShowSettingsButtonPopup;

            await _uiService.HideScreen(ConstUI.MenuScreen, true);
        }

        private void ShowSettingsButtonPopup()
        {
            _startSettingsController.Run(default).Forget();
        }
    }
}