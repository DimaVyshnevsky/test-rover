using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core
{
    public class AssetProvider : IAssetProvider
    {
        private class AssetEntry
        {
            public AsyncOperationHandle Handle;
            public int RefCount;
        }

        private readonly Dictionary<string, AssetEntry> _assets = new Dictionary<string, AssetEntry>();
        private readonly HashSet<GameObject> _instances = new HashSet<GameObject>();

        private bool _initialized;

        public async UniTask Initialize(CancellationToken ct = default)
        {
            if (_initialized)
                return;

            AsyncOperationHandle initHandle = Addressables.InitializeAsync();

            while (!initHandle.IsDone)
            {
                if (ct.IsCancellationRequested)
                {
                    Addressables.Release(initHandle);
                    ct.ThrowIfCancellationRequested();
                }

                await UniTask.Yield();
            }

            _initialized = true;
        }

        public async UniTask<T> Load<T>(string key, CancellationToken ct = default) where T : class
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key is null or empty.", key);

            EnsureInitialized();

            AssetEntry entry;

            if (_assets.TryGetValue(key, out entry))
            {
                entry.RefCount++;

                return entry.Handle.Result as T;
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);

            entry = new AssetEntry();
            entry.Handle = handle;
            entry.RefCount = 1;
            _assets[key] = entry;

            try
            {
                T result = await handle.Task.AsUniTask().AttachExternalCancellation(ct);
                return result;
            }
            catch
            {
                Addressables.Release(handle);
                _assets.Remove(key);
                throw;
            }
        }

        public async UniTask<IList<T>> LoadByLabel<T>(string label, CancellationToken ct = default) where T : class
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Label is null or empty.", label);

            EnsureInitialized();

            string key = $"LABEL:{label}";

            AssetEntry entry;

            if (_assets.TryGetValue(key, out entry))
            {
                entry.RefCount++;
                return entry.Handle.Result as IList<T>;
            }

            AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, null);

            entry = new AssetEntry();
            entry.Handle = handle;
            entry.RefCount = 1;
            _assets[key] = entry;

            try
            {
                IList<T> result = await handle.Task.AsUniTask().AttachExternalCancellation(ct);
                return result;
            }
            catch
            {
                Addressables.Release(handle);
                _assets.Remove(key);
                throw;
            }
        }

        public async UniTask<GameObject> Instantiate(string key, CancellationToken ct = default)
        {
            EnsureInitialized();

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key);

            GameObject go = await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (go != null)
                _instances.Add(go);

            return go;
        }

        public async UniTask<GameObject> Instantiate(string key, Vector3 position, Quaternion rotation, CancellationToken ct = default)
        {
            EnsureInitialized();

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, position, rotation);

            GameObject go = await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (go != null)
                _instances.Add(go);

            return go;
        }

        public async UniTask<GameObject> Instantiate(string key, Transform parent, CancellationToken ct = default)
        {
            EnsureInitialized();

            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(key, parent);

            GameObject go = await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (go != null)
                _instances.Add(go);

            return go;
        }

        public void Release(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!_assets.TryGetValue(key, out AssetEntry entry))
                return;

            entry.RefCount--;

            if (entry.RefCount < 0)
                entry.RefCount = 0;

            if (entry.RefCount == 0)
            {
                Addressables.Release(entry.Handle);
                _assets.Remove(key);
            }
        }

        public void ReleaseByLabel(string label)
        {
            string key = $"LABEL:{label}";
            Release(key);
        }

        public void ReleaseInstance(GameObject go)
        {
            if (go == null)
                return;

            if (!_instances.Contains(go))
            {
                Addressables.ReleaseInstance(go);
                return;
            }

            Addressables.ReleaseInstance(go);
            _instances.Remove(go);
        }

        public void ReleaseAll()
        {
            foreach (GameObject go in _instances)
            {
                if (go != null)
                    Addressables.ReleaseInstance(go);
            }

            _instances.Clear();

            foreach (AssetEntry entry in _assets.Values)
            {
                Addressables.Release(entry.Handle);
            }

            _assets.Clear();
        }

        public void Dispose()
        {
            ReleaseAll();
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
                Debug.LogError($"{nameof(AssetProvider)} used before Initialize()");
        }
    }
}