//---------------------------------------------------------------------
// <copyright file="UnitTestPayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.OData;
    using System.Collections;
    using System.Linq;

    public class UnitTestPayloadGenerator
    {
        private static readonly Uri MetadataDocumentUri = new Uri("http://http://temp.org/$metadata");
        private static readonly ODataMessageWriterSettings Settings = new ODataMessageWriterSettings();
        private static readonly ODataResourceSerializationInfo MySerializationInfo = new ODataResourceSerializationInfo()
        {
            NavigationSourceEntityTypeName = "Null",
            NavigationSourceName = "MySet",
            ExpectedTypeName = "Null"
        };

        static UnitTestPayloadGenerator()
        {
            Settings.SetServiceDocumentUri(MetadataDocumentUri);
            // Settings.EnableAtom = true;
        }
        
        public UnitTestPayloadGenerator(HttpStatusCode statusCode, string contentType)
        {
            this.StatusCode = statusCode;
            this.ContentType = contentType;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ContentType { get; private set; }

        public string GetSamplePropertyPayload(string propertyName, object propertyValue)
        {
            var message = new SimpleResponseMessage(this.StatusCode, this.ContentType);
            using (var writer = new ODataMessageWriter(message, Settings))
            {
                WriteProperty(writer, propertyName, propertyValue);
            }

            return message.GetMessageString();
        }

        public string GetSampleEntityPayload(object entity, IEnumerable<string> projectedProperties)
        {
            var message = new SimpleResponseMessage(this.StatusCode, this.ContentType);
            using (var writer = new ODataMessageWriter(message, Settings))
            {
                WriteEntry(writer.CreateODataResourceWriter(), entity, projectedProperties);
            }

            return message.GetMessageString();
        }

        public string GetSampleFeedPayload(IEnumerable entities, IEnumerable<string> projectedProperties)
        {
            var message = new SimpleResponseMessage(this.StatusCode, this.ContentType);
            using (var writer = new ODataMessageWriter(message, Settings))
            {
                var feedWriter = writer.CreateODataResourceSetWriter();
                feedWriter.WriteStart(new ODataResourceSet() { Id = new Uri("http://temp.org/feed"), SerializationInfo = MySerializationInfo });
                foreach (var entity in entities)
                {
                    WriteEntry(feedWriter, entity, projectedProperties);
                }

                feedWriter.WriteEnd();
            }

            return message.GetMessageString();
        }

        private static void WriteEntry(ODataWriter writer, object entity, IEnumerable<string> projectedProperties)
        {
            var entry = new ODataResource()
            {
                Id = new Uri("http://temp.org/" + Guid.NewGuid()),
                SerializationInfo = MySerializationInfo
            };

            entry.Properties = entity.GetType().GetProperties().Select(p => new ODataProperty() { Name = p.Name, Value = p.GetValue(entity, null) });

            writer.WriteStart(entry);
            writer.WriteEnd();
        }

        private static void WriteProperty(ODataMessageWriter writer, string name, object value)
        {
            writer.WriteProperty(new ODataProperty() { Name = name, Value = value });
        }

        private class SimpleResponseMessage : IODataResponseMessage
        {
            public SimpleResponseMessage(HttpStatusCode statusCode, string contentType)
            {
                this.StatusCode = (int)statusCode;
                this.HeaderDictionary = new Dictionary<string, string>()
                {
                    { "Content-Type", contentType },
                };

                this.Stream = new MemoryStream();
            }

            public IEnumerable<KeyValuePair<string, string>> Headers
            {
                get { return this.HeaderDictionary; }
            }

            public int StatusCode { get; set; }

            internal IDictionary<string, string> HeaderDictionary { get; private set; }

            internal Stream Stream { get; private set; }

            public string GetHeader(string headerName)
            {
                string headerValue;
                if (!this.HeaderDictionary.TryGetValue(headerName, out headerValue))
                {
                    headerValue = null;
                }

                return headerValue;
            }

            public void SetHeader(string headerName, string headerValue)
            {
                this.HeaderDictionary[headerName] = headerValue;
            }

            public Stream GetStream()
            {
                return this.Stream;
            }

            internal string GetMessageString()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("HTTP/1.1 {0} {1}", this.StatusCode, (HttpStatusCode)this.StatusCode);
                builder.AppendLine();
                foreach (var header in this.Headers)
                {
                    builder.AppendFormat("{0}: {1}", header.Key, header.Value);
                    builder.AppendLine();
                }

                builder.AppendLine();

                builder.Append(Encoding.UTF8.GetString(((MemoryStream)this.Stream).ToArray()));

                return builder.ToString();
            }
        }
    }
}
