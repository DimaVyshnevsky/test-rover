using System.Threading;
using CesiumForUnity;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    public class RoverSpawnController : BaseController<SpawnRoverRequest>
    {
        private readonly GameObjectFactory _factory;

        private RoverView _roverView;

        public RoverSpawnController(GameObjectFactory factory)
        {
            _factory = factory;
        }

        public override async UniTask Run(SpawnRoverRequest request, CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            SpawnRover(request.LevelConfig, request.TerrainTransform);
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            UnityEngine.Object.Destroy(_roverView.gameObject);
        }

        private void SpawnRover(LevelConfig config, Transform terrainTransform)
        {
            _roverView = _factory.Create<RoverView>(config.RoverPrefab, Vector3.zero, Quaternion.identity, terrainTransform);
            CesiumGlobeAnchor anchor = _roverView.gameObject.AddComponent<CesiumGlobeAnchor>();

            anchor.longitudeLatitudeHeight = new double3(
                config.StartPosition.Longitude,
                config.StartPosition.Latitude,
                config.StartPosition.Height
            );

            _roverView.Show(config.RoverConfig);
        }
    }
}