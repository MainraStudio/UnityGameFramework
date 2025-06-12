using UnityEngine;

[CreateAssetMenu(fileName = "NL_ObjectNotes_Readme", menuName = "Nesbit Labs/Developer Readme", order = 1)]
public class NL_ObjectNotes_Readme : ScriptableObject
{
    [TextArea(2, 5)]
    public string thankYouMessage = "Thank you for using Nesbit Labs Object Notes!";
    
    public string version = "v1.0.0";

    [TextArea(5, 20)]
    public string changeLog = "- Initial release\n- Object notes with title + to-do list\n- Color tags + scene icons\n- Notes Overview window";

}
