using System.Collections.Generic;
using Newtonsoft.Json;
using Syncthing.Clients;
using Syncthing.Helpers;
using Syncthing.Models.Response;

namespace Syncthing.Models.Request
{
    /// <summary>
    /// Describes a new folder to create via the <see cref="IFoldersClient.Create(NewFolder)"/> method.
    /// </summary>
    public class NewFolder
    {
        public NewFolder(string id, string path)
        {
            Ensure.ArgumentNotNullOrEmptyString(id, nameof(id));
            Ensure.ArgumentNotNullOrEmptyString(path, nameof(path));

            Id = id;
            Path = path;
        }

        /// <summary>
        /// Required. The folder ID, must be unique. (mandatory)
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Optional. The label of a folder is a human readable and descriptive local name.  May be different on each device,
        /// empty, and/or identical to other folder labels. (optional)
        /// </summary>
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; } = null;

        /// <summary>
        /// Required. The path to the directory where the folder is stored on this device; not sent to other devices. (mandatory)
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Optional. Controls how the folder is handled by Syncthing.
        /// Possible values are from Type <see cref="FolderType"/>. 
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public FolderType? Type { get; set; } = null;

        /// <summary>
        /// Optional. The rescan interval, in seconds. Can be set to zero to disable when external plugins are used to trigger
        /// rescans.
        /// </summary>
        [JsonProperty("rescanIntervalS", NullValueHandling = NullValueHandling.Ignore)]
        public int? RescanIntervalS { get; set; } = null;

        /// <summary>
        /// Optional. If enabled this detects changes to files in the folder and scans them.
        /// </summary>
        [JsonProperty("fsWatcherEnabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? FsWatcherEnabled { get; set; } = null;

        /// <summary>
        /// Optional. The duration during which changes detected are accumulated, before a scan is scheduled
        /// (only takes effect if <see cref="FsWatcherEnabled"/> is true).
        /// </summary>
        [JsonProperty("fsWatcherDelayS", NullValueHandling = NullValueHandling.Ignore)]
        public int? FsWatcherDelayS { get; set; } = null;

        /// <summary>
        /// Optional. True if the folder should ignore permissions.
        /// </summary>
        [JsonProperty("ignorePerms", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IgnorePerms { get; set; } = null;

        /// <summary>
        /// Optional. Automatically correct UTF-8 normalization errors found in file names.
        /// </summary>
        [JsonProperty("autoNormalize", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutoNormalize { get; set; } = null;

        /// <summary>
        /// Optional. All mentioned devices are those that will be sharing the folder in question.
        /// </summary>
        [JsonProperty("devices", NullValueHandling = NullValueHandling.Ignore)]
        public List<FolderDevice> Devices { get; set; } = null;

        /// <summary>
        /// Optional. The minimum required free space that should be available on the disk this folder resides.
        /// </summary>
        [JsonProperty("minDiskFree", NullValueHandling = NullValueHandling.Ignore)]
        public MinDiskFree MinDiskFree { get; set; } = null;
    }
}