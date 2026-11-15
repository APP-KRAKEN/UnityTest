using UnityEngine;

namespace MobileFoundation
{
    public class TouchLookArea : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IPointerUpHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IInitializePotentialDragHandler
    {
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private float _referenceHeight = 1080f;
        private Vector2 _previous;
        private int? _pointer;
        private void OnEnable() { if (_input != null) _input.InputCleared += ResetControl; }
        private void OnDisable() { if (_input != null) _input.InputCleared -= ResetControl; ResetControl(); }
        public void OnInitializePotentialDrag(UnityEngine.EventSystems.PointerEventData data) { data.useDragThreshold = false; }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData data)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || data.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = data.pointerId; _previous = data.position;
        }
        public void OnDrag(UnityEngine.EventSystems.PointerEventData data)
        {
            if (_pointer != data.pointerId || _input == null || _input.Blocked) return;
            _input.AddTouchLook((data.position - _previous) * (_referenceHeight / Mathf.Max(1, Screen.height)));
            _previous = data.position;
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData data) { if (_pointer == data.pointerId) ResetControl(); }
        private void OnApplicationFocus(bool focused) { if (!focused) ResetControl(); }
        private void OnApplicationPause(bool paused) { if (paused) ResetControl(); }
        private void ResetControl() { _pointer = null; _previous = Vector2.zero; }
    }
}
