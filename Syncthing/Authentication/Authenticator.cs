using System.Collections.Generic;
using System.Threading.Tasks;
using Syncthing.Helpers;
using Syncthing.Http;

namespace Syncthing.Authentication
{
    class Authenticator
    {
        readonly Dictionary<AuthenticationType, IAuthenticationHandler> authenticators =
            new Dictionary<AuthenticationType, IAuthenticationHandler>
            {
                { AuthenticationType.Anonymous, new AnonymousAuthenticator() },
                { AuthenticationType.ApiKey, new ApiKeyAuthenticator() }
            };

        public Authenticator(ICredentialStore credentialStore)
        {
            Ensure.ArgumentNotNull(credentialStore, nameof(credentialStore));

            CredentialStore = credentialStore;
        }

        public async Task Apply(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            var credentials = await CredentialStore.GetCredentials().ConfigureAwait(false) ?? Credentials.Anonymous;
            authenticators[credentials.AuthenticationType].Authenticate(request, credentials);
        }

        public ICredentialStore CredentialStore { get; set; }
    }
}