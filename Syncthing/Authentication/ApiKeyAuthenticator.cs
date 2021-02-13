using System;
using System.Globalization;
using Syncthing.Helpers;
using Syncthing.Http;

namespace Syncthing.Authentication
{
    public class ApiKeyAuthenticator: IAuthenticationHandler
    {
        public void Authenticate(IRequest request, Credentials credentials)
        {
            Ensure.ArgumentNotNull(request, nameof(request));
            Ensure.ArgumentNotNull(credentials, nameof(credentials));
            Ensure.ArgumentNotNull(credentials.Password, nameof(credentials.Password));

            if (credentials.Login != null)
            {
                throw new InvalidOperationException("The Login is not null for a token authentication request. You " +
                                                    "probably did something wrong.");
            }

            request.Headers["X-API-Key"] = string.Format(CultureInfo.InvariantCulture, credentials.Password);
        }
    }
}