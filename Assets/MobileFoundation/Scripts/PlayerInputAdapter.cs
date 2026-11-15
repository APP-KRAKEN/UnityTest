using UnityEngine;

namespace MobileFoundation
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInputAdapter : MonoBehaviour
    {
        [SerializeField] private UnityEngine.InputSystem.InputActionAsset _actions;
        private UnityEngine.InputSystem.InputActionAsset _runtimeActions;
        private UnityEngine.InputSystem.InputActionMap _player;
        private Vector2 _desktopMove, _touchMove, _desktopLook, _touchLook;
        private bool _desktopSprint, _touchSprint, _lookHeld, _jump, _blocked;
        public event System.Action PauseRequested;
        public event System.Action InputCleared;
        public bool Blocked => _blocked;
        public Vector2 Move => _blocked ? Vector2.zero : Vector2.ClampMagnitude(_desktopMove + _touchMove, 1f);
        public bool Sprint => !_blocked && (_desktopSprint || _touchSprint);

        private void Awake()
        {
            if (_actions == null) { Debug.LogError("Player Input Actions asset is not assigned.", this); enabled = false; return; }
            _runtimeActions = Instantiate(_actions);
            _player = _runtimeActions.FindActionMap("Player", true);
            var move = _player.FindAction("Move", true);
            move.performed += OnMove; move.canceled += OnMove;
            var look = _player.FindAction("Look", true);
            look.performed += OnLook; look.canceled += OnLook;
            var hold = _player.FindAction("LookHold", true);
            hold.performed += OnLookHold; hold.canceled += OnLookHold;
            var sprint = _player.FindAction("Sprint", true);
            sprint.performed += OnSprint; sprint.canceled += OnSprint;
            _player.FindAction("Jump", true).performed += OnJump;
            _player.FindAction("Pause", true).performed += OnPause;
        }
        private void OnEnable() { if (_player != null) _player.Enable(); }
        private void OnDisable() { if (_player != null) _player.Disable(); ClearInput(); }
        private void OnDestroy() { if (_runtimeActions != null) Destroy(_runtimeActions); }
        private void OnApplicationFocus(bool focused) { if (!focused) ClearInput(); }
        private void OnApplicationPause(bool paused) { if (paused) ClearInput(); }
        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context) { _desktopMove = _blocked ? Vector2.zero : context.ReadValue<Vector2>(); }
        private void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext context) { _desktopLook = !_blocked && _lookHeld ? context.ReadValue<Vector2>() : Vector2.zero; }
        private void OnLookHold(UnityEngine.InputSystem.InputAction.CallbackContext context) { _lookHeld = !_blocked && context.ReadValueAsButton(); if (!_lookHeld) _desktopLook = Vector2.zero; }
        private void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext context) { _desktopSprint = !_blocked && context.ReadValueAsButton(); }
        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context) { PressJump(); }
        private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext context) { PauseRequested?.Invoke(); }
        public void SetTouchMove(Vector2 value) { _touchMove = _blocked ? Vector2.zero : Vector2.ClampMagnitude(value, 1f); }
        public void AddTouchLook(Vector2 value) { if (!_blocked) _touchLook += value; }
        public void SetTouchSprint(bool value) { _touchSprint = !_blocked && value; }
        public void PressJump() { if (!_blocked) _jump = true; }
        public bool ConsumeJump() { bool pressed = !_blocked && _jump; _jump = false; return pressed; }
        public Vector2 ConsumeLook() { var look = _blocked ? Vector2.zero : _desktopLook + _touchLook; _desktopLook = _touchLook = Vector2.zero; return look; }
        public void SetBlocked(bool blocked) { _blocked = blocked; ClearInput(); }
        public void ClearInput()
        {
            _desktopMove = _touchMove = _desktopLook = _touchLook = Vector2.zero;
            _desktopSprint = _touchSprint = _lookHeld = _jump = false;
            InputCleared?.Invoke();
        }
    }
}
