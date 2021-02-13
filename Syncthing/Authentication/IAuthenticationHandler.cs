using Syncthing.Http;

namespace Syncthing.Authentication
{
    interface IAuthenticationHandler
    {
        void Authenticate(IRequest request, Credentials credentials);
    }
}