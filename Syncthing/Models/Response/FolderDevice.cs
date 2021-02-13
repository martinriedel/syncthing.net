using Newtonsoft.Json;

namespace Syncthing.Models.Response
{
    public class FolderDevice
    {
        /// <summary>
        /// The id of the device.
        /// </summary>
        [JsonProperty("deviceID")]
        public string DeviceId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("introducedBy")]
        public string IntroducedBy { get; set; }

        [JsonProperty("encryptionPassword")]
        public string EncryptionPassword { get; set; }
    }
}