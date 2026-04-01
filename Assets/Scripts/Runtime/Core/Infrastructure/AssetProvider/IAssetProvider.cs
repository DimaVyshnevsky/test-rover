using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IAssetProvider
    {
        UniTask Initialize(CancellationToken ct = default);
        UniTask<T> Load<T>(string key, CancellationToken ct = default) where T : class;
        UniTask<IList<T>> LoadByLabel<T>(string label, CancellationToken ct = default) where T : class;
        UniTask<GameObject> Instantiate(string key, CancellationToken ct = default);
        UniTask<GameObject> Instantiate(string key, Vector3 position, Quaternion rotation, CancellationToken ct = default);
        UniTask<GameObject> Instantiate(string key, Transform parent, CancellationToken ct = default);
        void Release(string key);
        void ReleaseInstance(GameObject go);
        void ReleaseByLabel(string label);
        void ReleaseAll();
        void Dispose();
    }
}