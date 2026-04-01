using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI.Components
{
    public class FadeUIAnimationComponent : BaseUIAnimationComponent
    {
        [SerializeField] protected float _fadeDuration = 0.25f;
        [SerializeField] private Image _fadeImage;

        private UniTaskCompletionSource _showCompletionSource;
        private UniTaskCompletionSource _hideCompletionSource;
        private CancellationTokenRegistration _cancellationShowRegistration;
        private CancellationTokenRegistration _cancellationHideRegistration;
        private Tween _showFadeTween;
        private Tween _hideFadeTween;

        public override async UniTask PlayShowAnimation(CancellationToken cancellationToken)
        {
            if(_showFadeTween != null && _showFadeTween.IsActive())
                return;

            _showCompletionSource = new UniTaskCompletionSource();

            _cancellationShowRegistration = cancellationToken.Register(() =>
            {
                _showFadeTween?.Kill();
                _showCompletionSource?.TrySetCanceled();
                _cancellationShowRegistration.Dispose();
            });

            PlayShowAnimation();

            await _showCompletionSource.Task;
        }

        public override async UniTask PlayHideAnimation(CancellationToken cancellationToken)
        {
            if(_hideFadeTween != null && _hideFadeTween.IsActive())
                return;

            _hideCompletionSource = new UniTaskCompletionSource();

            _cancellationHideRegistration = cancellationToken.Register(() =>
            {
                _hideFadeTween?.Kill();
                _hideCompletionSource?.TrySetCanceled();
                _cancellationHideRegistration.Dispose();
            });

            PlayHideAnimation();

            await _hideCompletionSource.Task;
        }

        private void PlayShowAnimation()
        {
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1);
            _fadeImage.gameObject.SetActive(true);
            gameObject.SetActive(true);

            _showFadeTween = _fadeImage.DOFade(0, _fadeDuration)
                .From(1)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    _fadeImage.gameObject.SetActive(false);
                    _cancellationShowRegistration.Dispose();
                    _showCompletionSource?.TrySetResult();
                });;
        }

        private void PlayHideAnimation()
        {
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 0);
            _fadeImage.gameObject.SetActive(true);

            _hideFadeTween = _fadeImage.DOFade(1, _fadeDuration)
                .From(0)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
                .OnComplete(() =>
                {
                    _cancellationHideRegistration.Dispose();
                    _hideCompletionSource?.TrySetResult();
                });
        }
    }
}