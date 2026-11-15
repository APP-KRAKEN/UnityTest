using UnityEngine;

namespace MobileFoundation
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputAdapter))]
    public class ThirdPersonMotor : MonoBehaviour
    {
        [SerializeField] private Transform _cameraHeading;
        [SerializeField] private Transform _visual;
        [SerializeField] private float _moveSpeed = 4f;
        [SerializeField] private float _sprintSpeed = 7f;
        [SerializeField] private float _acceleration = 20f;
        [SerializeField] private float _turnSpeed = 720f;
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
        private void Awake() { _controller = GetComponent<CharacterController>(); _input = GetComponent<PlayerInputAdapter>(); }
        private void OnEnable() { if (_input != null) _input.InputCleared += ClearMotion; }
        private void OnDisable() { if (_input != null) _input.InputCleared -= ClearMotion; }
        private void Update()
        {
            if (_input == null || _input.Blocked || !_controller.enabled || Time.deltaTime <= 0f) return;
            float dt = Time.deltaTime;
            if (Grounded && _verticalVelocity <= 0f)
            {
                _lastGrounded = Time.time; _jumpUsed = false; _verticalVelocity = _groundStickSpeed;
            }
            if (_input.ConsumeJump()) _lastJumpPressed = Time.time;
            if (!_jumpUsed && Time.time - _lastGrounded <= _coyoteTime && Time.time - _lastJumpPressed <= _jumpBuffer)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                _lastJumpPressed = float.NegativeInfinity; _jumpUsed = true;
            }
            Vector2 move = _input.Move;
            float yaw = _cameraHeading == null ? transform.eulerAngles.y : _cameraHeading.eulerAngles.y;
            Vector3 direction = Quaternion.Euler(0f, yaw, 0f) * new Vector3(move.x, 0f, move.y);
            _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, direction * (_input.Sprint ? _sprintSpeed : _moveSpeed), _acceleration * dt);
            if (_visual != null && direction.sqrMagnitude > 0.001f) _visual.rotation = Quaternion.RotateTowards(_visual.rotation, Quaternion.LookRotation(direction), _turnSpeed * dt);
            _verticalVelocity += _gravity * dt;
            var flags = _controller.Move((_horizontalVelocity + Vector3.up * _verticalVelocity) * dt);
            if ((flags & CollisionFlags.Above) != 0 && _verticalVelocity > 0f) _verticalVelocity = 0f;
        }
        private void ClearMotion() { _horizontalVelocity = Vector3.zero; _lastJumpPressed = float.NegativeInfinity; }
        public void ResetTo(Transform spawn)
        {
            if (spawn == null || _controller == null) return;
            bool wasEnabled = _controller.enabled; _controller.enabled = false;
            transform.SetPositionAndRotation(spawn.position, spawn.rotation);
            _controller.enabled = wasEnabled;
            _horizontalVelocity = Vector3.zero; _verticalVelocity = 0f;
            _lastGrounded = _lastJumpPressed = float.NegativeInfinity; _jumpUsed = false;
            if (_visual != null) _visual.localRotation = Quaternion.identity;
        }
    }
}
