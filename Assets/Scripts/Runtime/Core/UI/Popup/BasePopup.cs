using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Core.UI
{
    public class BasePopup : MonoBehaviour
    {
        private UniTaskCompletionSource _completionSource;
        private CancellationTokenRegistration _cancellationRegistration;

        public UnityEvent ShowEvent;

        public virtual UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            _completionSource?.TrySetCanceled();
            _completionSource = new UniTaskCompletionSource();

            _cancellationRegistration = cancellationToken.Register(() =>
            {
                _completionSource.TrySetCanceled();
            });

            ShowEvent?.Invoke();

            return _completionSource.Task;
        }

        public void EnablePopup(bool state)
        {
            gameObject.SetActive(state);
        }

        public virtual void DestroyPopup()
        {
            _cancellationRegistration.Dispose();
            _completionSource?.TrySetResult();

            Destroy(gameObject);
        }
    }
}