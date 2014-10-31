namespace Gem
{
    public enum LogLevel
    {
        LOG,
        WARNING,
        ERROR,
        USER
    }

    public struct ConsoleMessage
    {
        private string m_Message;
        private LogLevel m_LogLevel;

        
       
        public ConsoleMessage(string aMessage)
        {
            m_Message = aMessage;
            m_LogLevel = LogLevel.LOG;
        }
        public ConsoleMessage(string aMessage, LogLevel aLogLevel)
        {
            m_Message = aMessage;
            m_LogLevel = aLogLevel;
        }

        public string message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
        public LogLevel logLevel
        {
            get { return m_LogLevel; }
            set { m_LogLevel = value; }
        }
    }
}