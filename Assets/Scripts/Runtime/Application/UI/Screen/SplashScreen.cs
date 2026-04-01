using System.Threading;
using Core.UI;
using Cysharp.Threading.Tasks;

namespace Application.UI
{
    public class SplashScreen : UiScreen
    {
        public override async UniTask HideAsync(bool destroy, CancellationToken cancellationToken = default)
        {
            await WaitSplashScreenAnimationFinish(cancellationToken);
            await base.HideAsync(destroy, cancellationToken);
        }

        private async UniTask WaitSplashScreenAnimationFinish(CancellationToken cancellationToken)
        {
            await UniTask.Delay(2000, cancellationToken: cancellationToken);
        }
    }
}