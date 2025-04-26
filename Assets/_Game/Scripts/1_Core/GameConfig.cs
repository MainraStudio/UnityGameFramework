using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    #region Product Information
    [SerializeField] private string _productName = "MyAwesomeGame";
    [SerializeField] private string _productVersion = "1.0.0";
    [SerializeField] private string _companyName = "M Mainra Games";
    #endregion

    #region Public Accessors
    public string ProductName => _productName;
    public string ProductVersion => _productVersion;
    public string CompanyName => _companyName;
    #endregion

    #region Validation
    private void OnValidate()
    {
        _productVersion = _productVersion.Trim();
        _productName = _productName ?? "Unnamed Game";
    }
    #endregion
    
}
