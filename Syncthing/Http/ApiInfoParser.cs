﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Syncthing.Helpers;

namespace Syncthing.Http
{
    internal static class ApiInfoParser
    {
        const RegexOptions RegexOptions =
#if HAS_REGEX_COMPILED_OPTIONS
            RegexOptions.Compiled |
#endif
             System.Text.RegularExpressions.RegexOptions.IgnoreCase;

        static readonly Regex LinkRelRegex = new Regex("rel=\"(next|prev|first|last)\"", RegexOptions);
        static readonly Regex LinkUriRegex = new Regex("<(.+)>", RegexOptions);

        static KeyValuePair<string, string> LookupHeader(IDictionary<string, string> headers, string key)
        {
            return headers.FirstOrDefault(h => string.Equals(h.Key, key, StringComparison.OrdinalIgnoreCase));
        }

        static bool Exists(KeyValuePair<string, string> kvp)
        {
            return !kvp.Equals(default(KeyValuePair<string, string>));
        }

        public static ApiInfo ParseResponseHeaders(IDictionary<string, string> responseHeaders)
        {
            Ensure.ArgumentNotNull(responseHeaders, nameof(responseHeaders));

            var httpLinks = new Dictionary<string, Uri>();
            var oauthScopes = new List<string>();
            var acceptedOauthScopes = new List<string>();
            string etag = null;

            var acceptedOauthScopesKey = LookupHeader(responseHeaders, "X-Accepted-OAuth-Scopes");
            if (Exists(acceptedOauthScopesKey))
            {
                acceptedOauthScopes.AddRange(acceptedOauthScopesKey.Value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()));
            }

            var oauthScopesKey = LookupHeader(responseHeaders, "X-OAuth-Scopes");
            if (Exists(oauthScopesKey))
            {
                oauthScopes.AddRange(oauthScopesKey.Value
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()));
            }

            var etagKey = LookupHeader(responseHeaders, "ETag");
            if (Exists(etagKey))
            {
                etag = etagKey.Value;
            }

            var linkKey = LookupHeader(responseHeaders, "Link");
            if (Exists(linkKey))
            {
                var links = linkKey.Value.Split(',');
                foreach (var link in links)
                {
                    var relMatch = LinkRelRegex.Match(link);
                    if (!relMatch.Success || relMatch.Groups.Count != 2) break;

                    var uriMatch = LinkUriRegex.Match(link);
                    if (!uriMatch.Success || uriMatch.Groups.Count != 2) break;

                    httpLinks.Add(relMatch.Groups[1].Value, new Uri(uriMatch.Groups[1].Value));
                }
            }

            return new ApiInfo(httpLinks, oauthScopes, acceptedOauthScopes, etag);
        }
    }
}
