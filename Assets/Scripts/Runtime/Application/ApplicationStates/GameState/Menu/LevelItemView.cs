using System;
using Application.GameState.RoverSimulation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Application.GameState.Menu.UI
{
    public class LevelItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelIdText;
        [SerializeField] private Button _playLevelButton;

        private LevelConfig _levelConfig;

        public event Action<LevelConfig> LevelButtonPressedEvent;

        public void Initialize(LevelConfig levelConfig)
        {
            _levelConfig = levelConfig;
            _levelIdText.text = _levelConfig.Id;
            _playLevelButton.onClick.AddListener(OnLevelPlayButtonPressed);
        }

        private void OnLevelPlayButtonPressed()
        {
            LevelButtonPressedEvent?.Invoke(_levelConfig);
        }
    }
}