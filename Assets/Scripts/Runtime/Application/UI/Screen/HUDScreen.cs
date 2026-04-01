using System;
using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class HUDScreen : UiScreen
    {
        [SerializeField] private Button _backMenuButton;
        [SerializeField] private Button _settingsButton;

        public event Action BackToMenuButtonPressEvent;
        public event Action SettingsButtonPressEvent;

        public override async UniTask ShowImmediately(CancellationToken cancellationToken)
        {
            _backMenuButton.onClick.AddListener(TryGoMenu);
            _settingsButton.onClick.AddListener(TryShowSettings);

            await base.ShowImmediately(cancellationToken);
        }

        private void TryGoMenu()
        {
            BackToMenuButtonPressEvent?.Invoke();
        }

        private void TryShowSettings()
        {
            SettingsButtonPressEvent?.Invoke();
        }
    }
}