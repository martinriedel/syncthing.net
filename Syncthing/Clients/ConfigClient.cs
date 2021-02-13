using System.Threading.Tasks;
using Syncthing.Exceptions;
using Syncthing.Helpers;
using Syncthing.Http;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    public class ConfigClient : ApiClient, IConfigClient
    {
        public ConfigClient(IApiConnection apiConnection) : base(apiConnection)
        {
            
        }

        /// <summary>
        /// Returns the entire config.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A <see cref="Config" /></returns>
        public Task<Config> Get()
        {
            return ApiConnection.Get<Config>(ApiUrls.Config());
        }
    }
}