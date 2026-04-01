using System.Threading;
using Cysharp.Threading.Tasks;

namespace Core.UI
{
    public interface IUiService
    {
        UniTask Initialize();
        bool IsScreenShowed(string id);
        T GetScreen<T>(string id) where T : UiScreen;
        UniTask ShowScreen(string id, CancellationToken cancellationToken = default);
        UniTask HideScreen(string id, bool destroy, CancellationToken cancellationToken = default);
        void HideScreenImmediately(string id, bool destroy);
        UniTask<BasePopup> ShowPopup(string id, BasePopupData data = null, CancellationToken cancellationToken = default);
        T GetPopup<T>(string id) where T : BasePopup;
        void HideAllScreensImmediately(bool destroy);
        UniTask HideAllAsyncScreens(bool destroy, CancellationToken cancellationToken = default);
    }
}