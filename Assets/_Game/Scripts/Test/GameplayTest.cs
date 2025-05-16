using UnityEngine;
using _Game.Scripts.Core.Interfaces;
using VContainer;

public class GameplayTest : MonoBehaviour
{
    [Inject] private ISceneLoader _sceneLoader;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_sceneLoader.ReloadSceneThroughLoading();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
