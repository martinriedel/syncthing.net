using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Syncthing.Authentication;
using Syncthing.Exceptions;
using Syncthing.Helpers;
#if !HAS_ENVIRONMENT
using System.Runtime.InteropServices;
#endif

namespace Syncthing.Http
{
    // NOTE: Every request method must go through the `RunRequest` code path. So if you need to add a new method
    // ensure it goes through there. :)
    /// <summary>
    /// A connection for making HTTP requests against URI endpoints.
    /// </summary>
    public class Connection : IConnection
    {
        static readonly ICredentialStore _anonymousCredentials = new InMemoryCredentialStore(Credentials.Anonymous);

        readonly Authenticator _authenticator;
        readonly JsonHttpPipeline _jsonPipeline = new JsonHttpPipeline();
        readonly IHttpClient _httpClient;

        /// <summary>
        /// Creates a new connection instance used to make requests of the Syncthing API.
        /// </summary>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://localhost:8384/
        /// instance</param>
        public Connection(Uri baseAddress)
            : this(baseAddress, _anonymousCredentials)
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the Syncthing API.
        /// </summary>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://localhost:8384/
        /// instance</param>
        /// <param name="credentialStore">Provides credentials to the client when making requests</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public Connection(Uri baseAddress, ICredentialStore credentialStore)
            : this(baseAddress, credentialStore, new HttpClientAdapter(HttpMessageHandlerFactory.CreateDefault))
        {
        }

        /// <summary>
        /// Creates a new connection instance used to make requests of the Syncthing API.
        /// </summary>
        /// <param name="baseAddress">
        /// The address to point this client to such as https://localhost:8384/
        /// instance</param>
        /// <param name="credentialStore">Provides credentials to the client when making requests</param>
        /// <param name="httpClient">A raw <see cref="IHttpClient"/> used to make requests</param>
        public Connection(
            Uri baseAddress,
            ICredentialStore credentialStore,
            IHttpClient httpClient)
        {
            Ensure.ArgumentNotNull(baseAddress, nameof(baseAddress));
            Ensure.ArgumentNotNull(credentialStore, nameof(credentialStore));
            Ensure.ArgumentNotNull(httpClient, nameof(httpClient));

            if (!baseAddress.IsAbsoluteUri)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "The base address '{0}' must be an absolute URI",
                        baseAddress), nameof(baseAddress));
            }

            BaseAddress = baseAddress;
            _authenticator = new Authenticator(credentialStore);
            _httpClient = httpClient;
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null,
                CancellationToken.None);
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, IDictionary<string, string> parameters, string accepts,
            CancellationToken cancellationToken)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Get, null, accepts, null, cancellationToken);
        }

        public Task<IApiResponse<T>> Get<T>(Uri uri, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Get, null, null, null, timeout, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpVerb.Patch, body, null, null, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Patch<T>(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpVerb.Patch, body, accepts, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Post(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, null, null, null, CancellationToken.None)
                .ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        public async Task<HttpStatusCode> Post(Uri uri, object body, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Post, body, accepts, null, CancellationToken.None)
                .ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            return SendData<T>(uri, HttpMethod.Post, null, null, null, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType,
            IDictionary<string, string> parameters)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri.ApplyParameters(parameters), HttpMethod.Post, body, accepts, contentType,
                CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP POST request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="body">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <param name="contentType">Specifies the media type of the request body</param>
        /// <param name="twoFactorAuthenticationCode">Two Factor Authentication Code</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType,
            string twoFactorAuthenticationCode)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));
            Ensure.ArgumentNotNullOrEmptyString(twoFactorAuthenticationCode, nameof(twoFactorAuthenticationCode));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, CancellationToken.None,
                twoFactorAuthenticationCode);
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, TimeSpan timeout)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, timeout, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Post<T>(Uri uri, object body, string accepts, string contentType, Uri baseAddress)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(body, nameof(body));

            return SendData<T>(uri, HttpMethod.Post, body, accepts, contentType, CancellationToken.None,
                baseAddress: baseAddress);
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, object body)
        {
            return SendData<T>(uri, HttpMethod.Put, body, null, null, CancellationToken.None);
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                null,
                null,
                CancellationToken.None,
                twoFactorAuthenticationCode);
        }

        public Task<IApiResponse<T>> Put<T>(Uri uri, object body, string twoFactorAuthenticationCode, string accepts)
        {
            return SendData<T>(uri,
                HttpMethod.Put,
                body,
                accepts,
                null,
                CancellationToken.None,
                twoFactorAuthenticationCode);
        }

        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            TimeSpan timeout,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.GreaterThanZero(timeout, nameof(timeout));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri,
                Timeout = timeout
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode,
                request);
        }

        Task<IApiResponse<T>> SendData<T>(
            Uri uri,
            HttpMethod method,
            object body,
            string accepts,
            string contentType,
            CancellationToken cancellationToken,
            string twoFactorAuthenticationCode = null,
            Uri baseAddress = null)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = method,
                BaseAddress = baseAddress ?? BaseAddress,
                Endpoint = uri
            };

            return SendDataInternal<T>(body, accepts, contentType, cancellationToken, twoFactorAuthenticationCode,
                request);
        }

        Task<IApiResponse<T>> SendDataInternal<T>(object body, string accepts, string contentType,
            CancellationToken cancellationToken, string twoFactorAuthenticationCode, Request request)
        {
            if (!string.IsNullOrEmpty(accepts))
            {
                request.Headers["Accept"] = accepts;
            }

            if (body != null)
            {
                request.Body = body;
                // Default Content Type
                request.ContentType = contentType ?? "application/json";
            }

            return Run<T>(request, cancellationToken);
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpVerb.Patch,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PATCH request.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns><seealso cref="IResponse"/> representing the received HTTP response</returns>
        public async Task<HttpStatusCode> Patch(Uri uri, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response =
                await SendData<object>(uri, new HttpMethod("PATCH"), null, accepts, null, CancellationToken.None)
                    .ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Put(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Put,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP PUT request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="accepts">Specifies accepted response media types.</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Put(Uri uri, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, HttpMethod.Put, null, accepts, null, CancellationToken.None)
                .ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="twoFactorAuthenticationCode">Two Factor Code</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, string twoFactorAuthenticationCode)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));

            var response = await SendData<object>(uri, HttpMethod.Delete, null, null, null, CancellationToken.None,
                twoFactorAuthenticationCode).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            var request = new Request
            {
                Method = HttpMethod.Delete,
                Body = data,
                BaseAddress = BaseAddress,
                Endpoint = uri
            };
            var response = await Run<object>(request, CancellationToken.None).ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request that expects an empty response.
        /// </summary>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        /// <returns>The returned <seealso cref="HttpStatusCode"/></returns>
        public async Task<HttpStatusCode> Delete(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            var response = await SendData<object>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None)
                .ConfigureAwait(false);
            return response.HttpResponse.StatusCode;
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// </summary>
        /// <typeparam name="T">The API resource's type.</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(data, nameof(data));

            return SendData<T>(uri, HttpMethod.Delete, data, null, null, CancellationToken.None);
        }

        /// <summary>
        /// Performs an asynchronous HTTP DELETE request.
        /// Attempts to map the response body to an object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type to map the response to</typeparam>
        /// <param name="uri">URI endpoint to send request to</param>
        /// <param name="data">The object to serialize as the body of the request</param>
        /// <param name="accepts">Specifies accept response media type</param>
        public Task<IApiResponse<T>> Delete<T>(Uri uri, object data, string accepts)
        {
            Ensure.ArgumentNotNull(uri, nameof(uri));
            Ensure.ArgumentNotNull(accepts, nameof(accepts));

            return SendData<T>(uri, HttpMethod.Delete, data, accepts, null, CancellationToken.None);
        }

        /// <summary>
        /// Base address for the connection.
        /// </summary>
        public Uri BaseAddress { get; private set; }

        public string UserAgent { get; private set; }

        /// <summary>
        /// Gets the <seealso cref="ICredentialStore"/> used to provide credentials for the connection.
        /// </summary>
        public ICredentialStore CredentialStore
        {
            get { return _authenticator.CredentialStore; }
        }

        /// <summary>
        /// Gets or sets the credentials used by the connection.
        /// </summary>
        /// <remarks>
        /// You can use this property if you only have a single hard-coded credential. Otherwise, pass in an
        /// <see cref="ICredentialStore"/> to the constructor.
        /// Setting this property will change the <see cref="ICredentialStore"/> to use
        /// the default <see cref="InMemoryCredentialStore"/> with just these credentials.
        /// </remarks>
        public Credentials Credentials
        {
            get
            {
                var credentialTask = CredentialStore.GetCredentials();
                if (credentialTask == null) return Credentials.Anonymous;
                return credentialTask.Result ?? Credentials.Anonymous;
            }
            // Note this is for convenience. We probably shouldn't allow this to be mutable.
            set
            {
                Ensure.ArgumentNotNull(value, nameof(value));
                _authenticator.CredentialStore = new InMemoryCredentialStore(value);
            }
        }

        async Task<IApiResponse<T>> Run<T>(IRequest request, CancellationToken cancellationToken)
        {
            _jsonPipeline.SerializeRequest(request);
            var response = await RunRequest(request, cancellationToken).ConfigureAwait(false);
            return _jsonPipeline.DeserializeResponse<T>(response);
        }

        // THIS IS THE METHOD THAT EVERY REQUEST MUST GO THROUGH!
        async Task<IResponse> RunRequest(IRequest request, CancellationToken cancellationToken)
        {
            await _authenticator.Apply(request).ConfigureAwait(false);
            var response = await _httpClient.Send(request, cancellationToken).ConfigureAwait(false);
            HandleErrors(response);
            return response;
        }

        static readonly Dictionary<HttpStatusCode, Func<IResponse, Exception>> HttpExceptionMap =
            new Dictionary<HttpStatusCode, Func<IResponse, Exception>>
            {
                {HttpStatusCode.Unauthorized, GetExceptionForUnauthorized},
                {HttpStatusCode.Forbidden, GetExceptionForForbidden},
                {HttpStatusCode.NotFound, response => new NotFoundException(response)}
            };

        static void HandleErrors(IResponse response)
        {
            Func<IResponse, Exception> exceptionFunc;
            if (HttpExceptionMap.TryGetValue(response.StatusCode, out exceptionFunc))
            {
                throw exceptionFunc(response);
            }

            if ((int) response.StatusCode >= 400)
            {
                throw new ApiException(response);
            }
        }

        static Exception GetExceptionForUnauthorized(IResponse response)
        {
            return new AuthorizationException(response);
        }

        static Exception GetExceptionForForbidden(IResponse response)
        {
            string body = response.Body as string ?? "";

            if (body.Contains("number of login attempts exceeded"))
            {
                return new LoginAttemptsExceededException(response);
            }

            if (body.Contains("abuse-rate-limits") || body.Contains("abuse detection mechanism"))
            {
                return new AbuseException(response);
            }

            return new ForbiddenException(response);
        }

        static string FormatUserAgent(ProductHeaderValue productInformation)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} ({1}; {2}; Octokit {3})",
                productInformation,
                GetPlatformInformation(),
                GetCultureInformation(),
                GetVersionInformation());
        }

        private static string _platformInformation;

        static string GetPlatformInformation()
        {
            if (string.IsNullOrEmpty(_platformInformation))
            {
                try
                {
                    _platformInformation = string.Format(CultureInfo.InvariantCulture,
#if !HAS_ENVIRONMENT
                        "{0}; {1}",
                        RuntimeInformation.OSDescription.Trim(),
                        RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant().Trim()
#else
                        "{0} {1}; {2}",
                        Environment.OSVersion.Platform,
                        Environment.OSVersion.Version.ToString(3),
                        Environment.Is64BitOperatingSystem ? "amd64" : "x86"
#endif
                    );
                }
                catch
                {
                    _platformInformation = "Unknown Platform";
                }
            }

            return _platformInformation;
        }

        static string GetCultureInformation()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        private static string _versionInformation;

        static string GetVersionInformation()
        {
            if (string.IsNullOrEmpty(_versionInformation))
            {
                _versionInformation = typeof(SyncthingClient)
                    .GetTypeInfo()
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            }

            return _versionInformation;
        }

        /// <summary>
        /// Set the request timeout.
        /// </summary>
        /// <param name="timeout">The Timeout value</param>
        public void SetRequestTimeout(TimeSpan timeout)
        {
            _httpClient.SetRequestTimeout(timeout);
        }
    }
}
