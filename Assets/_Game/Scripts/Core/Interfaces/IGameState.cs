namespace _Game.Scripts.Application.Manager.Core.GameSystem.Interfaces
{
	public abstract class GameState
	{
		public abstract void EnterState();
		public abstract void UpdateState();
		public abstract void ExitState();
	}
}