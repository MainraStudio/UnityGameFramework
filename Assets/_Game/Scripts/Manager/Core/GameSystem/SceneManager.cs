using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MainraFramework
{
    public class SceneManager
    {
        private readonly GameManager _gameManager;
        private float _smoothVelocity;
        private Stack<string> sceneStack = new Stack<string>();
        private Dictionary<string, bool> sceneCache = new Dictionary<string, bool>();
        private bool _isUnloadingResources;
        private readonly Dictionary<string, AsyncOperation> _loadingOperations = new Dictionary<string, AsyncOperation>();

        public event Action<float> OnProgressUpdated;
        public event Action<string> OnSceneLoadStart;
        public event Action<string> OnSceneLoadComplete;
        public event Action OnResourcesUnloaded;
        public float LoadingProgress { get; private set; }

        public SceneManager(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        public async Task<bool> UnloadUnusedResources()
        {
            if (_isUnloadingResources) return false;

            _isUnloadingResources = true;
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();

                var operation = Resources.UnloadUnusedAssets();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                OnResourcesUnloaded?.Invoke();
                return true;
            }
            finally
            {
                _isUnloadingResources = false;
            }
        }

        public async void LoadScene<T>(T sceneIdentifier, bool showLoadingScene = false, float fakeLoadingTime = 3f,
            Action onSceneLoaded = null, Action<string> onSceneUnloaded = null,
            Action<string> onActiveSceneChanged = null, bool unloadResources = true)
        {
            if (sceneIdentifier == null)
            {
                Debug.LogError("Scene identifier cannot be null.");
                return;
            }

            string sceneName = sceneIdentifier.ToString();
            if (!SceneExists(sceneIdentifier))
            {
                Debug.LogError($"Scene '{sceneName}' does not exist.");
                return;
            }

            if (_loadingOperations.ContainsKey(sceneName))
            {
                Debug.LogWarning($"Scene '{sceneName}' is already loading.");
                return;
            }

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            OnSceneLoadStart?.Invoke(sceneName);

            try
            {
                if (unloadResources)
                {
                    await UnloadUnusedResources();
                }

                if (showLoadingScene)
                {
                    await LoadSceneWithProgress(sceneIdentifier, fakeLoadingTime, onSceneLoaded, onActiveSceneChanged);
                }
                else
                {
                    LoadSceneDirectly(sceneIdentifier, onSceneLoaded, onActiveSceneChanged);
                }

                onSceneUnloaded?.Invoke(currentScene);
            }
            finally
            {
                if (_loadingOperations.ContainsKey(sceneName))
                {
                    _loadingOperations.Remove(sceneName);
                }
            }
        }

        private bool SceneExists<T>(T sceneIdentifier)
        {
            string sceneName = sceneIdentifier.ToString();
            if (sceneCache.TryGetValue(sceneName, out bool exists))
            {
                return exists;
            }

            exists = sceneIdentifier switch
            {
                string name => !string.IsNullOrEmpty(SceneUtility.GetScenePathByBuildIndex(
                    UnityEngine.SceneManagement.SceneManager.GetSceneByName(name).buildIndex)),
                int index => index >= 0 && index < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings,
                _ => false
            };

            sceneCache[sceneName] = exists;
            return exists;
        }

        private void LoadSceneDirectly<T>(T sceneIdentifier, Action onSceneLoaded, Action<string> onActiveSceneChanged)
        {
            string sceneName = sceneIdentifier.ToString();
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(sceneName);
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private async Task LoadSceneWithProgress<T>(T sceneIdentifier, float fakeLoadingTime, Action onSceneLoaded,
            Action<string> onActiveSceneChanged)
        {
            string loadingSceneName = Parameter.Scenes.LOADING;
            UnityEngine.SceneManagement.SceneManager.LoadScene(loadingSceneName);
            await Task.Yield();
        
            AsyncOperation operation = sceneIdentifier switch
            {
                string name => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name),
                int sceneIndex => UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex),
                _ => throw new ArgumentException("Unsupported scene identifier type.")
            };
        
            string sceneName = sceneIdentifier.ToString();
            _loadingOperations[sceneName] = operation;
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
        
            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(sceneName);
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        public async void PushScene(string sceneName, bool unloadResources = true)
        {
            if (!SceneExists(sceneName)) return;

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            sceneStack.Push(currentScene);

            if (unloadResources)
            {
                await UnloadUnusedResources();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public async void PopScene(bool unloadResources = true)
        {
            if (sceneStack.Count == 0)
            {
                Debug.LogWarning("Scene stack is empty. Cannot pop scene.");
                return;
            }

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);
            
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (unloadResources)
            {
                await UnloadUnusedResources();
            }

            string previousScene = sceneStack.Pop();
            UnityEngine.SceneManagement.SceneManager.LoadScene(previousScene, LoadSceneMode.Additive);
        }

        public void ReloadCurrentScene(bool unloadResources = true)
        {
            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            LoadScene(currentScene, unloadResources: unloadResources);
        }

        public async void UnloadScene(string sceneName, bool unloadResources = true)
        {
            if (!SceneExists(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' does not exist.");
                return;
            }

            var operation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (unloadResources)
            {
                await UnloadUnusedResources();
            }
        }

        public void NextScene(bool unloadResources = true)
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int nextIndex = (currentIndex + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            LoadScene(nextIndex, unloadResources: unloadResources);
        }

        public void PreviousScene(bool unloadResources = true)
        {
            int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int previousIndex = (currentIndex - 1 + UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings) 
                % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            LoadScene(previousIndex, unloadResources: unloadResources);
        }

        public void ClearSceneCache()
        {
            sceneCache.Clear();
        }

        public void ClearSceneStack()
        {
            sceneStack.Clear();
        }
    }
}