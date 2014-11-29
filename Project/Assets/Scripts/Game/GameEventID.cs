namespace Gem
{
    #region CHANGE LOG
    /* October,31,2014 - Nathan Hanlan, Added initial types of GameEventID
     * 
     */
    #endregion
    /// <summary>
    /// The raw ID value of each game event.
    /// </summary>
    public enum GameEventID
    {
        /// <summary>
        /// Represents empty data
        /// </summary>
        NONE,
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
        GAME_LOAD,
        /// <summary>
        /// A unit was revived
        /// </summary>
        UNIT_REVIVED,
        /// <summary>
        /// A unit was spawned
        /// </summary>
        UNIT_SPAWNED,
        /// <summary>
        /// A unit was killed
        /// </summary>
        UNIT_KILLED,
        
        /// <summary>
        /// Area trigger was triggered.
        /// </summary>
        TRIGGER_AREA,
        /// <summary>
        /// Area trigger has stopped being triggered.
        /// </summary>
        TRIGGER_AREA_EXIT,

    }
}