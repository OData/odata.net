//---------------------------------------------------------------------
// <copyright file="QueryOptionTestsHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.QueryOptionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class QueryOptionTestsHelper
    {
        private readonly Uri baseUri;
        private readonly IEdmModel model;
        private const string IncludeAnnotation = "odata.include-annotations";

        public QueryOptionTestsHelper(Uri baseUri, IEdmModel model)
        {
            this.baseUri = baseUri;
            this.model = model;
        }

        #region Help function

        public List<ODataResource> QueryFeed(string requestUri, string mimeType)
        {
            List<ODataResource> entries = new List<ODataResource>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataResourceSetReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            ODataResource entry = reader.Item as ODataResource;
                            entries.Add(entry);
                        }
                        else if (reader.State == ODataReaderState.ResourceSetEnd)
                        {
                            Assert.NotNull(reader.Item as ODataResourceSet);
                        }
                    }

                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        /// <summary>
        /// Specifically use to query single entry or multi entries(query with $expand)
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public List<ODataResource> QueryEntries(string requestUri, string mimeType)
        {
            List<ODataResource> entries = new List<ODataResource>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataResourceReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceEnd)
                        {
                            entries.Add(reader.Item as ODataResource);
                        }
                    }
                    Assert.Equal(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        public ODataResourceSet QueryInnerFeed(string requestUri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataResourceReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.ResourceSetEnd)
                        {
                            return (reader.Item as ODataResourceSet);
                        }
                    }

                }
            }
            return null;
        }

        public ODataProperty QueryProperty(string requestUri, string mimeType)
        {
            ODataProperty property = null;

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));

            requestMessage.SetHeader("Accept", mimeType);

            var responseMessage = requestMessage.GetResponse();
            Assert.Equal(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    property = messageReader.ReadProperty();
                }
            }
            return property;
        }

        public ODataEntityReferenceLink QueryReferenceLink(string uri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", mimeType);
            queryRequestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(200, queryResponseMessage.StatusCode);
            ODataEntityReferenceLink item = null;
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, model))
                {
                    item = messageReader.ReadEntityReferenceLink();
                }
            }

            return item;
        }

        public ODataEntityReferenceLinks QueryReferenceLinks(string uri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };

            var queryRequestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + uri, UriKind.Absolute));
            queryRequestMessage.SetHeader("Accept", mimeType);
            queryRequestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));

            var queryResponseMessage = queryRequestMessage.GetResponse();
            Assert.Equal(200, queryResponseMessage.StatusCode);
            ODataEntityReferenceLinks item = null;
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(queryResponseMessage, readerSettings, model))
                {
                    item = messageReader.ReadEntityReferenceLinks();
                }
            }

            return item;
        }
        #endregion

    }
}
