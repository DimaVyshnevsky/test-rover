using System;
using Core;
using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    [CreateAssetMenu(menuName = "Config/LevelConfig")]
    public class LevelConfig : BaseSettings
    {
        [field: SerializeField] public GeoCoordinate StartPosition { get; private set; }
        [field: SerializeField] public GameObject RoverPrefab { get; private set; } 
        [field: SerializeField] public GameObject TerrainPrefab { get; private set; }
        [field: SerializeField] public RoverConfig RoverConfig { get; private set; }
        
        [Header("Map")]
        [field: SerializeField] public MapDatasetType DatasetType { get; private set; } = MapDatasetType.CesiumWorldTerrain;
        [field: SerializeField] public long IonAssetId { get; private set; }
        [field: SerializeField] public string IonAccessToken { get; private set; }
        [field: SerializeField] public string CustomTilesetUrl { get; private set; }

        [Header("Map Quality")]
        [field: SerializeField][Range(1f, 64f)] public float MaximumScreenSpaceError { get; private set; } = 16f;
        [field: SerializeField] public bool PreloadAncestors { get; private set; } = true;
        [field: SerializeField] public bool PreloadSiblings { get; private set; } = true;
    }
    
    [Serializable]
    public class GeoCoordinate
    {
        public double Latitude;
        public double Longitude;
        public double Height;
    }

    public enum MapDatasetType
    {
        CesiumWorldTerrain,
        CesiumOsmBuildings,
        GooglePhotorealistic3DTiles,
        CustomUrl
    }
}