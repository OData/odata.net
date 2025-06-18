namespace NewStuff._Design._0_Convention.Sample
{
    using System;
    using System.Buffers;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.WebSockets;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;

    public sealed class HttpClientConvention : IConvention
    {
        private readonly Func<IHttpClient> httpClientFactory;

        public HttpClientConvention(Func<IHttpClient> httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        private interface IDisposer : IAsyncDisposable
        {
            T Register<T>(Func<T> factory) where T : IDisposable;

            T Register<T>(Func<Task<T>> factory) where T : IDisposable;

            void Unregister<T>(T disposable) where T : IDisposable;

            T Register2<T>(Func<T> factory) where T : IAsyncDisposable;

            void Unregister2<T>(T disposable) where T : IAsyncDisposable;
        }

        private sealed class Disposer : IDisposer
        {
            private readonly HashSet<IDisposable> disposables;

            private readonly HashSet<IAsyncDisposable> asyncDisposables;

            private readonly object @lock;

            private bool disposed;

            public Disposer()
            {
                this.disposables = new HashSet<IDisposable>();
                this.asyncDisposables = new HashSet<IAsyncDisposable>();

                this.@lock = new object(); //// TODO remove the lock
                this.disposed = false;
            }

            public async ValueTask DisposeAsync()
            {
                if (this.disposed)
                {
                    return;
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        return;
                    }

                    foreach (var disposable in this.disposables)
                    {
                        disposable.Dispose();
                    }

                    foreach (var disposable in this.asyncDisposables)
                    {
                        disposable.DisposeAsync().ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO make this async
                    }

                    this.disposed = true;
                }
            }

            public T Register<T>(Func<T> factory) where T : IDisposable
            {
                if (this.disposed)
                {
                    throw new Exception("TODO");
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        throw new Exception("TODO");
                    }

                    T? value = default;
                    try
                    {
                        value = factory();
                        this.disposables.Add(value);
                        return value;
                    }
                    catch
                    {
                        value?.Dispose();
                        throw;
                    }
                }
            }

            public T Register<T>(Func<Task<T>> factory) where T : IDisposable
            {
                if (this.disposed)
                {
                    throw new Exception("TODO");
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        throw new Exception("TODO");
                    }

                    T? value = default;
                    try
                    {
                        value = factory().ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO async
                        this.disposables.Add(value);
                        return value;
                    }
                    catch
                    {
                        value?.Dispose();
                        throw;
                    }
                }
            }

            public T Register2<T>(Func<T> factory) where T : IAsyncDisposable
            {
                if (this.disposed)
                {
                    throw new Exception("TODO");
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        throw new Exception("TODO");
                    }

                    T? value = default;
                    try
                    {
                        value = factory();
                        this.asyncDisposables.Add(value);
                        return value;
                    }
                    catch
                    {
                        value?.DisposeAsync().GetAwaiter().GetResult(); //// TODO async
                        throw;
                    }
                }
            }

            public void Unregister<T>(T disposable) where T : IDisposable
            {
                if (this.disposed)
                {
                    throw new Exception("TODO");
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        throw new Exception("TODO");
                    }

                    using (disposable)
                    {
                        this.disposables.Remove(disposable);
                    }
                }
            }

            public void Unregister2<T>(T disposable) where T : IAsyncDisposable
            {
                if (this.disposed)
                {
                    throw new Exception("TODO");
                }

                lock (this.@lock)
                {
                    if (this.disposed)
                    {
                        throw new Exception("TODO");
                    }

                    try
                    {
                        this.asyncDisposables.Remove(disposable);
                    }
                    finally
                    {
                        disposable.DisposeAsync().GetAwaiter().GetResult(); //// TODO async //// TODO also make this a using statement
                    }
                }
            }
        }

        public async Task<IGetRequestWriter> Get()
        {
            return await Task.FromResult(new GetRequestWriter(this.httpClientFactory)).ConfigureAwait(false); //// TODO you need an idispoabel for the httpclient
        }

        private sealed class GetRequestWriter : IGetRequestWriter
        {
            private readonly Func<IHttpClient> httpClientFactory;

            private readonly IDisposer disposer;
            private bool disposed;

            public GetRequestWriter(Func<IHttpClient> httpClientFactory)
            {
                this.httpClientFactory = httpClientFactory;

                this.disposer = new Disposer();
                this.disposed = false;
            }

            public async ValueTask DisposeAsync()
            {
                if (this.disposed)
                {
                    return;
                }

                await this.disposer.DisposeAsync().ConfigureAwait(false);

                this.disposed = true;
            }

            public async Task<IUriWriter<IGetHeaderWriter>> Commit()
            {
                var httpClient = this.disposer.Register(() => this.httpClientFactory());

                return await Task.FromResult<IUriWriter<IGetHeaderWriter>>(new UriWriter<IGetHeaderWriter>(requestUri => new GetHeaderWriter(this.disposer, httpClient, requestUri))).ConfigureAwait(false);
            }

            private sealed class GetHeaderWriter : IGetHeaderWriter
            {
                private readonly IDisposer disposer;
                private readonly IHttpClient httpClient;
                private readonly Uri requestUri;

                public GetHeaderWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                {
                    this.disposer = disposer;
                    this.httpClient = httpClient;
                    this.requestUri = requestUri;
                }

                public async Task<IGetBodyWriter> Commit()
                {
                    return await Task.FromResult<IGetBodyWriter>(new GetBodyWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                }

                private sealed class GetBodyWriter : IGetBodyWriter
                {
                    private readonly IDisposer disposer;
                    private readonly IHttpClient httpClient;
                    private readonly Uri requestUri;

                    public GetBodyWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                    {
                        this.disposer = disposer;
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public async Task<IGetResponseReader> Commit()
                    {
                        //// TODO malformed headers will throw here
                        var httpResponseMessage = this.disposer.Register(
                            async () => await this.httpClient.GetAsync(this.requestUri).ConfigureAwait(false));

                        return await Task.FromResult(new GetResponseReader(this.disposer, httpResponseMessage)).ConfigureAwait(false);
                    }
                }

                public async Task<ICustomHeaderWriter<IGetHeaderWriter>> CommitCustomHeader()
                {
                    return await Task.FromResult<ICustomHeaderWriter<IGetHeaderWriter>>(new CustomHeaderWriter<IGetHeaderWriter>(this.httpClient, this.requestUri, (client, uri) => new GetHeaderWriter(this.disposer, client, uri))).ConfigureAwait(false);
                }

                public async Task<IOdataMaxPageSizeHeaderWriter> CommitOdataMaxPageSize()
                {
                    return await Task.FromResult(new OdataMaxPageSizeHeaderWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                }

                private sealed class OdataMaxPageSizeHeaderWriter : IOdataMaxPageSizeHeaderWriter
                {
                    private readonly IDisposer disposer;
                    private readonly IHttpClient httpClient;
                    private readonly Uri requestUri;

                    public OdataMaxPageSizeHeaderWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                    {
                        this.disposer = disposer;
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public async Task<IGetHeaderWriter> Commit(OdataMaxPageSize odataMaxPageSize)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("OData-MaxPageSize", "100"); //// TODO use `odatamaxpagesize` once the type has actually been implemented

                        return await Task.FromResult(new GetHeaderWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                    }
                }

                public async Task<IOdataMaxVersionHeaderWriter> CommitOdataMaxVersion()
                {
                    return await Task.FromResult(new OdataMaxVersionHeaderWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                }

                private sealed class OdataMaxVersionHeaderWriter : IOdataMaxVersionHeaderWriter
                {
                    private readonly IDisposer disposer;
                    private readonly IHttpClient httpClient;
                    private readonly Uri requestUri;

                    public OdataMaxVersionHeaderWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                    {
                        this.disposer = disposer;
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public async Task<IGetHeaderWriter> Commit(OdataVersion odataVersion)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "v4.01"); //// TODO use `odataversion` once you've actually implemented that type

                        return await Task.FromResult(new GetHeaderWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                    }
                }
            }
        }

        public IPatchRequestWriter Patch()
        {
            return new PatchRequestWriter(this.httpClientFactory);
        }

        private sealed class PatchRequestWriter : IPatchRequestWriter
        {
            private readonly Func<IHttpClient> httpClientFactory;

            private readonly IDisposer disposer;
            private bool disposed;

            public PatchRequestWriter(Func<IHttpClient> httpClientFactory)
            {
                this.httpClientFactory = httpClientFactory;

                this.disposer = new Disposer();
                this.disposed = false;
            }

            public async ValueTask DisposeAsync()
            {
                if (this.disposed)
                {
                    return;
                }

                await this.disposer.DisposeAsync().ConfigureAwait(false);

                this.disposed = true;
            }

            public async Task<IUriWriter<IPatchHeaderWriter>> Commit()
            {
                var httpClient = this.disposer.Register(this.httpClientFactory);

                return await Task.FromResult(new UriWriter<IPatchHeaderWriter>(uri => new PatchHeaderWriter(this.disposer, httpClient, uri))).ConfigureAwait(false);
            }

            private sealed class PatchHeaderWriter : IPatchHeaderWriter
            {
                private readonly IDisposer disposer;
                private readonly IHttpClient httpClient;
                private readonly Uri requestUri;

                public PatchHeaderWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                {
                    this.disposer = disposer;
                    this.httpClient = httpClient;
                    this.requestUri = requestUri;
                }

                public async Task<IPatchRequestBodyWriter> Commit()
                {
                    var writeStream = this.disposer.Register(() => this.httpClient.PatchStream(this.requestUri));
                    var streamWriter = this.disposer.Register(() => new StreamWriter(writeStream, null, -1, true)); //// TODO can you know the encoding here? should that be configurable?

                    await streamWriter.WriteLineAsync("{").ConfigureAwait(false);

                    return new PatchRequestBodyWriter(this.disposer, writeStream, streamWriter, true);
                }

                private sealed class PatchRequestBodyWriter : IPatchRequestBodyWriter
                {
                    private readonly IDisposer disposer;
                    private readonly WriteStream writeStream;
                    private readonly StreamWriter streamWriter;
                    private readonly bool isFirstProperty;

                    public PatchRequestBodyWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, bool isFirstProperty)
                    {
                        this.disposer = disposer;
                        this.writeStream = writeStream;
                        this.streamWriter = streamWriter;
                        this.isFirstProperty = isFirstProperty;
                    }

                    public async Task<IGetResponseReader> Commit()
                    {
                        if (!this.isFirstProperty)
                        {
                            await streamWriter.WriteLineAsync().ConfigureAwait(false);
                        }

                        await streamWriter.WriteLineAsync("}").ConfigureAwait(false);
                        await streamWriter.DisposeAsync().ConfigureAwait(false);

                        var httpResponseMessage = this.disposer.Register(async () => await this.writeStream.Final().ConfigureAwait(false));
                        return new GetResponseReader(disposer, httpResponseMessage);
                    }

                    public async Task<IPropertyWriter<IPatchRequestBodyWriter>> CommitProperty()
                    {
                        return await Task.FromResult(new PropertyWriter<IPatchRequestBodyWriter>(this.disposer, this.writeStream, this.streamWriter, () => new PatchRequestBodyWriter(this.disposer, this.writeStream, this.streamWriter, false), this.isFirstProperty)).ConfigureAwait(false);
                    }

                    private sealed class PropertyWriter<TNext> : IPropertyWriter<TNext>
                    {
                        private readonly IDisposer disposer;
                        private readonly WriteStream writeStream;
                        private readonly StreamWriter streamWriter;
                        private readonly Func<TNext> nextFactory;
                        private readonly bool isFirstProperty;

                        public PropertyWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory, bool isFirstProperty)
                        {
                            this.disposer = disposer;
                            this.writeStream = writeStream;
                            this.streamWriter = streamWriter;
                            this.nextFactory = nextFactory;
                            this.isFirstProperty = isFirstProperty;
                        }

                        public async Task<IPropertyNameWriter<TNext>> Commit()
                        {
                            return await Task.FromResult(new PropertyNameWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory, this.isFirstProperty)).ConfigureAwait(false);
                        }

                        private sealed class PropertyNameWriter : IPropertyNameWriter<TNext>
                        {
                            private readonly IDisposer disposer;
                            private readonly WriteStream writeStream;
                            private readonly StreamWriter streamWriter;
                            private readonly Func<TNext> nextFactory;
                            private readonly bool isFirstProperty;

                            public PropertyNameWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory, bool isFirstProperty)
                            {
                                this.disposer = disposer;
                                this.writeStream = writeStream;
                                this.streamWriter = streamWriter;
                                this.nextFactory = nextFactory;
                                this.isFirstProperty = isFirstProperty;
                            }

                            public async Task<IPropertyValueWriter<TNext>> Commit(PropertyName propertyName)
                            {
                                if (!this.isFirstProperty)
                                {
                                    await this.streamWriter.WriteLineAsync(",").ConfigureAwait(false);
                                }
                                else
                                {
                                    await this.streamWriter.WriteLineAsync().ConfigureAwait(false);
                                }

                                await this.streamWriter.WriteAsync($"\"{propertyName.Name}\": ").ConfigureAwait(false);

                                return new PropertyValueWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory);
                            }

                            private sealed class PropertyValueWriter : IPropertyValueWriter<TNext>
                            {
                                private readonly IDisposer disposer;
                                private readonly WriteStream writeStream;
                                private readonly StreamWriter streamWriter;
                                private readonly Func<TNext> nextFactory;

                                public PropertyValueWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory)
                                {
                                    this.disposer = disposer;
                                    this.writeStream = writeStream;
                                    this.streamWriter = streamWriter;
                                    this.nextFactory = nextFactory;
                                }

                                public async Task<IComplexPropertyValueWriter<TNext>> CommitComplex()
                                {
                                    await this.streamWriter.WriteLineAsync("{").ConfigureAwait(false);

                                    return new ComplexPropertyValueWriter<TNext>(this.disposer, this.writeStream, this.streamWriter, this.nextFactory, true);
                                }

                                private sealed class ComplexPropertyValueWriter<TComplex> : IComplexPropertyValueWriter<TComplex>
                                {
                                    private readonly IDisposer disposer;
                                    private readonly WriteStream writeStream;
                                    private readonly StreamWriter streamWriter;
                                    private readonly Func<TComplex> nextFactory;
                                    private readonly bool isFirstProperty;

                                    public ComplexPropertyValueWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TComplex> nextFactory, bool isFirstProperty)
                                    {
                                        this.disposer = disposer;
                                        this.writeStream = writeStream;
                                        this.streamWriter = streamWriter;
                                        this.nextFactory = nextFactory;
                                        this.isFirstProperty = isFirstProperty;
                                    }

                                    public async Task<TComplex> Commit()
                                    {
                                        if (!this.isFirstProperty)
                                        {
                                            await this.streamWriter.WriteLineAsync().ConfigureAwait(false);
                                        }

                                        await this.streamWriter.WriteAsync("}").ConfigureAwait(false);

                                        return this.nextFactory();
                                    }

                                    public async Task<IPropertyWriter<IComplexPropertyValueWriter<TComplex>>> CommitProperty()
                                    {
                                        return await Task.FromResult(new PropertyWriter<IComplexPropertyValueWriter<TComplex>>(this.disposer, this.writeStream, this.streamWriter, () => new ComplexPropertyValueWriter<TComplex>(this.disposer, this.writeStream, this.streamWriter, this.nextFactory, false), true)).ConfigureAwait(false);
                                    }
                                }

                                public async Task<IMultiValuedPropertyValueWriter<TNext>> CommitMultiValued()
                                {
                                    await this.streamWriter.WriteLineAsync("[").ConfigureAwait(false);

                                    return new MultiValuedPropertyValueWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory, true);
                                }

                                private sealed class MultiValuedPropertyValueWriter : IMultiValuedPropertyValueWriter<TNext>
                                {
                                    private readonly IDisposer disposer;
                                    private readonly WriteStream writeStream;
                                    private readonly StreamWriter streamWriter;
                                    private readonly Func<TNext> nextFactory;
                                    private readonly bool isFirstElement;

                                    public MultiValuedPropertyValueWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory, bool isFirstElement)
                                    {
                                        this.disposer = disposer;
                                        this.writeStream = writeStream;
                                        this.streamWriter = streamWriter;
                                        this.nextFactory = nextFactory;
                                        this.isFirstElement = isFirstElement;
                                    }

                                    public async Task<TNext> Commit()
                                    {
                                        await this.streamWriter.WriteAsync("]").ConfigureAwait(false);

                                        return this.nextFactory();
                                    }

                                    public async Task<IComplexPropertyValueWriter<IMultiValuedPropertyValueWriter<TNext>>> CommitValue()
                                    {
                                        if (!this.isFirstElement)
                                        {
                                            await this.streamWriter.WriteLineAsync(",").ConfigureAwait(false);
                                        }

                                        await this.streamWriter.WriteLineAsync("{").ConfigureAwait(false);

                                        return new ComplexPropertyValueWriter<IMultiValuedPropertyValueWriter<TNext>>(this.disposer, this.writeStream, this.streamWriter, () => new MultiValuedPropertyValueWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory, false), true);
                                    }
                                }

                                public async Task<INullPropertyValueWriter<TNext>> CommitNull()
                                {
                                    return await Task.FromResult(new NullPropertyValueWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory)).ConfigureAwait(false);
                                }

                                private sealed class NullPropertyValueWriter : INullPropertyValueWriter<TNext>
                                {
                                    private readonly IDisposer disposer;
                                    private readonly WriteStream writeStream;
                                    private readonly StreamWriter streamWriter;
                                    private readonly Func<TNext> nextFactory;

                                    public NullPropertyValueWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory)
                                    {
                                        this.disposer = disposer;
                                        this.writeStream = writeStream;
                                        this.streamWriter = streamWriter;
                                        this.nextFactory = nextFactory;
                                    }

                                    public async Task<TNext> Commit()
                                    {
                                        await this.streamWriter.WriteAsync("null").ConfigureAwait(false);

                                        return this.nextFactory();
                                    }
                                }

                                public async Task<IPrimitivePropertyValueWriter<TNext>> CommitPrimitive()
                                {
                                    return await Task.FromResult(new PrimitivePropertyValueWriter(this.disposer, this.writeStream, this.streamWriter, this.nextFactory)).ConfigureAwait(false);
                                }

                                private sealed class PrimitivePropertyValueWriter : IPrimitivePropertyValueWriter<TNext>
                                {
                                    private readonly IDisposer disposer;
                                    private readonly WriteStream writeStream;
                                    private readonly StreamWriter streamWriter;
                                    private readonly Func<TNext> nextFactory;

                                    public PrimitivePropertyValueWriter(IDisposer disposer, WriteStream writeStream, StreamWriter streamWriter, Func<TNext> nextFactory)
                                    {
                                        this.disposer = disposer;
                                        this.writeStream = writeStream;
                                        this.streamWriter = streamWriter;
                                        this.nextFactory = nextFactory;
                                    }

                                    public async Task<TNext> Commit(PrimitivePropertyValue primitivePropertyValue)
                                    {
                                        await this.streamWriter.WriteAsync($"\"{primitivePropertyValue.Value}\"").ConfigureAwait(false); //// TODO you need to differentiate between string and non-string primitives

                                        return this.nextFactory();
                                    }
                                }
                            }
                        }
                    }
                }

                public async Task<ICustomHeaderWriter<IPatchHeaderWriter>> CommitCustomHeader()
                {
                    return await Task.FromResult(new CustomHeaderWriter<IPatchHeaderWriter>(this.httpClient, this.requestUri, (client, uri) => new PatchHeaderWriter(this.disposer, client, uri))).ConfigureAwait(false);
                }

                public async Task<IEtagWriter> CommitEtag()
                {
                    return await Task.FromResult(new EtagWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                }

                private sealed class EtagWriter : IEtagWriter
                {
                    private readonly IDisposer disposer;
                    private readonly IHttpClient httpClient;
                    private readonly Uri requestUri;

                    public EtagWriter(IDisposer disposer, IHttpClient httpClient, Uri requestUri)
                    {
                        this.disposer = disposer;
                        this.httpClient = httpClient;
                        this.requestUri = requestUri;
                    }

                    public async Task<IPatchHeaderWriter> Commit(Etag etag)
                    {
                        this.httpClient.DefaultRequestHeaders.Add("ETag", "TODO implment etag header");
                        return await Task.FromResult(new PatchHeaderWriter(this.disposer, this.httpClient, this.requestUri)).ConfigureAwait(false);
                    }
                }
            }
        }

        public IPatchRequestWriter Post()
        {
            return new PatchRequestWriter(this.httpClientFactory);
        }

        private sealed class GetResponseReader : IGetResponseReader
        {
            private readonly IDisposer disposer;
            private readonly HttpResponseMessage httpResponseMessage;

            private bool disposed;

            public GetResponseReader(IDisposer disposer, HttpResponseMessage httpResponseMessage)
            {
                this.disposer = disposer;
                this.httpResponseMessage = httpResponseMessage;

                this.disposed = false;
            }

            public async ValueTask DisposeAsync()
            {
                if (this.disposed)
                {
                    return;
                }

                await this.disposer.DisposeAsync().ConfigureAwait(false);

                this.disposed = true;
            }

            public async Task<IGetResponseHeaderReader> Next()
            {
                var headersEnumerator = this.disposer.Register(() => this.httpResponseMessage.Headers.GetEnumerator());

                return await Task.FromResult(new GetResponseHeaderReader(this.disposer, this.httpResponseMessage, headersEnumerator)).ConfigureAwait(false);
            }

            private sealed class GetResponseHeaderReader : IGetResponseHeaderReader
            {
                private readonly IDisposer disposer;
                private readonly HttpResponseMessage httpResponseMessage;
                private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                public GetResponseHeaderReader(IDisposer disposer, HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                {
                    this.disposer = disposer;
                    this.httpResponseMessage = httpResponseMessage;
                    this.headers = headers;
                }

                public async Task<GetResponseHeaderToken> Next()
                {
                    if (this.headers.MoveNext())
                    {
                        if (string.Equals(this.headers.Current.Key, "Content-Type", StringComparison.OrdinalIgnoreCase)) //// TODO should ignroing case be configurable
                        {
                            return new GetResponseHeaderToken.ContentType(new ContentTypeHeaderReader(this.disposer, this.httpResponseMessage, this.headers));
                        }
                        else
                        {
                            return new GetResponseHeaderToken.Custom(new CustomHeaderReader(this.disposer, this.httpResponseMessage, this.headers));
                        }
                    }
                    else
                    {
                        this.disposer.Unregister(this.headers);

                        var responseContent = this.disposer.Register(async () => await this.httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
                        //// TODO this doesn't actually "stream" the data; use this instead: https://stackoverflow.com/questions/43747477/how-to-parse-huge-json-file-as-stream-in-json-net
                        var jsonDocument = await JsonDocument.ParseAsync(responseContent).ConfigureAwait(false);

                        var element = jsonDocument.RootElement;
                        if (element.ValueKind != JsonValueKind.Object)
                        {
                            throw new Exception("TODO the root needs to be an object");
                        }

                        var objectEnumerator = this.disposer.Register(() => element.EnumerateObject());
                        return new GetResponseHeaderToken.Body(new GetResponseBodyReader(this.disposer, objectEnumerator));
                    }
                }

                private sealed class CustomHeaderReader : ICustomHeaderReader<IGetResponseHeaderReader>
                {
                    private readonly IDisposer disposer;
                    private readonly HttpResponseMessage httpResponseMessage;
                    private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                    public CustomHeaderReader(IDisposer disposer, HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                    {
                        this.disposer = disposer;
                        this.httpResponseMessage = httpResponseMessage;
                        this.headers = headers;
                        this.HeaderFieldName = new HeaderFieldName(this.headers.Current.Key);
                    }

                    public HeaderFieldName HeaderFieldName { get; }

                    public async Task<CustomHeaderToken<IGetResponseHeaderReader>> Next()
                    {
                        if (this.headers.Current.Value.Any())
                        {
                            return await Task.FromResult(new CustomHeaderToken<IGetResponseHeaderReader>.FieldValue(new HeaderFieldValueReader(this.disposer, this.httpResponseMessage, this.headers))).ConfigureAwait(false);
                        }
                        else
                        {
                            return await Task.FromResult(new CustomHeaderToken<IGetResponseHeaderReader>.Header(new GetResponseHeaderReader(this.disposer, this.httpResponseMessage, this.headers))).ConfigureAwait(false);
                        }
                    }

                    private sealed class HeaderFieldValueReader : IHeaderFieldValueReader<IGetResponseHeaderReader>
                    {
                        private readonly IDisposer disposer;
                        private readonly HttpResponseMessage httpResponseMessage;
                        private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                        public HeaderFieldValueReader(IDisposer disposer, HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                        {
                            this.disposer = disposer;
                            this.httpResponseMessage = httpResponseMessage;
                            this.headers = headers;
                            this.HeaderFieldValue = new HeaderFieldValue(headers.Current.Value.First()); //// TODO `.first` is not handling duplicate headers or headers with multiple provided values
                        }

                        public HeaderFieldValue HeaderFieldValue { get; }

                        public async Task<IGetResponseHeaderReader> Next()
                        {
                            return await Task.FromResult(new GetResponseHeaderReader(this.disposer, this.httpResponseMessage, this.headers)).ConfigureAwait(false);
                        }
                    }
                }

                private sealed class ContentTypeHeaderReader : IContentTypeHeaderReader
                {
                    private readonly IDisposer disposer;
                    private readonly HttpResponseMessage httpResponseMessage;
                    private readonly IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers;

                    public ContentTypeHeaderReader(IDisposer disposer, HttpResponseMessage httpResponseMessage, IEnumerator<KeyValuePair<string, IEnumerable<string>>> headers)
                    {
                        this.disposer = disposer;
                        this.httpResponseMessage = httpResponseMessage;
                        this.headers = headers;
                        this.ContentType = new ContentType(this.headers.Current.Value.First()); //// TODO `.first` is not handling duplicate headers or headers with multiple provided values
                    }

                    public ContentType ContentType { get; }

                    public async Task<IGetResponseHeaderReader> Next()
                    {
                        return await Task.FromResult(new GetResponseHeaderReader(this.disposer, this.httpResponseMessage, this.headers)).ConfigureAwait(false);
                    }
                }

                private sealed class GetResponseBodyReader : IGetResponseBodyReader
                {
                    private readonly IDisposer disposer;
                    private JsonElement.ObjectEnumerator propertyEnumerator;

                    public GetResponseBodyReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator)
                    {
                        this.disposer = disposer;
                        this.propertyEnumerator = propertyEnumerator;
                    }

                    public async Task<GetResponseBodyToken> Next()
                    {
                        if (this.propertyEnumerator.MoveNext())
                        {
                            var property = this.propertyEnumerator.Current;
                            if (string.Equals(property.Name, "@odata.context")) //// TODO ignore case?
                            {
                                return await Task.FromResult(new GetResponseBodyToken.OdataContext(new OdataContextReader(this.disposer, this.propertyEnumerator, property.Value))).ConfigureAwait(false);
                            }
                            else if (string.Equals(property.Name, "@odata.nextLink")) //// TODO ignore case?
                            {
                                return await Task.FromResult(new GetResponseBodyToken.NextLink(new NextLinkReader(this.disposer, this.propertyEnumerator, property.Value))).ConfigureAwait(false);
                            }
                            else
                            {
                                return await Task.FromResult(new GetResponseBodyToken.Property(new PropertyReader<IGetResponseBodyReader>(this.disposer, this.propertyEnumerator, enumerator => new GetResponseBodyReader(this.disposer, enumerator)))).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            this.disposer.Unregister(this.propertyEnumerator);

                            return await Task.FromResult(GetResponseBodyToken.End.Instance).ConfigureAwait(false);
                        }
                    }

                    private sealed class OdataContextReader : IOdataContextReader<IGetResponseBodyReader>
                    {
                        private readonly IDisposer disposer;
                        private JsonElement.ObjectEnumerator propertyEnumerator;

                        public OdataContextReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, JsonElement propertyValue)
                        {
                            if (propertyValue.ValueKind != JsonValueKind.String)
                            {
                                throw new Exception("TODO the context must be a string");
                            }

                            this.OdataContext = new OdataContext(propertyValue.GetString()!);
                            this.disposer = disposer;
                            this.propertyEnumerator = propertyEnumerator;
                        }

                        public OdataContext OdataContext { get; }

                        public async Task<IGetResponseBodyReader> Next()
                        {
                            return await Task.FromResult(new GetResponseBodyReader(this.disposer, this.propertyEnumerator)).ConfigureAwait(false);
                        }
                    }

                    private sealed class NextLinkReader : INextLinkReader
                    {
                        private readonly IDisposer disposer;
                        private JsonElement.ObjectEnumerator propertyEnumerator;

                        public NextLinkReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, JsonElement propertyValue)
                        {
                            if (propertyValue.ValueKind != JsonValueKind.String)
                            {
                                throw new Exception("TODO nextlink must be a string");
                            }

                            this.NextLink = new NextLink(new Uri(propertyValue.GetString()!));
                            this.disposer = disposer;
                            this.propertyEnumerator = propertyEnumerator;
                        }

                        public NextLink NextLink { get; }

                        public async Task<IGetResponseBodyReader> Next()
                        {
                            return await Task.FromResult(new GetResponseBodyReader(this.disposer, this.propertyEnumerator)).ConfigureAwait(false);
                        }
                    }

                    private sealed class PropertyReader<T> : IPropertyReader<T>
                    {
                        private readonly IDisposer disposer;
                        private JsonElement.ObjectEnumerator propertyEnumerator;
                        private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;

                        public PropertyReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory)
                        {
                            this.disposer = disposer;
                            this.propertyEnumerator = propertyEnumerator;
                            this.nextFactory = nextFactory;
                        }

                        public async Task<IPropertyNameReader<T>> Next()
                        {
                            return await Task.FromResult(new PropertyNameReader(this.disposer, this.propertyEnumerator, this.nextFactory)).ConfigureAwait(false);
                        }

                        private sealed class PropertyNameReader : IPropertyNameReader<T>
                        {
                            private readonly IDisposer disposer;
                            private JsonElement.ObjectEnumerator propertyEnumerator;
                            private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;

                            public PropertyNameReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory)
                            {
                                this.PropertyName = new PropertyName(propertyEnumerator.Current.Name);
                                this.disposer = disposer;
                                this.propertyEnumerator = propertyEnumerator;
                                this.nextFactory = nextFactory;
                            }

                            public PropertyName PropertyName { get; }

                            public async Task<IPropertyValueReader<T>> Next()
                            {
                                return await Task.FromResult(new PropertyValueReader(this.disposer, this.propertyEnumerator, this.nextFactory)).ConfigureAwait(false);
                            }

                            private sealed class PropertyValueReader : IPropertyValueReader<T>
                            {
                                private readonly IDisposer disposer;
                                private JsonElement.ObjectEnumerator propertyEnumerator;
                                private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;

                                public PropertyValueReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory)
                                {
                                    this.disposer = disposer;
                                    this.propertyEnumerator = propertyEnumerator;
                                    this.nextFactory = nextFactory;
                                }

                                public async Task<PropertyValueToken<T>> Next()
                                {
                                    var propertyValue = this.propertyEnumerator.Current.Value;
                                    if (propertyValue.ValueKind == JsonValueKind.Null)
                                    {
                                        return await Task.FromResult(new PropertyValueToken<T>.Null(new NullPropertyValueReader(this.disposer, this.propertyEnumerator, this.nextFactory))).ConfigureAwait(false);
                                    }
                                    else if (propertyValue.ValueKind == JsonValueKind.Object)
                                    {
                                        var objectEnumerator = this.disposer.Register(() => propertyValue.EnumerateObject());
                                        return await Task.FromResult(new PropertyValueToken<T>.Complex(new ComplexPropertyValueReader<T>(this.disposer, this.propertyEnumerator, this.nextFactory, objectEnumerator))).ConfigureAwait(false);
                                    }
                                    else if (propertyValue.ValueKind == JsonValueKind.Array)
                                    {
                                        var arrayEnumerator = this.disposer.Register(() => propertyValue.EnumerateArray());
                                        return await Task.FromResult(new PropertyValueToken<T>.MultiValued(new MultiValuedPropertyValueReader(this.disposer, this.propertyEnumerator, this.nextFactory, arrayEnumerator))).ConfigureAwait(false);
                                    }
                                    else
                                    {
                                        // primitive
                                        return await Task.FromResult(new PropertyValueToken<T>.Primitive(new PrimitivePropertyValueReader(this.disposer, this.propertyEnumerator, this.nextFactory, propertyValue.GetRawText()))).ConfigureAwait(false);
                                    }
                                }

                                private sealed class PrimitivePropertyValueReader : IPrimitivePropertyValueReader<T>
                                {
                                    private readonly IDisposer disposer;
                                    private JsonElement.ObjectEnumerator propertyEnumerator;
                                    private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;

                                    public PrimitivePropertyValueReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory, string value)
                                    {
                                        this.PrimitivePropertyValue = new PrimitivePropertyValue(value);
                                        this.disposer = disposer;
                                        this.propertyEnumerator = propertyEnumerator;
                                        this.nextFactory = nextFactory;
                                    }

                                    public PrimitivePropertyValue PrimitivePropertyValue { get; }

                                    public async Task<T> Next()
                                    {
                                        return await Task.FromResult(this.nextFactory(this.propertyEnumerator)).ConfigureAwait(false);
                                    }
                                }

                                private sealed class ComplexPropertyValueReader<TComplex> : IComplexPropertyValueReader<TComplex>
                                {
                                    private readonly IDisposer disposer;
                                    private JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                    private readonly Func<JsonElement.ObjectEnumerator, TComplex> nextFactory;
                                    private JsonElement.ObjectEnumerator propertyValueEnumerator;

                                    public ComplexPropertyValueReader(IDisposer disposer, JsonElement.ObjectEnumerator parentPropertyEnumerator, Func<JsonElement.ObjectEnumerator, TComplex> nextFactory, JsonElement.ObjectEnumerator propertyValueEnumerator)
                                    {
                                        this.disposer = disposer;
                                        this.parentPropertyEnumerator = parentPropertyEnumerator;
                                        this.nextFactory = nextFactory;
                                        this.propertyValueEnumerator = propertyValueEnumerator;
                                    }

                                    public async Task<ComplexPropertyValueToken<TComplex>> Next()
                                    {
                                        if (this.propertyValueEnumerator.MoveNext())
                                        {
                                            var property = this.propertyValueEnumerator.Current;
                                            if (string.Equals(property.Name, "@odata.context")) //// TODO ignore case?
                                            {
                                                return await Task.FromResult(new ComplexPropertyValueToken<TComplex>.OdataContext(new OdataContextReader(this.disposer, this.parentPropertyEnumerator, this.nextFactory, this.propertyValueEnumerator, property.Value))).ConfigureAwait(false);
                                            }
                                            else if (string.Equals(property.Name, "@odata.id")) //// TODO ignore case?
                                            {
                                                return await Task.FromResult(new ComplexPropertyValueToken<TComplex>.OdataId(new OdataIdReader(this.disposer, this.parentPropertyEnumerator, this.nextFactory, this.propertyValueEnumerator, property.Value))).ConfigureAwait(false);
                                            }

                                            return await Task.FromResult(new ComplexPropertyValueToken<TComplex>.Property(new PropertyReader<IComplexPropertyValueReader<TComplex>>(this.disposer, this.propertyValueEnumerator, enumerator => new ComplexPropertyValueReader<TComplex>(this.disposer, this.parentPropertyEnumerator, this.nextFactory, enumerator)))).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            this.disposer.Unregister(this.propertyValueEnumerator);

                                            return await Task.FromResult(new ComplexPropertyValueToken<TComplex>.End(this.nextFactory(this.parentPropertyEnumerator))).ConfigureAwait(false);
                                        }
                                    }

                                    private sealed class OdataContextReader : IOdataContextReader<IComplexPropertyValueReader<TComplex>>
                                    {
                                        private readonly IDisposer disposer;
                                        private JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                        private readonly Func<JsonElement.ObjectEnumerator, TComplex> nextFactory;
                                        private JsonElement.ObjectEnumerator propertyValueEnumerator;

                                        public OdataContextReader(IDisposer disposer, JsonElement.ObjectEnumerator parentPropertyEnumerator, Func<JsonElement.ObjectEnumerator, TComplex> nextFactory, JsonElement.ObjectEnumerator propertyValueEnumerator, JsonElement propertyValue)
                                        {
                                            if (propertyValue.ValueKind != JsonValueKind.String)
                                            {
                                                throw new Exception("TODO the context must be a string");
                                            }

                                            this.OdataContext = new OdataContext(propertyValue.GetString()!);
                                            this.disposer = disposer;
                                            this.parentPropertyEnumerator = parentPropertyEnumerator;
                                            this.nextFactory = nextFactory;
                                            this.propertyValueEnumerator = propertyValueEnumerator;
                                        }

                                        public OdataContext OdataContext { get; }

                                        public async Task<IComplexPropertyValueReader<TComplex>> Next()
                                        {
                                            return await Task.FromResult(new ComplexPropertyValueReader<TComplex>(this.disposer, this.parentPropertyEnumerator, this.nextFactory, this.propertyValueEnumerator)).ConfigureAwait(false);
                                        }
                                    }

                                    private sealed class OdataIdReader : IOdataIdReader<IComplexPropertyValueReader<TComplex>>
                                    {
                                        private readonly IDisposer disposer;
                                        private JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                        private readonly Func<JsonElement.ObjectEnumerator, TComplex> nextFactory;
                                        private JsonElement.ObjectEnumerator propertyValueEnumerator;

                                        public OdataIdReader(IDisposer disposer, JsonElement.ObjectEnumerator parentPropertyEnumerator, Func<JsonElement.ObjectEnumerator, TComplex> nextFactory, JsonElement.ObjectEnumerator propertyValueEnumerator, JsonElement propertyValue)
                                        {
                                            if (propertyValue.ValueKind != JsonValueKind.String)
                                            {
                                                throw new Exception("TODO the context must be a string");
                                            }

                                            this.OdataId = new OdataId(propertyValue.GetString()!);
                                            this.disposer = disposer;
                                            this.parentPropertyEnumerator = parentPropertyEnumerator;
                                            this.nextFactory = nextFactory;
                                            this.propertyValueEnumerator = propertyValueEnumerator;
                                        }

                                        public OdataId OdataId { get; }

                                        public async Task<IComplexPropertyValueReader<TComplex>> Next()
                                        {
                                            return await Task.FromResult(new ComplexPropertyValueReader<TComplex>(this.disposer, this.parentPropertyEnumerator, this.nextFactory, this.propertyValueEnumerator)).ConfigureAwait(false);
                                        }
                                    }
                                }

                                private sealed class MultiValuedPropertyValueReader : IMultiValuedPropertyValueReader<T>
                                {
                                    private readonly IDisposer disposer;
                                    private JsonElement.ObjectEnumerator parentPropertyEnumerator;
                                    private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;
                                    private JsonElement.ArrayEnumerator arrayEnumerator;

                                    public MultiValuedPropertyValueReader(IDisposer disposer, JsonElement.ObjectEnumerator parentPropertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory, JsonElement.ArrayEnumerator arrayEnumerator)
                                    {
                                        this.disposer = disposer;
                                        this.parentPropertyEnumerator = parentPropertyEnumerator;
                                        this.nextFactory = nextFactory;
                                        this.arrayEnumerator = arrayEnumerator;
                                    }

                                    public async Task<MultiValuedPropertyValueToken<T>> Next()
                                    {
                                        if (this.arrayEnumerator.MoveNext())
                                        {
                                            if (this.arrayEnumerator.Current.ValueKind != JsonValueKind.Object)
                                            {
                                                throw new Exception("TODO only objects are allowed in collections (except for the times where that's not true)");
                                            }

                                            var objectEnumerator = this.disposer.Register(() => this.arrayEnumerator.Current.EnumerateObject());
                                            return await Task.FromResult(new MultiValuedPropertyValueToken<T>.Object(new ComplexPropertyValueReader<IMultiValuedPropertyValueReader<T>>(this.disposer, this.parentPropertyEnumerator, parentEnumerator => new MultiValuedPropertyValueReader(this.disposer, parentPropertyEnumerator, this.nextFactory, this.arrayEnumerator), objectEnumerator))).ConfigureAwait(false);
                                            //// TODO what about collections of primitives?
                                        }
                                        else
                                        {
                                            this.disposer.Unregister(this.arrayEnumerator);

                                            return await Task.FromResult(new MultiValuedPropertyValueToken<T>.End(this.nextFactory(this.parentPropertyEnumerator))).ConfigureAwait(false);
                                        }
                                    }
                                }

                                private sealed class NullPropertyValueReader : INullPropertyValueReader<T>
                                {
                                    private readonly IDisposer disposer;
                                    private JsonElement.ObjectEnumerator propertyEnumerator;
                                    private readonly Func<JsonElement.ObjectEnumerator, T> nextFactory;

                                    public NullPropertyValueReader(IDisposer disposer, JsonElement.ObjectEnumerator propertyEnumerator, Func<JsonElement.ObjectEnumerator, T> nextFactory)
                                    {
                                        this.disposer = disposer;
                                        this.propertyEnumerator = propertyEnumerator;
                                        this.nextFactory = nextFactory;
                                    }

                                    public async Task<T> Next()
                                    {
                                        return await Task.FromResult(this.nextFactory(this.propertyEnumerator)).ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private sealed class CustomHeaderWriter<T> : ICustomHeaderWriter<T>
        {
            private readonly IHttpClient httpClient;
            private readonly Uri requestUri;
            private readonly Func<IHttpClient, Uri, T> nextFactory;

            public CustomHeaderWriter(IHttpClient httpClient, Uri requestUri, Func<IHttpClient, Uri, T> nextFactory)
            {
                this.httpClient = httpClient;
                this.requestUri = requestUri;
                this.nextFactory = nextFactory;
            }

            public async Task<IHeaderFieldValueWriter<T>> Commit(HeaderFieldName headerFieldName)
            {
                return await Task.FromResult(new HeaderFieldValueWriter(this.httpClient, this.requestUri, this.nextFactory, headerFieldName.Name)).ConfigureAwait(false);
            }

            private sealed class HeaderFieldValueWriter : IHeaderFieldValueWriter<T>
            {
                private readonly IHttpClient httpClient;
                private readonly Uri requestUri;
                private readonly Func<IHttpClient, Uri, T> nextFactory;
                private readonly string headerName;

                public HeaderFieldValueWriter(IHttpClient httpClient, Uri requestUri, Func<IHttpClient, Uri, T> nextFactory, string headerName)
                {
                    this.httpClient = httpClient;
                    this.requestUri = requestUri;
                    this.nextFactory = nextFactory;
                    this.headerName = headerName;
                }

                public async Task<T> Commit()
                {
                    this.httpClient.DefaultRequestHeaders.Add(this.headerName, (string?)null);

                    return await Task.FromResult(this.nextFactory(this.httpClient, this.requestUri)).ConfigureAwait(false);
                }

                public async Task<T> Commit(HeaderFieldValue headerFieldValue)
                {
                    this.httpClient.DefaultRequestHeaders.Add(this.headerName, headerFieldValue.Value);

                    return await Task.FromResult(this.nextFactory(this.httpClient, this.requestUri)).ConfigureAwait(false);
                }
            }
        }

        private sealed class UriWriter<T> : IUriWriter<T>
        {
            private readonly Func<Uri, T> nextFactory;

            public UriWriter(Func<Uri, T> nextFactory)
            {
                this.nextFactory = nextFactory;
            }

            public async Task<IUriSchemeWriter<T>> Commit()
            {
                return await Task.FromResult(new UriSchemeWriter(new StringBuilder(), this.nextFactory)).ConfigureAwait(false);
            }

            private sealed class UriSchemeWriter : IUriSchemeWriter<T>
            {
                private readonly StringBuilder builder;
                private readonly Func<Uri, T> nextFactory;

                public UriSchemeWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                {
                    this.builder = builder; //// TODO this means that writer instances won't be re-usable, are you ok with that?
                    this.nextFactory = nextFactory;
                }

                public async Task<IUriDomainWriter<T>> Commit(UriScheme uriScheme)
                {
                    this.builder.Append($"{uriScheme.Scheme}://");
                    return await Task.FromResult(new UriDomainWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                }

                private sealed class UriDomainWriter : IUriDomainWriter<T>
                {
                    private readonly StringBuilder builder;
                    private readonly Func<Uri, T> nextFactory;

                    public UriDomainWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                    {
                        this.builder = builder;
                        this.nextFactory = nextFactory;
                    }

                    public async Task<IUriPortWriter<T>> Commit(UriDomain uriDomain)
                    {
                        this.builder.Append(uriDomain.Domain);
                        return await Task.FromResult(new UriPortWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                    }

                    private sealed class UriPortWriter : IUriPortWriter<T>
                    {
                        private readonly StringBuilder builder;
                        private readonly Func<Uri, T> nextFactory;

                        public UriPortWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                        {
                            this.builder = builder;
                            this.nextFactory = nextFactory;
                        }

                        public async Task<IUriPathSegmentWriter<T>> Commit()
                        {
                            return await Task.FromResult(new UriPathSegmentWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                        }

                        public async Task<IUriPathSegmentWriter<T>> Commit(UriPort uriPort)
                        {
                            this.builder.Append($":{uriPort.Port}");
                            return await Task.FromResult(new UriPathSegmentWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                        }

                        private sealed class UriPathSegmentWriter : IUriPathSegmentWriter<T>
                        {
                            private readonly StringBuilder builder;
                            private readonly Func<Uri, T> nextFactory;

                            public UriPathSegmentWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                            {
                                this.builder = builder;
                                this.nextFactory = nextFactory;
                            }

                            public async Task<IQueryOptionWriter<T>> Commit()
                            {
                                return await Task.FromResult(new QueryOptionWriter(this.builder, this.nextFactory, false)).ConfigureAwait(false);
                            }

                            private sealed class QueryOptionWriter : IQueryOptionWriter<T>
                            {
                                private readonly StringBuilder builder;
                                private readonly Func<Uri, T> nextFactory;
                                private readonly bool queryParametersWritten;

                                public QueryOptionWriter(StringBuilder builder, Func<Uri, T> nextFactory, bool queryParametersWritten)
                                {
                                    this.builder = builder;
                                    this.nextFactory = nextFactory;
                                    this.queryParametersWritten = queryParametersWritten;
                                }

                                public async Task<T> Commit()
                                {
                                    return await Task.FromResult(this.nextFactory(new Uri(this.builder.ToString()))).ConfigureAwait(false);
                                }

                                public async Task<IFragmentWriter<T>> CommitFragment()
                                {
                                    return await Task.FromResult(new FragmentWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                                }

                                private sealed class FragmentWriter : IFragmentWriter<T>
                                {
                                    private readonly StringBuilder builder;
                                    private readonly Func<Uri, T> nextFactory;

                                    public FragmentWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                                    {
                                        this.builder = builder;
                                        this.nextFactory = nextFactory;
                                    }

                                    public async Task<T> Commit(Fragment fragment)
                                    {
                                        this.builder.Append($"#{fragment.Value}");

                                        return await Task.FromResult(this.nextFactory(new Uri(this.builder.ToString()))).ConfigureAwait(false);
                                    }
                                }

                                public async Task<IQueryParameterWriter<T>> CommitParameter()
                                {
                                    return await Task.FromResult(new QueryParameterWriter(this.builder, this.nextFactory, this.queryParametersWritten)).ConfigureAwait(false);
                                }

                                private sealed class QueryParameterWriter : IQueryParameterWriter<T>
                                {
                                    private readonly StringBuilder builder;
                                    private readonly Func<Uri, T> nextFactory;
                                    private readonly bool queryParametersWritten;

                                    public QueryParameterWriter(StringBuilder builder, Func<Uri, T> nextFactory, bool queryParametersWritten)
                                    {
                                        this.builder = builder;
                                        this.nextFactory = nextFactory;
                                        this.queryParametersWritten = queryParametersWritten;
                                    }

                                    public async Task<IQueryValueWriter<T>> Commit(QueryParameter queryParameter)
                                    {
                                        if (this.queryParametersWritten)
                                        {
                                            this.builder.Append("&");
                                        }
                                        else
                                        {
                                            this.builder.Append("?");
                                        }

                                        this.builder.Append(queryParameter.Name);

                                        return await Task.FromResult(new QueryValueWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                                    }

                                    private sealed class QueryValueWriter : IQueryValueWriter<T>
                                    {
                                        private readonly StringBuilder builder;
                                        private readonly Func<Uri, T> nextFactory;

                                        public QueryValueWriter(StringBuilder builder, Func<Uri, T> nextFactory)
                                        {
                                            this.builder = builder;
                                            this.nextFactory = nextFactory;
                                        }

                                        public async Task<IQueryOptionWriter<T>> Commit()
                                        {
                                            return await Task.FromResult(new QueryOptionWriter(this.builder, this.nextFactory, true)).ConfigureAwait(false);
                                        }

                                        public async Task<IQueryOptionWriter<T>> Commit(QueryValue queryValue)
                                        {
                                            this.builder.Append($"={queryValue.Value}");

                                            return await Task.FromResult(new QueryOptionWriter(this.builder, this.nextFactory, true)).ConfigureAwait(false);
                                        }
                                    }
                                }
                            }

                            public async Task<IUriPathSegmentWriter<T>> Commit(UriPathSegment uriPathSegment)
                            {
                                this.builder.Append($"/{uriPathSegment.Segment}");

                                return await Task.FromResult(new UriPathSegmentWriter(this.builder, this.nextFactory)).ConfigureAwait(false);
                            }
                        }
                    }
                }
            }
        }
    }
}
