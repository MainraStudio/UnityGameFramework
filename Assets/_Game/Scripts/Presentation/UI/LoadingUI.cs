using _Game.Scripts.Application.Manager.Core.GameSystem;
using UnityEngine;
using UnityEngine.UI;
namespace _Game.Scripts.Presentation.UI
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField] private Slider sliderLoadingBar;
        private void Update()
        {
            sliderLoadingBar.value=GameManager.Instance.SceneManager.LoadingProgress;
        }
    }
}
