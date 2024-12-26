using UnityEngine;

namespace mainra.core
{
    public static class GameConstants
        {
            // Tags
            public static class Tags
            {
                public const string Player = "Player";
                public const string Enemy = "Enemy";
                public const string NPC = "NPC";
                public const string Item = "Item";
            }
    
            // Scene Names
            public static class Scenes
            {
                public const string MainMenu = "MainMenu";
                public const string Gameplay = "Gameplay";
                public const string GameOver = "GameOver";
                public const string Credits = "Credits";
            }
    
            // Animation Parameters
            public static class AnimParams
            {
                public const string IsRunning = "isRunning";
                public const string IsJumping = "isJumping";
                public const string AttackTrigger = "attackTrigger";
            }
    
            // Layers
            public static class Layers
            {
                public const string Ground = "Ground";
                public const string Water = "Water";
                public const string PlayerLayer = "PlayerLayer";
            }
    
            // UI Element Names
            public static class UI
            {
                public const string StartButton = "StartButton";
                public const string PauseButton = "PauseButton";
                public const string ScoreText = "ScoreText";
                public const string HealthBar = "HealthBar";
            }
    
            // Audio Clip Names
            public static class AudioClips
            {
                public const string BGM_Menu = "BGM_MainTheme";
                public const string ButtonClick = "ButtonClick";
                public const string VictorySound = "VictorySound";
    
                public const string SFX_CollectCoin = "CollectCoinSound";
            }
        }
}
