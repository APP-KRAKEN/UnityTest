using UnityEngine;

namespace MobileFoundation
{
    public sealed class ThirdPersonOrbitCamera : MonoBehaviour
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
        public void Configure(Transform target, PlayerInputAdapter input) { _target = target; _input = input; }
        private void Start() { ResetView(); }
        public void ResetView()
        {
            if (_target == null) return;
            _pivot = _target.position; _velocity = Vector3.zero;
            _yaw = _target.eulerAngles.y; _pitch = _initialPitch; _currentDistance = _distance;
            Place(true);
        }
        private void LateUpdate()
        {
            if (_target == null || _input == null || _input.Blocked || Time.deltaTime <= 0) return;
            Vector2 look = _input.ConsumeLook();
            _yaw = Mathf.Repeat(_yaw + look.x * _sensitivity, 360);
            _pitch = Mathf.Clamp(_pitch - look.y * _sensitivity, _minPitch, _maxPitch);
            _pivot = Vector3.SmoothDamp(_pivot, _target.position, ref _velocity, _followSmoothTime);
            Place(false);
        }
        private void Place(bool instant)
        {
            var rotation = Quaternion.Euler(_pitch, _yaw, 0);
            var backwards = rotation * Vector3.back;
            float allowedDistance = _distance;
            if (Physics.SphereCast(_pivot, _collisionRadius, backwards, out RaycastHit hit, _distance, _environmentMask, QueryTriggerInteraction.Ignore))
                allowedDistance = Mathf.Max(0, hit.distance - _collisionPadding);
            _currentDistance = instant || allowedDistance < _currentDistance ? allowedDistance : Mathf.MoveTowards(_currentDistance, allowedDistance, _recoverySpeed * Time.deltaTime);
            transform.SetPositionAndRotation(_pivot + backwards * _currentDistance, rotation);
        }
    }
}
