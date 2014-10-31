namespace Gem
{
    /// <summary>
    /// The raw ID value of each game event.
    /// </summary>
    public enum GameEventID
    {
        /// <summary>
        /// Game loading begins
        /// </summary>
        GAME_LEVEL_LOAD_BEGIN,
        /// <summary>
        /// Game loading ends
        /// </summary>
        GAME_LEVEL_LOAD_FINISH,
        /// <summary>
        /// Game unloading begins
        /// </summary>
        GAME_LEVEL_UNLOAD_BEGIN,
        /// <summary>
        /// Game unloading ends
        /// </summary>
        GAME_LEVEL_UNLOAD_FINISH,
        /// <summary>
        /// Game was paused
        /// </summary>
        GAME_PAUSED,
        /// <summary>
        /// Game was unpaused
        /// </summary>
        GAME_UNPAUSED,
        /// <summary>
        /// Game was saved
        /// </summary>
        GAME_SAVE,
        /// <summary>
        /// Game was loaded
        /// </summary>
        GAME_LOAD
    }
}