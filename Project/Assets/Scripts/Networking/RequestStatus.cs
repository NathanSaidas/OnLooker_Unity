namespace Gem
{
    /// <summary>
    /// Represents the state of a request
    /// </summary>
    public enum RequestStatus
    {
        NONE,
        /// <summary>
        /// The status of the request was good. Meaning its a valid request
        /// </summary>
        GOOD,
        /// <summary>
        /// The status of the request was bad. Meaning its not a valid request.
        /// </summary>
        BAD,
        /// <summary>
        /// The status of the request is still pending. The request is in transport between server / client
        /// </summary>
        PENDING,

    }
}