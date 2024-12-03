// Engine/GameState.cs
using System;

namespace JRPG.Engine
{
    public enum GameStateType
    {
        MainMenu,    // When the player is in the main menu
        InGame,      // During actual gameplay
        Paused       // When the game is paused
    }

    public class GameState
    {
        // Track the current state with a backing field
        private GameStateType _currentState = GameStateType.MainMenu;
        
        // Public property to read the current state
        public GameStateType CurrentState 
        { 
            get => _currentState;
            private set
            {
                if (_currentState != value)
                {
                    _currentState = value;
                    // Notify any listeners that the state has changed
                    OnStateChanged?.Invoke(_currentState);
                }
            }
        }

        // Event that other parts of the game can listen to for state changes
        public event Action<GameStateType> OnStateChanged;

        public void ChangeState(GameStateType newState)
        {
            // Only change state if it's different from the current state
            if (newState != _currentState)
            {
                CurrentState = newState;
            }
        }
    }
}