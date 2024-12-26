using MainraFramework;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : BaseUI
{
    [SerializeField] private Button play;
    [SerializeField] private Button settings;
    [SerializeField] private Button quit;

    private UIManager uIManager;

    private void Awake()
    {
        uIManager = UIManager.Instance;
        //uIManager.AddButtonListenerWithSFX(play, GameManager.Instance.StartCoroutine());
        //uIManager.AddButtonListenerWithSFX(settings, GameManager.Instance.sta);
        quit.onClick.AddListener(() => Application.Quit());
    }
}
