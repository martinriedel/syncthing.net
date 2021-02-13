namespace Syncthing.Authentication
{
    /// <summary>
    /// Authentication protocols supported by the GitHub API
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