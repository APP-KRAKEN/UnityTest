using UnityEngine;

namespace MobileFoundation
{
    public sealed class MobileJoystick : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IPointerUpHandler
    {
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _radius = 90;
        [SerializeField] private float _deadZone = 0.08f;
        private RectTransform _rect;
        private int? _pointer;
        public void Configure(PlayerInputAdapter input) { _input = input; }
        private void Awake() { _rect = (RectTransform)transform; }
        private void OnEnable() { if (_input != null) _input.Cleared += ResetPointer; }
        private void OnDisable() { if (_input != null) _input.Cleared -= ResetPointer; ResetPointer(); }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData e)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || e.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = e.pointerId; OnDrag(e);
        }
        public void OnDrag(UnityEngine.EventSystems.PointerEventData e)
        {
            if (_pointer != e.pointerId) return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, e.position, e.pressEventCamera, out Vector2 local);
            var value = Vector2.ClampMagnitude((local - _rect.rect.center) / _radius, 1);
            _handle.anchoredPosition = value * _radius;
            _input.SetTouchMove(value.magnitude < _deadZone ? Vector2.zero : value);
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData e) { if (_pointer == e.pointerId) ResetPointer(); }
        private void ResetPointer() { _pointer = null; if (_handle != null) _handle.anchoredPosition = Vector2.zero; if (_input != null) _input.SetTouchMove(Vector2.zero); }
    }
}
