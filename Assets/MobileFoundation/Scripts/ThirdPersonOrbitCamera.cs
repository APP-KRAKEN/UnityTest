using UnityEngine;

namespace MobileFoundation
{
    public class ThirdPersonOrbitCamera : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private LayerMask _environmentMask = 1;
        [SerializeField] private float _distance = 5.5f;
        [SerializeField] private float _sensitivity = 0.12f;
        [SerializeField] private float _minPitch = -25f;
        [SerializeField] private float _maxPitch = 65f;
        [SerializeField] private float _initialPitch = 18f;
        [SerializeField] private float _followSmoothTime = 0.06f;
        [SerializeField] private float _recoverySpeed = 8f;
        [SerializeField] private float _collisionRadius = 0.2f;
        [SerializeField] private float _collisionPadding = 0.08f;
        private Vector3 _pivot, _velocity;
        private float _yaw, _pitch, _currentDistance;
        private void Start() { Snap(); }
        private void LateUpdate()
        {
            if (_target == null || _input == null || _input.Blocked || Time.deltaTime <= 0f) return;
            Vector2 look = _input.ConsumeLook();
            _yaw += look.x * _sensitivity;
            _pitch = Mathf.Clamp(_pitch - look.y * _sensitivity, _minPitch, _maxPitch);
            _pivot = Vector3.SmoothDamp(_pivot, _target.position, ref _velocity, _followSmoothTime);
            ApplyPose(Time.deltaTime, false);
        }
        private void ApplyPose(float deltaTime, bool snap)
        {
            var rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            Vector3 backward = rotation * Vector3.back;
            float desired = _distance;
            if (Physics.SphereCast(_pivot, _collisionRadius, backward, out RaycastHit hit, _distance, _environmentMask, QueryTriggerInteraction.Ignore)) desired = Mathf.Max(0f, hit.distance - _collisionPadding);
            _currentDistance = snap || desired < _currentDistance ? desired : Mathf.MoveTowards(_currentDistance, desired, _recoverySpeed * deltaTime);
            transform.SetPositionAndRotation(_pivot + backward * _currentDistance, rotation);
        }
        public void Snap()
        {
            if (_target == null) return;
            _pivot = _target.position; _velocity = Vector3.zero;
            _yaw = _target.root.eulerAngles.y; _pitch = _initialPitch;
            _currentDistance = _distance; ApplyPose(0f, true);
        }
    }
}
