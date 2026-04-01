using System.Threading;
using Application.Game.TicTacToy;
using Core.StateMachine;
using Application.UI;
using Application.Services.UserData;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using ILogger = Core.ILogger;

namespace Application.Game.Menu
{
    public class MenuStateController : StateController
    {
        private readonly IUiService _uiService;
        private readonly IAudioService _audioService;
        private readonly UserDataProvider _userDataProvider;

        private MenuScreen _menuScreen;

        public MenuStateController(ILogger logger,
            IUiService uiService,
            UserDataProvider userDataProvider,
            IAudioService audioService) : base(logger)
        {
            _uiService = uiService;
            _userDataProvider = userDataProvider;
            _audioService = audioService;
        }

        public override UniTask Enter(CancellationToken cancellationToken)
        {
            _menuScreen = _uiService.GetScreen<MenuScreen>(ConstUI.MenuScreen);
            _menuScreen.SettingsButtonPressEvent += ShowSettingsButtonPopup;
            _menuScreen.PlayerVsPlayerPressEvent += GoPlayerVsPlayer;
            _menuScreen.PlayerVsBotPressEvent += GoPlayerVsBot;
            _menuScreen.BotVsBotPressEvent += GoBotVsBot;
            _menuScreen.Initialize();
            _menuScreen.ShowAsync(default).Forget();

            return UniTask.CompletedTask;
        }

        public override async UniTask Exit()
        {
            _menuScreen.SettingsButtonPressEvent -= ShowSettingsButtonPopup;
            _menuScreen.PlayerVsPlayerPressEvent -= GoPlayerVsPlayer;
            _menuScreen.PlayerVsBotPressEvent -= GoPlayerVsBot;
            _menuScreen.BotVsBotPressEvent -= GoBotVsBot;

            await _uiService.HideScreen(ConstUI.MenuScreen,true);
        }

        private void ShowSettingsButtonPopup()
        {
            SettingsPopup settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstUI.SettingsPopup);

            settingsPopup.SoundVolumeChangeEvent += OnChangeSoundVolume;
            settingsPopup.MusicVolumeChangeEvent += OnChangeMusicVolume;
            settingsPopup.GameDifficultyModeChangeEvent += OnChangeGameDifficultyMode;

            var userData = _userDataProvider.GetUserData();

            var isSoundVolume = userData.SettingsData.IsSoundVolume;
            var isMusicVolume = userData.SettingsData.IsMusicVolume;
            var gameDifficultyMode = userData.SettingsData.GameDifficultyMode;

            settingsPopup.Show(new SettingsPopupData(isSoundVolume, isMusicVolume, gameDifficultyMode)).Forget();
        }

        private void OnChangeSoundVolume(bool state)
        {
            _audioService.SetVolume(Core.Services.Audio.AudioType.Sound, state ? 1 : 0);
            var userData = _userDataProvider.GetUserData();
            userData.SettingsData.IsSoundVolume = state;
        }

        private void OnChangeMusicVolume(bool state)
        {
            _audioService.SetVolume(Core.Services.Audio.AudioType.Music, state ? 1 : 0);
            var userData = _userDataProvider.GetUserData();
            userData.SettingsData.IsMusicVolume = state;
        }

        private void OnChangeGameDifficultyMode(GameDifficultyMode gameDifficultyMode)
        {
            var userData = _userDataProvider.GetUserData();
            userData.SettingsData.GameDifficultyMode = gameDifficultyMode;
        }

        private void GoPlayerVsPlayer()
        {
            GoTo<PlayerPlayerTicTacToyStateController>().Forget();
        }

        private void GoPlayerVsBot()
        {
            GoTo<PlayerBotTicTacToyStateController>().Forget();
        }

        private void GoBotVsBot()
        {
            GoTo<BotBotTicTacToyStateController>().Forget();
        }
    }
}