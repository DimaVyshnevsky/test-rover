using System.Threading;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.StateMachine;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.BootstrapState
{
    public class BootstrapState : StateController
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IUiService _uiService;
        private readonly ISettingProvider _settingProvider;
        private readonly IAudioService _audioService;
        private readonly UserDataProvider _userDataProvider;
        private readonly AudioSettingsBootstrapController _audioSettingsBootstrapController;

        public BootstrapState(IAssetProvider assetProvider,
            IUiService uiService,
            ILogger logger,
            ISettingProvider settingProvider,
            UserDataProvider userDataProvider,
            AudioSettingsBootstrapController audioSettingsBootstrapController,
            IAudioService audioService) : base(logger)
        {
            _assetProvider = assetProvider;
            _uiService = uiService;
            _settingProvider = settingProvider;
            _userDataProvider = userDataProvider;
            _audioSettingsBootstrapController = audioSettingsBootstrapController;
            _audioService = audioService;
        }

        public override async UniTask Enter(CancellationToken cancellationToken)
        {
            await _assetProvider.Initialize(default);
            await _uiService.Initialize();

            _uiService.ShowScreenImmediately(ConstUI.LoadingScreen, default).Forget();

            _userDataProvider.Initialize();
            await _audioService.Initialize();
            await _settingProvider.Initialize();
            await _audioSettingsBootstrapController.Run(default);

            GoTo<GameState.GameState>(default).Forget();
        }
    }
}