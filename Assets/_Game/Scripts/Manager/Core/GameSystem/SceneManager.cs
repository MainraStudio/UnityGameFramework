using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MainraFramework
{
    public class SceneManager
    {
        private readonly GameManager _gameManager;
        private float _smoothVelocity;
        private Stack<string> sceneStack = new Stack<string>();

        public event Action<float> OnProgressUpdated;
        public float LoadingProgress { get; private set; }

        public SceneManager(GameManager gameManager)
        {
            _gameManager = gameManager ?? throw new ArgumentNullException(nameof(gameManager));
        }

        public void LoadScene<T>(T sceneIdentifier, bool showLoadingScene = false, float fakeLoadingTime = 3f, Action onSceneLoaded = null, Action<string> onSceneUnloaded = null, Action<string> onActiveSceneChanged = null)
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

            if (showLoadingScene)
            {
                _gameManager.StartCoroutine(LoadSceneWithProgress(sceneIdentifier, fakeLoadingTime, onSceneLoaded, onActiveSceneChanged));
            }
            else
            {
                LoadSceneDirectly(sceneIdentifier, onSceneLoaded, onActiveSceneChanged);
            }
        }

        private bool SceneExists<T>(T sceneIdentifier)
        {
            if (sceneIdentifier is string sceneName)
            {
                for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
                {
                    if (SceneUtility.GetScenePathByBuildIndex(i).Contains(sceneName))
                    {
                        return true;
                    }
                }
            }
            else if (sceneIdentifier is int sceneIndex)
            {
                return sceneIndex >= 0 && sceneIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            }
            return false;
        }

        private void LoadSceneDirectly<T>(T sceneIdentifier, Action onSceneLoaded, Action<string> onActiveSceneChanged)
        {
            string sceneName = null;

            switch (sceneIdentifier)
            {
                case string name:
                    sceneName = name;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                    break;
                case int index:
                    sceneName = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(index).name;
                    UnityEngine.SceneManagement.SceneManager.LoadScene(index);
                    break;
                default:
                    Debug.LogError("Unsupported scene identifier type.");
                    return;
            }

            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(sceneName);
        }

        private IEnumerator LoadSceneWithProgress<T>(T sceneIdentifier, float fakeLoadingTime, Action onSceneLoaded, Action<string> onActiveSceneChanged)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(Parameter.Scenes.LOADING);
            yield return null;

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
                    yield return new WaitForSeconds(1f);
                    operation.allowSceneActivation = true;
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            string loadedSceneName = sceneIdentifier is string name ? name : UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex((int)(object)sceneIdentifier).name;
            onSceneLoaded?.Invoke();
            onActiveSceneChanged?.Invoke(loadedSceneName);
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
    }
}