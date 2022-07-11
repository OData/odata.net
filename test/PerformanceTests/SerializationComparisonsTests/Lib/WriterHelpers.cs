using System;
using System.IO;
using System.Text;
using Microsoft.OData;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    public static class WriterHelpers
    {
        static DefaultJsonWriterFactory jsonWriterFactory = new DefaultJsonWriterFactory();

        public static IJsonWriter CreateODataJsonWriter(this Stream stream, Encoding encoding = null)
        {
            return jsonWriterFactory.CreateJsonWriter(
                new StreamWriter(stream, encoding ?? Encoding.UTF8), false);
        }

        public static IJsonWriterAsync CreateODataJsonWriterAsync(this Stream stream, Encoding encoding = null)
        {
            return jsonWriterFactory.CreateAsynchronousJsonWriter(
                new StreamWriter(stream, encoding ?? Encoding.UTF8), false);
        }

        public static IJsonWriter CreateODataUtf8JsonWriter(this Stream stream, Encoding encoding = null)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;
            return factory.CreateJsonWriter(stream, false, encoding ?? Encoding.UTF8);
        }

        public static IODataResponseMessage CreateUtf8Message(this Stream stream)
        {
            ODataMessage message = new ODataMessage { Stream = stream };
            message.SetHeader("Content-Type", "application/json; charset=UTF-8");
            return message;
        }

        public static IODataResponseMessage CreateODataMessage(this Stream stream, Action<IContainerBuilder> configure = null, string charset = "UTF-8")
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;

            IContainerBuilder services = new ContainerBuilder();
            services.AddDefaultODataServices();
            configure?.Invoke(services);

            ODataMessage message = new ODataMessage
            {
                Stream = stream,
                Container = services.BuildContainer()
            };

            message.SetHeader("Content-Type", $"application/json; charset={charset}");

            return message;
        }

        public static IODataResponseMessage CreateUtf16Message(this Stream stream)
        {
            return stream.CreateODataMessage(null, charset: "UTF-16");
        }

        public static IODataResponseMessage CreateNoopMessage(this Stream stream)
        {
            NoopJsonWriterFactory factory = new NoopJsonWriterFactory();

            return stream.CreateODataMessage(services =>
            {
                services.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, _ => factory);
                services.AddService<IJsonWriterFactoryAsync>(ServiceLifetime.Singleton, _ => factory);
            });
        }

        public static IODataResponseMessage CreateUtf8JsonWriterMessage(this Stream stream)
        {
            DefaultStreamBasedJsonWriterFactory factory = DefaultStreamBasedJsonWriterFactory.Default;

            IContainerBuilder services = new ContainerBuilder();
            services.AddDefaultODataServices();
            return stream.CreateODataMessage(services =>
            {
                services.AddService<IStreamBasedJsonWriterFactory>(ServiceLifetime.Singleton, _ => factory);
            });
        }
    }
}
