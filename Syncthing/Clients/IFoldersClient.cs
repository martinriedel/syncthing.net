using System.Collections.Generic;
using System.Threading.Tasks;
using Syncthing.Exceptions;
using Syncthing.Models.Request;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    public interface IFoldersClient
    {
        /// <summary>
        /// Returns all folders as an array.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-rest-config-devices">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Folder" />.</returns>
        Task<List<Folder>> Get();
        
        /// <summary>
        /// Returns the folder for the given ID.
        /// </summary>
        /// <param name="id">Id of the folder.</param>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-id-rest-config-devices-id">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Folder" />.</returns>
        Task<Folder> Get(string id);

        /// <summary>
        /// A new folder will be added or an existing one will be edited.
        /// </summary>
        /// <param name="newFolder"></param>
        /// /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-id-rest-config-devices-id">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        Task CreateOrEdit(NewFolder newFolder);
    }
}