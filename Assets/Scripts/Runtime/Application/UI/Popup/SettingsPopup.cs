using System;
using System.Threading;
using Application.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Toggle _soundVolumeToggle;
        [SerializeField] private Toggle _musicVolumeToggle;

        public event Action<bool> SoundVolumeChangeEvent;
        public event Action<bool> MusicVolumeChangeEvent;

        public override async UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            SettingsPopupData settingsPopupData = data as SettingsPopupData;

            var isSoundVolume = settingsPopupData.IsSoundVolume;
            _soundVolumeToggle.onValueChanged.Invoke(isSoundVolume);
            _soundVolumeToggle.isOn = isSoundVolume;

            var isMusicVolume = settingsPopupData.IsMusicVolume;
            _musicVolumeToggle.onValueChanged.Invoke(isMusicVolume);
            _musicVolumeToggle.isOn = isMusicVolume;

            _closeButton.onClick.AddListener(DestroyPopup);

            _soundVolumeToggle.onValueChanged.AddListener(OnSoundVolumeToggleValueChanged);
            _musicVolumeToggle.onValueChanged.AddListener(OnMusicVolumeToggleValueChanged);

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
    }
}