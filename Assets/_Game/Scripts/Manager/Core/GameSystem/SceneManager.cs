using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainraFramework
{
    public class SceneManager
    {
        private readonly GameManager _gameManager;
        private float _smoothVelocity;
        private Stack<string> sceneStack = new Stack<string>();

        public event Action<float> OnProgressUpdated;
        public event Action<string> OnSceneLoadStart;
        public event Action<string> OnSceneLoadComplete;
        public float LoadingProgress { get; private set; }

        public SceneManager(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        public async void LoadScene<T>(T sceneIdentifier, bool showLoadingScene = false, float fakeLoadingTime = 3f, Action onSceneLoaded = null, Action<string> onSceneUnloaded = null, Action<string> onActiveSceneChanged = null)
        {
            if (sceneIdentifier == null)
            {
                Debug.LogError("Scene identifier cannot be null.");
                return;
            }

            if (!SceneExists(sceneIdentifier))
            {
                Debug.LogError($"Scene '{sceneIdentifier}' does not exist.");
                return;
            }

            OnSceneLoadStart?.Invoke(sceneIdentifier.ToString());

            if (showLoadingScene)
            {
                await LoadSceneWithProgress(sceneIdentifier, fakeLoadingTime, onSceneLoaded, onActiveSceneChanged);
            }
            else
            {
                LoadSceneDirectly(sceneIdentifier, onSceneLoaded, onActiveSceneChanged);
            }
        }

        private bool SceneExists<T>(T sceneIdentifier)
        {
            return sceneIdentifier switch
            {
                string sceneName => SceneUtility.GetScenePathByBuildIndex(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName).buildIndex) != null,
                int sceneIndex => sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings,
                _ => false
            };
        }

        private void LoadSceneDirectly<T>(T sceneIdentifier, Action onSceneLoaded, Action<string> onActiveSceneChanged)
        {
            string sceneName = sceneIdentifier switch
            {
                string name => name,
                int index => UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).name,
                _ => throw new ArgumentException("Unsupported scene identifier type.")
            };

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(sceneName);
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private async Task LoadSceneWithProgress<T>(T sceneIdentifier, float fakeLoadingTime, Action onSceneLoaded, Action<string> onActiveSceneChanged)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(Parameter.Scenes.LOADING);
            await Task.Yield();

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
                float targetProgress = Mathf.Clamp01(elapsedTime / fakeLoadingTime);
                LoadingProgress = Mathf.SmoothDamp(LoadingProgress, targetProgress, ref _smoothVelocity, 0.3f);
                OnProgressUpdated?.Invoke(LoadingProgress);

                if (LoadingProgress >= 0.999f && elapsedTime >= fakeLoadingTime)
                {
                    await Task.Delay(1000);
                    operation.allowSceneActivation = true;
                }

                elapsedTime += Time.deltaTime;
                await Task.Yield();
            }

            string loadedSceneName = sceneIdentifier is string name ? name : UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex((int)(object)sceneIdentifier).name;
            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(loadedSceneName);
            OnSceneLoadComplete?.Invoke(loadedSceneName);
        }

        public void PushScene(string sceneName)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneStack.Push(currentScene);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void PopScene()
        {
            if (sceneStack.Count > 0)
            {
                string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);

                string previousScene = sceneStack.Pop();
                UnityEngine.SceneManagement.SceneManager.LoadScene(previousScene, LoadSceneMode.Additive);
            }
            else
            {
                Debug.LogWarning("Scene stack is empty. Cannot pop scene.");
            }
        }

        public void ReloadCurrentScene()
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            LoadScene(currentScene);
        }

        public void UnloadScene(string sceneName)
        {
            if (SceneExists(sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            }
            else
            {
                Debug.LogError($"Scene '{sceneName}' does not exist.");
            }
        }

        public void NextScene()
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int nextIndex = (currentIndex + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            LoadScene(nextIndex);
        }

        public void PreviousScene()
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int previousIndex = (currentIndex - 1 + UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            LoadScene(previousIndex);
        }
    }
}