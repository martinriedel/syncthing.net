using System;

namespace Syncthing.Helpers
{
    /// <summary>
    /// Class for retrieving Syncthing URLs.
    /// </summary>
    public static class ApiUrls
    {
        /// <summary>
        /// Returns the <see cref="Uri"/> for the config.
        /// </summary>
        /// <returns></returns>
        public static Uri Config()
        {
            return "rest/config".FormatUri();
        }
        
        /// <summary>
        /// Returns the <see cref="Uri"/> for the devices.
        /// </summary>
        /// <returns></returns>
        public static Uri Devices()
        {
            return "rest/config/devices".FormatUri();
        }
        
        /// <summary>
        /// Returns the <see cref="Uri"/> for one device with given ID.
        /// </summary>
        /// <returns></returns>
        public static Uri Devices(string id)
        {
            return "rest/config/devices/{0}".FormatUri(id);
        }

        /// <summary>
        /// Returns the <see cref="Uri"/> for the folders.
        /// </summary>
        /// <returns></returns>
        public static Uri Folders()
        {
            return "rest/config/folders".FormatUri();
        }
        
        /// <summary>
        /// Returns the <see cref="Uri"/> for one folder with given ID.
        /// </summary>
        /// <returns></returns>
        public static Uri Folders(string id)
        {
            return "rest/config/folders/{0}".FormatUri(id);
        }
    }
}