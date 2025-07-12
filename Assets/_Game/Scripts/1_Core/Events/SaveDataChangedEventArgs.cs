namespace _Game.Scripts.Core.Events
{
    public class SaveDataChangedEventArgs
    {
        public string Key { get; }
        public object Data { get; }
        public SaveDataChangedEventArgs(string key, object data)
        {
            Key = key;
            Data = data;
        }
    }
}