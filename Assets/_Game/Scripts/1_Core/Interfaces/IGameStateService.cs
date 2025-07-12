using System;
using System.Collections.Generic;
using _Game.Scripts.Core.Enums;

namespace _Game.Scripts.Core.Interfaces
{
    /// <summary>
    /// Interface for the game state service that manages the game's state machine.
    /// </summary>
    /// <remarks>
    /// This interface defines the contract for managing game states.
    /// Implementations should provide functionality to:
    /// - Get the current game state
    /// - Change the game state with validation
    /// - Track state history
    /// - Manage state context data
    /// - Notify subscribers of state changes (before and after)
    /// </remarks>
    public interface IGameStateService
    {
        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        /// <value>The current game state.</value>
        GameState CurrentState { get; }

        /// <summary>
        /// Gets the history of state changes.
        /// </summary>
        /// <value>List of previous states, most recent first.</value>
        IReadOnlyList<GameState> StateHistory { get; }

        /// <summary>
        /// Event that is triggered before the game state changes.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified before state changes.
        /// The event provides both the current and new state.
        /// </remarks>
        event Action<GameState, GameState> OnStateChanging;

        /// <summary>
        /// Event that is triggered after the game state changes.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified after state changes.
        /// The event provides the new state.
        /// </remarks>
        event Action<GameState> OnStateChanged;

        /// <summary>
        /// Changes the current game state to the specified new state.
        /// </summary>
        /// <param name="newState">The new state to transition to.</param>
        /// <param name="context">Optional context data for the new state.</param>
        /// <remarks>
        /// This method will:
        /// 1. Validate the state transition using attributes
        /// 2. Trigger the OnStateChanging event
        /// 3. Update the current state
        /// 4. Add to state history
        /// 5. Update state context
        /// 6. Trigger the OnStateChanged event
        /// </remarks>
        void SetState(GameState newState, object context = null);

        /// <summary>
        /// Gets the context data for the current state.
        /// </summary>
        /// <typeparam name="T">The type of the context data.</typeparam>
        /// <returns>The context data if available, default(T) otherwise.</returns>
        T GetCurrentStateContext<T>();

        /// <summary>
        /// Gets the context data for a specific state.
        /// </summary>
        /// <typeparam name="T">The type of the context data.</typeparam>
        /// <param name="state">The state to get context for.</param>
        /// <returns>The context data if available, default(T) otherwise.</returns>
        T GetStateContext<T>(GameState state);
    }
}