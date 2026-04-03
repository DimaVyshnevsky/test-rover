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

                if (!wheel.IsGrounded)
                    continue;

                ApplySuspension(wheel);

                float power = wheel.IsLeft ? left : right;

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

            wheel.IsGrounded = Physics.SphereCast(
                wheel.Point.position,
                _roverConfig.WheelRadius * 0.9f,
                -wheel.Point.up,
                out wheel.Hit,
                castDistance,
                _roverConfig.GroundMask,
                QueryTriggerInteraction.Ignore);

            if (!wheel.IsGrounded)
            {
                wheel.Compression = 0f;
                return;
            }

            float length = wheel.Hit.distance - _roverConfig.WheelRadius;
            float offset = _roverConfig.SuspensionRestLength - length;
            wheel.Compression = Mathf.Clamp01(offset / _roverConfig.SuspensionRange);
        }

        private void ApplySuspension(WheelView wheel)
        {
            Vector3 pointVelocity = _rigidbody.GetPointVelocity(wheel.Point.position);
            float verticalVelocity = Vector3.Dot(wheel.Point.up, pointVelocity);

            float length = wheel.Hit.distance - _roverConfig.WheelRadius;
            float offset = _roverConfig.SuspensionRestLength - length;

            float springForce = offset * _roverConfig.SpringStrength;
            float damperForce = -verticalVelocity * _roverConfig.DamperStrength;
            float totalForce = Mathf.Max(0f, springForce + damperForce);

            Vector3 force = wheel.Point.up * totalForce;
            _rigidbody.AddForceAtPosition(force, wheel.Point.position, ForceMode.Force);
        }

        private void ApplyDrive(WheelView wheel, float power)
        {
            Vector3 driveDir = Vector3.ProjectOnPlane(wheel.Point.forward, wheel.Hit.normal).normalized;
            Vector3 force = driveDir * (power * _roverConfig.MotorForce);

            _rigidbody.AddForceAtPosition(force, wheel.Point.position, ForceMode.Force);
        }

        private void ApplyLateralGrip(WheelView wheel)
        {
            Vector3 velocity = _rigidbody.GetPointVelocity(wheel.Point.position);

            Vector3 driveDir = Vector3.ProjectOnPlane(wheel.Point.forward, wheel.Hit.normal).normalized;
            Vector3 lateralDir = Vector3.Cross(wheel.Hit.normal, driveDir).normalized;

            float sideSpeed = Vector3.Dot(velocity, lateralDir);
            Vector3 sideForce = -lateralDir * (sideSpeed * _roverConfig.LateralGrip);

            _rigidbody.AddForceAtPosition(sideForce, wheel.Point.position, ForceMode.Force);
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
            if (wheel.IsGrounded)
            {
                Vector3 pos = wheel.Hit.point + wheel.Hit.normal * _roverConfig.WheelRadius;
                wheel.Visual.position = pos;
            }

            float speed = power * 360f * Time.fixedDeltaTime;
            wheel.WheelRotation += speed;
            wheel.Visual.rotation = wheel.Point.rotation * Quaternion.Euler(wheel.WheelRotation, 0f, 0);
        }
    }
}