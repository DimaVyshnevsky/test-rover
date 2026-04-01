using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public class MonoPool<TView> where TView : MonoBehaviour
    {
        private readonly ILogger _logger;

        private BaseFactory<TView> _factory;
        private List<TView> _items;
        private bool _initialize;
        private Transform _container;

        public MonoPool(ILogger logger)
        {
            _logger = logger;
        }

        public void Initialize(BaseFactory<TView> factory, int poolSize)
        {
            if (_initialize)
                return;

            _container = new GameObject("Container").transform;
            _factory = factory;
            _items = new List<TView>(poolSize);

            for (int i = 0; i < poolSize; i++)
                CreateNewItem();

            _initialize = true;
        }

        public TView Get()
        {
            if (!_initialize)
            {
                _logger.Error($"Pool not initialized");
                return null;
            }

            if (_items.Count == 0)
                CreateNewItem();

            var item = _items[0];
            _items.Remove(item);
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(TView item)
        {
            if (!_initialize)
            {
                _logger.Error($"Pool not initialized");
                return;
            }

            item.gameObject.SetActive(false);
            item.transform.SetParent(_container);
            _items.Add(item);
        }

        public void ClearAll()
        {
            Object.Destroy(_container);
            _items.Clear();
            _initialize = false;
        }

        private void CreateNewItem()
        {
            TView item = _factory.Create();
            item.gameObject.SetActive(false);
            _items.Add(item);
            item.transform.SetParent(_container);
        }
    }
}