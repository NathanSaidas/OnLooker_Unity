namespace Gem
{
    /// <summary>
    /// A data structure which holds event data.
    /// </summary>
    public struct GameEventData
    {
        /// <summary>
        /// The time the event was created
        /// </summary>
        private float m_TimeStamp;
        /// <summary>
        /// The raw ID of the event being triggered
        /// </summary>
        private GameEventID m_EventSubType;
        /// <summary>
        /// The main type of event occuring
        /// </summary>
        private GameEventType m_EventType;
        /// <summary>
        /// The caller of the event.
        /// </summary>
        private object m_Sender;
        /// <summary>
        /// An object responsible for triggering an event.
        /// </summary>
        private object m_TriggeringObject;

        public GameEventData(float aTimeStamp, GameEventID aEventSubType, GameEventType aEventType, object aSender, object aTriggeringObject)
        {
            m_TimeStamp = aTimeStamp;
            m_EventSubType = aEventSubType;
            m_EventType = aEventType;
            m_Sender = aSender;
            m_TriggeringObject = aTriggeringObject;
        }
        /// <summary>
        /// The time the event was created
        /// </summary>
        public float timeStamp
        {
            set { m_TimeStamp = value; }
        }
        /// <summary>
        /// The raw ID of the event being triggered
        /// </summary>
        public GameEventID eventSubType
        {
            get { return m_EventSubType; }
        }
        /// <summary>
        /// The main type of event occuring
        /// </summary>
        public GameEventType eventType
        {
            get { return m_EventType; }
        }
        /// <summary>
        /// The caller of the event.
        /// </summary>
        public object sender
        {
            get { return m_Sender; }
        }
        /// <summary>
        /// An object responsible for triggering an event.
        /// </summary>
        public object triggeringObject
        {
            get { return m_TriggeringObject; }
        }

    }
}