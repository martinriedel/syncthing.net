using Newtonsoft.Json;

namespace Syncthing.Models.Response
{
    public enum FolderType
    {
        /// <summary>
        /// The folder is in default mode. Sending local and accepting remote changes. Note that this type was
        /// previously called “readwrite” which is deprecated but still accepted in incoming configs.
        /// </summary>
        [JsonProperty("sendreceive")]
        Sendreceive,
        /// <summary>
        /// The folder is in “send only” mode – it will not be modified by Syncthing on this device. Note that this
        /// type was previously called “readonly” which is deprecated but still accepted in incoming configs.
        /// </summary>
        [JsonProperty("sendonly")]
        Sendonly,
        /// <summary>
        /// The folder is in “receive only” mode – it will not propagate changes to other devices.
        /// </summary>
        [JsonProperty("receiveonly")]
        Receiveonly
    }
}