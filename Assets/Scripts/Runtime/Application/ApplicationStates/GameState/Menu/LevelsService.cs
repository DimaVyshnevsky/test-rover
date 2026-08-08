using System.Collections.Generic;
using System.Linq;
using Application.GameState.RoverSimulation;
using Application.Services;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.GameState.Menu
{
    public class LevelsService
    {
        private readonly IAssetProvider _assetProvider;

        private List<LevelConfig> _levels;

        public LevelsService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask<List<LevelConfig>> GetAllLevels()
        {
            if (_levels == null)
            {
                var levelConfigs = await _assetProvider.LoadByLabel<LevelConfig>(ConstConfigs.DefaultLevelsLabel);
                _levels = levelConfigs.ToList();
            }

            return _levels;
        }
    }
}