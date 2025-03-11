using UnityEngine;

public interface ISaveData 
{
    void SaveData<T>(T data);
    T LoadData<T>();
}
