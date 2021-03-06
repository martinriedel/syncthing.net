﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Syncthing.Helpers;

namespace Syncthing.Http
{
    /// <summary>
    /// Extra information returned as part of each API response.
    /// </summary>
    public class ApiInfo
    {
        public ApiInfo(IDictionary<string, Uri> links,
            IList<string> oauthScopes,
            IList<string> acceptedOauthScopes,
            string etag)
        {
            Ensure.ArgumentNotNull(links, nameof(links));
            Ensure.ArgumentNotNull(oauthScopes, nameof(oauthScopes));
            Ensure.ArgumentNotNull(acceptedOauthScopes, nameof(acceptedOauthScopes));

            Links = new ReadOnlyDictionary<string, Uri>(links);
            OauthScopes = new ReadOnlyCollection<string>(oauthScopes);
            AcceptedOauthScopes = new ReadOnlyCollection<string>(acceptedOauthScopes);
            Etag = etag;
        }

        /// <summary>
        /// Oauth scopes that were included in the token used to make the request.
        /// </summary>
        public IReadOnlyList<string> OauthScopes { get; private set; }

        /// <summary>
        /// Oauth scopes accepted for this particular call.
        /// </summary>
        public IReadOnlyList<string> AcceptedOauthScopes { get; private set; }

        /// <summary>
        /// Etag
        /// </summary>
        public string Etag { get; private set; }

        /// <summary>
        /// Links to things like next/previous pages
        /// </summary>
        public IReadOnlyDictionary<string, Uri> Links { get; private set; }

        /// <summary>
        /// Allows you to clone ApiInfo 
        /// </summary>
        /// <returns>A clone of <seealso cref="ApiInfo"/></returns>
        public ApiInfo Clone()
        {
            return new ApiInfo(Links.Clone(),
                               OauthScopes.Clone(),
                               AcceptedOauthScopes.Clone(),
                               Etag != null ? new string(Etag.ToCharArray()) : null);
        }
    }
}
