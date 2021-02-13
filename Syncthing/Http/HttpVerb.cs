using System.Net.Http;

namespace Syncthing.Http
{
    internal static class HttpVerb
    {
        static readonly HttpMethod patch = new HttpMethod("PATCH");

        internal static HttpMethod Patch
        {
            get { return patch; }
        }
    }
}