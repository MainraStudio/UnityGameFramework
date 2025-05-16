using UnityEngine;

namespace DebugToolsPlus.Tests
{
    public class DebugToolsPlusTest : MonoBehaviour
    {
        private void Start()
        {
            DebugTool.StartRecording();

            DebugTool.Log("DEBUG TOOLS PLUS", $"{DebugTool.FormatText("Initialize", DColor.AQUAMARINE)} {DebugTool.FormatText("test", DColor.YELLOW)}.", DColor.PINK);

            for (int i = 0; i < DColors.ColorLength; i++)
            {
                DebugTool.Log("TEST", "TestMessage", i);
                DebugTool.Log("TEST", "TestMessage", i, true);
                DebugTool.LogWarning("TEST", "TestMessage", i);
                DebugTool.LogWarning("TEST", "TestMessage", i, true);
                DebugTool.LogError("TEST", "TestMessage", i);
                DebugTool.LogError("TEST", "TestMessage", i, true);
            }

            DebugTool.Log("DEBUG TOOLS PLUS", $"{DebugTool.FormatText("End", DColor.AQUAMARINE)} {DebugTool.FormatText("test", DColor.YELLOW)}.", DColor.PINK);

            DebugTool.StopRecording();
        }
    }
}
