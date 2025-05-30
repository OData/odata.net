namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(Uri requestUri);

        Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content);

        IRequestHeaders DefaultRequestHeaders { get; }
    }

    public interface IRequestHeaders
    {
        void Add(string name, string? value);
    }

    public sealed class MockHttpClient : IHttpClient
    {
        private readonly IReadOnlyList<HttpContent> responseContent;

        private readonly List<MemoryStream> requestPayloads;
        private readonly RequestHeaders defaultRequestHeaders;
        private int currentContentIndex;

        public MockHttpClient(IReadOnlyList<HttpContent> responseContent)
        {
            this.responseContent = responseContent;

            this.requestPayloads = new List<MemoryStream>();
            this.defaultRequestHeaders = new RequestHeaders();
            this.currentContentIndex = 0;
        }

        public IReadOnlyList<MemoryStream> RequestPayloads
        {
            get
            {
                return this.requestPayloads;
            }
        }

        public IRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return this.defaultRequestHeaders;
            }
        }

        private sealed class RequestHeaders : IRequestHeaders
        {
            private readonly List<Tuple<string, string?>> values;

            public RequestHeaders()
            {
                this.values = new List<Tuple<string, string?>>();
            }

            public IEnumerable<Tuple<string, string?>> Values
            {
                get
                {
                    return this.values;
                }
            }

            public void Add(string name, string? value)
            {
                this.values.Add(Tuple.Create(name, value));
            }
        }

        public void Dispose()
        {
            foreach (var stream in this.requestPayloads)
            {
                stream.Dispose();
            }
        }

        public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            MemoryStream? stream = null;
            try
            {
                stream = new MemoryStream();
                using (var streamWriter = new StreamWriter(stream, leaveOpen: true))
                {
                    streamWriter.WriteLine($"GET {requestUri.ToString()} HTTP/1.1");
                    foreach (var header in this.defaultRequestHeaders.Values)
                    {
                        streamWriter.WriteLine($"{header.Item1}: {header.Item2 ?? string.Empty}");
                    }

                    streamWriter.WriteLine();
                }

                HttpResponseMessage? httpResponseMessage = null;
                try
                {
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = this.responseContent[this.currentContentIndex];

                    stream.Position = 0;
                    this.requestPayloads.Add(stream);

                    return await Task.FromResult(httpResponseMessage).ConfigureAwait(false);
                }
                catch
                {
                    httpResponseMessage?.Dispose();
                    throw;
                }
            }
            catch
            {
                stream?.Dispose();
                throw;
            }
        }

        public async Task<HttpResponseMessage> PatchAsync(Uri requestUri, HttpContent content)
        {
            MemoryStream? stream = null;
            try
            {
                stream = new MemoryStream();
                using (var streamWriter = new StreamWriter(stream, leaveOpen: true))
                {
                    streamWriter.WriteLine($"PATCH {requestUri.ToString()} HTTP/1.1");
                    foreach (var header in this.defaultRequestHeaders.Values)
                    {
                        streamWriter.WriteLine($"{header.Item1}: {header.Item2 ?? string.Empty}");
                    }

                    streamWriter.WriteLine();

                    content.CopyTo(stream, null, CancellationToken.None);
                }

                HttpResponseMessage? httpResponseMessage = null;
                try
                {
                    httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                    httpResponseMessage.Content = this.responseContent[this.currentContentIndex];

                    stream.Position = 0;
                    this.requestPayloads.Add(stream);

                    return await Task.FromResult(httpResponseMessage).ConfigureAwait(false);
                }
                catch
                {
                    httpResponseMessage?.Dispose();
                    throw;
                }
            }
            catch
            {
                stream?.Dispose();
                throw;
            }
        }
    }
}
