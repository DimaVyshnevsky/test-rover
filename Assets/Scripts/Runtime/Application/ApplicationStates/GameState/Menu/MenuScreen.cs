using System;
using System.Collections.Generic;
using Application.GameState.RoverSimulation;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Application.GameState.Menu.UI
{
    public class MenuScreen : UiScreen
    {
        [SerializeField] private List<LevelItemView> _levelItems;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _startLevelEditor;
        [SerializeField] private GameObject _itemViewPrefab;
        [SerializeField] private Transform _itemsContrainer;

        public event Action StartLevelEditorButtonPressEvent;
        public event Action SettingsButtonPressEvent;
        public event Action<LevelConfig> PlayLevelButtonPressEvent;

        public void Initialize(List<LevelConfig> levelConfigs)
        {
            _levelItems = new List<LevelItemView>(levelConfigs.Count);

            foreach (var config in levelConfigs)
            {
                var itemGameObject = Instantiate(_itemViewPrefab, _itemsContrainer);
                var itemView = itemGameObject.GetComponent<LevelItemView>();
                itemView.Initialize(config);
                itemView.LevelButtonPressedEvent += OnLevelPlayButtonPressed;
                _levelItems.Add(itemView);
            }

            _startLevelEditor.onClick.AddListener(OnStartLevelEditorButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
        }

        private void OnStartLevelEditorButtonPressed()
        {
            StartLevelEditorButtonPressEvent?.Invoke();
        }

        private void OnSettingsButtonPressed()
        {
            SettingsButtonPressEvent?.Invoke();
        }

        private void OnLevelPlayButtonPressed(LevelConfig levelConfig)
        {
            PlayLevelButtonPressEvent?.Invoke(levelConfig);
        }
    }
}