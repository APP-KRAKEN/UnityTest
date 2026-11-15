using UnityEngine;

namespace MobileFoundation
{
    public sealed class HoldInputButton : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler
    {
        public enum Command { Jump, Sprint }
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private Command _command;
        [SerializeField] private UnityEngine.UI.Image _graphic;
        [SerializeField] private Color _normal = new Color(0.08f, 0.16f, 0.22f, 0.88f);
        [SerializeField] private Color _pressed = new Color(0.1f, 0.65f, 0.7f, 1);
        private int? _pointer;
        public void Configure(PlayerInputAdapter input) { _input = input; }
        private void OnEnable() { if (_input != null) _input.Cleared += ResetPointer; }
        private void OnDisable() { if (_input != null) _input.Cleared -= ResetPointer; ResetPointer(); }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData e)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || e.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = e.pointerId;
            if (_command == Command.Jump) _input.RequestJump(); else _input.SetTouchSprint(true);
            if (_graphic != null) _graphic.color = _pressed;
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData e) { if (_pointer == e.pointerId) ResetPointer(); }
        private void ResetPointer()
        {
            _pointer = null;
            if (_command == Command.Sprint && _input != null) _input.SetTouchSprint(false);
            if (_graphic != null) _graphic.color = _normal;
        }
    }
}
