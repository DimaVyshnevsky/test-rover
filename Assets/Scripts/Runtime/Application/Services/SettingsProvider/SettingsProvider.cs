using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.Services
{
    public class SettingsProvider : ISettingProvider
    {
        private readonly IAssetProvider _assetProvider;
        private Dictionary<string, BaseSettings> _settings;

        public SettingsProvider(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask Initialize()
        {
            var settings = await _assetProvider.LoadByLabel<BaseSettings>(ConstConfigs.ConfigLabel);
            _settings = new Dictionary<string, BaseSettings>(settings.Count);

            foreach (var setting in settings)
                _settings.TryAdd(setting.name, setting);
        }

        public void Dispose()
        {
            _settings?.Clear();
        }

        public T Get<T>(string id) where T : BaseSettings
        {
            if (_settings != null && _settings.ContainsKey(id))
            {
                var setting = _settings[id];
                return setting as T;
            }

            throw new Exception("No setting found");
        }
    }
}