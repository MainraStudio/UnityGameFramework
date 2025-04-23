namespace _Game.Scripts.Core.Enums
{
    /// <summary>
    /// Represents the different states that the game can be in.
    /// </summary>
    /// <remarks>
    /// This enum is used by the GameStateService to manage the game's state machine.
    /// Each state represents a distinct phase or mode of the game.
    /// </remarks>
    public enum GameState
    {
        /// <summary>
        /// The game is in the main menu or any other menu screen.
        /// </summary>
        Menu,

        /// <summary>
        /// The game is actively being played.
        /// </summary>
        Playing,

        /// <summary>
        /// The game is paused, but can be resumed.
        /// </summary>
        Paused,

        /// <summary>
        /// The game has ended, typically after a loss or completion.
        /// </summary>
        GameOver,

        /// <summary>
        /// The game is loading resources or transitioning between scenes.
        /// </summary>
        Loading
    }
}