using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Core.Factory
{
    public class GameObjectFactory
    {
        private readonly Dictionary<string, GameObject> _cachedAddressables;
        private readonly DiContainer _container;
        private readonly IAssetProvider _assetProvider;

        public GameObjectFactory(DiContainer container, IAssetProvider assetProvider)
        {
            _cachedAddressables = new Dictionary<string, GameObject>();
            _container = container;
            _assetProvider = assetProvider;
        }

        public void Dispose()
        {
            _cachedAddressables?.Clear();
        }

        public GameObject Create(GameObject prototype)
        {
            return _container.InstantiatePrefab(prototype);
        }

        public GameObject Create(GameObject prototype, Transform parent)
        {
            return _container.InstantiatePrefab(prototype, parent);
        }

        public GameObject Create(GameObject prototype, Vector3 position, Quaternion rotation, Transform parent)
        {
            return _container.InstantiatePrefab(prototype, position, rotation, parent);
        }

        public TComponent Create<TComponent>(GameObject prototype) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype);
        }

        public TComponent Create<TComponent>(GameObject prototype, Transform parent) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype, parent);
        }

        public TComponent Create<TComponent>(GameObject prototype, Vector3 position, Quaternion rotation,
            Transform parent) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype, position, rotation, parent);
        }

        public TComponent Create<TComponent>(TComponent prototype) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype);
        }

        public TComponent Create<TComponent>(TComponent prototype, Transform parent) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype, parent);
        }

        public TComponent Create<TComponent>(TComponent prototype, Vector3 position, Quaternion rotation,
            Transform parent) where TComponent : Component
        {
            return _container.InstantiatePrefabForComponent<TComponent>(prototype, position, rotation, parent);
        }

        public async UniTask<GameObject> Create(string addressableName, CancellationToken cancellationToken)
        {
            return _container.InstantiatePrefab(await GetPrototype(addressableName, cancellationToken));
        }

        public async UniTask<GameObject> Create(string addressableName, Transform parent, CancellationToken cancellationToken)
        {
            return _container.InstantiatePrefab(await GetPrototype(addressableName, cancellationToken), parent);
        }

        public async UniTask<GameObject> Create(string addressableName, Vector3 position, Quaternion rotation, Transform parent, CancellationToken cancellationToken)
        {
            return _container.InstantiatePrefab(await GetPrototype(addressableName, cancellationToken), position, rotation, parent);
        }

        public async UniTask<TComponent> Create<TComponent>(string addressableName, CancellationToken cancellationToken) where TComponent : Component
        {
            var prototype = await GetPrototype(addressableName, cancellationToken);
            var component = _container.InstantiatePrefabForComponent<TComponent>(prototype);
            component.gameObject.name = prototype.name;

            return component;
        }

        public async UniTask<TComponent> Create<TComponent>(string addressableName, Transform parent, CancellationToken cancellationToken) where TComponent : Component
        {
            var prototype = await GetPrototype(addressableName, cancellationToken);
            var component = _container.InstantiatePrefabForComponent<TComponent>(prototype, parent);
            component.gameObject.name = prototype.name;

            return component;
        }

        public async UniTask<TComponent> Create<TComponent>(string addressableName, Vector3 position, Quaternion rotation, Transform parent, CancellationToken cancellationToken) where TComponent : Component
        {
            var prototype = await GetPrototype(addressableName, cancellationToken);
            var component = _container.InstantiatePrefabForComponent<TComponent>(prototype, position, rotation, parent);
            component.gameObject.name = prototype.name;

            return component;
        }
        
        private async UniTask<GameObject> GetPrototype(string addressableName, CancellationToken cancellationToken)
        {
            if (_cachedAddressables.TryGetValue(addressableName, out var prototype))
                return prototype;

            prototype = await _assetProvider.Load<GameObject>(addressableName, cancellationToken);
            _cachedAddressables[addressableName] = prototype;

            return prototype;
        }
    }
}