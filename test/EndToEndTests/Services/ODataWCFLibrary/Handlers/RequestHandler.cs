//---------------------------------------------------------------------
// <copyright file="RequestHandler.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.Text;
    using Microsoft.OData;
    using Microsoft.OData.UriParser;
    using Microsoft.Test.OData.Services.ODataWCFService.DataSource;

    /// <summary>
    /// Base class for processing and responding to a client request.
    /// </summary>
    public abstract class RequestHandler
    {
        /// <summary>
        /// Create a RequestHandler on current context.
        /// </summary>
        /// <param name="httpMethod"></param>
        protected RequestHandler(HttpMethod httpMethod, IODataDataSource dataSource)
        {
            this.HttpMethod = httpMethod;
            this.RequestUri = Utility.RebuildUri(OperationContext.Current.RequestContext.RequestMessage.Properties.Via);

            this.DataSource = dataSource;
            this.RootContainer = this.DataSource.Container;
            this.RequestContainer = null;

            this.RequestAcceptHeader = WebOperationContext.Current.IncomingRequest.Accept;
            this.RequestHeaders = WebOperationContext.Current.IncomingRequest.Headers.ToDictionary();

            this.ServiceRootUri = Utility.RebuildUri(new Uri(OperationContext.Current.Host.BaseAddresses.First().AbsoluteUri.TrimEnd('/') + "/"));

            this.QueryContext = new QueryContext(this.ServiceRootUri, this.RequestUri, this.DataSource.Model, this.RequestContainer);

            string preference = this.RequestHeaders.ContainsKey(ServiceConstants.HttpHeaders.Prefer) ? this.RequestHeaders[ServiceConstants.HttpHeaders.Prefer] : string.Empty;
            this.PreferenceContext = new PreferenceContext(preference);
        }

        protected RequestHandler(RequestHandler other, HttpMethod httpMethod, Uri requestUri = null, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            // TODO: [tiano] We should have a deep check in to prevent infinite loop caused by bad code.
            this.HttpMethod = httpMethod;

            if (requestUri == null)
            {
                this.RequestUri = Utility.RebuildUri(other.RequestUri);
            }
            else
            {
                this.RequestUri = Utility.RebuildUri(requestUri);
            }


            this.DataSource = other.DataSource;
            this.RootContainer = other.RootContainer;
            this.RequestContainer = other.RequestContainer;

            if (headers == null)
            {
                this.RequestAcceptHeader = other.RequestAcceptHeader;
                this.RequestHeaders = new Dictionary<string, string>(other.RequestHeaders);
            }
            else
            {
                this.RequestHeaders = new Dictionary<string, string>();

                foreach (KeyValuePair<string, string> kvp in headers)
                {
                    this.RequestHeaders[kvp.Key] = kvp.Value;
                }

                this.RequestAcceptHeader = this.RequestHeaders.ContainsKey("Accept") ? this.RequestHeaders["Accept"] : string.Empty;
            }

            this.ServiceRootUri = Utility.RebuildUri(other.ServiceRootUri);

            this.QueryContext = new QueryContext(this.ServiceRootUri, this.RequestUri, this.DataSource.Model, this.RequestContainer);

            this.PreferenceContext = other.PreferenceContext;
        }

        public HttpMethod HttpMethod
        {
            get;
            private set;
        }

        public Uri ServiceRootUri
        {
            get;
            private set;
        }

        public IODataDataSource DataSource
        {
            get;
            private set;
        }

        public IServiceProvider RootContainer
        {
            get;
            private set;
        }

        public IServiceProvider RequestContainer
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the QueryContext.
        /// </summary>
        public QueryContext QueryContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the PreferenceContext
        /// </summary>
        public PreferenceContext PreferenceContext
        {
            get;
            private set;
        }

        public Dictionary<string, string> RequestHeaders
        {
            get;
            private set;
        }

        public string RequestAcceptHeader
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the URI for the incoming request.
        /// </summary>
        public Uri RequestUri
        {
            get;
            private set;
        }

        protected bool TryDispatch(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            RequestHandler handler = this.DispatchHandler();

            if (handler != null)
            {
                handler.Process(requestMessage, responseMessage);
                return true;
            }

            return false;
        }

        protected virtual RequestHandler DispatchHandler()
        {
            return null;
        }

        public virtual void Process(IODataRequestMessage requestMessage, IODataResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }

        public virtual Stream Process(Stream requestStream)
        {
            RequestHandler handler = this.DispatchHandler();
            if (handler != null)
            {
                return handler.Process(requestStream);
            }

            StreamPipe pipe = new StreamPipe();
            this.Process(this.CreateRequestMessage(requestStream), this.CreateResponseMessage(pipe.WriteStream));

            return pipe.ReadStream;
        }

        public virtual Stream ProcessAsynchronously(Stream requestStream)
        {
            DateTime now = DateTime.Now;
            string asyncToken = now.Ticks.ToString(CultureInfo.InvariantCulture);

            AsyncTask asyncTask = null;
            if (requestStream == null)
            {
                asyncTask = new AsyncTask(this, this.CreateRequestMessage(null), now.AddSeconds(AsyncTask.DefaultDuration));
            }
            else
            {
                StreamPipe requestPipe = new StreamPipe();
                using (requestPipe.WriteStream)
                {
                    requestStream.CopyTo(requestPipe.WriteStream); // read the input stream to memory
                }

                var requestMessage = this.CreateRequestMessage(requestPipe.ReadStream);
                asyncTask = new AsyncTask(this, requestMessage, now.AddSeconds(AsyncTask.DefaultDuration));
            }
            AsyncTask.AddTask(asyncToken, asyncTask);

            StreamPipe responsePipe = new StreamPipe();
            var responseMessage = new ODataResponseMessage(responsePipe.WriteStream, 202); //202 Accepted
            responseMessage.PreferenceAppliedHeader().RespondAsync = true;
            ResponseWriter.WriteAsyncPendingResponse(responseMessage, asyncToken);
            return responsePipe.ReadStream;
        }


        #region Reader API

        protected virtual IODataRequestMessage CreateRequestMessage(Stream messageBody)
        {
            return new ODataRequestMessage(messageBody, this.RequestHeaders, this.RequestUri, this.HttpMethod.ToString())
            {
                Container = this.RequestContainer
            };
        }

        protected virtual IODataResponseMessage CreateResponseMessage(Stream stream)
        {
            return new ODataResponseMessage(stream, 200)
            {
                Container = this.RequestContainer
            };
        }

        protected virtual ODataMessageReader CreateMessageReader(IODataRequestMessage message)
        {
            return new ODataMessageReader(message, this.GetReaderSettings());
        }

        protected virtual ODataMessageWriter CreateMessageWriter(IODataResponseMessage message)
        {
            return new ODataMessageWriter(message, this.GetWriterSettings());
        }

        #endregion

        #region Write API

        protected virtual ODataMessageReaderSettings GetReaderSettings()
        {
            // Let ODataMessageReader get ODataMessageReaderSettings from the container.
            return null;
        }

        protected virtual ODataMessageWriterSettings GetWriterSettings()
        {
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings
            {
                BaseUri = this.ServiceRootUri,
                ODataUri = new ODataUri()
                {
                    RequestUri = this.RequestUri,
                    ServiceRoot = this.ServiceRootUri,
                    Path = this.QueryContext.QueryPath,
                    SelectAndExpand = this.QueryContext.QuerySelectExpandClause,
                },
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType,
                // EnableIndentation = true
            };

            // TODO: howang why here?
            if (this.QueryContext != null)
            {
                if (this.QueryContext.CanonicalUri == null)
                {
                    settings.ODataUri.RequestUri = this.QueryContext.QueryUri;
                    settings.ODataUri.Path = this.QueryContext.QueryPath;
                }
                else
                {
                    settings.ODataUri.RequestUri = this.QueryContext.CanonicalUri;
                    ODataUriParser uriParser = new ODataUriParser(this.DataSource.Model, ServiceConstants.ServiceBaseUri, this.QueryContext.CanonicalUri);
                    settings.ODataUri.Path = uriParser.ParsePath();
                }
            }

            // TODO: howang read the encoding from request.
            settings.SetContentType(string.IsNullOrEmpty(this.QueryContext.FormatOption) ? this.RequestAcceptHeader : this.QueryContext.FormatOption, Encoding.UTF8.WebName);

            return settings;
        }

        #endregion

        /// <summary>
        /// Annotation for marking a new entry with bound associated entries.
        /// </summary>
        protected class BoundNavigationPropertyAnnotation
        {
            public IList<Tuple<ODataNestedResourceInfo, object>> BoundProperties { get; set; }
        }

        /// <summary>
        /// Annotation for marking a navigation property or feed with new entry instances belonging to it.
        /// </summary>
        protected class ChildInstanceAnnotation
        {
            public IList<object> ChildInstances { get; set; }
        }

        protected static void AddChildInstanceAnnotation(ODataItem item, object childEntry)
        {
            var annotation = item.GetAnnotation<ChildInstanceAnnotation>();
            if (annotation == null)
            {
                annotation = new ChildInstanceAnnotation { ChildInstances = new List<object>() };
                item.SetAnnotation(annotation);
            }

            annotation.ChildInstances.Add(childEntry);
        }
        protected static void AddChildInstanceAnnotations(ODataItem item, IList<object> childEntries)
        {
            var annotation = item.GetAnnotation<ChildInstanceAnnotation>();
            if (annotation == null)
            {
                annotation = new ChildInstanceAnnotation { ChildInstances = childEntries };
                item.SetAnnotation(annotation);
            }
        }

        protected static void AddBoundNavigationPropertyAnnotation(ODataItem item, ODataNestedResourceInfo navigationLink, object boundValue)
        {
            var annotation = item.GetAnnotation<BoundNavigationPropertyAnnotation>();
            if (annotation == null)
            {
                annotation = new BoundNavigationPropertyAnnotation { BoundProperties = new List<Tuple<ODataNestedResourceInfo, object>>() };
                item.SetAnnotation(annotation);
            }

            annotation.BoundProperties.Add(new Tuple<ODataNestedResourceInfo, object>(navigationLink, boundValue));
        }
    }
}
