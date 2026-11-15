using UnityEngine;

namespace MobileFoundation
{
    public class SandboxSession : MonoBehaviour
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
        private void OnEnable()
        {
            _previousTimeScale = Time.timeScale; _previousFrameRate = Application.targetFrameRate;
            Application.targetFrameRate = _targetFrameRate;
            if (_input != null) { _input.PauseRequested += TogglePause; _input.SetBlocked(false); }
            if (_pauseButton != null) _pauseButton.onClick.AddListener(Pause);
            if (_resumeButton != null) _resumeButton.onClick.AddListener(Resume);
            if (_resetButton != null) _resetButton.onClick.AddListener(ResetPlayer);
            Paused = false; if (_pauseOverlay != null) _pauseOverlay.SetActive(false);
        }
        private void OnDisable()
        {
            if (_input != null) { _input.PauseRequested -= TogglePause; _input.SetBlocked(false); }
            if (_pauseButton != null) _pauseButton.onClick.RemoveListener(Pause);
            if (_resumeButton != null) _resumeButton.onClick.RemoveListener(Resume);
            if (_resetButton != null) _resetButton.onClick.RemoveListener(ResetPlayer);
            Time.timeScale = _previousTimeScale; Application.targetFrameRate = _previousFrameRate;
            Paused = false; if (_pauseOverlay != null) _pauseOverlay.SetActive(false);
        }
        private void Update() { if (!Paused && _motor != null && _motor.transform.position.y < _killHeight) ResetPlayer(); }
        private void OnApplicationFocus(bool focused) { if (!focused) Pause(); }
        private void OnApplicationPause(bool paused) { if (paused) Pause(); }
        private void TogglePause() { if (Paused) Resume(); else Pause(); }
        public void Pause()
        {
            if (Paused) return;
            _previousTimeScale = Time.timeScale; Paused = true;
            if (_input != null) _input.SetBlocked(true);
            Time.timeScale = 0f; if (_pauseOverlay != null) _pauseOverlay.SetActive(true);
        }
        public void Resume()
        {
            if (!Paused) return;
            Paused = false; Time.timeScale = _previousTimeScale;
            if (_input != null) _input.SetBlocked(false);
            if (_pauseOverlay != null) _pauseOverlay.SetActive(false);
        }
        public void ResetPlayer()
        {
            if (_input != null) _input.ClearInput();
            if (_motor != null) _motor.ResetTo(_spawn);
            if (_camera != null) _camera.Snap();
        }
    }
}
