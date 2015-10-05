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
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        public List<ODataEntry> QueryFeed(string requestUri, string mimeType)
        {
            List<ODataEntry> entries = new List<ODataEntry>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataFeedReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            ODataEntry entry = reader.Item as ODataEntry;
                            entries.Add(entry);
                        }
                        else if (reader.State == ODataReaderState.FeedEnd)
                        {
                            Assert.IsNotNull(reader.Item as ODataFeed);
                        }
                    }

                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
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
        public List<ODataEntry> QueryEntries(string requestUri, string mimeType)
        {
            List<ODataEntry> entries = new List<ODataEntry>();

            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            requestMessage.SetHeader("Prefer", string.Format("{0}={1}", IncludeAnnotation, "*"));
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataEntryReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.EntryEnd)
                        {
                            entries.Add(reader.Item as ODataEntry);
                        }
                    }
                    Assert.AreEqual(ODataReaderState.Completed, reader.State);
                }
            }
            return entries;
        }

        public ODataFeed QueryInnerFeed(string requestUri, string mimeType)
        {
            ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings() { BaseUri = baseUri };
            var requestMessage = new HttpWebRequestMessage(new Uri(baseUri.AbsoluteUri + requestUri, UriKind.Absolute));
            requestMessage.SetHeader("Accept", mimeType);
            var responseMessage = requestMessage.GetResponse();
            Assert.AreEqual(200, responseMessage.StatusCode);

            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                using (var messageReader = new ODataMessageReader(responseMessage, readerSettings, model))
                {
                    var reader = messageReader.CreateODataEntryReader();

                    while (reader.Read())
                    {
                        if (reader.State == ODataReaderState.FeedEnd)
                        {
                            return (reader.Item as ODataFeed);
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
            Assert.AreEqual(200, responseMessage.StatusCode);

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
            Assert.AreEqual(200, queryResponseMessage.StatusCode);
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
            Assert.AreEqual(200, queryResponseMessage.StatusCode);
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
