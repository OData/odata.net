namespace Microsoft.Epm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate Task<HttpServerResponse> HttpRequestHandler(HttpServerRequest request);

    public class HttpServer
    {
        private readonly HttpRequestHandler httpRequestHandler;

        public HttpServer(HttpRequestHandler httpRequestHandler)
        {
            this.httpRequestHandler = httpRequestHandler;
        }

        public async Task ListenAsync()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://+:8080/");
                listener.Start();

                while (true)
                {
                    Console.WriteLine("Listening");
                    HttpListenerContext context;
                    try
                    {
                        context = await listener.GetContextAsync().ConfigureAwait(false);
                    }
                    catch (HttpListenerException)
                    {
                        //// TODO
                        throw;
                    }

                    Console.WriteLine($"Received: TODO");

                    _ = Task.Factory.StartNew(HandleRequest, (context, this.httpRequestHandler), CancellationToken.None);
                }
            }
        }

        private static async Task HandleRequest(object? contextObject)
        {
            if (contextObject == null)
            {
                throw new InvalidOperationException("TODO");
            }

            var context = ((HttpListenerContext Context, HttpRequestHandler Handler))contextObject;
            if (context.Context == null)
            {
                throw new InvalidOperationException("TODO");
            }

            //// TODO handle all exceptions that could happen

            var request = new HttpServerRequest()
            {
                HttpMethod = context.Context.Request.HttpMethod,
                Headers = GetHeaders(context.Context.Request.Headers),
                Body = context.Context.Request.InputStream,
            };
            var response = await context.Handler(request);

            SetHeaders(context.Context.Response.Headers, response.Headers);
            context.Context.Response.StatusCode = response.StatusCode;
            await response.Body.CopyToAsync(context.Context.Response.OutputStream);
            context.Context.Response.OutputStream.Dispose(); //// TODO disposes aren't called correctly for exception cases
        }

        private static IEnumerable<string> GetHeaders(NameValueCollection headers)
        {
            foreach (var key in headers.AllKeys)
            {
                yield return $"{key}: {headers[key]}";
            }
        }

        private static void SetHeaders(NameValueCollection currentHeaders, IEnumerable<string> newHeaders)
        {
            currentHeaders.Clear();
            foreach (var header in newHeaders)
            {
                var nameValue = header.Split(':', 2);
                if (nameValue.Length < 0)
                {
                    throw new InvalidOperationException("TODO");
                }

                //// TODO this is the same as ElementAtOrDefault that is defined in peachy.core; how can we share that code without breaking the veil that these are separate projects entirely?
                currentHeaders.Add(nameValue[0], nameValue.Length < 2 ? string.Empty : nameValue[1]);
            }
        }
    }
}