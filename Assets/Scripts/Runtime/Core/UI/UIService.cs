using System;
using System.Collections.Generic;
using System.Threading;
using Core.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.UI
{
    public sealed class UiService : IUiService
    {
        private const string UiServiceViewContainer = "UiServiceViewContainer";
        private const string UILabel = "UI";

        private readonly Dictionary<string, UiScreen> _shownScreens = new Dictionary<string, UiScreen>();

        private IAssetProvider _assetProvider;
        private GameObjectFactory _factory;
        private UiServiceViewContainer _uiServiceViewContainer;
        private Dictionary<string, GameObject> _uiPrototypes;

        [Inject] 
        private void Construct(GameObjectFactory factory, IAssetProvider assetProvider)
        {
            _factory = factory;
            _assetProvider = assetProvider;
        }

        public async UniTask Initialize()
        {
            GameObject container = await _assetProvider.Instantiate(UiServiceViewContainer);
            _uiServiceViewContainer = container.GetComponent<UiServiceViewContainer>();

            var uiPrototypes = await _assetProvider.LoadByLabel<GameObject>(UILabel);
            _uiPrototypes = new Dictionary<string, GameObject>(uiPrototypes.Count);

            foreach (var prototype in uiPrototypes)
                _uiPrototypes.TryAdd(prototype.name, prototype);
        }

        public void Dispose()
        {
            _uiPrototypes?.Clear();

            if(_uiServiceViewContainer != null)
                _assetProvider.ReleaseInstance(_uiServiceViewContainer.gameObject);
        }

        #region Screen

        public bool IsScreenShowed(string id)
        {
            return TryGetShownScreen(id, out _);
        }

        public async UniTask ShowScreen(string id, CancellationToken cancellationToken = default)
        {
            if (TryGetShownScreen(id, out UiScreen screen))
            {
                await screen.ShowAsync(cancellationToken);
            }
            else
            {
                screen = CreateScreen(id);
                _shownScreens.Add(id, screen);
                await screen.ShowAsync(cancellationToken);
            }
        }

        public T GetScreen<T>(string id) where T : UiScreen
        {
            if (!TryGetShownScreen(id, out UiScreen screen))
            {
                screen = CreateScreen(id);
                _shownScreens.Add(id, screen);
                screen.HideImmediately(false);
            }

            return screen as T;
        }

        public async UniTask HideScreen(string id, bool destroy, CancellationToken cancellationToken = default)
        {
            if (TryGetShownScreen(id, out UiScreen screen))
            {
                await screen.HideAsync(destroy, cancellationToken);

                if(destroy)
                    _shownScreens.Remove(id);
            }
        }

        public void HideScreenImmediately(string id, bool destroy)
        {
            if (TryGetShownScreen(id, out UiScreen screen))
            {
                screen.HideImmediately(destroy);

                if(destroy)
                    _shownScreens.Remove(id);
            }
        }

        public void HideAllScreensImmediately(bool destroy)
        {
            foreach (var screen in _shownScreens)
                screen.Value.HideImmediately(destroy);

            if(destroy)
                _shownScreens.Clear();
        }

        public async UniTask HideAllAsyncScreens(bool destroy, CancellationToken cancellationToken = default)
        {
            List<UniTask> hiddenScreens = new List<UniTask>();
            foreach (var screen in _shownScreens)
                hiddenScreens.Add(screen.Value.HideAsync(destroy, cancellationToken));

            if(destroy)
                _shownScreens.Clear();

            await UniTask.WhenAll(hiddenScreens);
        }

        private bool TryGetShownScreen(string id, out UiScreen screen)
        {
            if (_shownScreens.TryGetValue(id, out screen))
                return true;

            return false;
        }

        private UiScreen CreateScreen(string id)
        {
            if (_uiPrototypes.TryGetValue(id, out GameObject prototype))
                return _factory.Create<UiScreen>(prototype, _uiServiceViewContainer.ScreenParent);

            throw new ArgumentException($"{nameof(UiService)} Prototype for '{id}' is not registered.");
        }

        #endregion

        #region Popup

        public async UniTask<BasePopup> ShowPopup(string id, BasePopupData data = null, CancellationToken cancellationToken = default)
        {
            if (_uiPrototypes.TryGetValue(id, out GameObject prototype))
            {
                var popup = _factory.Create<BasePopup>(prototype, _uiServiceViewContainer.ScreenParent);
                await popup.Show(data, cancellationToken);
                return popup;
            }

            throw new ArgumentException($"Prototype for '{id}' is not registered.");
        }

        public T GetPopup<T>(string id) where T : BasePopup
        {
            if (_uiPrototypes.TryGetValue(id, out GameObject prototype))
            {
                var popup = _factory.Create<T>(prototype, _uiServiceViewContainer.ScreenParent);
                popup.EnablePopup(false);
                return popup;
            }

            throw new ArgumentException($"Prototype for '{id}' is not registered.");
        }

        #endregion
    }
}