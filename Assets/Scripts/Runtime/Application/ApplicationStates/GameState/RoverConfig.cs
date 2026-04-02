using Core;
using UnityEngine;

namespace Application.GameState
{
    [CreateAssetMenu(menuName = "Config/RoverConfig")]
    public class RoverConfig : BaseSettings
    {
        public float MotorForce = 2500f;
        public float MaxSpeed = 8f;
        public float SuspensionRestLength = 0.4f;
        public float SuspensionRange = 0.2f;
        public float SpringStrength = 9000f;
        public float DamperStrength = 1200f;
        public float LateralGrip = 1200f;
        public float WheelRadius = 0.22f;
    }
}