using System.Threading;
using CesiumForUnity;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;

namespace Application.GameState.RoverSimulation
{
    public class TerrainSpawnController : BaseController<SpawnTerrainRequest>
    {
        private readonly GameObjectFactory _factory;

        private CesiumGeoreference _georeference;
        private CancellationTokenRegistration _cancellationTokenRegistration;

        public TerrainSpawnController(GameObjectFactory factory)
        {
            _factory = factory;
        }

        public override UniTask Run(SpawnTerrainRequest request, CancellationToken cancellationToken)
        {
            base.Run(cancellationToken);
            _cancellationTokenRegistration = cancellationToken.Register(ForceStop);

            _georeference = _factory.Create<CesiumGeoreference>(request.LevelConfig.TerrainPrefab);
            SetupGeoreference(_georeference, request.LevelConfig);

            request.TerrainTransformResponse = _georeference.transform;

            return UniTask.CompletedTask;
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            UnityEngine.Object.Destroy(_georeference.gameObject);
            _cancellationTokenRegistration.Dispose();
        }

        private void SetupGeoreference(CesiumGeoreference georeference, LevelConfig config)
        {
            georeference.SetOriginLongitudeLatitudeHeight(
                config.StartPosition.Longitude,
                config.StartPosition.Latitude,
                config.StartPosition.Height
            );
            georeference.Initialize();
        }

        private void ForceStop()
        {
            if(CurrentState == ControllerState.Run)
                Stop().Forget();
        }
    }
}