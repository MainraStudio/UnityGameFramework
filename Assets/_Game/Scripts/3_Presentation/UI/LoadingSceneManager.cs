using UnityEngine;
using UnityEngine.UI;
using TMPro;
using _Game.Scripts.Core.Interfaces;
using VContainer;

namespace _Game.Scripts.Presentation.UI
{
    public class LoadingSceneManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Slider _progressBar;
        [SerializeField] private TextMeshProUGUI _progressText;
        [SerializeField] private TextMeshProUGUI _loadingText;
        [SerializeField] private float _minLoadingTime = 1f;

        private ISceneLoader _sceneLoader;
        private float _loadingStartTime;
        private bool _isLoading;

        [Inject]
        private void Construct(ISceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Start()
        {
            StartLoading();
        }

        private void Update()
        {
            if (!_isLoading) return;

            UpdateLoadingProgress();
        }

        private void StartLoading()
        {
            _isLoading = true;
            _loadingStartTime = Time.time;
            _loadingText.text = "Loading...";
            
            // Mulai loading scene target
            var operation = _sceneLoader.LoadTargetSceneAsync();
            if (operation != null)
            {
                operation.allowSceneActivation = false;
            }
        }

        private void UpdateLoadingProgress()
        {
            float progress = _sceneLoader.GetLoadingProgress();
            float elapsedTime = Time.time - _loadingStartTime;
            
            // Pastikan loading screen ditampilkan minimal _minLoadingTime
            if (elapsedTime < _minLoadingTime)
            {
                progress = Mathf.Min(progress, elapsedTime / _minLoadingTime);
            }

            _progressBar.value = progress;
            _progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            // Aktifkan scene jika loading sudah selesai dan waktu minimum sudah terpenuhi
            if (progress >= 1f && elapsedTime >= _minLoadingTime)
            {
                _sceneLoader.ActivateLoadedScene();
            }
        }
    }
} 