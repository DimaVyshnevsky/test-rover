using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.Services
{
    public class SettingsProvider : ISettingProvider
    {
        private readonly IAssetProvider _assetProvider;
        private Dictionary<Type, BaseSettings> _settings;

        public SettingsProvider(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask Initialize()
        {
            var settings = await _assetProvider.LoadByLabel<BaseSettings>(ConstConfigs.ConfigLabel);
            _settings = new Dictionary<Type, BaseSettings>(settings.Count);

            foreach (var setting in settings)
                _settings.TryAdd(setting.GetType(), setting);
        }

        public void Dispose()
        {
            _settings?.Clear();
        }

        public T Get<T>() where T : BaseSettings
        {
            if (_settings != null && _settings.ContainsKey(typeof(T)))
            {
                var setting = _settings[typeof(T)];
                return setting as T;
            }

            throw new Exception("No setting found");
        }
    }
}