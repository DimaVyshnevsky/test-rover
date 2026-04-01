using System.Threading;
using Application.Services.UserData;
using Application.UI;
using Core;
using Core.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.GameState
{
    public sealed class StartSettingsController : BaseController
    {
        private readonly IUiService _uiService;
        private readonly UserDataProvider _userDataProvider;
        private readonly IAudioService _audioService;

        public StartSettingsController(IUiService uiService, UserDataProvider userDataProvider, IAudioService audioService)
        {
            _uiService = uiService;
            _userDataProvider = userDataProvider;
            _audioService = audioService;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            SettingsPopup settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstUI.SettingsPopup);

            settingsPopup.SoundVolumeChangeEvent += OnChangeSoundVolume;
            settingsPopup.MusicVolumeChangeEvent += OnChangeMusicVolume;

            var userData = _userDataProvider.GetUserData();

            var isSoundVolume = userData.SettingsData.IsSoundVolume;
            var isMusicVolume = userData.SettingsData.IsMusicVolume;

            settingsPopup.Show(new SettingsPopupData(isSoundVolume, isMusicVolume), cancellationToken).Forget();
            CurrentState = ControllerState.Complete;
            return UniTask.CompletedTask;
        }
        
        
        private void OnChangeSoundVolume(bool state)
        {
            _audioService.SetVolume(AudioType.Sound, state ? 1 : 0);
            var userData = _userDataProvider.GetUserData();
            userData.SettingsData.IsSoundVolume = state;
        }

        private void OnChangeMusicVolume(bool state)
        {
            _audioService.SetVolume(AudioType.Music, state ? 1 : 0);
            var userData = _userDataProvider.GetUserData();
            userData.SettingsData.IsMusicVolume = state;
        }
    }
}