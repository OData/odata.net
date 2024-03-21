//---------------------------------------------------------------------
// <copyright file="WriterHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.OData;
using Microsoft.OData.Json;

namespace ExperimentsLib
{
    public static class WriterHelpers
    {
        private static readonly DefaultJsonWriterFactory jsonWriterFactory = new DefaultJsonWriterFactory();

        public static IJsonWriter CreateODataJsonWriter(this Stream stream, Encoding encoding = null)
        {
            return jsonWriterFactory.CreateJsonWriter(
                stream, false, encoding ?? Encoding.UTF8);
        }

        public static IJsonWriter CreateODataJsonWriterAsync(this Stream stream, Encoding encoding = null)
        {
            return jsonWriterFactory.CreateJsonWriter(
                stream, false, encoding ?? Encoding.UTF8);
        }

        public static IJsonWriter CreateODataUtf8JsonWriter(this Stream stream, Encoding encoding = null)
        {
            var factory = ODataUtf8JsonWriterFactory.Default;
            return factory.CreateJsonWriter(stream, false, encoding ?? Encoding.UTF8);
        }

        public static IJsonWriter CreateODataUtf8JsonWriterAsync(this Stream stream, Encoding encoding = null)
        {
            var factory = ODataUtf8JsonWriterFactory.Default;
            return factory.CreateJsonWriter(stream, false, encoding ?? Encoding.UTF8);
        }

        public static IODataResponseMessage CreateJsonWriterMessage(this Stream stream, string charset = "UTF-8")
        {
            return stream.CreateODataMessage(null, charset);
        }

        public static IODataResponseMessage CreateNoopMessage(this Stream stream)
        {
            NoopJsonWriterFactory factory = new NoopJsonWriterFactory();

            return stream.CreateODataMessage(services =>
            {
                services.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, _ => factory);
            });
        }

        public static IODataResponseMessage CreateUtf8JsonWriterMessage(this Stream stream, string charset="UTF-8")
        {
            var factory = ODataUtf8JsonWriterFactory.Default;

            IContainerBuilder services = new ContainerBuilder();
            services.AddDefaultODataServices();
            return stream.CreateODataMessage(services =>
            {
                services.AddService<IJsonWriterFactory>(ServiceLifetime.Singleton, _ => factory);
            }, charset);
        }

        public static IODataResponseMessage CreateODataMessage(this Stream stream, Action<IContainerBuilder> configure = null, string charset = "UTF-8")
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;

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
    }
}
