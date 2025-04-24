namespace _Game.Scripts.Core.Interfaces
{
    public interface ISaveDataService
    {
        void Save<T>(string key, T data);
        T Load<T>(string key);
        bool Delete(string key);
    }
}