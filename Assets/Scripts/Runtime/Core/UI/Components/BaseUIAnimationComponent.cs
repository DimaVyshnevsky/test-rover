using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.UI
{
    public abstract class BaseUIAnimationComponent : MonoBehaviour
    {
        public abstract UniTask PlayShowAnimation(CancellationToken cancellationToken);
        public abstract UniTask PlayHideAnimation(CancellationToken cancellationToken);
    }
}