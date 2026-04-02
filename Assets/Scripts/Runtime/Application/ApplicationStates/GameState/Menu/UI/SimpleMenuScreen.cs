using System;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Application.GameState.Menu.UI
{
    public class SimpleMenuScreen : UiScreen
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;

        public event Action PlayButtonPressEvent;
        public event Action SettingsButtonPressEvent;

        private void OnDestroy()
        {
            _playButton.onClick.RemoveAllListeners();
            _settingsButton.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            _playButton.onClick.AddListener(OnPlayButtonPress);
            _settingsButton.onClick.AddListener(OnSettingsButtonPress);
        }

        private void OnPlayButtonPress()
        {
            PlayButtonPressEvent?.Invoke();
        }

        private void OnSettingsButtonPress()
        {
            SettingsButtonPressEvent?.Invoke();
        }
    }
}