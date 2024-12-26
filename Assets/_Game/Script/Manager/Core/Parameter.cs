namespace MainraFramework
{
    public static class Parameter
    {
        // Tags
        public static class Tags
        {
            public const string PLAYER = "Player";
            public const string ENEMY = "Enemy";
            public const string NPC = "NPC";
            public const string ITEM = "Item";
            public const string PICKABLE = "Pickable";
            public const string GROUND = "Ground";
        }

        // Scene Names
        public static class Scenes
        {
            public const string MAINMENU = "MainMenu";
            public const string GAMEPLAY = "Gameplay";
            public const string GAMEOVER = "GameOver";
            public const string CREDITS = "Credits";
        }

        // Animation Parameters
        public static class AnimParams
        {
            public const string ISRUNNING = "isRunning";
            public const string ISJUMPING = "isJumping";
            public const string ATTACKTRIGGER = "attackTrigger";
        }

        // Layers
        public static class Layers
        {
            public const string GROUND = "Ground";
            public const string WATER = "Water";
            public const string PLAYERLAYER = "PlayerLayer";
        }

        // UI Element Names
        public static class UI
        {
            public const string STARTBUTTON = "StartButton";
            public const string PAUSEBUTTON = "PauseButton";
            public const string SCORETEXT = "ScoreText";
            public const string HEALTHBAR = "HealthBar";
        }

        public static class AudioMixer
        {
            public const string MasterVolumeParameter = "MasterVolume";
            public const string BGMVolumeParameter = "MusicVolume";
            public const string SFXVolumeParameter = "SfxVolume";
        }

        // Audio Clip Names
        public static class AudioClips
        {
            public const string BGM_MENU = "gameplay";
            public const string BGM_PAUSE = "gameplay2";
            public const string SFX_BUTTONCLICK = "ButtonClick";
            public const string BGM_WIN = "VictorySound";
            public const string SFX_COLLECTCOIN = "coin";
            public const string SFX_WIN = "Win";
        }
        
        public static class PlayerPrefs
        {
            public const string MASTER_VOLUME = "MasterVolume";
            public const string MUSIC_VOLUME = "MusicVolume";
            public const string SFX_VOLUME = "SfxVolume";
        }
    }
}