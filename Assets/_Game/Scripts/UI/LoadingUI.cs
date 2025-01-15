using System;
using MainraFramework;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : BaseUI
{
    [SerializeField] private Slider sliderLoadingBar;
    private void Update()
    {
        sliderLoadingBar.value=GameManager.Instance.SceneManager.LoadingProgress;
    }
}
