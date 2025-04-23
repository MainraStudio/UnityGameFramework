using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Scripts.Core.Enums
{
    /// <summary>
    /// Attribute to define valid transitions for a game state.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ValidTransitionsAttribute : Attribute
    {
        public GameState[] ValidStates { get; }

        public ValidTransitionsAttribute(params GameState[] validStates)
        {
            ValidStates = validStates;
        }
    }

    /// <summary>
    /// Represents the different states that the game can be in.
    /// </summary>
    /// <remarks>
    /// This enum is used by the GameStateService to manage the game's state machine.
    /// Each state represents a distinct phase or mode of the game.
    /// Valid transitions are defined using the ValidTransitionsAttribute.
    /// </remarks>
    public enum GameState
    {
        /// <summary>
        /// The game is in the main menu or any other menu screen.
        /// </summary>
        [ValidTransitions(GameState.Loading, GameState.Playing)]
        Menu,

        /// <summary>
        /// The game is actively being played.
        /// </summary>
        [ValidTransitions(GameState.Paused, GameState.GameOver)]
        Playing,

        /// <summary>
        /// The game is paused, but can be resumed.
        /// </summary>
        [ValidTransitions(GameState.Playing, GameState.Menu)]
        Paused,

        /// <summary>
        /// The game has ended, typically after a loss or completion.
        /// </summary>
        [ValidTransitions(GameState.Menu, GameState.Loading)]
        GameOver,

        /// <summary>
        /// The game is loading resources or transitioning between scenes.
        /// </summary>
        [ValidTransitions(GameState.Menu, GameState.Playing)]
        Loading
    }

    /// <summary>
    /// Extension methods for GameState enum.
    /// </summary>
    public static class GameStateExtensions
    {
        /// <summary>
        /// Gets the valid transitions for a game state.
        /// </summary>
        /// <param name="state">The game state to check.</param>
        /// <returns>Array of valid states that can be transitioned to.</returns>
        public static GameState[] GetValidTransitions(this GameState state)
        {
            var field = state.GetType().GetField(state.ToString());
            var attribute = (ValidTransitionsAttribute)Attribute.GetCustomAttribute(field, typeof(ValidTransitionsAttribute));
            return attribute?.ValidStates ?? Array.Empty<GameState>();
        }

        /// <summary>
        /// Checks if a transition to the specified state is valid.
        /// </summary>
        /// <param name="currentState">The current game state.</param>
        /// <param name="newState">The state to transition to.</param>
        /// <returns>True if the transition is valid, false otherwise.</returns>
        public static bool IsValidTransition(this GameState currentState, GameState newState)
        {
            return currentState.GetValidTransitions().Contains(newState);
        }
    }
}