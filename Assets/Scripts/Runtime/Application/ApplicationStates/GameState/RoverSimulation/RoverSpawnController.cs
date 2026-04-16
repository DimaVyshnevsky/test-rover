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
        private CancellationTokenRegistration _cancellationTokenRegistration;

        public RoverSpawnController(GameObjectFactory factory)
        {
            _factory = factory;
        }

        public override UniTask Run(SpawnRoverRequest request, CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);
            _cancellationTokenRegistration = cancellationToken.Register(ForceStop);

            SpawnRover(request.LevelConfig, request.TerrainTransform);

            return UniTask.CompletedTask;
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            UnityEngine.Object.Destroy(_roverView.gameObject);
            _cancellationTokenRegistration.Dispose();
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

        private void ForceStop()
        {
            if(CurrentState == ControllerState.Run)
                Stop().Forget();
        }
    }
}