using Syncthing.Clients;
using Syncthing.Http;

namespace Syncthing
{
    public interface ISyncthingClient
    {
        /// <summary>
        /// Provides a client connection to make rest requests to HTTP endpoints.
        /// </summary>
        IConnection Connection { get; }
        
        /// <summary>
        /// Access Syncthing's Config API
        /// </summary>
        /// /// <remarks>
        /// Refer to the API documentation for more information: https://docs.syncthing.net/users/config.html
        /// </remarks>
        IConfigClient Config { get; }
    }
}