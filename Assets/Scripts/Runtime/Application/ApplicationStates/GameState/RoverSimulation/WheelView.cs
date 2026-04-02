using UnityEngine;

namespace Application.GameState.RoverSimulation
{
    public class WheelView : MonoBehaviour
    {
        public Transform point;
        public Transform visual;
        public bool isLeft;

        [HideInInspector] public bool isGrounded;
        [HideInInspector] public RaycastHit hit;
        [HideInInspector] public float compression;
        [HideInInspector] public float wheelRotation;
    }
}