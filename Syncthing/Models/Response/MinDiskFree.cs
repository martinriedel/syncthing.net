using Newtonsoft.Json;

namespace Syncthing.Models.Response
{
    /// <summary>
    /// The minimum required free space that should be available on the disk this folder resides. The folder will be
    /// stopped when the value drops below the threshold.
    /// </summary>
    public class MinDiskFree
    {
        /// <summary>
        /// Set to zero to disable.
        /// </summary>
        [JsonProperty("value")]
        public int Value { get; set; }

        /// <summary>
        /// Accepted units are %, kB, MB, GB and TB.
        /// </summary>
        [JsonProperty("unit")]
        public string Unit { get; set; }
    }
}