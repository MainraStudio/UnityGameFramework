using System;
namespace _Game.Scripts.Application.Manager.Core.GameSystem.Interfaces
{
	public interface IManager
	{
		// Fitur umum
		void Initialize();
		void Start();
		void Pause();
		void Resume();
		void Stop();
		void Destroy();

		// Fitur pengelolaan data
		void LoadData();
		void SaveData();
		void ResetData();

		// Fitur pengelolaan status
		void SetStatus(bool status);
		bool GetStatus();

		// Fitur pengelolaan event
		void RaiseEvent(string eventName);
		void AddEventListener(string eventName, Action action);
		void RemoveEventListener(string eventName, Action action);

		// Fitur pengelolaan log
		void Log(string message);
		void LogError(string message);
		void LogWarning(string message);

		// Fitur pengelolaan debug
		void DebugLog(string message);
		void DebugError(string message);
		void DebugWarning(string message);
	}
}