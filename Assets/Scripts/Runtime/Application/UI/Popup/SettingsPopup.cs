using System;
using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _soundVolumeToggle;
        [SerializeField] private Toggle _musicVolumeToggle;
        [SerializeField] private Toggle _isJoystickControlToggle;

        public event Action<bool> SoundVolumeChangeEvent;
        public event Action<bool> MusicVolumeChangeEvent;
        public event Action<bool> JoystickControlEnableEvent;

        public override async UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            SettingsPopupData settingsPopupData = data as SettingsPopupData;

            var isSoundVolume = settingsPopupData.IsSoundVolume;
            _soundVolumeToggle.onValueChanged.Invoke(isSoundVolume);
            _soundVolumeToggle.isOn = isSoundVolume;

            var isMusicVolume = settingsPopupData.IsMusicVolume;
            _musicVolumeToggle.onValueChanged.Invoke(isMusicVolume);
            _musicVolumeToggle.isOn = isMusicVolume;

            var isJoystickControl = settingsPopupData.IsJoystickEnable;
            _isJoystickControlToggle.onValueChanged.Invoke(isJoystickControl);
            _isJoystickControlToggle.isOn = isJoystickControl;

            _soundVolumeToggle.onValueChanged.AddListener(OnSoundVolumeToggleValueChanged);
            _musicVolumeToggle.onValueChanged.AddListener(OnMusicVolumeToggleValueChanged);
            _isJoystickControlToggle.onValueChanged.AddListener(OnJoystickEnableToggleValueChanged);

            _closeButton.onClick.AddListener(DestroyPopup);

            await base.Show(data, cancellationToken);
        }

        private void OnSoundVolumeToggleValueChanged(bool value)
        {
            SoundVolumeChangeEvent?.Invoke(value);
        }

        private void OnMusicVolumeToggleValueChanged(bool value)
        {
            MusicVolumeChangeEvent?.Invoke(value);
        }

        private void OnJoystickEnableToggleValueChanged(bool value)
        {
            JoystickControlEnableEvent?.Invoke(value);
        }
    }
}