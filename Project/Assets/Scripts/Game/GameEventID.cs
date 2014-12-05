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
        /// A unit has begin their attack
        /// </summary>
        UNIT_ATTACK_BEGIN,
        /// <summary>
        /// A unit has began executing an ability
        /// </summary>
        UNIT_ATTACK_EXECUTE,
        /// <summary>
        /// A unit has stopped executing a ability by choice
        /// </summary>
        UNIT_ATTACK_STOPPED,
        /// <summary>
        /// A unit has finished executing their ability
        /// </summary>
        UNIT_ATTACK_FINISHED,
        /// <summary>
        /// A unit has cancelled their ability
        /// </summary>
        UNIT_ATTACK_CANCELLED,
        
        /// <summary>
        /// Area trigger was triggered.
        /// </summary>
        TRIGGER_AREA,
        /// <summary>
        /// Area trigger has stopped being triggered.
        /// </summary>
        TRIGGER_AREA_EXIT,
        
        GAME_TERMINAL_USE,
        GAME_TERMINAL_ON,
        GAME_TERMINAL_OFF,
        UNIT_LEARN_ABILITY,
        GAME_DOOR_OPEN,
        GAME_DOOR_CLOSE,

    }
}