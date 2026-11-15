using UnityEngine;

namespace MobileFoundation
{
    public sealed class TouchLookArea : MonoBehaviour, UnityEngine.EventSystems.IPointerDownHandler, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IPointerUpHandler
    {
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private float _referenceHeight = 1080f;
        private int? _pointer;
        private Vector2 _previous;
        public void Configure(PlayerInputAdapter input) { _input = input; }
        private void OnEnable() { if (_input != null) _input.Cleared += ResetPointer; }
        private void OnDisable() { if (_input != null) _input.Cleared -= ResetPointer; ResetPointer(); }
        public void OnPointerDown(UnityEngine.EventSystems.PointerEventData e)
        {
            if (_pointer.HasValue || _input == null || _input.Blocked || e.button != UnityEngine.EventSystems.PointerEventData.InputButton.Left) return;
            _pointer = e.pointerId; _previous = e.position;
        }
        public void OnDrag(UnityEngine.EventSystems.PointerEventData e)
        {
            if (_pointer != e.pointerId) return;
            _input.AddTouchLook((e.position - _previous) * (_referenceHeight / Mathf.Max(1, Screen.height)));
            _previous = e.position;
        }
        public void OnPointerUp(UnityEngine.EventSystems.PointerEventData e) { if (_pointer == e.pointerId) ResetPointer(); }
        private void ResetPointer() { _pointer = null; }
    }
}
