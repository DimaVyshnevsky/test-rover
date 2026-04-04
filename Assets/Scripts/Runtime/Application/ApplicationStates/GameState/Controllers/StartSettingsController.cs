using System.Threading;
using Application.GameState.RoverSimulation;
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
        private readonly RoverInputModel _roverInputModel;

        public StartSettingsController(IUiService uiService,
            UserDataProvider userDataProvider,
            IAudioService audioService,
            RoverInputModel roverInputModel)
        {
            _uiService = uiService;
            _userDataProvider = userDataProvider;
            _audioService = audioService;
            _roverInputModel = roverInputModel;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);

            SettingsPopup settingsPopup = _uiService.GetPopup<SettingsPopup>(ConstUI.SettingsPopup);

            settingsPopup.SoundVolumeChangeEvent += OnChangeSoundVolume;
            settingsPopup.MusicVolumeChangeEvent += OnChangeMusicVolume;
            settingsPopup.JoystickControlEnableEvent += OnChangeJoystickControlType;

            var userData = _userDataProvider.GetUserData();

            var isSoundVolume = userData.SettingsData.IsSoundVolume;
            var isMusicVolume = userData.SettingsData.IsMusicVolume;
            var isJoystickControl = _roverInputModel.RoverControlType == RoverControlType.Joystick;

            settingsPopup.Show(new SettingsPopupData(isSoundVolume, isMusicVolume, isJoystickControl), cancellationToken).Forget();

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

        private void OnChangeJoystickControlType(bool state)
        {
            if (state)
            {
                _roverInputModel.RoverControlType = RoverControlType.Joystick;
                return;                
            }

            _roverInputModel.RoverControlType = RoverControlType.Keyboard;
        }
    }
}