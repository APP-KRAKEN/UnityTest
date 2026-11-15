using UnityEngine;

namespace MobileFoundation
{
    public class HoldInputButton : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler
    {
        private enum Command { Jump = 0, Sprint = 1 }
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private Command _command;
        [SerializeField] private UnityEngine.UI.Graphic _graphic;
        [SerializeField] private Color _normal = new Color(0.08f, 0.16f, 0.22f, 0.88f);
        [SerializeField] private Color _pressed = new Color(0.1f, 0.65f, 0.7f, 1f);
        private int? _pointer;
        private void OnEnable() { if (_input != null) _input.InputCleared += ResetControl; }
        private void OnDisable() { if (_input != null) _input.InputCleared -= ResetControl; ResetControl(); }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || data.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = data.pointerId;
            if (_graphic != null) _graphic.color = _pressed;
            if (_command == Command.Jump) _input.PressJump(); else _input.SetTouchSprint(true);
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData data) { if (_pointer == data.pointerId) ResetControl(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ResetControl(); }
        private void OnApplicationPause(bool paused) { if (paused) ResetControl(); }
        private void ResetControl()
        {
            _pointer = null;
            if (_graphic != null) _graphic.color = _normal;
            if (_input != null && _command == Command.Sprint) _input.SetTouchSprint(false);
        }
    }
}
