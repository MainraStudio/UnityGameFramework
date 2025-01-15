using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

namespace MainraFramework
{
    /// <summary>
    /// Manages scene transitions, including optional loading screens with smooth progress tracking.
    /// </summary>
    public class SceneManager
    {
        private readonly GameManager _gameManager;
        private float _smoothVelocity;

        /// <summary>
        /// Event triggered when the loading progress is updated.
        /// Can be used to connect with UI elements, such as a loading bar.
        /// </summary>
        public event Action<float> OnProgressUpdated;

        /// <summary>
        /// The current loading progress of the scene, ranging from 0 to 1.
        /// </summary>
        public float LoadingProgress { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneManager"/> class.
        /// </summary>
        /// <param name="gameManager">The GameManager instance for managing coroutines.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="gameManager"/> is null.</exception>
        public SceneManager(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        /// <summary>
        /// Loads a scene either directly or with a loading screen, using the provided identifier (name or index).
        /// </summary>
        /// <typeparam name="T">The type of the scene identifier (string for name, int for index).</typeparam>
        /// <param name="sceneIdentifier">The name or index of the scene to load.</param>
        /// <param name="showLoadingScene">Indicates whether a loading screen should be displayed during the transition.</param>
        /// <param name="fakeLoadingTime">Simulated time for the loading progress, in seconds. Default is 3 seconds.</param>
        /// <exception cref="ArgumentException">Thrown if the scene identifier type is unsupported.</exception>
        public void LoadScene<T>(T sceneIdentifier, bool showLoadingScene = false, float fakeLoadingTime = 3f)
        {
            if (sceneIdentifier == null)
            {
                Debug.LogError("Scene identifier cannot be null.");
                return;
            }

            if (showLoadingScene)
            {
                _gameManager.StartCoroutine(LoadSceneWithProgress(sceneIdentifier, fakeLoadingTime));
            }
            else
            {
                LoadSceneDirectly(sceneIdentifier);
            }
        }

        /// <summary>
        /// Directly loads a scene without displaying a loading screen.
        /// </summary>
        /// <typeparam name="T">The type of the scene identifier (string for name, int for index).</typeparam>
        /// <param name="sceneIdentifier">The name or index of the scene to load.</param>
        private void LoadSceneDirectly<T>(T sceneIdentifier)
        {
            switch (sceneIdentifier)
            {
                case string sceneName:
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                    break;
                case int sceneIndex:
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                    break;
                default:
                    Debug.LogError("Unsupported scene identifier type.");
                    break;
            }
        }

        /// <summary>
        /// Coroutine for loading a scene with progress tracking and an optional fake loading delay.
        /// </summary>
        /// <typeparam name="T">The type of the scene identifier (string for name, int for index).</typeparam>
        /// <param name="sceneIdentifier">The name or index of the scene to load.</param>
        /// <param name="fakeLoadingTime">Simulated time for the loading progress, in seconds.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        /// <exception cref="ArgumentException">Thrown if the scene identifier type is unsupported.</exception>
        private IEnumerator LoadSceneWithProgress<T>(T sceneIdentifier, float fakeLoadingTime)
        {
            // Load the loading screen scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(Parameter.Scenes.LOADING);
            yield return null;

            // Start loading the target scene asynchronously
            AsyncOperation operation = sceneIdentifier switch
            {
                string sceneName => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName),
                int sceneIndex => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex),
                _ => throw new ArgumentException("Unsupported scene identifier type.")
            };

            operation.allowSceneActivation = false;

            float elapsedTime = 0f;

            while (!operation.isDone)
            {
                // Calculate the target progress as a linear interpolation
                float targetProgress = Mathf.Clamp01(elapsedTime / fakeLoadingTime);

                // Smoothly update progress
                LoadingProgress = Mathf.SmoothDamp(LoadingProgress, targetProgress, ref _smoothVelocity, 0.3f);

                // Trigger the progress update event
                OnProgressUpdated?.Invoke(LoadingProgress);

                // Activate the scene when loading completes
                if (LoadingProgress >= 0.999f && elapsedTime >= fakeLoadingTime)
                {
                    yield return new WaitForSeconds(1f); // Optional delay for smoother transition
                    operation.allowSceneActivation = true;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
    }
}
