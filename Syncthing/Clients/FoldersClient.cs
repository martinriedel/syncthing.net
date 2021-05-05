using System.Collections.Generic;
using System.Threading.Tasks;
using Syncthing.Exceptions;
using Syncthing.Helpers;
using Syncthing.Http;
using Syncthing.Models.Request;
using Syncthing.Models.Response;

namespace Syncthing.Clients
{
    public class FoldersClient : ApiClient, IFoldersClient
    {
        public FoldersClient(IApiConnection apiConnection) : base(apiConnection)
        {
            
        }

        /// <summary>
        /// Returns all folders as an array.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-rest-config-devices">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Folder" />.</returns>
        public async Task<List<Folder>> Get()
        {
            return await ApiConnection.Get<List<Folder>>(ApiUrls.Folders());
        }

        /// <summary>
        /// Returns the folder for the given ID.
        /// </summary>
        /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-id-rest-config-devices-id">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Folder" />.</returns>
        public async Task<Folder> Get(string id)
        {
            Ensure.ArgumentNotNullOrEmptyString(id, nameof(id));
            
            return await ApiConnection.Get<Folder>(ApiUrls.Folders(id));
        }

        /// <summary>
        /// A new folder will be added.
        /// </summary>
        /// <param name="newFolder"></param>
        /// /// <remarks>
        /// See the <a href="https://docs.syncthing.net/rest/config.html#rest-config-folders-id-rest-config-devices-id">Config Endpoints</a> for more information.
        /// </remarks>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        public async Task CreateOrEdit(NewFolder newFolder)
        {
            Ensure.ArgumentNotNull(newFolder, nameof(newFolder));
            Ensure.ArgumentNotNullOrEmptyString(newFolder.Id, nameof(newFolder.Id));
            Ensure.ArgumentNotNullOrEmptyString(newFolder.Path, nameof(newFolder.Path));

            await ApiConnection.Post(ApiUrls.Folders(), newFolder);
        }
        
        
    }
}