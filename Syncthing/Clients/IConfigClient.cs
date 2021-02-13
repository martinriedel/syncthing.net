
using System.Threading.Tasks;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    public interface IConfigClient
    {
        /// <summary>
        /// Returns the current configuration.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config">API documentation</a> for more information.
        /// </remarks>
        /// /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A <see cref="Config" /></returns>
        Task<Config> Get();
    }
}