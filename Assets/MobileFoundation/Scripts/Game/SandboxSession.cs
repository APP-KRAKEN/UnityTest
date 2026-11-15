using UnityEngine;

namespace MobileFoundation
{
    public sealed class SandboxSession : MonoBehaviour
    {
        [SerializeField] private PlayerInputAdapter _input;
        [SerializeField] private ThirdPersonMotor _motor;
        [SerializeField] private ThirdPersonOrbitCamera _camera;
        [SerializeField] private Transform _spawn;
        [SerializeField] private GameObject _pauseOverlay;
        [SerializeField] private UnityEngine.UI.Button _pauseButton;
        [SerializeField] private UnityEngine.UI.Button _resumeButton;
        [SerializeField] private UnityEngine.UI.Button _resetButton;
        [SerializeField] private float _killHeight = -12f;
        [SerializeField] private int _targetFrameRate = 60;
        private float _previousTimeScale;
        private int _previousFrameRate;
        public bool Paused { get; private set; }
        private void Awake() { _previousTimeScale = Time.timeScale; _previousFrameRate = Application.targetFrameRate; Application.targetFrameRate = _targetFrameRate; }
        private void OnEnable()
        {
            _input.PauseRequested += TogglePause;
            _pauseButton.onClick.AddListener(TogglePause);
            _resumeButton.onClick.AddListener(Resume);
            _resetButton.onClick.AddListener(ResetPlayer);
        }
        private void Start() { ResetPlayer(); }
        private void Update() { if (!Paused && _motor.transform.position.y < _killHeight) ResetPlayer(); }
        public void TogglePause() { SetPaused(!Paused); }
        public void Resume() { SetPaused(false); }
        public void SetPaused(bool value)
        {
            if (Paused == value) return;
            Paused = value;
            _input.SetBlocked(value);
            _pauseOverlay.SetActive(value);
            Time.timeScale = value ? 0 : _previousTimeScale;
        }
        public void ResetPlayer()
        {
            _input.Clear();
            _motor.ResetAt(_spawn.position, _spawn.rotation);
            _camera.ResetView();
        }
        private void OnApplicationPause(bool paused) { if (paused) SetPaused(true); }
        private void OnApplicationFocus(bool focused) { if (!focused) SetPaused(true); }
        private void OnDisable()
        {
            if (_input != null) { _input.PauseRequested -= TogglePause; _input.SetBlocked(false); }
            if (_pauseButton != null) _pauseButton.onClick.RemoveListener(TogglePause);
            if (_resumeButton != null) _resumeButton.onClick.RemoveListener(Resume);
            if (_resetButton != null) _resetButton.onClick.RemoveListener(ResetPlayer);
            Time.timeScale = _previousTimeScale;
            Application.targetFrameRate = _previousFrameRate;
        }
    }
}
