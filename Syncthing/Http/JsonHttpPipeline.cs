using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Syncthing.Helpers;
using Syncthing.Models.Response;

namespace Syncthing.Http
{
    public class JsonHttpPipeline
    {
        public JsonHttpPipeline()
        {
        }

        public void SerializeRequest(IRequest request)
        {
            Ensure.ArgumentNotNull(request, nameof(request));

            if (!request.Headers.ContainsKey("Accept"))
            {
                request.Headers["Accept"] = "application/json";
            }

            if (request.Method == HttpMethod.Get || request.Body == null) return;
            if (request.Body is string || request.Body is Stream || request.Body is HttpContent) return;

            request.Body = JsonConvert.SerializeObject(request.Body);
        }

        public IApiResponse<T> DeserializeResponse<T>(IResponse response)
        {
            Ensure.ArgumentNotNull(response, nameof(response));

            if (response.ContentType != null && response.ContentType.Equals("application/json", StringComparison.Ordinal))
            {
                var body = response.Body as string;
                // simple json does not support the root node being empty. Will submit a pr but in the mean time....
                if (!string.IsNullOrEmpty(body) && body != "{}")
                {
                    var typeIsDictionary = typeof(IDictionary<,>).IsAssignableFrom(typeof(T));
                    var typeIsEnumerable = typeof(IEnumerable).IsAssignableFrom(typeof(T));
                    var responseIsObject = body.StartsWith("{", StringComparison.Ordinal);

                    // If we're expecting an array, but we get a single object, just wrap it.
                    // This supports an API that dynamically changes the return type based on the content.
                    if (!typeIsDictionary && typeIsEnumerable && responseIsObject)
                    {
                        body = "[" + body + "]";
                    }
                    var json = JsonConvert.DeserializeObject<T>(body);
                    return new ApiResponse<T>(response, json);
                }
            }
            return new ApiResponse<T>(response);
        }
    }
}