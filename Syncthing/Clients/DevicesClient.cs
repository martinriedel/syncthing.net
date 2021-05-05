using System.Collections.Generic;
using System.Threading.Tasks;
using Syncthing.Exceptions;
using Syncthing.Helpers;
using Syncthing.Http;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    public class DevicesClient : ApiClient, IDevicesClient
    {
        public DevicesClient(IApiConnection apiConnection) : base(apiConnection)
        {
            
        }
        
        /// <summary>
        /// Returns all devices as an array.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-rest-config-devices">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Device" />.</returns>
        public Task<List<Device>> Get()
        {
            return ApiConnection.Get<List<Device>>(ApiUrls.Devices());
        }

        /// <summary>
        /// Returns the Device for the given ID.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-id-rest-config-devices-id">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>One <see cref="Device" />.</returns>
        public Task<Device> Get(string id)
        {
            Ensure.ArgumentNotNullOrEmptyString(id, nameof(id));
            
            return ApiConnection.Get<Device>(ApiUrls.Devices(id));
        }
    }
}