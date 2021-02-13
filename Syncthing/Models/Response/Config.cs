using System.Collections.Generic;
using Newtonsoft.Json;

namespace Syncthing.Models.Response
{
    public class Config
    {
        /// <summary>
        /// The config version. Increments whenever a change is made that requires migration from previous formats.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("folders")]
        public List<Folder> Folders { get; set; }
    }
}