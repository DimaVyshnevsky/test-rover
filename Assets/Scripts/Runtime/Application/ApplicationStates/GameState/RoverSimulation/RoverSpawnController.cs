using System.Threading;
using CesiumForUnity;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    public class RoverSpawnController : BaseController
    {
        private readonly ISettingProvider _settingProvider;
        private readonly GameObjectFactory _factory;
        private readonly RoverLevelModel _roverLevelModel;

        private RoverView _roverView;

        public RoverSpawnController(ISettingProvider settingProvider,
            GameObjectFactory factory,
            RoverLevelModel roverLevelModel)
        {
            _settingProvider = settingProvider;
            _factory = factory;
            _roverLevelModel = roverLevelModel;
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            var levelConfig = _settingProvider.Get<LevelConfig>($"LevelConfig_{GetLevelIndex()}");
            SpawnRover(levelConfig);
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            UnityEngine.Object.Destroy(_roverView.gameObject);
        }

        private void SpawnRover(LevelConfig config)
        {
            _roverView = _factory.Create<RoverView>(config.RoverPrefab, Vector3.zero, Quaternion.identity, null);
            CesiumGlobeAnchor anchor = _roverView.GetComponent<CesiumGlobeAnchor>();

            anchor.longitudeLatitudeHeight = new double3(
                config.StartPosition.Longitude,
                config.StartPosition.Latitude,
                config.StartPosition.Height
            );

            _roverView.Show(config.RoverConfig);
        }

        private int GetLevelIndex()
        {
            return _roverLevelModel.LevelIndex;
        }
    }
}