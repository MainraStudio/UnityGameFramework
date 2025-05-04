using UnityEngine;

public class FrameRateUnlocker : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
