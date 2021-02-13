using Syncthing.Clients;
using Syncthing.Helpers;
using Syncthing.Http;

namespace Syncthing
{
    public class SyncthingClient : ISyncthingClient
    {
        /// <summary>
        /// Provides a client connection to make rest requests to HTTP endpoints.
        /// </summary>
        public IConnection Connection { get; private set; }
        
        public SyncthingClient(IConnection connection)
        {
            Ensure.ArgumentNotNull(connection, nameof(connection));

            Connection = connection;
            var apiConnection = new ApiConnection(connection);
            Config = new ConfigClient(apiConnection);
        }
        
        /// <summary>
        /// Access Syncthing's Config API
        /// </summary>
        /// /// <remarks>
        /// Refer to the API documentation for more information: https://docs.syncthing.net/users/config.html
        /// </remarks>
        public IConfigClient Config { get; private set; }
    }
}