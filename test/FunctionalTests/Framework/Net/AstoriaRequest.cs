//---------------------------------------------------------------------
// <copyright file="AstoriaRequest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
#if !ClientSKUFramework
using System.Data.Test.Astoria.CallOrder;
#endif

using System.Xml.Linq;
using System.Data.Test.Astoria.FullTrust;

namespace System.Data.Test.Astoria
{
    public class AstoriaRequest : AstoriaRequestResponseBase
    {
        private const string ApplicationJson = "application/json";

        #region inter-dependent properties, with infrastructure to differentiate between internal and external sets/gets
        #region Query
        private QueryNode _Query = null;
        private bool _QueryModified = false;

        protected QueryNode Query_Internal
        {
            get
            {
                return _Query;
            }

            set
            {
                if (!_QueryModified)
                {
                    _Query = value;
                    RefreshURI();
                }
            }
        }

        public QueryNode Query
        {
            get
            {
                return Query_Internal;
            }
            set
            {
                _Query = value;
                _QueryModified = true;
                RefreshURI();
            }
        }
        #endregion

        #region TunnelledVerb
        private RequestVerb? _TunnelledVerb = null;
        private bool _TunnelledVerbModified = false;

        protected RequestVerb? TunnelledVerb_Internal
        {
            get
            {
                return _TunnelledVerb;
            }

            set
            {
                if (!_TunnelledVerbModified)
                {
                    _TunnelledVerb = value;
                    RefreshXHTTPMethod();
                    RefreshExpectedStatusCode();
                }
            }
        }

        public RequestVerb? TunnelledVerb
        {
            get
            {
                return TunnelledVerb_Internal;
            }
            set
            {
                _TunnelledVerb = value;
                _TunnelledVerbModified = true;
                RefreshXHTTPMethod();
                RefreshExpectedStatusCode();
            }
        }
        #endregion

        #region Format
        private SerializationFormatKind _Format = SerializationFormatKind.Default;
        private bool _FormatModified = false;

        protected SerializationFormatKind Format_Internal
        {
            get
            {
                return _Format;
            }

            set
            {
                if (!_FormatModified)
                {
                    _Format = value;
                    RefreshContentType();
                    RefreshAccept();
                    RefreshPayload();
                }
            }
        }

        public SerializationFormatKind Format
        {
            get
            {
                return Format_Internal;
            }
            set
            {
                _Format = value;
                _FormatModified = true;
                RefreshContentType();
                RefreshAccept();
                RefreshPayload();
            }
        }
        #endregion

        #region ContentType
        private string _ContentType;
        private bool _ContentTypeSet = false;
        private bool _ContentTypeModified = false;

        protected string ContentType_Internal
        {
            get
            {
                if (!_ContentTypeSet)
                    RefreshContentType();
                return _ContentType;
            }

            set
            {
                if (!_ContentTypeModified)
                {
                    _ContentTypeSet = true;
                    _ContentType = value;
                    if (value != null)
                        Headers["Content-Type"] = value;
                    else if (Headers.ContainsKey("Content-Type"))
                        Headers.Remove("Content-Type");
                }
            }
        }

        public override sealed string ContentType
        {
            get
            {
                return ContentType_Internal;
            }
            set
            {
                _ContentType = value;
                _ContentTypeSet = true;
                _ContentTypeModified = true;
                if (value != null)
                    Headers["Content-Type"] = value;
                else if (Headers.ContainsKey("Content-Type"))
                    Headers.Remove("Content-Type");
            }
        }

        protected virtual void RefreshContentType()
        {
            if (Payload == null && (Verb_Internal == RequestVerb.Get || Verb_Internal == RequestVerb.Delete))
            {
                ContentType_Internal = null;
            }
            else
            {
                ResourceProperty property = GetPropertyFromQuery();
                bool propertyUri = (property != null && !property.IsNavigation && !(property.Type is CollectionType));
                bool valueUri = URI_Internal.Contains("$value");
                bool linksUri = URI_Internal.Contains("$ref");

                if (propertyUri && !property.IsComplexType && valueUri)
                {
                    if (property.Type.ClrType == typeof(byte[]))
                    {
                        ContentType_Internal = SerializationFormatKinds.MimeApplicationOctetStream;
                    }
                    else
                    {
                        ContentType_Internal = SerializationFormatKinds.MimeTextPlain;
                    }
                }
                else if (propertyUri && !valueUri && (Format == SerializationFormatKind.Atom || Format == SerializationFormatKind.Default))
                {
                    ContentType_Internal = SerializationFormatKinds.MimeApplicationXml;
                }
                else if (linksUri && (Format == SerializationFormatKind.Atom || Format == SerializationFormatKind.Default))
                {
                    ContentType_Internal = "application/xml";
                }
                else if (Format == SerializationFormatKind.JSON)
                {
                    ContentType_Internal = ApplicationJson + ";odata.metadata=verbose";
                }
                else
                {
                    ContentType_Internal = SerializationFormatKinds.ContentTypeFromKind(Format);
                }

                ContentType_Internal = RequestUtil.RandomizeContentTypeCapitalization(ContentType_Internal);
            }
        }
        #endregion

        #region Accept
        private string _Accept;
        private bool _AcceptSet = false;
        private bool _AcceptModified = false;

        protected string Accept_Internal
        {
            get
            {
                if (!_AcceptSet)
                    RefreshAccept();
                return _Accept;
            }

            set
            {
                if (!_AcceptModified)
                {
                    _AcceptSet = true;
                    _Accept = value;
                    Headers["Accept"] = value;
                }
            }
        }

        public string Accept
        {
            get
            {
                return Accept_Internal;
            }
            set
            {
                _Accept = value;
                _AcceptSet = true;
                _AcceptModified = true;
                Headers["Accept"] = value;
            }
        }
        protected virtual void RefreshAccept()
        {
            ResourceProperty property = GetPropertyFromQuery();
            bool propertyUri = (property != null && !property.IsNavigation && !(property.Type is CollectionType));
            bool valueUri = URI_Internal.Contains("$value");
            bool linksUri = URI_Internal.Contains("$ref");

            if (valueUri)
            {
                Accept_Internal = null;
            }
            else if (this.MetadataOnly)
            {
                Accept_Internal = "*/*";
            }
            else if ((propertyUri || linksUri) && (Format == SerializationFormatKind.Atom || Format == SerializationFormatKind.Default))
            {
                Accept_Internal = "application/xml";
            }
            else if(Format == SerializationFormatKind.JSON)
            {
                Accept_Internal = ApplicationJson + ";odata.metadata=verbose";
            }
            else
            {
                Accept_Internal = SerializationFormatKinds.ContentTypeFromKind(Format);
            }

            Accept_Internal = RequestUtil.RandomizeContentTypeCapitalization(Accept_Internal);
        }
        #endregion

        #region URI
        private string _URI;
        private bool _URISet = false;
        private bool _URIModified = false;

        protected string URI_Internal
        {
            get
            {
                if (!_URISet)
                    RefreshURI();
                return _URI;
            }

            set
            {
                if (!_URIModified)
                {
                    _URISet = true;
                    _URI = value;
                    RefreshAccept();
                    RefreshContentType();
                    RefreshPayload();
                    RefreshExpectedStatusCode();
                }
            }
        }

        public string URI
        {
            get
            {
                return URI_Internal;
            }
            set
            {
                _URI = value;
                _URISet = true;
                _URIModified = true;
                RefreshAccept();
                RefreshContentType();
                RefreshPayload();
                RefreshExpectedStatusCode();
            }
        }
        protected virtual void RefreshURI()
        {
            if (Query_Internal != null)
            {
                UriQueryBuilder uriBuilder = new UriQueryBuilder(Workspace, Workspace.ServiceUri);
                URI_Internal = uriBuilder.Build(Query_Internal);
            }
            else
                URI_Internal = Workspace.ServiceUri;
        }
        #endregion

        #region XHTTPMethod
        private string _XHTTPMethod;
        private bool _XHTTPMethodSet = false;
        private bool _XHTTPMethodModified = false;

        protected string XHTTPMethod_Internal
        {
            get
            {
                if (!_XHTTPMethodSet)
                    RefreshXHTTPMethod();
                return _XHTTPMethod;
            }

            set
            {
                if (!_XHTTPMethodModified)
                {
                    _XHTTPMethodSet = true;
                    _XHTTPMethod = value;
                    if (value != null)
                        Headers["X-HTTP-Method"] = value;
                    else
                        Headers.Remove("X-HTTP-Method");
                }
            }
        }

        public string XHTTPMethod
        {
            get
            {
                return XHTTPMethod_Internal;
            }
            set
            {
                _XHTTPMethod = value;
                _XHTTPMethodSet = true;
                _XHTTPMethodModified = true;
                if (value != null)
                    Headers["X-HTTP-Method"] = value;
                else
                    Headers.Remove("X-HTTP-Method");
            }
        }

        protected void RefreshXHTTPMethod()
        {
            if (TunnelledVerb_Internal.HasValue)
                XHTTPMethod_Internal = TunnelledVerb.Value.ToHttpMethod();
            else
                XHTTPMethod_Internal = null;
        }
        #endregion

        #region Payload
        private string _Payload;
        private bool _PayloadSet = false;
        private bool _PayloadModified = false;

        protected string Payload_Internal
        {
            get
            {
                if (!_PayloadSet)
                    RefreshPayload();
                return _Payload;
            }

            set
            {
                if (!_PayloadModified)
                {
                    _PayloadSet = true;
                    _Payload = value;
                    if (value != null)
                        _PayloadBytes = Encoding.UTF8.GetBytes(value);
                    else
                        _PayloadBytes = null;
                }
            }
        }

        public override sealed string Payload
        {
            get
            {
                return Payload_Internal;
            }
            set
            {
                Payload_Internal = value;
                _PayloadModified = true;
            }
        }

        private byte[] _PayloadBytes;
        private bool _PayloadBytesSet = false;
        protected byte[] PayloadBytes_Internal
        {
            get
            {
                if (!_PayloadBytesSet)
                    RefreshPayload();
                return _PayloadBytes;
            }

            set
            {
                if (!_PayloadModified)
                {
                    _PayloadBytesSet = true;
                    _PayloadBytes = value;
                    if (value != null)
                        _Payload = Encoding.UTF8.GetString(value);
                    else
                        _Payload = null;
                }
            }
        }

        public byte[] PayloadBytes
        {
            get
            {
                return PayloadBytes_Internal;
            }
            set
            {
                PayloadBytes_Internal = value;
                _PayloadModified = true;
            }
        }

        // BatchRequest will override this
        protected virtual void RefreshPayload()
        {
            if (UpdateTree == null)
                Payload_Internal = null;
            else
            {
                if (ContentType_Internal != null && ContentType_Internal.Equals(SerializationFormatKinds.MimeApplicationOctetStream, StringComparison.InvariantCultureIgnoreCase))
                {
                    object value = (UpdateTree as ResourceInstanceSimpleProperty).PropertyValue;
                    if (value == null)
                        PayloadBytes_Internal = null;
                    else if (value.GetType() == typeof(byte[]))
                    {
                        PayloadBytes_Internal = (byte[])value;
                        return;
                    }
                }
                UpdatePayloadBuilder payloadBuilder = UpdatePayloadBuilder.CreateUpdatePayloadBuilder(this.Workspace, Format_Internal, Verb_Internal);
                Payload_Internal = payloadBuilder.Build(UpdateTree);
            }
        }
        #endregion

        #region ExpectedStatusCode
        private HttpStatusCode _ExpectedStatusCode;
        private bool _ExpectedStatusCodeSet = false;
        private bool _ExpectedStatusCodeModified = false;

        protected HttpStatusCode ExpectedStatusCode_Internal
        {
            get
            {
                if (!_ExpectedStatusCodeSet)
                    RefreshExpectedStatusCode();
                return _ExpectedStatusCode;
            }

            set
            {
                if (!_ExpectedStatusCodeModified)
                {
                    _ExpectedStatusCodeSet = true;
                    _ExpectedStatusCode = value;
                }
            }
        }

        public HttpStatusCode ExpectedStatusCode
        {
            get
            {
                return ExpectedStatusCode_Internal;
            }
            set
            {
                _ExpectedStatusCode = value;
                _ExpectedStatusCodeSet = true;
                _ExpectedStatusCodeModified = true;
            }
        }

        public virtual HttpStatusCode DefaultExpectedStatusCode()
        {
            RequestVerb verb;
            if (TunnelledVerb_Internal.HasValue)
                verb = TunnelledVerb_Internal.Value;
            else
                verb = Verb_Internal;

            HttpStatusCode code;
            switch (verb)
            {
                case RequestVerb.Get:
                    code = HttpStatusCode.OK;
                    break;

                case RequestVerb.Put:
                case RequestVerb.Patch:
                case RequestVerb.Delete:
                    code = HttpStatusCode.NoContent;
                    break;

                case RequestVerb.Post:
                    if (URI_Internal.Contains("$ref"))
                        code = HttpStatusCode.NoContent;
                    else if (URI_Internal.Contains("$batch"))
                        code = HttpStatusCode.Accepted;
                    else
                        code = HttpStatusCode.Created;
                    break;

                default:
                    code = HttpStatusCode.NotImplemented;
                    break;
            }
            return code;
        }

        protected virtual void RefreshExpectedStatusCode()
        {
            ExpectedStatusCode_Internal = DefaultExpectedStatusCode();
        }
        #endregion

        #region Verb
        private RequestVerb _Verb;
        private bool _VerbSet = false;
        private bool _VerbModified = false;

        protected RequestVerb Verb_Internal
        {
            get
            {
                if (!_VerbSet)
                    AstoriaTestLog.FailAndThrow("Cannot use Verb before it is defined");
                return _Verb;
            }

            set
            {
                if (!_VerbModified)
                {
                    _VerbSet = true;
                    _Verb = value;
                    RefreshExpectedStatusCode();
                }
            }
        }

        public RequestVerb Verb
        {
            get
            {
                return Verb_Internal;
            }
            set
            {
                _Verb = value;
                _VerbSet = true;
                _VerbModified = true;
                RefreshExpectedStatusCode();
            }
        }
        #endregion

        #region MetadataOnly
        private bool _MetadataOnly = false;
        public bool MetadataOnly
        {
            get { return _MetadataOnly; }
            set
            {
                _MetadataOnly = value;
                if (_MetadataOnly == true)
                {
                    URI_Internal = Workspace.ServiceUri + "/$metadata";
                    RefreshAccept();
                }
            }
        }
        #endregion

        #region UpdateTree
        private ResourceBodyTree _UpdateTree;
        public ResourceBodyTree UpdateTree
        {
            get
            {
                return _UpdateTree;
            }
            set
            {
                if (value != _UpdateTree)
                {
                    _UpdateTree = value;
                    RefreshPayload();
                }
            }
        }
        #endregion
        #endregion

        #region events
        public event EventHandler<RequestEventArgs> OnSendEvent = SnapshotBeforeUpdate;
        public event EventHandler<ResponseEventArgs> OnReceiveEvent = SnapshotAfterUpdate;

        internal void OnSend(object caller)
        {
            if (OnSendEvent != null)
                OnSendEvent(caller, new RequestEventArgs(this));
        }

        internal void OnReceive(object caller, AstoriaResponse response)
        {
            if (OnReceiveEvent != null)
                OnReceiveEvent(caller, new ResponseEventArgs(response));
        }

        private static CommonPayload SnapshotEntityState(AstoriaRequest request)
        {
            if (!request.SnapshotForUpdate)
                return null;

            request.SnapshotForUpdate = true;

            // try to figure out entity-level request

            Workspace workspace = request.Workspace;
            List<string> neededSegments = new List<string>() { workspace.ServiceUri };

            string[] segments = request.URI.Replace(workspace.ServiceUri, null).Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            string firstSegment = segments[0];
            neededSegments.Add(firstSegment);

            if (firstSegment.Contains('('))
                firstSegment = firstSegment.Substring(0, firstSegment.IndexOf('('));
            ResourceContainer container = workspace.ServiceContainer.ResourceContainers[firstSegment];

            if (container == null)
                return null;

            ResourceType baseType = container.BaseType;
            bool etagExpected = baseType.Properties.Any(p => p.Facets.ConcurrencyModeFixed);
            for (int i = 1; i < segments.Length; i++)
            {
                string propertyName = segments[i];
                if (propertyName.Contains('('))
                    propertyName = propertyName.Substring(0, propertyName.IndexOf('('));

                ResourceProperty property = baseType.Properties[propertyName] as ResourceProperty;
                if (property == null || !property.IsNavigation)
                    break;

                etagExpected = property.OtherAssociationEnd.ResourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed);
                neededSegments.Add(segments[i]);
            }

            AstoriaRequest get = workspace.CreateRequest();
            get.Format = request.Format;
            get.ETagHeaderExpected = etagExpected;
            get.URI = string.Join("/", neededSegments.ToArray());
            AstoriaResponse getResponse = get.GetResponse();
            getResponse.Verify();

            return getResponse.CommonPayload;
        }

        private static void SnapshotBeforeUpdate(object sender, RequestEventArgs args)
        {
            args.Request.BeforeUpdatePayload = SnapshotEntityState(args.Request);
        }

        private static void SnapshotAfterUpdate(object sender, ResponseEventArgs args)
        {
            args.Response.Request.AfterUpdatePayload = SnapshotEntityState(args.Response.Request);
        }

        #endregion

        protected internal AstoriaRequest(Workspace w)
            : base(w)
        {
            RequestSender = AstoriaRequestSender.GetSenderByTestProperties();

            Batched = false;
            IsBlobRequest = false;
            BeforeUpdatePayload = null;
            UnderlyingRequestFailed = false;
#if !ClientSKUFramework
            APICallLogEntries = null;
#endif
        }

        public override sealed string ETagHeader
        {
            get
            {
                return base.ETagHeader;
            }
            set
            {
                base.ETagHeader = value;
            }
        }

        public string Host
        {
            get
            {
                string host;
                if (Headers.TryGetValue("Host", out host))
                    return host;
                return null;
            }
            set
            {
                Headers["Host"] = value;
            }
        }

        public string RelativeUri
        {
            get
            {
                // this is inefficient to recompute every time, but annoying to keep in sync with URI
                if (Host != null && URI_Internal.StartsWith(Host, StringComparison.InvariantCultureIgnoreCase))
                    return URI_Internal.Substring(Host.Length);
                else
                    return URI_Internal.Replace(Workspace.ServiceRoot.AbsoluteUri, string.Empty);
            }
        }

        public RequestVerb EffectiveVerb
        {
            get
            {
                if (TunnelledVerb_Internal.HasValue && Verb_Internal == RequestVerb.Post)
                    return TunnelledVerb_Internal.Value;
                return Verb_Internal;
            }
        }

        private bool? _SnapshotForUpdate = null;
        public virtual bool SnapshotForUpdate
        {
            get
            {
                // don't cache the return value until it has been explicitly set
                //
                if (_SnapshotForUpdate.HasValue)
                    return _SnapshotForUpdate.Value;

                if (EffectiveVerb != RequestVerb.Put && EffectiveVerb != RequestVerb.Patch)
                    return false;

                if (ErrorExpected)
                    return false;

                if (ExpectedStatusCode >= HttpStatusCode.BadRequest)
                    return false;

                if (Batched)
                    return false;

                if (this is BlobsRequest)
                    return false;

                return true;
            }
            set
            {
                _SnapshotForUpdate = value;
            }
        }

        public bool UseRelativeUri = false;
        public bool ETagHeaderExpected = false;
        public Action<AstoriaResponse> ExtraVerification = null;
        public AstoriaRequestSender RequestSender = null;

        public ResourceIdentifier ExpectedErrorIdentifier = null;
        public object[] ExpectedErrorArguments = new object[] { };

        #region HTTP stream reader/writer.

        public delegate long HttpWriter(Stream requestStream);
        public delegate string HttpReader(Stream responseStream);
        public HttpWriter HttpStreamWriter { set; get; }
        public HttpReader HttpStreamReader { set; get; }

        #endregion

        public CommonPayload BeforeUpdatePayload
        {
            get;
            protected set;
        }

        public CommonPayload AfterUpdatePayload
        {
            get;
            protected set;
        }

        public bool Batched
        {
            get;
            protected internal set;
        }

        public virtual bool IsBlobRequest
        {
            get;
            set;
        }

        public bool UnderlyingRequestFailed
        {
            get;
            protected set;
        }

        private bool? _errorExpectedOverride = null;
        public bool ErrorExpected
        {
            get
            {
                if (_errorExpectedOverride.HasValue)
                    return _errorExpectedOverride.Value;
                return ExpectedErrorIdentifier != null;
            }
            set
            {
                _errorExpectedOverride = value;
            }
        }

        // BatchRequest will override this in order to print out its internal requests
        public virtual void LogRequest(StringBuilder builder, bool logPayload)
        {
            LogRequest(builder, logPayload, false);
        }

        public virtual void LogRequest(StringBuilder builder, bool logPayload, bool logHeaders)
        {
            builder.Append(Verb_Internal.ToString());
            if (TunnelledVerb_Internal.HasValue)
                builder.Append(" (" + TunnelledVerb_Internal.Value.ToString() + " tunnelled)");
            if (UseRelativeUri)
                builder.Append(": " + RelativeUri);
            else
                builder.Append(": " + URI_Internal);

            if (logHeaders)
            {
                builder.AppendLine();
                foreach (var pair in this.Headers)
                    builder.AppendLine(pair.Key + ": " + pair.Value);
            }
            else
            {
                builder.AppendLine(" (" + Accept_Internal + ")");
            }

            if (Payload_Internal != null && logPayload)
                builder.AppendLine(Payload_Internal);
        }

        public void LogRequest()
        {
            StringBuilder builder = new StringBuilder();

            LogRequest(builder, !Workspace.Settings.SuppressTrivialLogging);

            AstoriaTestLog.WriteIgnore(builder.ToString());
        }

        public static StandardRequests StandardRequests(Workspace workspace)
        {
            return new StandardRequests(workspace);
        }

        public virtual AstoriaResponse GetResponse()
        {
            LogRequest();

            OnSend(this);

            // NOTHING should come in between this and actually sending the request
#if !ClientSKUFramework
            SetupAPICallLog();
#endif
            AstoriaResponse response;
            if (AstoriaTestProperties.BatchAllRequests)
            {
                BatchRequest batchRequest = new BatchRequest(Workspace);
                if (Verb_Internal == RequestVerb.Get)
                    batchRequest.Add(this);
                else
                {
                    BatchChangeset changeset = batchRequest.GetChangeset();
                    changeset.Add(this);
                }
                BatchResponse batchResponse = batchRequest.GetResponse() as BatchResponse;
                response = batchResponse.Responses.FirstOrDefault();
            }
            else
                response = RequestSender.SendRequest(this);

#if !ClientSKUFramework

            // NOTHING should come in between this and actually recieving the response
            RetrieveAPICallLog();
#endif

            OnReceive(this, response);

            return response;
        }

        public ResourceProperty GetPropertyFromQuery()
        {
            if (!(Query_Internal is ProjectExpression))
                return null;

            ProjectExpression projection = (ProjectExpression)Query_Internal;
            if (!projection.Projections.Any())
                return null;

            ExpNode node = projection.Projections.First;

            if (node is NestedPropertyExpression)
                return (node as NestedPropertyExpression).PropertyExpressions.Last().Property as ResourceProperty;

            if (node is PropertyExpression)
                return (node as PropertyExpression).Property as ResourceProperty;

            return null;
        }

        #region API call log infrastructure

        public bool LogAPICalls = false;

#if !ClientSKUFramework
        public List<APICallLogEntry> APICallLogEntries
        {
            get;
            private set;
        }

#endif


        public XElement APICallLogXml
        {
            get;
            private set;
        }

#if !ClientSKUFramework
        protected void SetupAPICallLog()
        {


            string directoryPath = Path.Combine(Workspace.DataService.DestinationFolder, APICallLog.DirectoryName);
            string markerFilePath = Path.Combine(directoryPath, APICallLog.MarkerFileName);

            if (!this.LogAPICalls)
            {
                AstoriaTestLog.IsFalse(TrustedMethods.IsFileExists(markerFilePath), "Marker file should have been cleaned up by prior request");
                AstoriaTestLog.IsFalse(TrustedMethods.IsDirectoryNotEmpty(directoryPath), "Directory should be empty!");
                return;
            }

            IOUtil.EnsureDirectoryExists(directoryPath);
            IOUtil.CreateEmptyFile(markerFilePath);

        }
#endif

	#if !ClientSKUFramework

        protected void RetrieveAPICallLog()
        {
            string directoryPath = Path.Combine(Workspace.DataService.DestinationFolder, APICallLog.DirectoryName);
            string markerFilePath = Path.Combine(directoryPath, APICallLog.MarkerFileName);

            if (!this.LogAPICalls)
            {
                AstoriaTestLog.IsFalse(TrustedMethods.IsFileExists(markerFilePath), "Marker file should have been cleaned up by prior request");
                return;
            }

            IOUtil.EnsureFileDeleted(markerFilePath);

            this.APICallLogXml = new XElement("APICallLog");
            this.APICallLogEntries = new List<APICallLogEntry>();

            string[] files;
            // re-query each time, in case any new files were created
            while ((files = Directory.GetFiles(directoryPath, "*.xml")).Length > 0)
            {
                foreach (string filePath in files.OrderBy(f => f)) //NEED TO BE IN CORRECT ORDER
                {
                    string text = File.ReadAllText(filePath);
                    XElement element = null;
                    try
                    {
                        element = XElement.Parse(text);
                    }
                    catch
                    { }

                    if (element != null)
                    {
                        this.APICallLogXml.Add(element);
                        this.APICallLogEntries.Add(APICallLogEntry.FromXml(element));
                    }
                    else
                    {
                        this.APICallLogXml.Add(text);
                        this.APICallLogEntries.Add(new APICallLogEntry(text, new KeyValuePair<string, string>[0])); //this will hopefully ALWAYS fail to compare
                    }

                    IOUtil.EnsureFileDeleted(filePath);
                }
            }

            //if (APICallLogEntries.Any(e => e.MethodName == APICallLog.UnterminatedCall))
            //{
            //    foreach (APICallLogEntry entry in APICallLogEntries)
            //        AstoriaTestLog.WriteLine("(Observed) - " + entry.ToString());
            //    AstoriaTestLog.FailAndContinue(new Microsoft.Test.ModuleCore.TestFailedException("Un-terminated API calls detected"));
            //}
        }

	#endif

        #endregion
    }

    public class RequestEventArgs : EventArgs
    {
        public RequestEventArgs(AstoriaRequest request)
        {
            Request = request;
        }

        public AstoriaRequest Request
        {
            get;
            private set;
        }
    }

    public class ResponseEventArgs : EventArgs
    {
        public ResponseEventArgs(AstoriaResponse response)
        {
            Response = response;
        }

        public AstoriaResponse Response
        {
            get;
            private set;
        }
    }
}
