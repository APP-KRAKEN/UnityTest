using UnityEngine;

namespace MobileFoundation
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaPanel : MonoBehaviour
    {
        private RectTransform _rect;
        private Rect _lastArea;
        private Vector2Int _lastSize;
        private void Awake() { _rect = (RectTransform)transform; }
        private void OnEnable() { Apply(); }
        private void Update() { if (_lastArea != Screen.safeArea || _lastSize.x != Screen.width || _lastSize.y != Screen.height) Apply(); }
        private void Apply()
        {
            if (_rect == null || Screen.width <= 0 || Screen.height <= 0) return;
            _lastArea = Screen.safeArea; _lastSize = new Vector2Int(Screen.width, Screen.height);
            Vector2 size = _lastSize;
            _rect.anchorMin = _lastArea.position / size;
            _rect.anchorMax = (_lastArea.position + _lastArea.size) / size;
            _rect.offsetMin = _rect.offsetMax = Vector2.zero;
        }
    }
}
