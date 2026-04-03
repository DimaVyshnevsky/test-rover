using Core;
using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    [CreateAssetMenu(menuName = "Config/RoverConfig")]
    public class RoverConfig : BaseSettings
    {
        [Header("Suspension")]
        [field: SerializeField] public float SuspensionRestLength { get; private set; } = 0.4f;
        [field: SerializeField] public float SuspensionRange { get; private set; } = 0.2f;
        [field: SerializeField] public float SpringStrength { get; private set; } = 9000f;
        [field: SerializeField] public float DamperStrength { get; private set; } = 1200f;

        [Header("Drive")]
        [field: SerializeField] public float MotorForce { get; private set; } = 2500f;
        [field: SerializeField] public float LateralGrip { get; private set; } = 1200f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 8f;
        [field: SerializeField] public float WheelRadius { get; private set; } = 0.2f;
        [field: SerializeField] public LayerMask GroundMask { get; private set; }
    }
}