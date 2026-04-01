using System;
using System.Threading;
using Application.Services.UserData;
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
        [SerializeField] private Button _difficultyModeButton;
        [SerializeField] private TextMeshProUGUI _difficultyModeText;
        [SerializeField] private Toggle _soundVolumeToggle;
        [SerializeField] private Toggle _musicVolumeToggle;

        private GameDifficultyMode _gameDifficultyMode;
        public event Action<bool> SoundVolumeChangeEvent;
        public event Action<bool> MusicVolumeChangeEvent;
        public event Action<GameDifficultyMode> GameDifficultyModeChangeEvent;

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
            
            _difficultyModeButton.onClick.AddListener(OnDifficultyModeChanged);
            _difficultyModeText.text = settingsPopupData.GameDifficultyMode.ToString();
            _gameDifficultyMode = settingsPopupData.GameDifficultyMode;

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

        private void OnDifficultyModeChanged()
        {
            _gameDifficultyMode = _gameDifficultyMode == GameDifficultyMode.Easy ? GameDifficultyMode.Hard : GameDifficultyMode.Easy;
            _difficultyModeText.text = _gameDifficultyMode.ToString();

            GameDifficultyModeChangeEvent?.Invoke(_gameDifficultyMode);
        }
    }
}