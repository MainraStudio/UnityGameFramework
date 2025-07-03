using Akiba.DocuServe;
using UnityEditor;
using UnityEngine;

namespace Akiba.SOCreator
{
    public class AkibaSOCDoc : DocuServeWindow
    {
        [MenuItem("Tools/Akiba/SOC/Documentation")]

        public static void Init()
        {
            EditorWindow docWindow = GetWindow(typeof(AkibaSOCDoc), false);
            docWindow.Show();
        }

        protected override void Awake()
        {
            base.windowTitle = "SOC Documentation";
            base.windowMinSize = new Vector2(800, 500);
            base.windowMaxSize = new Vector2(1200, 700);
            base.defaultTextAlignment = DefaultTextAlignment.Left;
            base.docPath = "..\\..\\SOCDoc.txt";
            base.AssignOriginPoint(nameof(AkibaSOCDoc));

            base.Awake();

            base.LoadFrom(nameof(AkibaSOCDoc));
        }

        protected override void OnGUI()
        {
            base.OnGUI();
        }
    }
}