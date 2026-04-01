using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public abstract class UiScreen : MonoBehaviour
    {
        [SerializeField] private BaseUIAnimationComponent _animationComponent;

        private UniTaskCompletionSource _showCompletionSource;
        private CancellationTokenRegistration _cancellationShowRegistration;
        private bool _isShown;
        private bool _isDestroyed;

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if(IsScreenShown() || IsScreenDestroyed())
                return;

            _isShown = true;
            _showCompletionSource = new UniTaskCompletionSource();

            cancellationToken.Register(() =>
            {
                _showCompletionSource?.TrySetCanceled();
            });

            await _animationComponent.PlayShowAnimation(cancellationToken);
            await _showCompletionSource.Task;
        }

        public virtual async UniTask ShowImmediately(CancellationToken cancellationToken)
        {
            if(IsScreenShown() || IsScreenDestroyed())
                return;

            _isShown = true;
            _showCompletionSource = new UniTaskCompletionSource();

            _cancellationShowRegistration = cancellationToken.Register(() =>
            {
                _showCompletionSource?.TrySetCanceled();
            });

            gameObject.SetActive(true);

            await _showCompletionSource.Task;
        }

        public virtual async UniTask HideAsync(bool destroy, CancellationToken cancellationToken = default)
        {
            if(!IsScreenShown() || IsScreenDestroyed())
                return;

            if (destroy)
                _isDestroyed = true;

            await _animationComponent.PlayHideAnimation(cancellationToken);

            Hide(destroy);
        }

        public virtual void HideImmediately(bool destroy)
        {
            if(!IsScreenShown() || IsScreenDestroyed())
                return;

            if (destroy)
                _isDestroyed = true;

            Hide(destroy);
        }

        private void Hide(bool destroy)
        {
            _isShown = false;
            _showCompletionSource?.TrySetResult();
            _cancellationShowRegistration.Dispose();

            if (destroy)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.SetActive(false);
        }

        private bool IsScreenShown()
        {
            return _isShown;
        }

        private bool IsScreenDestroyed()
        {
            return _isDestroyed;
        }
    }
}