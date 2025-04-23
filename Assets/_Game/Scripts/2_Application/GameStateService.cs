using System;
using System.Collections.Generic;
using UnityEngine;
using _Game.Scripts.Core.Enums;
using _Game.Scripts.Core.Interfaces;

namespace _Game.Scripts.Application
{
    /// <summary>
    /// Service responsible for managing the game's state transitions and providing state information.
    /// This service implements the IGameStateService interface and follows the singleton pattern.
    /// </summary>
    /// <remarks>
    /// The GameStateService is a central component that manages the game's state machine.
    /// It provides methods to change states and events to notify subscribers of state changes.
    /// </remarks>
    public class GameStateService : IGameStateService
    {
        private GameState _currentState = GameState.Menu;
        private readonly List<GameState> _stateHistory = new();
        private const int MAX_HISTORY_SIZE = 10;
        
        /// <summary>
        /// Gets the current state of the game.
        /// </summary>
        /// <value>The current game state.</value>
        public GameState CurrentState => _currentState;

        /// <summary>
        /// Gets the history of state changes.
        /// </summary>
        /// <value>List of previous states, most recent first.</value>
        public IReadOnlyList<GameState> StateHistory => _stateHistory.AsReadOnly();

        /// <summary>
        /// Event that is triggered before the game state changes.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified before state changes.
        /// The event provides both the current and new state.
        /// </remarks>
        public event Action<GameState, GameState> OnStateChanging;

        /// <summary>
        /// Event that is triggered after the game state changes.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to be notified after state changes.
        /// The event provides the new state.
        /// </remarks>
        public event Action<GameState> OnStateChanged;

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
        /// 6. Log the state change for debugging purposes
        /// </remarks>
        public void SetState(GameState newState)
        {
            if (_currentState == newState)
            {
                Debug.LogWarning($"Attempted to set game state to {newState}, but it's already in that state.");
                return;
            }

            if (!IsValidTransition(_currentState, newState))
            {
                Debug.LogError($"Invalid state transition from {_currentState} to {newState}");
                return;
            }

            Debug.Log($"Game state changing from {_currentState} to {newState}");
            
            // Notify subscribers before state change
            OnStateChanging?.Invoke(_currentState, newState);

            // Update state and history
            _stateHistory.Insert(0, _currentState);
            if (_stateHistory.Count > MAX_HISTORY_SIZE)
            {
                _stateHistory.RemoveAt(_stateHistory.Count - 1);
            }
            
            _currentState = newState;
            
            // Notify subscribers after state change
            OnStateChanged?.Invoke(_currentState);
        }

        /// <summary>
        /// Validates if a state transition is allowed.
        /// </summary>
        /// <param name="currentState">The current state.</param>
        /// <param name="newState">The state to transition to.</param>
        /// <returns>True if the transition is valid, false otherwise.</returns>
        private bool IsValidTransition(GameState currentState, GameState newState)
        {
            // Define valid state transitions
            switch (currentState)
            {
                case GameState.Menu:
                    return newState == GameState.Loading || newState == GameState.Playing;
                
                case GameState.Playing:
                    return newState == GameState.Paused || newState == GameState.GameOver;
                
                case GameState.Paused:
                    return newState == GameState.Playing || newState == GameState.Menu;
                
                case GameState.GameOver:
                    return newState == GameState.Menu || newState == GameState.Loading;
                
                case GameState.Loading:
                    return newState == GameState.Menu || newState == GameState.Playing;
                
                default:
                    return false;
            }
        }
    }
} 