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
    }
}