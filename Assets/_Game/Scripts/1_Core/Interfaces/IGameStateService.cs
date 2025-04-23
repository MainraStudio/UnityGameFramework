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
        /// <remarks>
        /// This method will:
        /// 1. Validate the state transition
        /// 2. Trigger the OnStateChanging event
        /// 3. Update the current state
        /// 4. Add to state history
        /// 5. Trigger the OnStateChanged event
        /// </remarks>
        void SetState(GameState newState);
    }
}