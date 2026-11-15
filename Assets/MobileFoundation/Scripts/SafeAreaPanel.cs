using UnityEngine;

namespace MobileFoundation
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaPanel : MonoBehaviour
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
            _rect.anchorMin = new Vector2(_lastArea.xMin / Screen.width, _lastArea.yMin / Screen.height);
            _rect.anchorMax = new Vector2(_lastArea.xMax / Screen.width, _lastArea.yMax / Screen.height);
            _rect.offsetMin = _rect.offsetMax = Vector2.zero;
        }
    }
}
