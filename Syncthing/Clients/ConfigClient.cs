using System.Threading.Tasks;
using Syncthing.Exceptions;
using Syncthing.Helpers;
using Syncthing.Http;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    internal class ConfigClient : ApiClient, IConfigClient
    {
        internal ConfigClient(IApiConnection apiConnection) : base(apiConnection)
        {
            Folders = new FoldersClient(apiConnection);
            Devices = new DevicesClient(apiConnection);
        }

        /// <summary>
        /// Returns the entire config.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A <see cref="Config" /></returns>
        public async Task<Config> Get()
        {
            return await ApiConnection.Get<Config>(ApiUrls.Config());
        }


        public IFoldersClient Folders { get; }
        public IDevicesClient Devices { get; }
    }
}