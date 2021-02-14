namespace Syncthing.Authentication
{
    /// <summary>
    /// Authentication protocols. Syncthing uses the X-API-Key header
    /// </summary>
    public enum AuthenticationType
    {
        /// <summary>
        /// No credentials provided
        /// </summary>
        Anonymous,
        /// <summary>
        /// Api key
        /// </summary>
        ApiKey
    }
}