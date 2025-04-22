namespace _Game.Scripts.Core.Events
{
    public class SceneTransitionEventArgs
    {
        public string SceneName { get; }
        public float Progress { get; }

        public SceneTransitionEventArgs(string sceneName, float progress = 0f)
        {
            SceneName = sceneName;
            Progress = progress;
        }
    }
}