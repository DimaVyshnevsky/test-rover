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

        public event Action BackToMenuButtonPressEvent;

        public override async UniTask ShowImmediately(CancellationToken cancellationToken)
        {
            _backMenuButton.onClick.AddListener(TryGoMenu);

            await base.ShowImmediately(cancellationToken);
        }

        private void TryGoMenu()
        {
            BackToMenuButtonPressEvent?.Invoke();
        }
    }
}