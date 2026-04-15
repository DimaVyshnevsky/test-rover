using System.Threading;
using CesiumForUnity;
using Core;
using Core.Factory;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    public class TerrainSpawnController : BaseController
    {
        private readonly ISettingProvider _settingProvider;
        private readonly GameObjectFactory _factory;
        private readonly RoverLevelModel _roverLevelModel;

        private CesiumGeoreference _georeference;

        public TerrainSpawnController(ISettingProvider settingProvider,
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

            _georeference = _factory.Create<CesiumGeoreference>(levelConfig.TerrainPrefab);
            SetupGeoreference(_georeference, levelConfig);
            var tileset = CreateTileset(_georeference);
            SetupTileset(tileset, levelConfig);
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

        private Cesium3DTileset CreateTileset(CesiumGeoreference georeference)
        {
            GameObject go = new GameObject("Cesium3DTileset");
            go.transform.SetParent(georeference.transform, false);
            return go.AddComponent<Cesium3DTileset>();
        }

        private void SetupTileset(Cesium3DTileset tileset, LevelConfig config)
        {
            tileset.maximumScreenSpaceError = config.MaximumScreenSpaceError;
            tileset.preloadAncestors = config.PreloadAncestors;
            tileset.preloadSiblings = config.PreloadSiblings;

            switch (config.DatasetType)
            {
                case MapDatasetType.CesiumWorldTerrain:
                    tileset.tilesetSource = CesiumDataSource.FromCesiumIon;
                    tileset.ionAssetID = 1;
                    tileset.ionAccessToken = config.IonAccessToken;
                    break;

                case MapDatasetType.CesiumOsmBuildings:
                    tileset.tilesetSource = CesiumDataSource.FromCesiumIon;
                    tileset.ionAssetID = 96188;
                    tileset.ionAccessToken = config.IonAccessToken;
                    break;

                case MapDatasetType.GooglePhotorealistic3DTiles:
                    tileset.tilesetSource = CesiumDataSource.FromCesiumIon;
                    tileset.ionAssetID = config.IonAssetId;
                    tileset.ionAccessToken = config.IonAccessToken;
                    break;

                case MapDatasetType.CustomUrl:
                    tileset.tilesetSource = CesiumDataSource.FromUrl;
                    tileset.url = config.CustomTilesetUrl;
                    break;
            }
        }

        private int GetLevelIndex()
        {
            return _roverLevelModel.LevelIndex;
        }
    }
}