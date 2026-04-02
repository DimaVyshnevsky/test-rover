using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    public class WheelView : MonoBehaviour
    {
        public Transform Point;
        public Transform Visual;
        public bool IsLeft;

        [HideInInspector] public bool IsGrounded;
        [HideInInspector] public RaycastHit Hit;
        [HideInInspector] public float Compression;
        [HideInInspector] public float WheelRotation;
    }
}