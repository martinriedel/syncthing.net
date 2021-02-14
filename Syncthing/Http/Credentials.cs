using Syncthing.Authentication;
using Syncthing.Helpers;

namespace Syncthing.Http
{
    public class Credentials
    {
        public static readonly Credentials Anonymous = new Credentials();

        private Credentials()
        {
            AuthenticationType = AuthenticationType.Anonymous;
        }

        public Credentials(string key) : this(key, AuthenticationType.ApiKey) { }

        public Credentials(string key, AuthenticationType authenticationType)
        {
            Ensure.ArgumentNotNullOrEmptyString(key, nameof(key));

            Login = null;
            Password = key;
            AuthenticationType = authenticationType;
        }

        public string Login
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public AuthenticationType AuthenticationType
        {
            get;
            private set;
        }
    }
}