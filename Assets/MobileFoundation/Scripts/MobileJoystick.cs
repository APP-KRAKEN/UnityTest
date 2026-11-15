using UnityEngine;

namespace MobileFoundation
{
    public class MobileJoystick : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IInitializePotentialDragHandler
    {
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private RectTransform _handle;
        [SerializeField] private float _radius = 90f;
        [SerializeField] private float _deadZone = 0.08f;
        private RectTransform _rect;
        private int? _pointer;
        private void Awake() { _rect = (RectTransform)transform; }
        private void OnEnable() { if (_input != null) _input.InputCleared += ResetControl; }
        private void OnDisable() { if (_input != null) _input.InputCleared -= ResetControl; ResetControl(); }
        public void OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData data) { data.useDragThreshold = false; }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || data.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = data.pointerId; OnDrag(data);
        }
        public void OnDrag(UnityEngine.EventSystems.PointerEventData data)
        {
            if (_pointer != data.pointerId || _input == null || _input.Blocked) return;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, data.position, data.pressEventCamera, out Vector2 local)) return;
            Vector2 offset = Vector2.ClampMagnitude(local - _rect.rect.center, Mathf.Max(1f, _radius));
            if (_handle != null) _handle.anchoredPosition = offset;
            Vector2 value = offset / Mathf.Max(1f, _radius);
            _input.SetTouchMove(value.magnitude < _deadZone ? Vector2.zero : value);
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData data) { if (_pointer == data.pointerId) ResetControl(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ResetControl(); }
        private void OnApplicationPause(bool paused) { if (paused) ResetControl(); }
        private void ResetControl() { _pointer = null; if (_handle != null) _handle.anchoredPosition = Vector2.zero; if (_input != null) _input.SetTouchMove(Vector2.zero); }
    }
}
