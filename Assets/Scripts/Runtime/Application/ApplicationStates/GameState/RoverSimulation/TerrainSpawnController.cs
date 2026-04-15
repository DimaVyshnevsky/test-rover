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

        public TerrainSpawnController(GameObjectFactory factory)
        {
            _factory = factory;
        }

        public override async UniTask Run(SpawnTerrainRequest request, CancellationToken cancellationToken)
        {
            await base.Run(cancellationToken);

            _georeference = _factory.Create<CesiumGeoreference>(request.LevelConfig.TerrainPrefab);
            SetupGeoreference(_georeference, request.LevelConfig);

            request.TerrainTransformResponse = _georeference.transform;
        }

        public override async UniTask Stop()
        {
            await base.Stop();

            UnityEngine.Object.Destroy(_georeference.gameObject);
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
    }
}