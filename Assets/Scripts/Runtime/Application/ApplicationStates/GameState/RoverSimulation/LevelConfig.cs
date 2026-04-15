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
    }
    
    [Serializable]
    public class GeoCoordinate
    {
        public double Latitude;
        public double Longitude;
        public double Height;
    }
}