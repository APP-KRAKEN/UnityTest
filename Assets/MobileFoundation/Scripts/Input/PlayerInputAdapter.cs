using System;
using UnityEngine;

namespace MobileFoundation
{
    public sealed class PlayerInputAdapter : MonoBehaviour
    {
        [SerializeField] private UnityEngine.InputSystem.InputActionAsset _actions;
        private UnityEngine.InputSystem.InputActionAsset _runtimeActions;
        private UnityEngine.InputSystem.InputActionMap _player;
        private Vector2 _desktopMove, _touchMove, _desktopLook, _touchLook;
        private bool _desktopSprint, _touchSprint, _lookHeld, _jump;
        private bool _blocked;
        public event Action PauseRequested;
        public event Action Cleared;
        public Vector2 Move => _blocked ? Vector2.zero : Vector2.ClampMagnitude(_touchMove.sqrMagnitude > 0 ? _touchMove : _desktopMove, 1);
        public bool Sprint => !_blocked && (_desktopSprint || _touchSprint);
        public bool Blocked => _blocked;

        private void Awake()
        {
            _runtimeActions = Instantiate(_actions);
            _player = _runtimeActions.FindActionMap("Player", true);
            Bind("Move", OnMove); Bind("Look", OnLook); Bind("LookHeld", OnLookHeld);
            Bind("Sprint", OnSprint); Bind("Jump", OnJump); Bind("Pause", OnPause);
        }
        private void Bind(string name, Action<UnityEngine.InputSystem.InputAction.CallbackContext> callback)
        {
            var action = _player.FindAction(name, true);
            action.performed += callback;
            action.canceled += callback;
        }
        private void OnEnable() { if (_player != null) _player.Enable(); }
        private void OnDisable() { if (_player != null) _player.Disable(); Clear(); }
        private void OnDestroy() { if (_runtimeActions != null) Destroy(_runtimeActions); }
        private void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext c) { _desktopMove = _blocked ? Vector2.zero : c.ReadValue<Vector2>(); }
        private void OnLook(UnityEngine.InputSystem.InputAction.CallbackContext c) { _desktopLook = !_blocked && _lookHeld ? c.ReadValue<Vector2>() : Vector2.zero; }
        private void OnLookHeld(UnityEngine.InputSystem.InputAction.CallbackContext c) { _lookHeld = !_blocked && c.ReadValueAsButton(); if (!_lookHeld) _desktopLook = Vector2.zero; }
        private void OnSprint(UnityEngine.InputSystem.InputAction.CallbackContext c) { _desktopSprint = !_blocked && c.ReadValueAsButton(); }
        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext c) { if (!_blocked && c.performed) _jump = true; }
        private void OnPause(UnityEngine.InputSystem.InputAction.CallbackContext c) { if (c.performed) PauseRequested?.Invoke(); }
        public void SetTouchMove(Vector2 value) { _touchMove = _blocked ? Vector2.zero : Vector2.ClampMagnitude(value, 1); }
        public void SetTouchSprint(bool value) { _touchSprint = !_blocked && value; }
        public void AddTouchLook(Vector2 delta) { if (!_blocked) _touchLook += delta; }
        public void RequestJump() { if (!_blocked) _jump = true; }
        public bool ConsumeJump() { bool value = _jump; _jump = false; return value && !_blocked; }
        public Vector2 ConsumeLook()
        {
            var value = _blocked ? Vector2.zero : _desktopLook + _touchLook;
            _desktopLook = _touchLook = Vector2.zero;
            return value;
        }
        public void SetBlocked(bool value) { _blocked = value; Clear(); }
        public void Clear()
        {
            _desktopMove = _touchMove = _desktopLook = _touchLook = Vector2.zero;
            _desktopSprint = _touchSprint = _lookHeld = _jump = false;
            Cleared?.Invoke();
        }
        private void OnApplicationFocus(bool focus) { if (!focus) Clear(); }
    }
}
