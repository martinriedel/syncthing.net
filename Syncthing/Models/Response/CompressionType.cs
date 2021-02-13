using Newtonsoft.Json;

namespace Syncthing.Models.Response
{
    public enum CompressionType
    {
        /// <summary>
        /// Compress metadata packets, such as index information. Metadata is usually very compression friendly so this
        /// is a good default.
        /// </summary>
        [JsonProperty("metadata")]
        Metadata,
        /// <summary>
        /// Compress all packets, including file data. This is recommended if the folders contents are mainly
        /// compressible data such as documents or text files.
        /// </summary>
        [JsonProperty("always")]
        Always,
        /// <summary>
        /// Disable all compression.
        /// </summary>
        [JsonProperty("never")]
        Never
    }
}