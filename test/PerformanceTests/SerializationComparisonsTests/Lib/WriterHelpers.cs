//---------------------------------------------------------------------
// <copyright file="WriterHelpers.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
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
                stream, isIeee754Compatible: false, encoding: encoding ?? Encoding.UTF8);
        }

        public static IJsonWriter CreateODataJsonWriterAsync(this Stream stream, Encoding encoding = null)
        {
            return jsonWriterFactory.CreateJsonWriter(
                stream, isIeee754Compatible: false, encoding: encoding ?? Encoding.UTF8);
        }

        public static IJsonWriter CreateODataUtf8JsonWriter(this Stream stream, Encoding encoding = null)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            return factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding: encoding ?? Encoding.UTF8);;
        }

        public static IJsonWriter CreateODataUtf8JsonWriterAsync(this Stream stream, Encoding encoding = null)
        {
            ODataUtf8JsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;
            return factory.CreateJsonWriter(stream, isIeee754Compatible: false, encoding: encoding ?? Encoding.UTF8);
        }

        public static IODataResponseMessage CreateJsonWriterMessage(this Stream stream, string charset = "UTF-8")
        {
            return stream.CreateODataMessage(configure: null, charset: charset);
        }

        public static IODataResponseMessage CreateNoopMessage(this Stream stream)
        {
            NoopJsonWriterFactory factory = new NoopJsonWriterFactory();

            return stream.CreateODataMessage(services =>
            {
                services.AddSingleton<IJsonWriterFactory>(factory);
            });
        }

        public static IODataResponseMessage CreateUtf8JsonWriterMessage(this Stream stream, string charset="UTF-8")
        {
            IJsonWriterFactory factory = ODataUtf8JsonWriterFactory.Default;


            return stream.CreateODataMessage(services =>
            {
                services.AddSingleton<IJsonWriterFactory>(factory);
            }, charset);
        }

        public static IODataResponseMessage CreateODataMessage(this Stream stream, Action<IServiceCollection> configure = null, string charset = "UTF-8")
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDefaultODataServices();
            

            configure?.Invoke(services);

            ODataMessage message = new ODataMessage
            {
                Stream = stream,
                ServiceProvider = services.BuildServiceProvider()
            };

            message.SetHeader("Content-Type", $"application/json; charset={charset}");

            return message;
        }
    }
}
