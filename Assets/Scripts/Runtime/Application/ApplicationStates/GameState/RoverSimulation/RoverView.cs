using UnityEngine;
using Zenject;

namespace Application.GameState.RoverSimulation
{
    public class RoverView : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private WheelView[] _wheels;
        [SerializeField] private Transform _centerOfMass;

        private RoverConfig _roverConfig;
        private bool _isInitialize;
        private RoverInputModel _roverInputModel;

        [Inject]
        public void Construct(RoverInputModel roverInputModel)
        {
            _roverInputModel = roverInputModel;
        }

        private void FixedUpdate()
        {
            if(!_isInitialize)
                return;

            float move = _roverInputModel.Move;
            float turn = _roverInputModel.Turn;

            float left = Mathf.Clamp(move - turn, -1f, 1f);
            float right = Mathf.Clamp(move + turn, -1f, 1f);

            for (int i = 0; i < _wheels.Length; i++)
            {
                WheelView wheel = _wheels[i];

                UpdateGroundHit(wheel);

                if (!wheel.isGrounded)
                    continue;

                ApplySuspension(wheel);

                float power = wheel.isLeft ? left : right;

                ApplyDrive(wheel, power);
                ApplyLateralGrip(wheel);
                UpdateWheelVisual(wheel, power);
            }

            LimitSpeed();
        }

        public void Show(RoverConfig config)
        {
            _roverConfig = config;

            if (_centerOfMass != null)
                _rigidbody.centerOfMass = transform.InverseTransformPoint(_centerOfMass.position);

            _isInitialize = true;
        }

        private void UpdateGroundHit(WheelView wheel)
        {
            float castDistance = _roverConfig.SuspensionRestLength + _roverConfig.SuspensionRange + _roverConfig.WheelRadius;

            wheel.isGrounded = Physics.SphereCast(
                wheel.point.position,
                _roverConfig.WheelRadius * 0.9f,
                -wheel.point.up,
                out wheel.hit,
                castDistance);

            if (!wheel.isGrounded)
            {
                wheel.compression = 0f;
                return;
            }

            float length = wheel.hit.distance - _roverConfig.WheelRadius;
            float offset = _roverConfig.SuspensionRestLength - length;
            wheel.compression = Mathf.Clamp01(offset / _roverConfig.SuspensionRange);
        }

        private void ApplySuspension(WheelView wheel)
        {
            Vector3 pointVelocity = _rigidbody.GetPointVelocity(wheel.point.position);
            float verticalVelocity = Vector3.Dot(wheel.point.up, pointVelocity);

            float length = wheel.hit.distance - _roverConfig.WheelRadius;
            float offset = _roverConfig.SuspensionRestLength - length;

            float springForce = offset * _roverConfig.SpringStrength;
            float damperForce = -verticalVelocity * _roverConfig.DamperStrength;

            Vector3 force = wheel.point.up * (springForce + damperForce);
            _rigidbody.AddForceAtPosition(force, wheel.point.position, ForceMode.Force);
        }

        private void ApplyDrive(WheelView wheel, float power)
        {
            Vector3 driveDir = Vector3.ProjectOnPlane(wheel.point.forward, wheel.hit.normal).normalized;
            Vector3 force = driveDir * (power * _roverConfig.MotorForce);

            _rigidbody.AddForceAtPosition(force, wheel.point.position, ForceMode.Force);
        }

        private void ApplyLateralGrip(WheelView wheel)
        {
            Vector3 velocity = _rigidbody.GetPointVelocity(wheel.point.position);
            float sideSpeed = Vector3.Dot(wheel.point.right, velocity);

            Vector3 sideForce = -wheel.point.right * (sideSpeed * _roverConfig.LateralGrip);
            _rigidbody.AddForceAtPosition(sideForce, wheel.point.position, ForceMode.Force);
        }

        private void LimitSpeed()
        {
            float speed = _rigidbody.linearVelocity.magnitude;
            if (speed <= _roverConfig.MaxSpeed)
                return;

            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _roverConfig.MaxSpeed;
        }

        private void UpdateWheelVisual(WheelView wheel, float power)
        {
            if (wheel.isGrounded)
            {
                Vector3 pos = wheel.hit.point + wheel.hit.normal * _roverConfig.WheelRadius;
                wheel.visual.position = pos;
            }

            float speed = power * 360f * Time.fixedDeltaTime;
            wheel.wheelRotation += speed;
            wheel.visual.rotation = wheel.point.rotation * Quaternion.Euler(wheel.wheelRotation, 0f, 0f);
        }
    }
}