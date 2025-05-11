using UnityEngine;

namespace DebugToolsPlus.Tests
{
    public class DebugToolsPlusTest : MonoBehaviour
    {
        private void Start()
        {
            D.StartRecording();

            D.Log("DEBUG TOOLS PLUS", $"{D.FormatText("Initialize", DColor.AQUAMARINE)} {D.FormatText("test", DColor.YELLOW)}.", DColor.PINK);

            for (int i = 0; i < DColors.ColorLength; i++)
            {
                D.Log("TEST", "TestMessage", i);
                D.Log("TEST", "TestMessage", i, true);
                D.LogWarning("TEST", "TestMessage", i);
                D.LogWarning("TEST", "TestMessage", i, true);
                D.LogError("TEST", "TestMessage", i);
                D.LogError("TEST", "TestMessage", i, true);
            }

            D.Log("DEBUG TOOLS PLUS", $"{D.FormatText("End", DColor.AQUAMARINE)} {D.FormatText("test", DColor.YELLOW)}.", DColor.PINK);

            D.StopRecording();
        }
    }
}
