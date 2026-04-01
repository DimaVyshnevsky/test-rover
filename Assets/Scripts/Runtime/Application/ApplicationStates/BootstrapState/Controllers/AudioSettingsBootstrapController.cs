using System.Threading;
using Application.Services.Audio;
using Application.Services.UserData;
using Core;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using AudioType = Core.Services.Audio.AudioType;

namespace Application.BootstrapState
{
    public class AudioSettingsBootstrapController : BaseController
    {
        private readonly IAudioService _audioService;
        private readonly UserDataProvider _userDataProvider;

        private CancellationTokenSource _cancellationTokenSource;

        public AudioSettingsBootstrapController(IAudioService audioService,
            UserDataProvider userDataProvider)
        {
            _audioService = audioService;
            _userDataProvider = userDataProvider;
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            SetVolume();
            PlayMusic();
        }

        public override UniTask Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            return base.Stop();
        }

        private void PlayMusic()
        {
            _audioService.PlayMusic(ConstAudio.Music);
        }

        private void SetVolume()
        {
            var isSoundVolume = _userDataProvider.GetUserData().SettingsData.IsSoundVolume;
            _audioService.SetVolume(AudioType.Sound, isSoundVolume ? 1 : 0);

            var isMusicVolume = _userDataProvider.GetUserData().SettingsData.IsMusicVolume;
            _audioService.SetVolume(AudioType.Music, isMusicVolume ? 1 : 0);
        }
    }
}