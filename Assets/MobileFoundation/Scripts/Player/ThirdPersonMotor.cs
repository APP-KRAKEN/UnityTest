using UnityEngine;

namespace MobileFoundation
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputAdapter))]
    public sealed class ThirdPersonMotor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _cameraHeading;
        [SerializeField] private Transform _visual;
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;
        [SerializeField] private float _acceleration = 20f;
        [SerializeField] private float _turnSpeed = 720f;
        [Header("Jump")]
        [SerializeField] private float _jumpHeight = 1.35f;
        [SerializeField] private float _gravity = -25f;
        [SerializeField] private float _groundStickSpeed = -2f;
        [SerializeField] private float _coyoteTime = 0.12f;
        [SerializeField] private float _jumpBuffer = 0.12f;
        private CharacterController _controller;
        private PlayerInputAdapter _input;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;
        private float _lastGrounded = float.NegativeInfinity;
        private float _lastJumpPressed = float.NegativeInfinity;
        private bool _jumpUsed;
        public bool Grounded => _controller != null && _controller.isGrounded;
        public float VerticalVelocity => _verticalVelocity;
        public void SetCameraHeading(Transform heading) { _cameraHeading = heading; }
        private void Awake() { _controller = GetComponent<CharacterController>(); _input = GetComponent<PlayerInputAdapter>(); }
        private void OnEnable() { _input.Cleared += ClearIntent; }
        private void OnDisable() { if (_input != null) _input.Cleared -= ClearIntent; }
        private void ClearIntent() { _lastJumpPressed = float.NegativeInfinity; _horizontalVelocity = Vector3.zero; }
        private void Update()
        {
            if (_input.Blocked || Time.deltaTime <= 0) return;
            float dt = Time.deltaTime;
            if (_input.ConsumeJump()) _lastJumpPressed = Time.time;
            if (_controller.isGrounded && _verticalVelocity <= 0)
            {
                _lastGrounded = Time.time;
                _jumpUsed = false;
                _verticalVelocity = _groundStickSpeed;
            }
            if (!_jumpUsed && Time.time - _lastGrounded <= _coyoteTime && Time.time - _lastJumpPressed <= _jumpBuffer)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _jumpUsed = true;
                _lastJumpPressed = _lastGrounded = float.NegativeInfinity;
            }
            Vector2 move = _input.Move;
            Vector3 forward = Vector3.ProjectOnPlane(_cameraHeading != null ? _cameraHeading.forward : Vector3.forward, Vector3.up).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            Vector3 direction = forward * move.y + right * move.x;
            Vector3 desired = direction * (_input.Sprint ? _sprintSpeed : _moveSpeed);
            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, desired, _acceleration * dt);
            if (_visual != null && direction.sqrMagnitude > 0.001f)
                _visual.rotation = Quaternion.RotateTowards(_visual.rotation, Quaternion.LookRotation(direction), _turnSpeed * dt);
            float verticalDisplacement = _verticalVelocity * dt + 0.5f * _gravity * dt * dt;
            _verticalVelocity += _gravity * dt;
            var collision = _controller.Move(_horizontalVelocity * dt + Vector3.up * verticalDisplacement);
            if ((collision & CollisionFlags.Above) != 0 && _verticalVelocity > 0) _verticalVelocity = 0;
        }
        public void ResetAt(Vector3 position, Quaternion rotation)
        {
            if (_controller == null) _controller = GetComponent<CharacterController>();
            _controller.enabled = false;
            transform.SetPositionAndRotation(position, rotation);
            _controller.enabled = true;
            _horizontalVelocity = Vector3.zero;
            _verticalVelocity = 0;
            _lastGrounded = _lastJumpPressed = float.NegativeInfinity;
            _jumpUsed = false;
            if (_visual != null) _visual.rotation = rotation;
            if (_input != null) _input.Clear();
        }
    }
}
