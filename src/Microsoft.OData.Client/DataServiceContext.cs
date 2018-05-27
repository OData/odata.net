//---------------------------------------------------------------------
// <copyright file="DataServiceContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

// #define TESTUNIXNEWLINE


namespace Microsoft.OData.Client
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.OData.Client.Annotation;
    using Microsoft.OData.Client.Metadata;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using ClientStrings = Microsoft.OData.Client.Strings;

    #endregion Namespaces
    /// <summary>
    /// Indicates DataServiceContext's behavior on undeclared property in entity/complex value.
    /// </summary>
    internal enum UndeclaredPropertyBehavior
    {
        /// <summary>
        /// The default value. Supports undeclared property.
        /// </summary>
        Support = 0,

        /// <summary>
        /// Throw on undeclared property.
        /// </summary>
        ThrowException = 1,
    }

    /// <summary>
    /// The <see cref="T:Microsoft.OData.Client.DataServiceContext" /> represents the runtime context of the data service.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506", Justification = "Central class of the API, likely to have many cross-references")]
    public class DataServiceContext
    {
        /// <summary>Same version as <see cref="maxProtocolVersion"/> but stored as instance of <see cref="Version"/> for easy comparisons.</summary>
        internal Version MaxProtocolVersionAsVersion;

        /// <summary>
        /// string constant for the 'serviceRoot' parameter to UriResolver
        /// </summary>
        private const string ServiceRootParameterName = "serviceRoot";

        /// <summary>The client model for the current context instance.</summary>
        private readonly ClientEdmModel model;

        /// <summary> Internal instance annotations in current context </summary>
        private readonly WeakDictionary<object, IDictionary<string, object>> instanceAnnotations = new WeakDictionary<object, IDictionary<string, object>>(InstanceAnnotationDictWeakKeyComparer.Default)
        {
            RemoveCollectedEntriesRules = new List<Func<object, bool>>
            {
                InstanceAnnotationDictWeakKeyComparer.Default.RemoveRule
            },
            CreateWeakKey = InstanceAnnotationDictWeakKeyComparer.Default.CreateKey
        };

        /// <summary>metadata annotations for current context</summary>
        private readonly WeakDictionary<object, IList<IEdmVocabularyAnnotation>> metadataAnnotationsDictionary = new WeakDictionary<object, IList<IEdmVocabularyAnnotation>>(EqualityComparer<object>.Default);

        /// <summary>The tracker for user-specified format information.</summary>
        private DataServiceClientFormat formatTracker;

        /// <summary>The maximum protocol version the client should support (send and receive).</summary>
        private ODataProtocolVersion maxProtocolVersion;

        /// <summary>
        /// Class which tracks all the entities and links for the given context
        /// </summary>
        private EntityTracker entityTracker;

        /// <summary>
        /// The response preference for add and update operations
        /// </summary>
        private DataServiceResponsePreference addAndUpdateResponsePreference;

        /// <summary>The resolver for baseUris</summary>
        private UriResolver baseUriResolver;

        /// <summary>Authentication interface for retrieving credentials for Web client authentication.</summary>
        private System.Net.ICredentials credentials;

        /// <summary>resolve type from a typename</summary>
        private Func<Type, string> resolveName;

        /// <summary>resolve typename from a type</summary>
        private Func<string, Type> resolveType;

#if !PORTABLELIB // Timeout not available
        /// <summary>time-out value in seconds, 0 for default</summary>
        private int timeout;
#endif
        /// <summary>whether to use post-tunneling for PUT/DELETE</summary>
        private bool postTunneling;

        /// <summary>Used to specify a strategy to send entity parameter.</summary>
        private EntityParameterSendOption entityParameterSendOption;

        /// <summary>Used to specify a value synchronization strategy.</summary>
        private MergeOption mergeOption;

        /// <summary>Default options to be used while doing savechanges.</summary>
        private SaveChangesOptions saveChangesDefaultOptions;

        /// <summary>Client will ignore 404 resource not found exception and return an empty set when this is set to true</summary>
        private bool ignoreResourceNotFoundException;

        /// <summary>Options that can overwrite ignoreMissingProperties.</summary>
        private UndeclaredPropertyBehavior undeclaredPropertyBehavior = UndeclaredPropertyBehavior.Support;

        /// <summary>The URL key delimiter to use.</summary>
        private DataServiceUrlKeyDelimiter urlKeyDelimiter;

        /// <summary>The HTTP stack to use for requests.</summary>
        private HttpStack httpStack;

        #region Test hooks for header and payload verification

#pragma warning disable 0169, 0649
        /// <summary>
        /// Test hook which gets called after the HttpWebRequest has been created and all headers have been set.
        /// Never set by product code.
        /// </summary>
        private Action<object> sendRequest;

        /// <summary>
        /// Test hook which gets called after we call HttpWebRequest.GetRequestStream, so that the test code can wrap the stream and see what gets written to it.
        /// Never set by product code.
        /// </summary>
        private Func<Stream, Stream> getRequestWrappingStream;

        /// <summary>
        /// Test hook which gets called after the HttpWebResponse is received.
        /// Never set by product code.
        /// </summary>
        private Action<object> sendResponse;

        /// <summary>
        /// Test hook which gets called after we call HttpWebRequest.GetResponseStream, so that the test code can wrap the stream and see what gets read from it.
        /// Never set by product code.
        /// </summary>
        private Func<Stream, Stream> getResponseWrappingStream;
#pragma warning restore 0169, 0649

        #endregion

        /// <summary>
        /// A flag indicating if the data service context is applying changes
        /// </summary>
        private bool applyingChanges;

        #region ctor

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> class.</summary>
        /// <remarks>It is expected that the BaseUri or ResolveEntitySet properties will be set before using the newly created context.</remarks>
        public DataServiceContext()
            : this(null)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> class with the specified <paramref name="serviceRoot" />.</summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <exception cref="T:System.ArgumentNullException">When the <paramref name="serviceRoot" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">If the <paramref name="serviceRoot" /> is not an absolute URI -or-If the <paramref name="serviceRoot" /> is a well formed URI without a query or query fragment.</exception>
        /// <remarks>
        /// The library expects the Uri to point to the root of a data service,
        /// but does not issue a request to validate it does indeed identify the root of a service.
        /// If the Uri does not identify the root of the service, the behavior of the client library is undefined.
        /// A Uri provided with a trailing slash is equivalent to one without such a trailing character.
        /// With Silverlight, the <paramref name="serviceRoot"/> can be a relative Uri
        /// that will be combined with System.Windows.Browser.HtmlPage.Document.DocumentUri.
        /// </remarks>
        public DataServiceContext(Uri serviceRoot)
            : this(serviceRoot, ODataProtocolVersion.V4)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> class with the specified <paramref name="serviceRoot" /> and targeting the specific <paramref name="maxProtocolVersion" />.</summary>
        /// <param name="serviceRoot">An absolute URI that identifies the root of a data service.</param>
        /// <param name="maxProtocolVersion">A <see cref="T:Microsoft.OData.Client.ODataProtocolVersion" /> value that is the maximum protocol version that the client understands.</param>
        /// <remarks>
        /// The library expects the Uri to point to the root of a data service,
        /// but does not issue a request to validate it does indeed identify the root of a service.
        /// If the Uri does not identify the root of the service, the behavior of the client library is undefined.
        /// A Uri provided with a trailing slash is equivalent to one without such a trailing character.
        /// With Silverlight, the <paramref name="serviceRoot"/> can be a relative Uri
        /// that will be combined with System.Windows.Browser.HtmlPage.Document.DocumentUri.
        /// </remarks>
        public DataServiceContext(Uri serviceRoot, ODataProtocolVersion maxProtocolVersion)
            : this(serviceRoot, maxProtocolVersion, ClientEdmModelCache.GetModel(maxProtocolVersion))
        {
        }

        /// <summary>
        /// Instantiates a new context with the specified <paramref name="serviceRoot"/> Uri.
        /// The library expects the Uri to point to the root of a data service,
        /// but does not issue a request to validate it does indeed identify the root of a service.
        /// If the Uri does not identify the root of the service, the behavior of the client library is undefined.
        /// </summary>
        /// <param name="serviceRoot">
        /// An absolute, well formed http or https URI without a query or fragment which identifies the root of a data service.
        /// A Uri provided with a trailing slash is equivalent to one without such a trailing character
        /// </param>
        /// <param name="maxProtocolVersion">max protocol version that the client understands.</param>
        /// <param name="model">The client edm model to use. Provided for testability.</param>
        /// <exception cref="ArgumentOutOfRangeException">If the <paramref name="maxProtocolVersion"/> is not a valid value.</exception>
        /// <remarks>
        /// With Silverlight, the <paramref name="serviceRoot"/> can be a relative Uri
        /// that will be combined with System.Windows.Browser.HtmlPage.Document.DocumentUri.
        /// </remarks>
        internal DataServiceContext(Uri serviceRoot, ODataProtocolVersion maxProtocolVersion, ClientEdmModel model)
        {
            Debug.Assert(model != null, "model != null");
            this.model = model;

            this.baseUriResolver = UriResolver.CreateFromBaseUri(serviceRoot, ServiceRootParameterName);
            this.maxProtocolVersion = Util.CheckEnumerationValue(maxProtocolVersion, "maxProtocolVersion");
            this.entityParameterSendOption = EntityParameterSendOption.SendFullProperties;
            this.mergeOption = MergeOption.AppendOnly;
            this.entityTracker = new EntityTracker(model);
            this.MaxProtocolVersionAsVersion = Util.GetVersionFromMaxProtocolVersion(maxProtocolVersion);
            this.formatTracker = new DataServiceClientFormat(this);
            this.urlKeyDelimiter = DataServiceUrlKeyDelimiter.Parentheses;
            this.Configurations = new DataServiceClientConfigurations(this);
            this.httpStack = HttpStack.Auto;
            this.UsingDataServiceCollection = false;
            this.UsePostTunneling = false;

            // Need to use the same defaults when running sl in portable lib as when running in SL normally.
#if PORTABLELIB
            if (HttpWebRequestMessage.IsRunningOnSilverlight)
            {
                this.UsePostTunneling = true;
                this.UseDefaultCredentials = true;
            }
#endif
        }

        #endregion

        /// <summary>
        /// This event is fired before a request is sent to the server, giving
        /// the handler the opportunity to inspect, adjust and/or replace the
        /// WebRequest object used to perform the request.
        /// </summary>
        /// <remarks>
        /// When calling BeginSaveChanges and not using SaveChangesOptions.BatchWithSingleChangeset and SaveChangesOptions.BatchWithIndependentOperations,
        /// this event may be raised from a different thread.
        /// </remarks>
        public event EventHandler<SendingRequest2EventArgs> SendingRequest2;

        /// <summary>
        /// This event is fired before a request message object is built, giving
        /// the handler the opportunity to inspect, adjust and/or replace some
        /// request information before the message is built. This event should be
        /// used to modify the outgoing Url of the request or alter request headers.
        /// After the request is built, other modifications on the WebRequest object can be made
        /// in SendingRequest2.
        /// </summary>
        /// <remarks>
        /// When calling BeginSaveChanges and not using SaveChangesOptions.BatchWithSingleChangeset and SaveChangesOptions.BatchWithIndependentOperations,
        /// this event may be raised from a different thread.
        /// </remarks>
        public event EventHandler<BuildingRequestEventArgs> BuildingRequest
        {
            add
            {
                this.InnerBuildingRequest += value;
            }

            remove
            {
                this.InnerBuildingRequest -= value;
            }
        }

        /// <summary>
        /// This event fires when a response is received by the client.
        /// It fires for both top level responses and each operation or query within a batch response.
        /// </summary>
        /// <remarks>
        /// On top level requests, the event is fired before any processing is done.
        /// For inner batch operations, the event is also fired before any processing is done, with
        /// the exception that the content-ID of a changeset operation will be read before the event is fired.
        /// </remarks>
        public event EventHandler<ReceivingResponseEventArgs> ReceivingResponse;

        /// <summary>
        /// This event fires when SaveChanges or EndSaveChanges is called
        /// </summary>
        internal event EventHandler<SaveChangesEventArgs> ChangesSaved;

        /// <summary>
        /// Internal event instance used by the public SendingRequest event.
        /// </summary>
        private event EventHandler<SendingRequestEventArgs> InnerSendingRequest;

        /// <summary>
        /// Internal event instance used by the public BuildingRequest event.
        /// </summary>
        private event EventHandler<BuildingRequestEventArgs> InnerBuildingRequest;

        #region BaseUri, Credentials, MergeOption, Timeout, Links, Entities, Prefer header

        /// <summary>Gets or sets the delegate method that is used to resolve the entity set URI when the value cannot be determined from an edit-link or self-link URI.</summary>
        /// <returns>A delegate that takes a <see cref="T:System.String" /> and returns a <see cref="T:System.Uri" /> value.</returns>
        public Func<String, Uri> ResolveEntitySet
        {
            get
            {
                return this.baseUriResolver.ResolveEntitySet;
            }

            set
            {
                this.baseUriResolver = this.baseUriResolver.CloneWithOverrideValue(value);
            }
        }

        /// <summary>Gets the absolute URI identifying the root of the target data service. </summary>
        /// <returns>An absolute URI that identifies the root of a T data service.</returns>
        /// <remarks>
        /// A Uri provided with a trailing slash is equivalent to one without such a trailing character.
        /// Example: http://server/host/myservice.svc
        /// </remarks>
        public Uri BaseUri
        {
            get
            {
                return this.baseUriResolver.RawBaseUriValue;
            }

            set
            {
                if (this.baseUriResolver == null)
                {
                    this.baseUriResolver = UriResolver.CreateFromBaseUri(value, ServiceRootParameterName);
                }
                else
                {
                    this.baseUriResolver = this.baseUriResolver.CloneWithOverrideValue(value, null /*parameterName*/);
                }
            }
        }

        /// <summary>Gets or sets whether the client requests that the data service return entity data in the response message to a change request.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceResponsePreference" /> object that determines whether to request a response form the data service. </returns>
        /// <remarks>Whether POST/PUT/PATCH requests will process response from the server. Corresponds to Prefer header in HTTP POST/PUT/PATCH request.</remarks>
        public DataServiceResponsePreference AddAndUpdateResponsePreference
        {
            get
            {
                return this.addAndUpdateResponsePreference;
            }

            set
            {
                if (value != DataServiceResponsePreference.None)
                {
                    this.EnsureMinimumProtocolVersionV3();
                }

                this.addAndUpdateResponsePreference = value;
            }
        }

        /// <summary>Gets the maximum version of the Open Data Protocol (OData) that the client is allowed to use.</summary>
        /// <returns>The maximum version of OData that the client is allowed to use.</returns>
        /// <remarks>If the request or response would require higher version the client will fail.</remarks>
        public ODataProtocolVersion MaxProtocolVersion
        {
            get
            {
                return this.maxProtocolVersion;
            }

            internal set
            {
                this.maxProtocolVersion = value;
                this.MaxProtocolVersionAsVersion = Util.GetVersionFromMaxProtocolVersion(this.maxProtocolVersion);
            }
        }

        /// <summary>Gets or sets the authentication information that is used by each query created by using the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> object.</summary>
        /// <returns>The base authentication interface for retrieving credentials for Web client authentication.</returns>
        public System.Net.ICredentials Credentials
        {
            get { return this.credentials; }
            set { this.credentials = value; }
        }

        /// <summary>Gets or sets the option for sending entity parameters to service.</summary>
        /// <returns>One of the members of the <see cref="T:Microsoft.OData.Client.EntityParameterSendOption" /> enumeration.</returns>
        public EntityParameterSendOption EntityParameterSendOption
        {
            get { return this.entityParameterSendOption; }
            set { this.entityParameterSendOption = Util.CheckEnumerationValue(value, "EntityParameterSendOption"); }
        }

        /// <summary>Gets or sets the synchronization option for receiving entities from a data service.</summary>
        /// <returns>One of the members of the <see cref="T:Microsoft.OData.Client.MergeOption" /> enumeration.</returns>
        /// <remarks>
        /// Used to specify a synchronization strategy when sending/receiving entities to/from a data service.
        /// This value is read by the deserialization component of the client prior to materializing objects.
        /// As such, it is recommended to set this property to the appropriate materialization strategy
        /// before executing any queries/updates to the data service.
        /// The default value is <see cref="MergeOption"/>.AppendOnly.
        /// </remarks>
        public MergeOption MergeOption
        {
            get { return this.mergeOption; }
            set { this.mergeOption = Util.CheckEnumerationValue(value, "MergeOption"); }
        }

        /// <summary>Gets a value that indicates whether the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is currently applying changes to tracked objects.</summary>
        /// <returns>Returns true when changes are currently being applied; otherwise returns false.</returns>
        public bool ApplyingChanges
        {
            get { return this.applyingChanges; }
            internal set { this.applyingChanges = value; }
        }

        /// <summary>Gets or sets a function to override the default type resolution strategy used by the client library when you send entities to a data service.</summary>
        /// <returns>Returns a string that contains the name of the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</returns>
        /// <remarks>
        /// Enables one to override the default type resolution strategy used by the client library.
        /// Set this property to a delegate which identifies a function that resolves
        /// a type within the client application to a namespace-qualified type name.
        /// This enables the client to perform custom mapping between the type name
        /// provided in a response from the server and a type on the client.
        /// This method enables one to override the entity name that is serialized
        /// to the target representation (ATOM,JSON, etc) for the specified type.
        /// </remarks>
        public Func<Type, string> ResolveName
        {
            get { return this.resolveName; }
            set { this.resolveName = value; }
        }

        /// <summary>Gets or sets a function that is used to override the default type resolution option that is used by the client library when receiving entities from a data service.</summary>
        /// <returns>A function delegate that identifies an override function that is used to override the default type resolution option that is used by the client library.</returns>
        /// <remarks>
        /// Enables one to override the default type resolution strategy used by the client library.
        /// Set this property to a delegate which identifies a function that resolves a
        /// namespace-qualified type name to type within the client application.
        /// This enables the client to perform custom mapping between the type name
        /// provided in a response from the server and a type on the client.
        /// Overriding type resolution enables inserting a custom type name to type mapping strategy.
        /// It does not enable one to affect how a response is materialized into the identified type.
        /// </remarks>
        public Func<string, Type> ResolveType
        {
            get { return this.resolveType; }
            set { this.resolveType = value; }
        }

#if !PORTABLELIB // Timeout not available
        /// <summary>Gets or sets the time-out option (in seconds) that is used for the underlying HTTP request to the data service.</summary>
        /// <returns>An integer that indicates the time interval (in seconds) before time-out of a service request.</returns>
        /// <remarks>
        /// A value of 0 will use the default timeout of the underlying HTTP request.
        /// This value must be set before executing any query or update operations against
        /// the target data service for it to have effect on the on the request.
        /// The value may be changed between requests to a data service and the new value
        /// will be picked up by the next data service request.
        /// </remarks>
        public int Timeout
        {
            get
            {
                return this.timeout;
            }

            set
            {
                if (value < 0)
                {
                    throw Error.ArgumentOutOfRange("Timeout");
                }

                this.timeout = value;
            }
        }
#endif

        /// <summary>Gets or sets a Boolean value that indicates whether to use post tunneling.</summary>
        /// <returns>A Boolean value that indicates whether to use post tunneling.</returns>
        public bool UsePostTunneling
        {
            get { return this.postTunneling; }
            set { this.postTunneling = value; }
        }

        /// <summary>Gets the collection of all associations or links currently being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> object.</summary>
        /// <returns>A collection of <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> objects that represent all associations or links current being tracked by the current being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> object.</returns>
        /// <remarks>If no links are being tracked, a collection with 0 elements is returned.</remarks>
        public ReadOnlyCollection<LinkDescriptor> Links
        {
            get
            {
                return new ReadOnlyCollection<LinkDescriptor>(this.entityTracker.Links.OrderBy(l => l.ChangeOrder).ToList());
            }
        }

        /// <summary>Gets a list of all the resources currently being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <returns>A list of <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> objects that represent all the resources currently being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />. </returns>
        /// <remarks>If no resources are being tracked, a collection with 0 elements is returned.</remarks>
        public ReadOnlyCollection<EntityDescriptor> Entities
        {
            get
            {
#if DEBUG
                foreach (EntityDescriptor entityDescriptor in this.entityTracker.Entities)
                {
                    foreach (StreamDescriptor streamDescriptor in entityDescriptor.StreamDescriptors)
                    {
                        Debug.Assert(Object.ReferenceEquals(streamDescriptor.EntityDescriptor, entityDescriptor), "All the StreamDescriptor instances should contain the current EntityDescriptor instance in its EntityDescriptor property");
                    }
                }
#endif

                return new ReadOnlyCollection<EntityDescriptor>(this.entityTracker.Entities.OrderBy(d => d.ChangeOrder).ToList());
            }
        }

        /// <summary>Gets or sets the <see cref="T:Microsoft.OData.Client.SaveChangesOptions" /> values that are used by the <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" /> method.</summary>
        /// <returns>The current options for the save changes operation.</returns>
        public SaveChangesOptions SaveChangesDefaultOptions
        {
            get
            {
                return this.saveChangesDefaultOptions;
            }

            set
            {
                this.ValidateSaveChangesOptions(value);
                this.saveChangesDefaultOptions = value;
            }
        }

        #endregion

        /// <summary>Gets or sets whether an exception is raised when a 404 error (resource not found) is returned by the data service. </summary>
        /// <returns>When set to true, the client library returns an empty set instead of raising a <see cref="T:Microsoft.OData.Client.DataServiceQueryException" /> when the data service returns an HTTP 404: Resource Not Found error.</returns>
        public bool IgnoreResourceNotFoundException
        {
            get { return this.ignoreResourceNotFoundException; }
            set { this.ignoreResourceNotFoundException = value; }
        }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        public DataServiceClientConfigurations Configurations { get; private set; }

        /// <summary>
        /// Gets an object which allows the user to customize the format the client will use for making requests.
        /// </summary>
        public DataServiceClientFormat Format
        {
            get { return this.formatTracker; }
        }

        /// <summary>
        /// Gets or sets the URL key delimiter the client should use.
        /// </summary>
        public DataServiceUrlKeyDelimiter UrlKeyDelimiter
        {
            get
            {
                return this.urlKeyDelimiter;
            }

            set
            {
                Util.CheckArgumentNull(value, "value");
                this.urlKeyDelimiter = value;
            }
        }

        /// <summary>
        /// Returns the instance of entity tracker which tracks all the entities and links tracked by the context.
        /// </summary>
        public EntityTracker EntityTracker
        {
            get
            {
                return this.entityTracker;
            }

            set
            {
                this.entityTracker = value;
            }
        }

        /// <summary>
        /// Disable instance annotation to be materialized.
        /// </summary>
        public bool DisableInstanceAnnotationMaterialization { get; set; }

        /// <summary>
        /// Whether enable writing odata annotation without prefix.
        /// </summary>
        public bool EnableWritingODataAnnotationWithoutPrefix { get; set; }

        /// <summary>Gets or sets whether to support undeclared properties.</summary>
        /// <returns>UndeclaredPropertyBehavior.</returns>
        internal UndeclaredPropertyBehavior UndeclaredPropertyBehavior
        {
            get { return this.undeclaredPropertyBehavior; }
            set { this.undeclaredPropertyBehavior = value; }
        }

        /// <summary>
        /// Gets or sets a System.Boolean value that controls whether default credentials are sent with requests.
        /// </summary>
        internal bool UseDefaultCredentials { get; set; }

        /// <summary>Gets a value that indicates the type of HTTP implementation to use when accessing the data service in Silverlight.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.HttpStack" /> value that indicates the HTTP implementation to use when accessing the data service.</returns>
        /// <remarks>Default value is HttpStack.Auto</remarks>
        internal HttpStack HttpStack
        {
            get { return this.httpStack; }
            set { this.httpStack = Util.CheckEnumerationValue(value, "HttpStack"); }
        }

        /// <summary>Indicates if there are subscribers for the SendingRequest2 event.</summary>
        internal bool HasSendingRequest2EventHandlers
        {
            [DebuggerStepThrough]
            get { return this.SendingRequest2 != null; }
        }

        /// <summary>
        /// INdicates if there are any subscribers for the BuildingRequestEvent.
        /// </summary>
        internal bool HasBuildingRequestEventHandlers
        {
            [DebuggerStepThrough]
            get { return this.InnerBuildingRequest != null; }
        }

        /// <summary>The tracker for user-specified format information.</summary>
        internal DataServiceClientFormat FormatTracker
        {
            get
            {
                return this.formatTracker;
            }

            set
            {
                this.formatTracker = value;
            }
        }

        /// <summary>Returns the instance of entity tracker which tracks all the entities and links tracked by the context.</summary>
        internal UriResolver BaseUriResolver
        {
            get
            {
                return this.baseUriResolver;
            }
        }

        /// <summary>
        /// Gets the client model.
        /// </summary>
        internal ClientEdmModel Model
        {
            get { return this.model; }
        }

        /// <summary>
        /// Indicates whether user is using <see cref="T:Microsoft.OData.Client.DataServiceCollection`1" /> to track changes.
        /// </summary>
        internal bool UsingDataServiceCollection { get; set; }

        /// <summary>The instance annotations in current context</summary>
        internal WeakDictionary<object, IDictionary<string, object>> InstanceAnnotations
        {
            get
            {
                return instanceAnnotations;
            }
        }

        /// <summary>
        /// Gets the MetadataAnnotationsDictionary
        /// </summary>
        internal WeakDictionary<object, IList<IEdmVocabularyAnnotation>> MetadataAnnotationsDictionary
        {
            get
            {
                return metadataAnnotationsDictionary;
            }
        }

        #region GetAnnotation

        /// <summary>
        /// Try to get instance annotations or metadata annotation associated with the specified object.
        /// </summary>
        /// <typeparam name="TResult">CLR type of the annotation</typeparam>
        /// <param name="source">The annotated object</param>
        /// <param name="term">The term name of an annotation</param>
        /// <param name="qualifier">Qualifier to apply</param>
        /// <param name="annotation">
        /// When this method returns, contains the annotation associated with the specified object and term, if the annotation is found;
        /// otherwise, the default value for the type of the annotation parameter.
        /// </param>
        /// <returns>true if the annotation is found</returns>
        public bool TryGetAnnotation<TResult>(object source, string term, string qualifier, out TResult annotation)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNull(term, "term");

            if (qualifier != null)
            {
                return AnnotationHelper.TryGetMetadataAnnotation<TResult>(this, source, term, qualifier, out annotation);
            }

            object instanceAnnotation;
            IDictionary<string, object> instanceAnnotationInfo;
            object key = source;

            // Try to get instance annotation
            if (this.InstanceAnnotations.TryGetValue(key, out instanceAnnotationInfo)
                && instanceAnnotationInfo.TryGetValue(term, out instanceAnnotation))
            {
                if (instanceAnnotation is TResult)
                {
                    annotation = (TResult)instanceAnnotation;
                    return true;
                }

                if (ClientTypeUtil.CanAssignNull(typeof(TResult)) && instanceAnnotation == null)
                {
                    annotation = default(TResult);
                    return true;
                }
            }

            return AnnotationHelper.TryGetMetadataAnnotation<TResult>(this, key, term, null, out annotation);
        }

        /// <summary>
        /// Try to get instance annotations or metadata annotation associated with the specified object.
        /// </summary>
        /// <typeparam name="TResult">CLR type of the annotation</typeparam>
        /// <param name="source">The annotated object</param>
        /// <param name="term">The term name of an annotation</param>
        /// <param name="annotation">
        /// When this method returns, contains the annotation associated with the specified object and term, if the annotation is found;
        /// otherwise, the default value for the type of the annotation parameter.
        /// </param>
        /// <returns>true if the annotation is found</returns>
        public bool TryGetAnnotation<TResult>(object source, string term, out TResult annotation)
        {
            return TryGetAnnotation<TResult>(source, term, null, out annotation);
        }

        /// <summary>
        /// Try to get instance annotations or metadata annotation for property or navigation property.
        /// Or try to get metadata annotation for property, navigation property, entitySet, singleton, operation or operation import.
        /// </summary>
        /// <typeparam name="TFunc">Type of the action or function the expression represents</typeparam>
        /// <typeparam name="TResult">CLR Type of annotation</typeparam>
        /// <param name="expression">The closure expression to access following items:
        ///     property
        ///     navigation property
        ///     entitySet
        ///     singleton
        ///     function
        ///     action
        ///     function import
        ///     action import
        /// </param>
        /// <param name="term">The term name of the annotation</param>
        /// <param name="qualifier">Qualifier to apply</param>
        /// <param name="annotation">
        /// When this method returns, contains the annotation associated with the specified expression and term, if the annotation is found;
        /// otherwise, the default value for the type of the annotation parameter.
        /// </param>
        /// <returns>true if the annotation is found</returns>
        public bool TryGetAnnotation<TFunc, TResult>(Expression<TFunc> expression, string term, string qualifier, out TResult annotation)
        {
            Util.CheckArgumentNull(expression, "expression");
            Util.CheckArgumentNull(term, "term");

            object key = null;
            Expression callerExpression = null;
            MemberInfo memberInfo = null;
            object caller = null;

            if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                var memberExp = (MemberExpression)expression.Body;
                callerExpression = memberExp.Expression;
                memberInfo = memberExp.Member;
            }
            else if (expression.Body.NodeType == ExpressionType.Call)
            {
                var callExp = (MethodCallExpression)expression.Body;
                memberInfo = callExp.Method;
                callerExpression = callExp.Object;
            }

            if (callerExpression != null)
            {
                try
                {
                    caller = Expression.Lambda(callerExpression).Compile().DynamicInvoke();
                }
                catch (InvalidCastException)
                {
                    // If this expression is invalid to compile, the TryGetAnnotation operation will return false.
                    annotation = default(TResult);
                    return false;
                }
            }

            key = new Tuple<object, MemberInfo>(caller, memberInfo);
            if (key != null)
            {
                return TryGetAnnotation<TResult>(key, term, qualifier, out annotation);
            }

            annotation = default(TResult);
            return false;
        }

        /// <summary>
        /// Try to get instance annotations or metadata annotation for property or navigation property.
        /// Or try to get metadata annotation for property, navigation property, entitySet, singleton, operation or operation import.
        /// </summary>
        /// <typeparam name="TFunc">Type of the action or function the expression represents</typeparam>
        /// <typeparam name="TResult">Type of annotation</typeparam>
        /// <param name="expression">The closure expression to access following items:
        ///     property
        ///     navigation property
        ///     entitySet
        ///     singleton
        ///     function
        ///     action
        ///     function import
        ///     action import
        /// </param>
        /// <param name="term">The term name of an annotation</param>
        /// <param name="annotation">
        /// When this method returns, contains the annotation associated with the specified expression and term, if the annotation is found;
        /// otherwise, the default value for the type of the annotation parameter.
        /// </param>
        /// <returns>true if the annotation is found</returns>
        ///
        public bool TryGetAnnotation<TFunc, TResult>(Expression<TFunc> expression, string term, out TResult annotation)
        {
            return TryGetAnnotation(expression, term, null, out annotation);
        }

        #endregion GetAnnotation

        #region Entity and Link Tracking

        /// <summary>Gets the <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> for the supplied entity object.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> instance for the <paramref name="entity" />, or null if an <see cref="T:Microsoft.OData.Client.EntityDescriptor" /> does not exist for the object.</returns>
        /// <param name="entity">The object for which to return the entity descriptor.</param>
        public EntityDescriptor GetEntityDescriptor(object entity)
        {
            Util.CheckArgumentNull(entity, "entity");
            return this.entityTracker.TryGetEntityDescriptor(entity);
        }

        /// <summary>Gets the <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> for a specific link that defines the relationship between two entities.</summary>
        /// <returns>The <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> instance for the specified relationship, or null if a <see cref="T:Microsoft.OData.Client.LinkDescriptor" /> does not exist for the relationship.</returns>
        /// <param name="source">Source object in the link</param>
        /// <param name="sourceProperty">The name of the navigation property on the <paramref name="source" /> object that returns the related object.</param>
        /// <param name="target">The related entity.</param>
        public LinkDescriptor GetLinkDescriptor(object source, string sourceProperty, object target)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNullAndEmpty(sourceProperty, "sourceProperty");
            Util.CheckArgumentNull(target, "target");

            return this.entityTracker.TryGetLinkDescriptor(source, sourceProperty, target);
        }

        #endregion

        #region CancelRequest
        /// <summary>Attempts to cancel the operation that is associated with the supplied <see cref="T:System.IAsyncResult" /> object.</summary>
        /// <param name="asyncResult">The <see cref="T:System.IAsyncResult" /> object from the operation being canceled.</param>
        /// <remarks>DataServiceContext is not safe to use until asyncResult.IsCompleted is true.</remarks>
        public void CancelRequest(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            BaseAsyncResult result = asyncResult as BaseAsyncResult;

            // verify this asyncResult orginated from this context or via query from this context
            if ((null == result) || (this != result.Source))
            {
                object context = null;
                DataServiceQuery query = null;
                if (null != result)
                {
                    query = result.Source as DataServiceQuery;

                    if (null != query)
                    {
                        DataServiceQueryProvider provider = query.Provider as DataServiceQueryProvider;
                        if (null != provider)
                        {
                            context = provider.Context;
                        }
                    }
                }

                if (this != context)
                {
                    throw Error.Argument(Strings.Context_DidNotOriginateAsync, "asyncResult");
                }
            }

            // at this point the result originated from this context or a query from this context
            if (!result.IsCompletedInternally)
            {
                result.SetAborted();

                ODataRequestMessageWrapper request = result.Abortable;
                if (null != request)
                {
                    // with Silverlight we can't wait around to check if the request was aborted
                    // because that would block callbacks for the abort from actually running.
                    request.Abort();
                }
            }
        }
        #endregion

        #region CreateQuery
        /// <summary>Creates a data service query for data of a specified generic type.</summary>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents a data service query.</returns>
        /// <param name="entitySetName">A string that resolves to a URI.</param>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <remarks>create a query based on (BaseUri + relativeUri)</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "required for this feature")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "required for this feature")]
        public DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            Util.CheckArgumentNullAndEmpty(entitySetName, "entitySetName");
            ValidateEntitySetName(ref entitySetName);

            ResourceSetExpression rse = new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(entitySetName), typeof(T), null, CountOption.None, null, null, null, null);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this));
        }

        /// <summary>Creates a data service query for a function with return type in a specified generic type.</summary>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents a data service query.</returns>
        /// <param name="resourcePath">A string ends with function invocation that resolves to a URI.</param>
        /// <param name="isComposable">Whether this function query is composable</param>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <remarks>create a query based on (BaseUri + relativeUri)</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "required for this feature")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "required for this feature")]
        public DataServiceQuery<T> CreateQuery<T>(string resourcePath, bool isComposable)
        {
            Util.CheckArgumentNullAndEmpty(resourcePath, "entitySetName");
            ValidateEntitySetName(ref resourcePath);

            ResourceSetExpression rse = new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(resourcePath), typeof(T), null, CountOption.None, null, null, null, null);
            var query = new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this), isComposable);
            return query;
        }

        /// <summary>Creates a data service query for a function invocation that returns a specified generic type.</summary>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents a data service query.</returns>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <remarks>create a query based on (BaseUri + relativeUri)</remarks>
        public DataServiceQuery<T> CreateFunctionQuery<T>()
        {
            ResourceSetExpression rse = new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, null, typeof(T), null, CountOption.None, null, null, null, null);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this));
        }

        /// <summary>Creates a data service query for function which return collection of data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="path">The path before the function.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents the function call.</returns>
        public DataServiceQuery<T> CreateFunctionQuery<T>(string path, string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            Dictionary<string, string> operationParameters = this.SerializeOperationParameters(parameters);
            ResourceSetExpression rse = new ResourceSetExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(path), typeof(T), null, CountOption.None, null, null, null, null, functionName, operationParameters, false);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this), isComposable);
        }

        /// <summary>Creates a data service single query for function which return single data.</summary>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <param name="path">The path before the function.</param>
        /// <param name="functionName">The function name.</param>
        /// <param name="isComposable">Whether this query is composable.</param>
        /// <param name="parameters">The function parameters.</param>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuerySingle`1" /> instance that represents the function call.</returns>
        public DataServiceQuerySingle<T> CreateFunctionQuerySingle<T>(string path, string functionName, bool isComposable, params UriOperationParameter[] parameters)
        {
            Dictionary<string, string> operationParameters = this.SerializeOperationParameters(parameters);
            SingletonResourceExpression rse = new SingletonResourceExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(path), typeof(T), null, CountOption.None, null, null, null, null, functionName, operationParameters, false);
            return new DataServiceQuerySingle<T>(new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this)), isComposable);
        }

        /// <summary>Creates a data service query for singleton data of a specified generic type.</summary>
        /// <returns>A new <see cref="T:Microsoft.OData.Client.DataServiceQuery`1" /> instance that represents a data service query.</returns>
        /// <param name="singletonName">A string that resolves to a URI.</param>
        /// <typeparam name="T">The type returned by the query</typeparam>
        /// <remarks>create a query based on (BaseUri + relativeUri)</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "required for this feature")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "required for this feature")]
        public DataServiceQuery<T> CreateSingletonQuery<T>(string singletonName)
        {
            Util.CheckArgumentNullAndEmpty(singletonName, "singletonName");
            ValidateEntitySetName(ref singletonName);

            SingletonResourceExpression rse = new SingletonResourceExpression(typeof(IOrderedQueryable<T>), null, Expression.Constant(singletonName), typeof(T), null, CountOption.None, null, null, null, null);
            return new DataServiceQuery<T>.DataServiceOrderedQuery(rse, new DataServiceQueryProvider(this));
        }

        #endregion

        #region GetMetadataUri
        /// <summary>Gets a URI of the location of .edmx metadata.</summary>
        /// <returns>A URI that identifies the location of the metadata description, in .edmx format, for the data service identified by the base URI that is passed to the constructor.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "required for this feature")]
        public Uri GetMetadataUri()
        {
            // TODO: resolve the location of the metadata endpoint for the service by using an HTTP OPTIONS request
            Uri metadataUri = UriUtil.CreateUri(UriUtil.UriToString(this.BaseUriResolver.GetBaseUriWithSlash()) + XmlConstants.UriMetadataSegment, UriKind.Absolute);
            return metadataUri;
        }
        #endregion

        #region LoadProperty

        /// <summary>Asynchronously loads the value of the specified property from the data service.</summary>
        /// <returns>An IAsyncResult that represents the status of the asynchronous operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property on the specified entity to load.</param>
        /// <param name="callback">The delegate called when a response to the request is received.</param>
        /// <param name="state">The user-defined state object that is used to pass context data to the callback method.</param>
        /// <remarks>actually doesn't modify the property until EndLoadProperty is called.</remarks>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, AsyncCallback callback, object state)
        {
            return this.BeginLoadProperty(entity, propertyName, (Uri)null /*nextLinkUri*/, callback, state);
        }

        /// <summary>Asynchronously loads the value of the specified property from the data service.</summary>
        /// <returns>A task that represents the response to the load operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property on the specified entity to load.</param>
        public Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName)
        {
            return Task<QueryOperationResponse>.Factory.FromAsync(this.BeginLoadProperty, this.EndLoadProperty, entity, propertyName, null);
        }

        /// <summary>Asynchronously loads a page of related entities from the data service by using the supplied next link URI.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> object that is used to track the status of the asynchronous operation. </returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <param name="nextLinkUri">The URI used to load the next results page.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, Uri nextLinkUri, AsyncCallback callback, object state)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, callback, state, nextLinkUri, null);
            result.BeginExecuteQuery();
            return result;
        }

        /// <summary>Asynchronously loads a page of related entities from the data service by using the supplied next link URI.</summary>
        /// <returns>A task that represents the response to the load operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property on the specified entity to load.</param>
        /// <param name="nextLinkUri">The URI used to load the next results page.</param>
        public Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName, Uri nextLinkUri)
        {
            return Task<QueryOperationResponse>.Factory.FromAsync(this.BeginLoadProperty, this.EndLoadProperty, entity, propertyName, nextLinkUri, null);
        }

        /// <summary>Asynchronously loads the next page of related entities from the data service by using the supplied query continuation object.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> that represents the status of the operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of related entity data to return from the data service.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        public IAsyncResult BeginLoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation, AsyncCallback callback, object state)
        {
            Util.CheckArgumentNull(continuation, "continuation");
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, callback, state, null /*requestUri*/, continuation);
            result.BeginExecuteQuery();
            return result;
        }

        /// <summary>Asynchronously loads the next page of related entities from the data service by using the supplied query continuation object.</summary>
        /// <returns>A Task that represents the response to the load operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property on the specified entity to load.</param>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of related entity data to return from the data service.</param>
        public Task<QueryOperationResponse> LoadPropertyAsync(object entity, string propertyName, DataServiceQueryContinuation continuation)
        {
            return Task<QueryOperationResponse>.Factory.FromAsync(this.BeginLoadProperty, this.EndLoadProperty, entity, propertyName, continuation, null);
        }

        /// <summary>Called to complete the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginLoadProperty(System.Object,System.String,System.AsyncCallback,System.Object)" /> operation.</summary>
        /// <returns>The response to the load operation.</returns>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</param>
        public QueryOperationResponse EndLoadProperty(IAsyncResult asyncResult)
        {
            LoadPropertyResult response = BaseAsyncResult.EndExecute<LoadPropertyResult>(this, Util.LoadPropertyMethodName, asyncResult);
            return response.LoadProperty();
        }

#if !PORTABLELIB // Synchronous methods not available
        /// <summary>Loads deferred content for a specified property from the data service.</summary>
        /// <returns>The response to the load operation.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <remarks>
        /// If <paramref name="entity"/> is in in detached or added state, this method will throw an InvalidOperationException
        /// since there is nothing it can load from the server.
        ///
        /// If <paramref name="entity"/> is in unchanged or modified state, this method will load its collection or
        /// reference elements as unchanged with unchanged bindings.
        ///
        /// If <paramref name="entity"/> is in deleted state, this method will load the entities linked to by its collection or
        /// reference property in the unchanged state with bindings in the deleted state.
        /// </remarks>
        public QueryOperationResponse LoadProperty(object entity, string propertyName)
        {
            return this.LoadProperty(entity, propertyName, (Uri)null);
        }

        /// <summary>Loads a page of related entities by using the supplied next link URI.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> that contains the results of the request.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <param name="nextLinkUri">The URI that is used to load the next results page.</param>
        /// <exception cref="T:System.InvalidOperationException">When <paramref name="entity" /> is in a <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.</exception>
        /// <remarks>
        /// If <paramref name="entity"/> is in in detached or added state, this method will throw an InvalidOperationException
        /// since there is nothing it can load from the server.
        ///
        /// If <paramref name="entity"/> is in unchanged or modified state, this method will load its collection or
        /// reference elements as unchanged with unchanged bindings.
        ///
        /// If <paramref name="entity"/> is in deleted state, this method will load the entities linked to by its collection or
        /// reference property in the unchanged state with bindings in the deleted state.
        /// </remarks>
        public QueryOperationResponse LoadProperty(object entity, string propertyName, Uri nextLinkUri)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null /*callback*/, null /*state*/, nextLinkUri, null /*continuation*/);
            result.ExecuteQuery();
            return result.LoadProperty();
        }

        /// <summary>Loads the next page of related entities from the data service by using the supplied query continuation object.</summary>
        /// <returns>The response that contains the next page of related entity data.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of related entities to load from the data service.</param>
        /// <exception cref="T:System.InvalidOperationException">When <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.</exception>
        /// <remarks>
        /// If <paramref name="entity"/> is in in detached or added state, this method will throw an InvalidOperationException
        /// since there is nothing it can load from the server.
        ///
        /// If <paramref name="entity"/> is in unchanged or modified state, this method will load its collection or
        /// reference elements as unchanged with unchanged bindings.
        ///
        /// If <paramref name="entity"/> is in deleted state, this method will load the entities linked to by its collection or
        /// reference property in the unchanged state with bindings in the deleted state.
        /// </remarks>
        public QueryOperationResponse LoadProperty(object entity, string propertyName, DataServiceQueryContinuation continuation)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null /*callback*/, null /*state*/, null /*requestUri*/, continuation);
            result.ExecuteQuery();
            return result.LoadProperty();
        }

        /// <summary>Loads the next page of related entities from the data service by using the supplied generic query continuation object.</summary>
        /// <returns>The response that contains the next page of related entity data.</returns>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of related entities to load from the data service.</param>
        /// <typeparam name="T">Element type of collection to load.</typeparam>
        /// <exception cref="T:System.InvalidOperationException">When <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.</exception>
        /// <remarks>
        /// If <paramref name="entity"/> is in in detached or added state, this method will throw an InvalidOperationException
        /// since there is nothing it can load from the server.
        ///
        /// If <paramref name="entity"/> is in unchanged or modified state, this method will load its collection or
        /// reference elements as unchanged with unchanged bindings.
        ///
        /// If <paramref name="entity"/> is in deleted state, this method will load the entities linked to by its collection or
        /// reference property in the unchanged state with bindings in the deleted state.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011", Justification = "allows compiler to infer 'T'")]
        public QueryOperationResponse<T> LoadProperty<T>(object entity, string propertyName, DataServiceQueryContinuation<T> continuation)
        {
            LoadPropertyResult result = this.CreateLoadPropertyRequest(entity, propertyName, null /*callback*/, null /*state*/, null /*requestUri*/, continuation);
            result.ExecuteQuery();
            return (QueryOperationResponse<T>)result.LoadProperty();
        }

#endif


        #endregion

        #region GetReadStreamUri
        /// <summary>Gets the URI that is used to return a binary data stream.</summary>
        /// <returns>The read URI of the binary data stream.</returns>
        /// <param name="entity">The entity that has a related binary stream to retrieve. </param>
        /// <exception cref="T:System.ArgumentNullException">If the entity specified is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</exception>
        /// <remarks>If the specified entity is a Media Link Entry, this method will return an URI which can be used to access the content of the Media Resource. This URI should only be used to GET/Read the content of the MR. It may not respond to POST/PUT/DELETE requests.</remarks>
        public Uri GetReadStreamUri(object entity)
        {
            Util.CheckArgumentNull(entity, "entity");
            EntityDescriptor box = this.entityTracker.GetEntityDescriptor(entity);
            return box.ReadStreamUri;
        }

        /// <summary>Gets the URI that is used to return a named binary data stream.</summary>
        /// <returns>The read URI of the binary data stream.</returns>
        /// <param name="entity">The entity that has the named binary data stream to retrieve.</param>
        /// <param name="name">The name of the stream to request.</param>
        /// <remarks>If the specified entity has a stream with the given name, this method will return an URI which can be used to access the content of the stream. This URI should only be used to GET/Read the content of the stream. It may not respond to POST/PUT/DELETE requests.</remarks>
        /// <exception cref="ArgumentNullException">If the entity specified is null.</exception>
        /// <exception cref="ArgumentException">If the name parameter is empty or the entity specified is not being tracked.</exception>
        public Uri GetReadStreamUri(object entity, string name)
        {
            Util.CheckArgumentNull(entity, "entity");
            Util.CheckArgumentNullAndEmpty(name, "name");
            this.EnsureMinimumProtocolVersionV3();
            EntityDescriptor entityDescriptor = this.entityTracker.GetEntityDescriptor(entity);
            StreamDescriptor namedStreamInfo;
            if (entityDescriptor.TryGetNamedStreamInfo(name, out namedStreamInfo))
            {
                return namedStreamInfo.SelfLink ?? namedStreamInfo.EditLink;
            }

            return null;
        }

        #endregion

        #region GetReadStream, BeginGetReadStream, EndGetReadStream

        /// <summary>Asynchronously gets the binary data stream that belongs to the specified entity, by using the specified message headers.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> object that is used to track the status of the asynchronous operation. </returns>
        /// <param name="entity">The entity that has a the binary data stream to retrieve. </param>
        /// <param name="args">Instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <exception cref="T:System.ArgumentNullException">Any of the parameters supplied to the method is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-The <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-The <paramref name="entity" /> is not a Media Link Entry and does not have a related binary data stream.</exception>
        public IAsyncResult BeginGetReadStream(object entity, DataServiceRequestArgs args, AsyncCallback callback, object state)
        {
            GetReadStreamResult result = this.CreateGetReadStreamResult(entity, args, callback, state, null /*name*/);
            result.Begin();
            return result;
        }

        /// <summary>Asynchronously gets the binary data stream that belongs to the specified entity, by using the specified message headers.</summary>
        /// <returns>A Task that represents an instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> which contains the response stream along with its metadata.</returns>
        /// <param name="entity">The entity that has a the binary data stream to retrieve. </param>
        /// <param name="args">Instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <exception cref="T:System.ArgumentNullException">Any of the parameters supplied to the method is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-The <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-The <paramref name="entity" /> is not a Media Link Entry and does not have a related binary data stream.</exception>
        public Task<DataServiceStreamResponse> GetReadStreamAsync(object entity, DataServiceRequestArgs args)
        {
            return Task<DataServiceStreamResponse>.Factory.FromAsync(this.BeginGetReadStream, this.EndGetReadStream, entity, args, null);
        }

        /// <summary>Asynchronously gets a named binary data stream that belongs to the specified entity, by using the specified message headers.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> object that is used to track the status of the asynchronous operation. </returns>
        /// <param name="entity">The entity that has the binary data stream to retrieve.</param>
        /// <param name="name">The name of the binary stream to request.</param>
        /// <param name="args">Instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        public IAsyncResult BeginGetReadStream(object entity, string name, DataServiceRequestArgs args, AsyncCallback callback, object state)
        {
            Util.CheckArgumentNullAndEmpty(name, "name");
            this.EnsureMinimumProtocolVersionV3();
            GetReadStreamResult result = this.CreateGetReadStreamResult(entity, args, callback, state, name);
            result.Begin();
            return result;
        }

        /// <summary>Asynchronously gets the binary data stream that belongs to the specified entity, by using the specified message headers.</summary>
        /// <returns>A task that represents an instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> which contains the response stream along with its metadata.</returns>
        /// <param name="entity">The entity that has a the binary data stream to retrieve. </param>
        /// <param name="name">The name of the binary stream to request.</param>
        /// <param name="args">Instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        public Task<DataServiceStreamResponse> GetReadStreamAsync(object entity, string name, DataServiceRequestArgs args)
        {
            return Task<DataServiceStreamResponse>.Factory.FromAsync(this.BeginGetReadStream, this.EndGetReadStream, entity, name, args, null);
        }

        /// <summary>Called to complete the asynchronous operation of retrieving a binary data stream.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> which contains the response stream along with its metadata.</returns>
        /// <param name="asyncResult">The result from the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginGetReadStream(System.Object,Microsoft.OData.Client.DataServiceRequestArgs,System.AsyncCallback,System.Object)" /> operation that contains the binary data stream.</param>
        /// <remarks>The method will block if the request have not finished yet.</remarks>
        public DataServiceStreamResponse EndGetReadStream(IAsyncResult asyncResult)
        {
            GetReadStreamResult result = BaseAsyncResult.EndExecute<GetReadStreamResult>(this, "GetReadStream", asyncResult);
            return result.End();
        }

#if !PORTABLELIB
        /// <summary>Gets the binary data stream that belongs to the specified entity.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> that represents the response.</returns>
        /// <param name="entity">The entity that has the binary stream to retrieve. </param>
        /// <exception cref="T:System.ArgumentNullException">The<paramref name=" entity" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-The <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-The <paramref name="entity" /> is not a Media Link Entry and does not have a related binary stream.</exception>
        public DataServiceStreamResponse GetReadStream(object entity)
        {
            DataServiceRequestArgs args = new DataServiceRequestArgs();
            return this.GetReadStream(entity, args);
        }

        /// <summary>Gets the binary data stream that belongs to the specified entity, by using the specified Content-Type message header.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> that represents the response.</returns>
        /// <param name="entity">The entity that has the binary data stream to retrieve. </param>
        /// <param name="acceptContentType">The Content-Type of the binary data stream requested from the data service, specified in the Accept header.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="entity" /> is null.-or- <paramref name="acceptContentType" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-The <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-The <paramref name="entity" /> is not a Media Link Entry and does not have a related stream.</exception>
        public DataServiceStreamResponse GetReadStream(object entity, string acceptContentType)
        {
            Util.CheckArgumentNullAndEmpty(acceptContentType, "acceptContentType");
            DataServiceRequestArgs args = new DataServiceRequestArgs();
            args.AcceptContentType = acceptContentType;
            return this.GetReadStream(entity, args);
        }

        /// <summary>Gets binary data stream for the specified entity by using the specified message headers.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> that represents the response.</returns>
        /// <param name="entity">The entity that has the binary stream to retrieve. </param>
        /// <param name="args">Instance of <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///   <paramref name="entity" /> is null.-or- <paramref name="args" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" />.-or-The <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-The <paramref name="entity" /> is not a Media Link Entry and does not have a related binary stream.</exception>
        public DataServiceStreamResponse GetReadStream(object entity, DataServiceRequestArgs args)
        {
            GetReadStreamResult result = this.CreateGetReadStreamResult(entity, args, null, null, null);
            return result.Execute();
        }

        /// <summary>Gets a named binary data stream that belongs to the specified entity, by using the specified Content-Type message header.</summary>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.DataServiceStreamResponse" /> that represents the response.</returns>
        /// <param name="entity">The entity that has the binary data stream to retrieve.</param>
        /// <param name="name">The name of the binary stream to request.</param>
        /// <param name="args">Instance of <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <exception cref="ArgumentNullException">Either entity or args parameters are null.</exception>
        /// <exception cref="ArgumentException">The specified entity is either not tracked, is in the added state.</exception>
        public DataServiceStreamResponse GetReadStream(object entity, string name, DataServiceRequestArgs args)
        {
            Util.CheckArgumentNullAndEmpty(name, "name");
            this.EnsureMinimumProtocolVersionV3();
            GetReadStreamResult result = this.CreateGetReadStreamResult(entity, args, null, null, name);
            return result.Execute();
        }

#endif
        #endregion

        #region SetSaveStream

        /// <summary>Sets a binary data stream that belongs to the specified entity, with the specified Content-Type and Slug headers in the request message.</summary>
        /// <param name="entity">The entity to which the data stream belongs.</param>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> from which to read the binary data. </param>
        /// <param name="closeStream">A <see cref="T:System.Boolean" /> value that determines whether the data stream is closed when the <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" /> method is completed. </param>
        /// <param name="contentType">The Content-Type header value for the request message.</param>
        /// <param name="slug">The Slug header value for the request message.</param>
        /// <exception cref="T:System.ArgumentNullException">Any of the parameters supplied to the method are null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not being tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" /> instance. -or-The entity has the <see cref="T:Microsoft.OData.Client.MediaEntryAttribute" /> applied. </exception>
        /// <remarks>Calling this method marks the entity as media link resource (MLE). It also marks the entity as modified
        /// so that it will participate in the next call to SaveChanges.</remarks>
        public void SetSaveStream(object entity, Stream stream, bool closeStream, string contentType, string slug)
        {
            Util.CheckArgumentNull(contentType, "contentType");
            Util.CheckArgumentNull(slug, "slug");

            DataServiceRequestArgs args = new DataServiceRequestArgs();
            args.ContentType = contentType;
            args.Slug = slug;
            this.SetSaveStream(entity, stream, closeStream, args);
        }

        /// <summary>Sets a binary data stream for the specified entity, with the specified headers in the request message.</summary>
        /// <param name="entity">The entity to which the binary stream belongs.</param>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> from which to read the binary data. </param>
        /// <param name="closeStream">A <see cref="T:System.Boolean" /> value that determines whether the data stream is closed when the <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" /> method is completed. </param>
        /// <param name="args">An instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <exception cref="T:System.ArgumentNullException">Any of the parameters supplied to the method are null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="entity" /> is not being tracked by this <see cref="T:Microsoft.OData.Client.DataServiceContext" /> instance. -or-The <paramref name="entity" /> has the <see cref="T:Microsoft.OData.Client.MediaEntryAttribute" /> applied. </exception>
        /// <remarks>Calling this method marks the entity as media link resource (MLE). It also marks the entity as modified
        /// so that it will participate in the next call to SaveChanges.</remarks>
        public void SetSaveStream(object entity, Stream stream, bool closeStream, DataServiceRequestArgs args)
        {
            Util.CheckArgumentNull(entity, "entity");
            Util.CheckArgumentNull(stream, "stream");
            Util.CheckArgumentNull(args, "args");

            EntityDescriptor box = this.entityTracker.GetEntityDescriptor(entity);
            ClientTypeAnnotation clientType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(entity.GetType()));
            if (clientType.MediaDataMember != null)
            {
                throw new ArgumentException(
                    Strings.Context_SetSaveStreamOnMediaEntryProperty(clientType.ElementTypeName),
                    "entity");
            }

            box.SaveStream = new DataServiceSaveStream(stream, closeStream, args);

            Debug.Assert(box.State != EntityStates.Detached, "We should never have a detached entity in the entityDescriptor dictionary.");
            switch (box.State)
            {
                case EntityStates.Added:
                    box.StreamState = EntityStates.Added;
                    break;

                case EntityStates.Modified:
                case EntityStates.Unchanged:
                    box.StreamState = EntityStates.Modified;
                    break;

                default:
                    throw new DataServiceClientException(Strings.Context_SetSaveStreamOnInvalidEntityState(Enum.GetName(typeof(EntityStates), box.State)));
            }

            // Note that there's no need to mark the entity as updated because we consider the presense
            // of the save stream as the mark that the MR for this MLE has been updated.

            // TODO: why we don't increment the change order number in this case, when the entity is in unmodified state?
        }

        /// <summary>Sets a binary data stream for the specified entity.</summary>
        /// <param name="entity">The entity to which the binary stream belongs.</param>
        /// <param name="name">The name of the binary stream to save.</param>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> from which to read the binary data.</param>
        /// <param name="closeStream">A <see cref="T:System.Boolean" /> value that determines whether the data stream is closed when the <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" /> method is completed.</param>
        /// <param name="contentType">The Content-Type header value for the request message.</param>
        /// <exception cref="ArgumentException">The entity is not being tracked or name is an empty string.</exception>
        /// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
        public void SetSaveStream(object entity, string name, Stream stream, bool closeStream, string contentType)
        {
            Util.CheckArgumentNullAndEmpty(contentType, "contentType");
            DataServiceRequestArgs args = new DataServiceRequestArgs();
            args.ContentType = contentType;
            this.SetSaveStream(entity, name, stream, closeStream, args);
        }

        /// <summary>Sets a named binary data stream that belongs to the specified entity, with the specified headers in the request message.</summary>
        /// <param name="entity">The entity to which the binary stream belongs.</param>
        /// <param name="name">The name of the binary stream to save.</param>
        /// <param name="stream">The <see cref="T:System.IO.Stream" /> from which to read the binary data.</param>
        /// <param name="closeStream">A <see cref="T:System.Boolean" /> value that determines whether the data stream is closed when the <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" /> method is completed.</param>
        /// <param name="args">An instance of the <see cref="T:Microsoft.OData.Client.DataServiceRequestArgs" /> class that contains settings for the HTTP request message.</param>
        /// <remarks>Calling this method marks the entity as media link resource (MLE). It also marks the entity as modified
        /// so that it will participate in the next call to SaveChanges.</remarks>
        /// <exception cref="ArgumentException">The entity is not being tracked. The entity has the MediaEntry attribute
        /// marking it to use the older way of handling MRs.</exception>
        /// <exception cref="ArgumentNullException">Any of the arguments is null.</exception>
        public void SetSaveStream(object entity, string name, Stream stream, bool closeStream, DataServiceRequestArgs args)
        {
            Util.CheckArgumentNull(entity, "entity");
            Util.CheckArgumentNullAndEmpty(name, "name");
            Util.CheckArgumentNull(stream, "stream");
            Util.CheckArgumentNull(args, "args");
            this.EnsureMinimumProtocolVersionV3();

            if (string.IsNullOrEmpty(args.ContentType))
            {
                throw Error.Argument(Strings.Context_ContentTypeRequiredForNamedStream, "args");
            }

            EntityDescriptor box = this.entityTracker.GetEntityDescriptor(entity);
            Debug.Assert(box.State != EntityStates.Detached, "We should never have a detached entity in the entityDescriptor dictionary.");
            if (box.State == EntityStates.Deleted)
            {
                throw new DataServiceClientException(Strings.Context_SetSaveStreamOnInvalidEntityState(Enum.GetName(typeof(EntityStates), box.State)));
            }

            StreamDescriptor namedStream = box.AddStreamInfoIfNotPresent(name);
            namedStream.SaveStream = new DataServiceSaveStream(stream, closeStream, args);
            namedStream.State = EntityStates.Modified;
            this.entityTracker.IncrementChange(namedStream);
        }

        #endregion

        #region ExecuteBatch, BeginExecuteBatch, EndExecuteBatch

        /// <summary>Asynchronously submits a group of queries as a batch to the data service.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> object that is used to track the status of the asynchronous operation. </returns>
        /// <param name="callback">The delegate that is called when a response to the batch request is received.</param>
        /// <param name="state">User-defined state object that is used to pass context data to the callback method.</param>
        /// <param name="queries">The array of query requests to include in the batch request.</param>
        public IAsyncResult BeginExecuteBatch(AsyncCallback callback, object state, params DataServiceRequest[] queries)
        {
            Util.CheckArgumentNotEmpty(queries, "queries");
            BatchSaveResult result = new BatchSaveResult(this, "ExecuteBatch", queries, SaveChangesOptions.BatchWithSingleChangeset, callback, state);
            result.BatchBeginRequest();
            return result;
        }

        /// <summary>Asynchronously submits a group of queries as a batch to the data service.</summary>
        /// <returns>An Task that represents the DataServiceResult object that indicates the result of the batch operation.</returns>
        /// <param name="queries">The array of query requests to include in the batch request.</param>
        public Task<DataServiceResponse> ExecuteBatchAsync(params DataServiceRequest[] queries)
        {
            return Task<DataServiceResponse>.Factory.FromAsync((callback, state) => this.BeginExecuteBatch(callback, state, queries), this.EndExecuteBatch, null);
        }

        /// <summary>Called to complete the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginExecuteBatch(System.AsyncCallback,System.Object,Microsoft.OData.Client.DataServiceRequest[])" />.</summary>
        /// <returns>The DataServiceResult object that indicates the result of the batch operation.</returns>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</param>
        public DataServiceResponse EndExecuteBatch(IAsyncResult asyncResult)
        {
            BatchSaveResult result = BaseAsyncResult.EndExecute<BatchSaveResult>(this, "ExecuteBatch", asyncResult);
            return result.EndRequest();
        }

#if !PORTABLELIB // Synchronous methods not available
        /// <summary>Synchronously submits a group of queries as a batch to the data service.</summary>
        /// <returns>The response to the batch operation.</returns>
        /// <param name="queries">Array of <see cref="T:Microsoft.OData.Client.DataServiceRequest[]" /> objects that make up the queries.</param>
        public DataServiceResponse ExecuteBatch(params DataServiceRequest[] queries)
        {
            Util.CheckArgumentNotEmpty(queries, "queries");

            BatchSaveResult result = new BatchSaveResult(this, "ExecuteBatch", queries, SaveChangesOptions.BatchWithSingleChangeset, null, null);
            result.BatchRequest();
            return result.EndRequest();
        }
#endif

        #endregion

        #region Execute, BeginExecute, EndExecute

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>An object that is used to track the status of the asynchronous operation. </returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI; it can contain $ query parameters.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state)
        {
            return this.InnerBeginExecute<TElement>(requestUri, callback, state, XmlConstants.HttpMethodGet, Util.ExecuteMethodName, null /*singleResult*/);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI; it can contain $ query parameters.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        public Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri)
        {
            return Task<IEnumerable<TElement>>.Factory.FromAsync(this.BeginExecute<TElement>, this.EndExecute<TElement>, requestUri, null);
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <remarks>
        /// This overload expects the <paramref name="requestUri"/> to end with a ServiceOperation
        /// or ServiceAction that returns void.
        /// </remarks>
        public IAsyncResult BeginExecute(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters)
        {
            return this.InnerBeginExecute<object>(requestUri, callback, state, httpMethod, Util.ExecuteMethodNameForVoidResults, false, operationParameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI; it can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        public Task<OperationResponse> ExecuteAsync(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
            return Task<OperationResponse>.Factory.FromAsync((callback, state) => this.BeginExecute(requestUri, callback, state, httpMethod, operationParameters), this.EndExecute, null);
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="singleResult">Attribute used on service operations to specify that they return a single instance of their return element.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            return this.InnerBeginExecute<TElement>(requestUri, callback, state, httpMethod, Util.ExecuteMethodName, singleResult, operationParameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI; it can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="singleResult">Attribute used on service operations to specify that they return a single instance of their return element.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        public Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            return Task<IEnumerable<TElement>>.Factory.FromAsync((callback, state) => this.BeginExecute<TElement>(requestUri, callback, state, httpMethod, singleResult, operationParameters), this.EndExecute<TElement>, null);
        }

        /// <summary>Asynchronously sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, params OperationParameter[] operationParameters)
        {
            bool? singleResult = this.IsSingletonType<TElement>();
            return this.InnerBeginExecute<TElement>(requestUri, callback, state, httpMethod, Util.ExecuteMethodName, singleResult, operationParameters);
        }

        /// <summary>Asynchronously sends the request so that this call does not block processing while waiting for the results from the service.</summary>
        /// <returns>A task represents the result of the operation. </returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI; it can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        public Task<IEnumerable<TElement>> ExecuteAsync<TElement>(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
            return Task<IEnumerable<TElement>>.Factory.FromAsync((callback, state) => this.BeginExecute<TElement>(requestUri, callback, state, httpMethod, operationParameters), this.EndExecute<TElement>, null);
        }

        /// <summary>Asynchronously sends a request to the data service to retrieve the next page of data in a paged query result.</summary>
        /// <returns>An <see cref="T:System.IAsyncResult" /> that represents the status of the operation.</returns>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of data to return from the data service.</param>
        /// <param name="callback">Delegate to invoke when results are available for client consumption.</param>
        /// <param name="state">User-defined state object passed to the callback.</param>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IAsyncResult BeginExecute<T>(DataServiceQueryContinuation<T> continuation, AsyncCallback callback, object state)
        {
            Util.CheckArgumentNull(continuation, "continuation");
            QueryComponents qc = continuation.CreateQueryComponents();
            Uri requestUri = qc.Uri;
            return (new DataServiceRequest<T>(requestUri, qc, continuation.Plan)).BeginExecute(this, this, callback, state, Util.ExecuteMethodName);
        }

        /// <summary>Asynchronously sends a request to the data service to retrieve the next page of data in a paged query result.</summary>
        /// <returns>A task that represents the results returned by the query operation.</returns>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of data to return from the data service.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        public Task<IEnumerable<TElement>> ExecuteAsync<TElement>(DataServiceQueryContinuation<TElement> continuation)
        {
            return Task<IEnumerable<TElement>>.Factory.FromAsync(this.BeginExecute, this.EndExecute<TElement>, continuation, null);
        }

        /// <summary>Called to complete the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginExecute``1(System.Uri,System.AsyncCallback,System.Object)" />.</summary>
        /// <returns>The results returned by the query operation.</returns>
        /// <param name="asyncResult">
        ///   <see cref="T:System.IAsyncResult" /> object.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">When<paramref name=" asyncResult" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When<paramref name=" asyncResult" /> did not originate from this <see cref="T:Microsoft.OData.Client.DataServiceContext" /> instance. -or- When the <see cref="M:Microsoft.OData.Client.DataServiceContext.EndExecute``1(System.IAsyncResult)" /> method was previously called.</exception>
        /// <exception cref="T:System.InvalidOperationException">When an error is raised either during execution of the request or when it converts the contents of the response message into objects.</exception>
        /// <exception cref="T:Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IEnumerable<TElement> EndExecute<TElement>(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            return DataServiceRequest.EndExecute<TElement>(this, this, Util.ExecuteMethodName, asyncResult);
        }

        /// <summary>Called to complete the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginExecute``1(System.Uri,System.AsyncCallback,System.Object)" />.</summary>
        /// <returns>The result of the operation.</returns>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</param>
        /// <remarks>This method should be used in combination with the BeginExecute overload which
        /// expects the request uri to end with a service operation or service action that returns void.</remarks>
        public OperationResponse EndExecute(IAsyncResult asyncResult)
        {
            Util.CheckArgumentNull(asyncResult, "asyncResult");
            QueryOperationResponse<object> result = (QueryOperationResponse<object>)DataServiceRequest.EndExecute<object>(this, this, Util.ExecuteMethodNameForVoidResults, asyncResult);
            if (result.Any())
            {
                throw new DataServiceClientException(Strings.Context_EndExecuteExpectedVoidResponse);
            }

            return result;
        }

#if !PORTABLELIB // Synchronous methods not available
        /// <summary>Sends a request to the data service to execute a specific URI.</summary>
        /// <returns>The results of the query operation.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI. Can contain $ query parameters.</param>
        /// <typeparam name="TElement">The type that the query returns.</typeparam>
        /// <exception cref="T:System.Net.WebException">When a response is not received from a request to the <paramref name="requestUri" />.</exception>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="requestUri" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="requestUri" /> is not a valid URI for the data service.</exception>
        /// <exception cref="T:System.InvalidOperationException">When an error is raised either during execution of the request or when it converts the contents of the response message into objects.</exception>
        /// <exception cref="T:Microsoft.OData.Client.DataServiceQueryException">When the data service returns an HTTP 404: Resource Not Found error.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri)
        {
            // We don't support operation parameters with "GET" yet.

            // This public API is for backwards compatability, which is why it always uses GET and sets singleResult to null
            return InnerSynchExecute<TElement>(requestUri, XmlConstants.HttpMethodGet, null);
        }

        /// <summary>Sends a request to the data service to retrieve the next page of data in a paged query result.</summary>
        /// <returns>The response that contains the next page of data in the query result.</returns>
        /// <param name="continuation">A <see cref="T:Microsoft.OData.Client.DataServiceQueryContinuation`1" /> object that represents the next page of data to return from the data service.</param>
        /// <typeparam name="T">The type returned by the query.</typeparam>
        public QueryOperationResponse<T> Execute<T>(DataServiceQueryContinuation<T> continuation)
        {
            Util.CheckArgumentNull(continuation, "continuation");
            QueryComponents qc = continuation.CreateQueryComponents();
            Uri requestUri = qc.Uri;
            DataServiceRequest request = new DataServiceRequest<T>(requestUri, qc, continuation.Plan);
            return request.Execute<T>(this, qc);
        }

        /// <summary>Sends a request to the data service to execute a specific URI by using a specific HTTP method.</summary>
        /// <returns>The response of the operation.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI. Can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <remarks>
        /// This overload expects the <paramref name="requestUri"/> to end with a ServiceOperation
        /// or ServiceAction that returns void.
        /// </remarks>
        /// <exception cref="ArgumentNullException">null requestUri</exception>
        /// <exception cref="ArgumentException">The <paramref name="httpMethod"/> is not GET, POST or DELETE.</exception>
        /// <exception cref="InvalidOperationException">problem materializing results of query into objects</exception>
        /// <exception cref="WebException">failure to get response for requestUri</exception>
        public OperationResponse Execute(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
            QueryOperationResponse<object> result = (QueryOperationResponse<object>)Execute<object>(requestUri, httpMethod, false, operationParameters);
            if (result.Any())
            {
                throw new DataServiceClientException(Strings.Context_ExecuteExpectedVoidResponse);
            }

            return result;
        }

        /// <summary>Sends a request to the data service to execute a specific URI by using a specific HTTP method.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI. Can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="singleResult">Attribute used on service operations to specify that they return a single instance of their return element.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        /// <exception cref="ArgumentNullException">null requestUri</exception>
        /// <exception cref="ArgumentException">The <paramref name="httpMethod"/> is not GET nor POST.</exception>
        /// <exception cref="InvalidOperationException">problem materializing results of query into objects</exception>
        /// <exception cref="WebException">failure to get response for requestUri</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Just for CTP")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, bool singleResult, params OperationParameter[] operationParameters)
        {
            return InnerSynchExecute<TElement>(requestUri, httpMethod, singleResult, operationParameters);
        }

        /// <summary>Sends a request to the data service to execute a specific URI by using a specific HTTP method.</summary>
        /// <returns>Returns <see cref="T:System.Collections.Generic.IEnumerable`1" />.</returns>
        /// <param name="requestUri">The URI to which the query request will be sent. The URI may be any valid data service URI. Can contain $ query parameters.</param>
        /// <param name="httpMethod">The HTTP data transfer method used by the client.</param>
        /// <param name="operationParameters">The operation parameters used.</param>
        /// <typeparam name="TElement">The type returned by the query.</typeparam>
        /// <exception cref="ArgumentNullException">null requestUri</exception>
        /// <exception cref="ArgumentException">The <paramref name="httpMethod"/> is not GET nor POST.</exception>
        /// <exception cref="InvalidOperationException">problem materializing results of query into objects</exception>
        /// <exception cref="WebException">failure to get response for requestUri</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Just for CTP")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        public IEnumerable<TElement> Execute<TElement>(Uri requestUri, string httpMethod, params OperationParameter[] operationParameters)
        {
            bool? singleResult = this.IsSingletonType<TElement>();
            return InnerSynchExecute<TElement>(requestUri, httpMethod, singleResult, operationParameters);
        }

#endif
        #endregion

        #region SaveChanges, BeginSaveChanges, EndSaveChanges

        /// <summary>Asynchronously submits the pending changes to the data service collected by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> since the last time changes were saved.</summary>
        /// <returns>An IAsyncResult that represents the status of the asynchronous operation.</returns>
        /// <param name="callback">The delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state object that is used to pass context data to the callback method.</param>
        public IAsyncResult BeginSaveChanges(AsyncCallback callback, object state)
        {
            return this.BeginSaveChanges(this.SaveChangesDefaultOptions, callback, state);
        }

        /// <summary>Asynchronously submits the pending changes to the data service collected by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> since the last time changes were saved.</summary>
        /// <returns>A task that represents a <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object that indicates the result of the batch operation.</returns>
        public Task<DataServiceResponse> SaveChangesAsync()
        {
            return SaveChangesAsync(this.SaveChangesDefaultOptions);
        }

        /// <summary>Asynchronously submits the pending changes to the data service collected by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> since the last time changes were saved.</summary>
        /// <returns>A task that represents a <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object that indicates the result of the batch operation.</returns>
        /// <param name="options">A member of the <see cref="T:Microsoft.OData.Client.SaveChangesOptions" /> enumeration for how the client can save the pending set of changes.</param>
        /// <param name="callback">The delegate to call when the operation is completed.</param>
        /// <param name="state">The user-defined state object that is used to pass context data to the callback method.</param>
        /// <remarks>
        /// BeginSaveChanges will asynchronously attach identity Uri returned by server to sucessfully added entites.
        /// EndSaveChanges will apply updated values to entities, raise ReadingEntity events and change entity states.
        /// </remarks>
        public IAsyncResult BeginSaveChanges(SaveChangesOptions options, AsyncCallback callback, object state)
        {
            this.ValidateSaveChangesOptions(options);
            BaseSaveResult result = BaseSaveResult.CreateSaveResult(this, Util.SaveChangesMethodName, null, options, callback, state);
            if (result.IsBatchRequest)
            {
                ((BatchSaveResult)result).BatchBeginRequest();
            }
            else
            {
                ((SaveResult)result).BeginCreateNextChange(); // may invoke callback before returning
            }

            return result;
        }

        /// <summary>Asynchronously submits the pending changes to the data service collected by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> since the last time changes were saved.</summary>
        /// <returns>A task that represents a <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object that indicates the result of the batch operation.</returns>
        /// <param name="options">A member of the <see cref="T:Microsoft.OData.Client.SaveChangesOptions" /> enumeration for how the client can save the pending set of changes.</param>
        public Task<DataServiceResponse> SaveChangesAsync(SaveChangesOptions options)
        {
            return Task<DataServiceResponse>.Factory.FromAsync(this.BeginSaveChanges, this.EndSaveChanges, options, null);
        }

        /// <summary>Called to complete the <see cref="M:Microsoft.OData.Client.DataServiceContext.BeginSaveChanges(System.AsyncCallback,System.Object)" /> operation.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> object that indicates the result of the batch operation.</returns>
        /// <param name="asyncResult">An <see cref="T:System.IAsyncResult" /> that represents the status of the asynchronous operation.</param>
        public DataServiceResponse EndSaveChanges(IAsyncResult asyncResult)
        {
            BaseSaveResult result = BaseAsyncResult.EndExecute<BaseSaveResult>(this, Util.SaveChangesMethodName, asyncResult);

            DataServiceResponse errors = result.EndRequest();

            if (this.ChangesSaved != null)
            {
                this.ChangesSaved(this, new SaveChangesEventArgs(errors));
            }

            return errors;
        }

#if !PORTABLELIB // Synchronous methods not available
        /// <summary>Saves the changes that the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is tracking to storage.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> that contains status, headers, and errors that result from the call to <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges.Remarks" />.</returns>
        public DataServiceResponse SaveChanges()
        {
            return this.SaveChanges(this.SaveChangesDefaultOptions);
        }

        /// <summary>Saves the changes that the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is tracking to storage.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Client.DataServiceResponse" /> that contains status, headers, and errors that result from the call to <see cref="M:Microsoft.OData.Client.DataServiceContext.SaveChanges" />.</returns>
        /// <param name="options">A member of the <see cref="T:Microsoft.OData.Client.SaveChangesOptions" /> enumeration for how the client can save the pending set of changes.</param>
        public DataServiceResponse SaveChanges(SaveChangesOptions options)
        {
            DataServiceResponse errors = null;
            this.ValidateSaveChangesOptions(options);

            BaseSaveResult result = BaseSaveResult.CreateSaveResult(this, Util.SaveChangesMethodName, null, options, null, null);
            if (result.IsBatchRequest)
            {
                ((BatchSaveResult)result).BatchRequest();
            }
            else
            {
                ((SaveResult)result).CreateNextChange();
            }

            errors = result.EndRequest();

            Debug.Assert(null != errors, "null errors");

            if (this.ChangesSaved != null)
            {
                this.ChangesSaved(this, new SaveChangesEventArgs(errors));
            }

            return errors;
        }
#endif
        #endregion

        #region Add, Attach, Delete, Detach, Update, TryGetEntity, TryGetUri

        /// <summary>Adds the specified link to the set of objects the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is tracking.</summary>
        /// <param name="source">The source object for the new link.</param>
        /// <param name="sourceProperty">The name of the navigation property on the source object that returns the related object.</param>
        /// <param name="target">The object related to the source object by the new link. </param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="source" />, <paramref name="sourceProperty" />, or <paramref name="target" /> are null.</exception>
        /// <exception cref="T:System.InvalidOperationException">If a link already exists.-or-If either the <paramref name="source" /> or <paramref name="target" /> objects are in a <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Deleted" /> state.-or-If <paramref name="sourceProperty" /> is not a collection.</exception>
        /// <remarks>
        /// Notifies the context that a new link exists between the <paramref name="source"/> and <paramref name="target"/> objects
        /// and that the link is represented via the source.<paramref name="sourceProperty"/> which is a collection.
        /// The context adds this link to the set of newly created links to be sent to
        /// the data service on the next call to SaveChanges().
        /// Links are one way relationships.  If a back pointer exists (ie. two way association),
        /// this method should be called a second time to notify the context object of the second link.
        /// </remarks>
        public void AddLink(object source, string sourceProperty, object target)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Added);

            LinkDescriptor descriptor = new LinkDescriptor(source, sourceProperty, target, this.model);
            this.entityTracker.AddLink(descriptor);
            descriptor.State = EntityStates.Added;
            this.entityTracker.IncrementChange(descriptor);
        }

        /// <summary>Notifies the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> to start tracking the specified link that defines a relationship between entity objects.</summary>
        /// <param name="source">The source object in the new link.</param>
        /// <param name="sourceProperty">The name of the property on the source object that represents the link between the source and target object.</param>
        /// <param name="target">The target object in the link that is bound to the source object specified in this call. The target object must be of the type identified by the source property or a subtype.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="source" />, <paramref name="sourceProperty" />, or <paramref name="target" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the link between the two entities already exists.-or-When <paramref name="source" /> or <paramref name="target" /> is in an <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Deleted" /> state.</exception>
        public void AttachLink(object source, string sourceProperty, object target)
        {
            this.AttachLink(source, sourceProperty, target, MergeOption.NoTracking);
        }

        /// <summary>Removes the specified link from the list of links being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <returns>Returns true if the specified entity was detached; otherwise false.</returns>
        /// <param name="source">The source object participating in the link to be marked for deletion.</param>
        /// <param name="sourceProperty">The name of the property on the source object that represents the source in the link between the source and the target.</param>
        /// <param name="target">The target object involved in the link that is bound to the source object. The target object must be of the type identified by the source property or a subtype.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="source" /> or <paramref name="sourceProperty" /> are null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="sourceProperty" /> is an empty string.</exception>
        /// <remarks>Any link being tracked by the context, regardless of its current state, can be detached.  </remarks>
        public bool DetachLink(object source, string sourceProperty, object target)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNullAndEmpty(sourceProperty, "sourceProperty");

            LinkDescriptor existing = this.entityTracker.TryGetLinkDescriptor(source, sourceProperty, target);
            if (existing == null)
            {
                return false;
            }

            this.entityTracker.DetachExistingLink(existing, false);
            return true;
        }

        /// <summary>Changes the state of the link to deleted in the list of links being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <param name="source">The source object in the link to be marked for deletion.</param>
        /// <param name="sourceProperty">The name of the navigation property on the source object that is used to access the target object.</param>
        /// <param name="target">The target object involved in the link that is bound to the source object. The target object must be of the type identified by the source property or a subtype.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="source" />, <paramref name="sourceProperty" />, or <paramref name="target" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">When <paramref name="source" /> or <paramref name="target" /> is in a <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Added" /> state.-or-When <paramref name="sourceProperty" /> is not a collection.</exception>
        /// <remarks>
        /// Notifies the context that a link exists between the <paramref name="source"/> and <paramref name="target"/> object
        /// and that the link is represented via the source.<paramref name="sourceProperty"/> which is a collection.
        /// The context adds this link to the set of deleted links to be sent to
        /// the data service on the next call to SaveChanges().
        /// If the specified link exists in the "Added" state, then the link is detached (see DetachLink method) instead.
        /// </remarks>
        public void DeleteLink(object source, string sourceProperty, object target)
        {
            bool delay = this.EnsureRelatable(source, sourceProperty, target, EntityStates.Deleted);

            LinkDescriptor existing = this.entityTracker.TryGetLinkDescriptor(source, sourceProperty, target);
            if (existing != null && (EntityStates.Added == existing.State))
            {   // Added -> Detached
                this.entityTracker.DetachExistingLink(existing, false);
            }
            else
            {
                if (delay)
                {   // can't have non-added relationship when source or target is in added state
                    throw Error.InvalidOperation(Strings.Context_NoRelationWithInsertEnd);
                }

                if (null == existing)
                {   // detached -> deleted
                    LinkDescriptor relation = new LinkDescriptor(source, sourceProperty, target, this.model);
                    this.entityTracker.AddLink(relation);
                    existing = relation;
                }

                if (EntityStates.Deleted != existing.State)
                {
                    existing.State = EntityStates.Deleted;

                    // It is the users responsibility to delete the link
                    // before deleting the entity when required.
                    this.entityTracker.IncrementChange(existing);
                }
            }
        }

        /// <summary>Notifies the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> that a new link exists between the objects specified and that the link is represented by the property specified by the <paramref name="sourceProperty" /> parameter.</summary>
        /// <param name="source">The source object for the new link.</param>
        /// <param name="sourceProperty">The property on the source object that identifies the target object of the new link.</param>
        /// <param name="target">The child object involved in the new link that is to be initialized by calling this method. The target object must be a subtype of the type identified by the <paramref name="sourceProperty" /> parameter. If <paramref name="target" /> is set to null, the call represents a delete link operation.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="source" />, <paramref name="sourceProperty" /> or <paramref name="target" /> are null.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the specified link already exists.-or-When the objects supplied as <paramref name="source" /> or <paramref name="target" /> are in the <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> or <see cref="F:Microsoft.OData.Client.EntityStates.Deleted" /> state.-or-When <paramref name="sourceProperty" /> is not a navigation property that defines a reference to a single related object.</exception>
        /// <remarks>
        /// Notifies the context that a modified link exists between the <paramref name="source"/> and <paramref name="target"/> objects
        /// and that the link is represented via the source.<paramref name="sourceProperty"/> which is a reference.
        /// The context adds this link to the set of modified created links to be sent to
        /// the data service on the next call to SaveChanges().
        /// Links are one way relationships.  If a back pointer exists (ie. two way association),
        /// this method should be called a second time to notify the context object of the second link.
        /// </remarks>
        public void SetLink(object source, string sourceProperty, object target)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Modified);

            LinkDescriptor relation = this.entityTracker.DetachReferenceLink(source, sourceProperty, target, MergeOption.NoTracking);
            if (null == relation)
            {
                relation = new LinkDescriptor(source, sourceProperty, target, this.model);
                this.entityTracker.AddLink(relation);
            }

            Debug.Assert(
                0 == relation.State ||
                Util.IncludeLinkState(relation.State),
                "set link entity state");

            if (EntityStates.Modified != relation.State)
            {
                relation.State = EntityStates.Modified;
                this.entityTracker.IncrementChange(relation);
            }
        }

        #endregion

        #region AddObject, AttachTo, DeleteObject, Detach, TryGetEntity, TryGetUri
        /// <summary>Adds the specified object to the set of objects that the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is tracking.</summary>
        /// <param name="entitySetName">The name of the entity set to which the resource will be added.</param>
        /// <param name="entity">The object to be tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entitySetName" /> or <paramref name="entity" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="entitySetName" /> is empty.-or-When <paramref name="entity" /> does not have a key property defined.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the entity is already being tracked by the context.</exception>
        /// <remarks>
        /// It does not follow the object graph and add related objects.
        /// Any leading or trailing forward slashes will automatically be trimmed from entitySetName.
        /// </remarks>
        public void AddObject(string entitySetName, object entity)
        {
            ValidateEntitySetName(ref entitySetName);
            ValidateEntityType(entity, this.Model);

            var resource = new EntityDescriptor(this.model)
            {
                Entity = entity,
                State = EntityStates.Added,
                EntitySetName = entitySetName,
            };

            resource.SetEntitySetUriForInsert(this.BaseUriResolver.GetEntitySetUri(entitySetName));

            this.EntityTracker.AddEntityDescriptor(resource);
            this.EntityTracker.IncrementChange(resource);
        }

        /// <summary>Adds a related object to the context and creates the link that defines the relationship between the two objects in a single request.</summary>
        /// <param name="source">The parent object that is being tracked by the context.</param>
        /// <param name="sourceProperty">The name of the navigation property that returns the related object based on an association between the two entities.</param>
        /// <param name="target">The related object that is being added.</param>
        public void AddRelatedObject(object source, string sourceProperty, object target)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNullAndEmpty(sourceProperty, "sourceProperty");
            Util.CheckArgumentNull(target, "target");

            // Validate that the source is an entity and is already tracked by the context.
            ValidateEntityType(source, this.Model);

            EntityDescriptor sourceResource = this.entityTracker.GetEntityDescriptor(source);

            // Check for deleted source entity
            if (sourceResource.State == EntityStates.Deleted)
            {
                throw Error.InvalidOperation(Strings.Context_AddRelatedObjectSourceDeleted);
            }

            // Validate that the property is valid and exists on the source
            ClientTypeAnnotation parentType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(source.GetType()));
            ClientPropertyAnnotation property = parentType.GetProperty(sourceProperty, UndeclaredPropertyBehavior.ThrowException);
            if (property.IsKnownType || !property.IsEntityCollection)
            {
                throw Error.InvalidOperation(Strings.Context_AddRelatedObjectCollectionOnly);
            }

            // Validate that the target is an entity
            ClientTypeAnnotation childType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(target.GetType()));
            ValidateEntityType(target, this.Model);

            // Validate that the property type matches with the target type
            ClientTypeAnnotation propertyElementType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(property.EntityCollectionItemType));
            if (!propertyElementType.ElementType.IsAssignableFrom(childType.ElementType))
            {
                // target is not of the correct type
                throw Error.Argument(Strings.Context_RelationNotRefOrCollection, "target");
            }

            var targetResource = new EntityDescriptor(this.model)
            {
                Entity = target,
                State = EntityStates.Added
            };

            targetResource.SetParentForInsert(sourceResource, sourceProperty);

            this.EntityTracker.AddEntityDescriptor(targetResource);

            // Add the link in the added state.
            LinkDescriptor end = targetResource.GetRelatedEnd();
            end.State = EntityStates.Added;
            this.entityTracker.AddLink(end);

            this.entityTracker.IncrementChange(targetResource);
        }

        /// <summary>Notifies the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> to start tracking the specified resource and supplies the location of the resource within the specified resource set.</summary>
        /// <param name="entitySetName">The name of the set that contains the resource.</param>
        /// <param name="entity">The resource to be tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />. The resource is attached in the Unchanged state.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> or <paramref name="entitySetName" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="entitySetName" /> is an empty string.-or-When the <paramref name="entity" /> does not have a key property defined.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the <paramref name="entity" /> is already being tracked by the context.</exception>
        /// <remarks>It does not follow the object graph and attach related objects.</remarks>
        public void AttachTo(string entitySetName, object entity)
        {
            this.AttachTo(entitySetName, entity, null);
        }

        /// <summary>Notifies the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> to start tracking the specified resource and supplies the location of the resource in the specified resource set.</summary>
        /// <param name="entitySetName">The string value that contains the name of the entity set to which to the entity is attached.</param>
        /// <param name="entity">The entity to add.</param>
        /// <param name="etag">An etag value that represents the state of the entity the last time it was retrieved from the data service. This value is treated as an opaque string; no validation is performed on it by the client library.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entitySetName" /> is null.-or-When <paramref name="entity" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="entitySetName" /> is an empty string.-or-When the supplied object does not have a key property.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the supplied object is already being tracked by the context</exception>
        /// <remarks>It does not follow the object graph and attach related objects.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", MessageId = "etag", Justification = "represents ETag in request")]
        public void AttachTo(string entitySetName, object entity, string etag)
        {
            ValidateEntitySetName(ref entitySetName);
            Util.CheckArgumentNull(entity, "entity");

            var descriptor = new EntityDescriptor(this.model)
            {
                Entity = entity,
                ETag = etag,
                State = EntityStates.Unchanged,
                EntitySetName = entitySetName,
            };

            ODataResourceMetadataBuilder entityMetadataBuilder = this.GetEntityMetadataBuilderInternal(descriptor);

            descriptor.EditLink = entityMetadataBuilder.GetEditLink();
            descriptor.Identity = entityMetadataBuilder.GetId();

            this.entityTracker.InternalAttachEntityDescriptor(descriptor, true /*failIfDuplicated*/);
        }

        /// <summary>Changes the state of the specified object to be deleted in the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</summary>
        /// <param name="entity">The tracked entity to be changed to the deleted state.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">When the object is not being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</exception>
        /// <remarks>
        /// Existings objects in the Added state become detached.
        /// </remarks>
        public void DeleteObject(object entity)
        {
            this.DeleteObjectInternal(entity, false /*failIfInAddedState*/);
        }

        /// <summary>Removes the entity from the list of entities that the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> is tracking.</summary>
        /// <returns>Returns true if the specified entity was detached; otherwise false.</returns>
        /// <param name="entity">The tracked entity to be detached from the <see cref="T:Microsoft.OData.Client.DataServiceContext" />.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> is null.</exception>
        public bool Detach(object entity)
        {
            Util.CheckArgumentNull(entity, "entity");

            EntityDescriptor resource = this.entityTracker.TryGetEntityDescriptor(entity);
            if (resource != null)
            {
                return this.entityTracker.DetachResource(resource);
            }

            return false;
        }

        /// <summary>Changes the state of the specified object in the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> to <see cref="F:Microsoft.OData.Client.EntityStates.Modified" />.</summary>
        /// <param name="entity">The tracked entity to be assigned to the <see cref="F:Microsoft.OData.Client.EntityStates.Modified" /> state.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> is null.</exception>
        /// <exception cref="T:System.ArgumentException">When <paramref name="entity" /> is in the <see cref="F:Microsoft.OData.Client.EntityStates.Detached" /> state.</exception>
        public void UpdateObject(object entity)
        {
            this.UpdateObjectInternal(entity, false /*failIfNotUnchanged*/);
        }

        /// <summary>Update a related object to the context.</summary>
        /// <param name="source">The parent object that is being tracked by the context.</param>
        /// <param name="sourceProperty">The name of the navigation property that returns the related object based on an association between the two entities.</param>
        /// <param name="target">The related object that is being updated.</param>
        public void UpdateRelatedObject(object source, string sourceProperty, object target)
        {
            Util.CheckArgumentNull(source, "source");
            Util.CheckArgumentNullAndEmpty(sourceProperty, "sourceProperty");
            Util.CheckArgumentNull(target, "target");

            // Validate that the source is an entity and is already tracked by the context.
            ValidateEntityType(source, this.Model);

            EntityDescriptor sourceResource = this.entityTracker.GetEntityDescriptor(source);

            // Check for deleted source entity
            if (sourceResource.State == EntityStates.Deleted)
            {
                throw Error.InvalidOperation(Strings.Context_AddRelatedObjectSourceDeleted);
            }

            // Validate that the property is valid and exists on the source
            ClientTypeAnnotation parentType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(source.GetType()));
            ClientPropertyAnnotation property = parentType.GetProperty(sourceProperty, UndeclaredPropertyBehavior.ThrowException);

            if (property.IsKnownType || property.IsEntityCollection)
            {
                throw Error.InvalidOperation(Strings.Context_UpdateRelatedObjectNonCollectionOnly);
            }

            // Validate that the target is an entity
            ClientTypeAnnotation childType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(target.GetType()));
            ValidateEntityType(target, this.Model);

            ClientTypeAnnotation propertyElementType = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(property.PropertyType));
            if (!propertyElementType.ElementType.IsAssignableFrom(childType.ElementType))
            {
                // target is not of the correct type
                throw Error.Argument(Strings.Context_RelationNotRefOrCollection, "target");
            }

            EntityDescriptor targetResource = this.entityTracker.TryGetEntityDescriptor(target);
            if (targetResource != null)
            {
                this.UpdateObject(target);
            }
            else
            {
                targetResource = new EntityDescriptor(this.model)
                {
                    Entity = target,
                    State = EntityStates.Modified,
                    EditLink = sourceResource.GetNestedResourceInfo(this.baseUriResolver, property)
                };

                targetResource.SetParentForUpdate(sourceResource, sourceProperty);
                this.EntityTracker.AddEntityDescriptor(targetResource);
                this.entityTracker.IncrementChange(targetResource);
            }
        }

        /// <summary>
        /// Changes the state of the given entity.
        /// Note that the 'Added' state is not supported by this method, and that AddObject or AddRelatedObject should be used instead.
        /// If the state 'Modified' is given, calling this method is exactly equivalent to calling UpdateObject.
        /// If the state 'Deleted' is given, calling this method is exactly equivalent to calling DeleteObject.
        /// If the state 'Detached' is given, calling this method is exactly equivalent to calling Detach.
        /// If the state 'Unchanged' is given, the state will be changed, but no other modifications will be made to the entity or entity descriptor associated with it.
        /// </summary>
        /// <param name="entity">The entity whose state to change.</param>
        /// <param name="state">The new state of the entity.</param>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChangeState", Justification = "Method name, will be removed when string is added to resources.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AddObject", Justification = "Method name, will be removed when string is added to resources.")]
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AddRelatedObject", Justification = "Method name, will be removed when string is added to resources.")]
        public void ChangeState(object entity, EntityStates state)
        {
            switch (state)
            {
                case EntityStates.Added:
                    throw Error.NotSupported(ClientStrings.Context_CannotChangeStateToAdded);

                case EntityStates.Modified:
                    this.UpdateObjectInternal(entity, true /*failIfNotUnchanged*/);
                    break;

                case EntityStates.Deleted:
                    this.DeleteObjectInternal(entity, true /*failIfInAddedState*/);
                    break;

                case EntityStates.Detached:
                    this.Detach(entity);
                    break;

                case EntityStates.Unchanged:
                    this.SetStateToUnchanged(entity);
                    break;

                default:
                    throw Error.InternalError(InternalError.UnvalidatedEntityState);
            }

#if DEBUG
            if (state != EntityStates.Detached)
            {
                var descriptor = this.entityTracker.TryGetEntityDescriptor(entity);
                Debug.Assert(descriptor != null, "Should have found entity descriptor.");
                Debug.Assert(descriptor.State == state, "ChangeState should have changed state to " + state + " not " + descriptor.State);
            }
            else
            {
                Debug.Assert(this.entityTracker.TryGetEntityDescriptor(entity) == null, "Entity should have been detached.");
            }
#endif
        }

        /// <summary>Test retrieval of an entity being tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> by reference to the URI of the entity.</summary>
        /// <returns>If an entity is found at <paramref name="identity" />, the entity is returned in the out parameter <paramref name="entity" /> and true is returned. If no entity is found, false is returned.</returns>
        /// <param name="identity">The URI of the tracked entity to be retrieved.</param>
        /// <param name="entity">The entity to be retrieved.</param>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="identity" /> is null.</exception>
        /// <remarks>entities in added state are not likely to have a identity</remarks>
        public bool TryGetEntity<TEntity>(Uri identity, out TEntity entity) where TEntity : class
        {
            entity = null;
            Util.CheckArgumentNull(identity, "relativeUri");

            EntityStates state;

            // ReferenceIdentity is a test hook to help verify we dont' use identity instead of editLink
            entity = (TEntity)this.EntityTracker.TryGetEntity(identity, out state);
            return (null != entity);
        }

        /// <summary>Retrieves the canonical URI associated with the specified entity, if available.</summary>
        /// <returns>Returns true if the canonical URI is returned in the out parameter. If the specified entity is not tracked by the <see cref="T:Microsoft.OData.Client.DataServiceContext" /> or is in the added state, no URI is available and false is returned.</returns>
        /// <param name="entity">The entity identified by the <paramref name="identity" />.</param>
        /// <param name="identity">The URI of the entity.</param>
        /// <exception cref="T:System.ArgumentNullException">When <paramref name="entity" /> is null.</exception>
        /// <remarks>Entities in added state are not likely to have an identity. Though the identity might use a dereferencable scheme, you MUST NOT assume it can be dereferenced.</remarks>
        public bool TryGetUri(object entity, out Uri identity)
        {
            identity = null;
            Util.CheckArgumentNull(entity, "entity");

            // if the entity's identity does not map back to the entity, don't return it
            EntityDescriptor resource = this.entityTracker.TryGetEntityDescriptor(entity);
            if (resource != null &&
                (null != resource.Identity) &&
                Object.ReferenceEquals(resource, this.entityTracker.TryGetEntityDescriptor(resource.Identity)))
            {
                // DereferenceIdentity is a test hook to help verify we dont' use identity instead of editLink
                identity = resource.Identity;
            }

            return (null != identity);
        }

        #endregion

        /// <summary>
        /// Get the bound <see cref="IEdmOperation"/> or <see cref="IEdmOperationImport"/> according to the client MethodInfo.
        /// </summary>
        /// <param name="methodInfo">The specified MethodInfo</param>
        /// <returns>return a bound <see cref="IEdmOperation"/> or an <see cref="IEdmOperationImport"/> if it is found, or return null.</returns>
        internal virtual IEdmVocabularyAnnotatable GetEdmOperationOrOperationImport(MethodInfo methodInfo)
        {
            var declaringType = methodInfo.DeclaringType;
            if (declaringType.IsSubclassOf(typeof(DataServiceContext)))
            {
                return AnnotationHelper.GetEdmOperationImport(this, methodInfo);
            }

            return AnnotationHelper.GetEdmOperation(this, methodInfo);
        }

        /// <summary>
        /// Asynchronously loads all pages of related entities for a specified property from the data service.
        /// </summary>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> that contains the results of the last page request.</returns>
        internal Task<QueryOperationResponse> LoadPropertyAllPagesAsync(object entity, string propertyName)
        {
            var currentTask = Task<QueryOperationResponse>.Factory.FromAsync(this.BeginLoadProperty, this.EndLoadProperty, entity, propertyName, null);
            var nextTask = currentTask.ContinueWith(t => this.ContinuePage(t.Result, entity, propertyName));
            return nextTask;
        }

#if !PORTABLELIB

        /// <summary>
        /// Loads all pages of related entities for a specified property from the data service.
        /// </summary>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> that contains the results of the last page request.</returns>
        internal QueryOperationResponse LoadPropertyAllPages(object entity, string propertyName)
        {
            DataServiceQueryContinuation continuation = null;
            QueryOperationResponse response;
            do
            {
                if (continuation == null)
                {
                    response = this.LoadProperty(entity, propertyName, (Uri)null);
                }
                else
                {
                    response = this.LoadProperty(entity, propertyName, continuation);
                }

                continuation = response.GetContinuation();
            }
            while (continuation != null);

            return response;
        }

        /// <summary>
        /// Execute the <paramref name="requestUri"/> using <paramref name="httpMethod"/>.
        /// </summary>
        /// <typeparam name="TElement">Element type of the result. </typeparam>
        /// <param name="requestUri">Request URI to execute.</param>
        /// <param name="httpMethod">HttpMethod to use. Only GET or POST are supported.</param>
        /// <param name="singleResult">If set to true, indicates that a single result is expected as a response.
        /// False indicates that a collection of TElement is assumed. Should be null for void, entry, and feed cases.
        /// This function will check if TElement is an entity type and set singleResult to null in this case.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>A QueryOperationResponse that is enumerable over the results and holds other response information.</returns>
        /// <exception cref="ArgumentNullException">null requestUri</exception>
        /// <exception cref="ArgumentException">The <paramref name="httpMethod"/> is not GET nor POST.</exception>
        /// <exception cref="InvalidOperationException">problem materializing results of query into objects</exception>
        /// <exception cref="WebException">failure to get response for requestUri</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "Type is used to infer result")]
        internal QueryOperationResponse<TElement> InnerSynchExecute<TElement>(Uri requestUri, string httpMethod, bool? singleResult, params OperationParameter[] operationParameters)
        {
            List<UriOperationParameter> uriOperationParameters = null;
            List<BodyOperationParameter> bodyOperationParameters = null;
            this.ValidateExecuteParameters<TElement>(ref requestUri, httpMethod, ref singleResult, out bodyOperationParameters, out uriOperationParameters, operationParameters);
            QueryComponents qc = new QueryComponents(
                requestUri,
                Util.ODataVersionEmpty,
                typeof(TElement),
                null /* projection */,
                null /* normalizer rewrites */,
                httpMethod,
                singleResult,
                bodyOperationParameters,
                uriOperationParameters);

            requestUri = qc.Uri;
            DataServiceRequest request = new DataServiceRequest<TElement>(requestUri, qc, null);
            return request.Execute<TElement>(this, qc);
        }
#endif

        /// <summary>Begins the execution of the request uri based on the http method.</summary>
        /// <typeparam name="TElement">element type of the result</typeparam>
        /// <param name="requestUri">request to execute</param>
        /// <param name="callback">User callback when results from execution are available.</param>
        /// <param name="state">user state in IAsyncResult</param>
        /// <param name="httpMethod">HttpMethod to use. Only GET and POST are supported.</param>
        /// <param name="method">async method name at the source.</param>
        /// <param name="singleResult">If set to true, indicates that a single result is expected as a response.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        /// <returns>async result object</returns>
        internal IAsyncResult InnerBeginExecute<TElement>(Uri requestUri, AsyncCallback callback, object state, string httpMethod, string method, bool? singleResult, params OperationParameter[] operationParameters)
        {
            List<UriOperationParameter> uriOperationParameters = null;
            List<BodyOperationParameter> bodyOperationParameters = null;
            this.ValidateExecuteParameters<TElement>(ref requestUri, httpMethod, ref singleResult, out bodyOperationParameters, out uriOperationParameters, operationParameters);
            QueryComponents qc = new QueryComponents(requestUri, Util.ODataVersionEmpty, typeof(TElement), null, null, httpMethod, singleResult, bodyOperationParameters, uriOperationParameters);
            requestUri = qc.Uri;
            return (new DataServiceRequest<TElement>(requestUri, qc, null)).BeginExecute(this, this, callback, state, method);
        }

        /// <summary>
        /// Track a binding.
        /// </summary>
        /// <param name="source">Source resource.</param>
        /// <param name="sourceProperty">Property on the source resource that relates to the target resource.</param>
        /// <param name="target">Target resource.</param>
        /// <param name="linkMerge">merge operation</param>
        internal void AttachLink(object source, string sourceProperty, object target, MergeOption linkMerge)
        {
            this.EnsureRelatable(source, sourceProperty, target, EntityStates.Unchanged);

            // TODO: attach link should never result in any changes ?? why did we ever incrementchange number on attach link
            this.entityTracker.AttachLink(source, sourceProperty, target, linkMerge);
        }

        #region HttpWebRequest Creation Methods

        /// <summary>
        ///  Creates the OData request message to write the headers and payload into.
        /// </summary>
        /// <param name="requestMessageArgs">The arguments for creating the message.</param>
        /// <param name="descriptor">Descriptor to expose in SendingRequest2</param>
        /// <returns>An instance of IODataRequestMessage with the given headers and version.</returns>
        internal ODataRequestMessageWrapper CreateODataRequestMessage(
            BuildingRequestEventArgs requestMessageArgs,
            Descriptor descriptor)
        {
            ODataRequestMessageWrapper requestMessage = new RequestInfo(this).WriteHelper.CreateRequestMessage(requestMessageArgs);
            requestMessage.FireSendingRequest2(descriptor);
            return requestMessage;
        }

        #endregion

        /// <summary>
        /// user hook to resolve name into a type
        /// </summary>
        /// <param name="wireName">name to resolve</param>
        /// <returns>Null if no type resolver is registered, otherwise returns whatever is returned by the type resolver.</returns>
        internal Type ResolveTypeFromName(string wireName)
        {
            Func<string, Type> resolve = this.ResolveType;
            if (null != resolve)
            {
                // if the ResolveType property is set, call the provided type resultion method
                return resolve(wireName);
            }

            return null;
        }

        /// <summary>
        /// The reverse of ResolveType, use for complex types and LINQ query expression building
        /// </summary>
        /// <param name="type">client type</param>
        /// <returns>type for the server</returns>
        internal string ResolveNameFromTypeInternal(Type type)
        {
            Debug.Assert(null != type, "null type");
            Func<Type, string> resolve = this.ResolveName;
            return ((null != resolve) ? resolve(type) : (String)null);
        }

        /// <summary>
        /// Fires the SendingRequest event.
        /// </summary>
        /// <param name="eventArgs">SendingRequestEventArgs instance containing all information about the request.</param>
        internal void FireSendingRequest(SendingRequestEventArgs eventArgs)
        {
            Debug.Assert(this.InnerSendingRequest != null, "this.InnerSendingRequest != null");
            Debug.Assert(this.SendingRequest2 == null, "this.SendingRequest2 == null");
            this.InnerSendingRequest(this, eventArgs);
        }

        /// <summary>
        /// Fires the SendingRequest2 event.
        /// </summary>
        /// <param name="eventArgs">SendingRequest2EventArgs instance containing all information about the request.</param>
        internal void FireSendingRequest2(SendingRequest2EventArgs eventArgs)
        {
            Debug.Assert(this.SendingRequest2 != null, "this.SendingRequest2 != null");
            this.SendingRequest2(this, eventArgs);
        }

        /// <summary>
        /// Fires the ReceivingResponse event.
        /// </summary>
        /// <param name="receivingResponseEventArgs">Args instance containing information about the response.</param>
        internal void FireReceivingResponseEvent(ReceivingResponseEventArgs receivingResponseEventArgs)
        {
            if (this.ReceivingResponse != null)
            {
                this.ReceivingResponse(this, receivingResponseEventArgs);
            }
        }

        #region GetResponse

        /// <summary>
        /// This method wraps the HttpWebRequest.GetSyncronousResponse method call. The reasons for doing this are to give us a place
        /// to invoke internal test hook callbacks that can validate the response headers, and also so that we can do
        /// debug validation to make sure that the headers have not changed since they were originally configured on the request.
        /// </summary>
        /// <param name="request">HttpWebRequest instance</param>
        /// <param name="handleWebException">If set to true, this method will only re-throw the WebException that was caught if
        /// the response in the exception is null. If set to false, this method will always re-throw in case of a WebException.</param>
        /// <returns>
        /// Returns the HttpWebResponse from the wrapped GetSyncronousResponse method.
        /// </returns>
        internal IODataResponseMessage GetSyncronousResponse(ODataRequestMessageWrapper request, bool handleWebException)
        {
            return this.GetResponseHelper(request, null, handleWebException);
        }

        /// <summary>
        /// This method wraps the HttpWebRequest.EndGetResponse method call. The reason for doing this is to give us a place
        /// to invoke internal test hook callbacks that can validate the response headers.
        /// </summary>
        /// <param name="request">HttpWebRequest instance</param>
        /// <param name="asyncResult">Async result obtained from previous call to BeginGetResponse.</param>
        /// <returns>Returns the HttpWebResponse from the wrapped EndGetResponse method.</returns>
        internal IODataResponseMessage EndGetResponse(ODataRequestMessageWrapper request, IAsyncResult asyncResult)
        {
            Debug.Assert(asyncResult != null, "Expected a non-null asyncResult for all scenarios calling EndGetResponse");
            return this.GetResponseHelper(request, asyncResult, true);
        }

        #endregion

        #region Test hooks for header and payload verification

        /// <summary>
        /// Invokes the sendRequest test hook callback with a reference to the HttpWebRequest
        /// </summary>
        /// <param name="request">HttpWebRequest to provide in the callback.</param>
        internal void InternalSendRequest(HttpWebRequest request)
        {
            if (this.sendRequest != null)
            {
                this.sendRequest(request);
            }
        }

        /// <summary>
        /// Invokes the getRequestWrappingStream test hook callback, so that the test code can wrap the stream and see what gets written to it.
        /// </summary>
        /// <param name="requestStream">Underlying HTTP stream to be wrapped</param>
        /// <returns>
        /// If the test hook is being used, returns the stream provided by the callback, otherwise returns the original stream.
        /// </returns>
        internal Stream InternalGetRequestWrappingStream(Stream requestStream)
        {
            return this.getRequestWrappingStream != null ? this.getRequestWrappingStream(requestStream) : requestStream;
        }

        /// <summary>
        /// Invokes the sendResponse test hook callback with a reference to the HttpWebResponse
        /// </summary>
        /// <param name="response">HttpWebResponse to provide in the callback.</param>
        internal void InternalSendResponse(HttpWebResponse response)
        {
            if (this.sendResponse != null)
            {
                this.sendResponse(response);
            }
        }

        /// <summary>
        /// Invokes the getResponseWrappingStream test hook callback, so that the test code can wrap the stream and see what gets read from it.
        /// </summary>
        /// <param name="responseStream">Underlying HTTP stream to be wrapped</param>
        /// <returns>
        /// If the test hook is being used, returns the stream provided by the callback, otherwise returns the original stream.
        /// </returns>
        internal Stream InternalGetResponseWrappingStream(Stream responseStream)
        {
            return this.getResponseWrappingStream != null ? this.getResponseWrappingStream(responseStream) : responseStream;
        }

        #endregion

        /// <summary>
        /// Gets an entity metadata builder to evaluate metadata which is not present in payloads, or for which the payload is not available.
        /// </summary>
        /// <param name="entitySetName">Name of the entity set to which the entity belongs.</param>
        /// <param name="entityInstance">The entity to build metadata for.</param>
        /// <returns>
        /// A metadata builder for the entity tracked by the given entity instance.
        /// </returns>
        /// <remarks>
        /// This is used for example to determine the edit link for an entity if the payload didn't have one, or to determine the URL for a navigation when building a query through LINQ.
        /// </remarks>
        internal virtual ODataResourceMetadataBuilder GetEntityMetadataBuilder(string entitySetName, IEdmStructuredValue entityInstance)
        {
            return new ConventionalODataEntityMetadataBuilder(this.baseUriResolver, entitySetName, entityInstance, this.UrlKeyDelimiter);
        }

        /// <summary>
        /// Fires the BuildingRequest event to get a new RequestMessageArgs object.
        /// </summary>
        /// <param name="method">Http method for the request.</param>
        /// <param name="requestUri">Base Uri for the request.</param>
        /// <param name="headers">Http stack to use for the request.</param>
        /// <param name="stack">Http stack to use for the request.</param>
        /// <param name="descriptor">Descriptor for the request, if there is one.</param>
        /// <returns>A new RequestMessageArgs object for building the request message.</returns>
        internal BuildingRequestEventArgs CreateRequestArgsAndFireBuildingRequest(string method, Uri requestUri, HeaderCollection headers, HttpStack stack, Descriptor descriptor)
        {
            BuildingRequestEventArgs requestMessageArgs = new BuildingRequestEventArgs(method, requestUri, headers, descriptor, stack);

            // Set default headers before firing BudingRequest event
            requestMessageArgs.HeaderCollection.SetDefaultHeaders();

            return this.FireBuildingRequest(requestMessageArgs);
        }

        /// <summary>
        /// Determines the type that
        /// </summary>
        /// <param name="typeName">Name of the type to resolve.</param>
        /// <param name="fullNamespace">Namespace of the type.</param>
        /// <param name="languageDependentNamespace">Namespace of the type, can be different in VB than the fullNamespace.</param>
        /// <returns>Type that the name resolved to or null if none found.</returns>
        /// <remarks>Function was added for Portable Lib support to handle the differences in accessing the assembly of the context.</remarks>
        protected internal Type DefaultResolveType(string typeName, string fullNamespace, string languageDependentNamespace)
        {
            if (typeName != null && typeName.StartsWith(fullNamespace, StringComparison.Ordinal))
            {
                int namespaceLength = fullNamespace != null ? fullNamespace.Length : 0;
                Type type = this.GetType().GetAssembly().GetType(string.Concat(languageDependentNamespace, typeName.Substring(namespaceLength)), false);
                if (type == null)
                {
                    return this.GetType().GetAssembly().GetTypes().ToList().Where(t =>
                    {
                        string serverDefinedName = typeName.Substring(namespaceLength + 1);
                        OriginalNameAttribute originalNameAttribute = (OriginalNameAttribute)t.GetCustomAttributes(typeof(OriginalNameAttribute), true).SingleOrDefault();
                        return originalNameAttribute != null && originalNameAttribute.OriginalName == serverDefinedName && t.Namespace == languageDependentNamespace;
                    }).SingleOrDefault();
                }
                else
                {
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines if a type is a singleton type
        /// </summary>
        /// <typeparam name="TElement">The type to be determined</typeparam>
        /// <returns>True for primitive or complex types,
        /// false for collection types of primitive and complex,
        /// and null for anything else (like entity, feed, etc.)</returns>
        private bool? IsSingletonType<TElement>()
        {
            var clrType = typeof(TElement);
            var edmType = this.Model.GetOrCreateEdmType(clrType).ToEdmTypeReference(ClientTypeUtil.CanAssignNull(clrType));

            if (edmType.IsPrimitive() || edmType.IsComplex())
            {
                return true;
            }

            if (edmType.IsCollection())
            {
                var elemType = edmType.AsCollection().ElementType();
                if (elemType.IsPrimitive() || elemType.IsComplex())
                {
                    return false;
                }
            }

            return null;
        }

        /// <summary>
        /// Continue to asynchronously loads the next page of related entities for a specified property from the data service.
        /// </summary>
        /// <param name="response">The response of previous page</param>
        /// <param name="entity">The entity that contains the property to load.</param>
        /// <param name="propertyName">The name of the property of the specified entity to load.</param>
        /// <returns>An instance of <see cref="T:Microsoft.OData.Client.QueryOperationResponse`1" /> that contains the results of the request.</returns>
        private QueryOperationResponse ContinuePage(QueryOperationResponse response, object entity, string propertyName)
        {
            var continuation = response.GetContinuation();
            if (continuation != null)
            {
                var currentTask = Task<QueryOperationResponse>.Factory.FromAsync(this.BeginLoadProperty(entity, propertyName, continuation, null, null), this.EndLoadProperty);
                var nextTask = currentTask.ContinueWith(t => this.ContinuePage(t.Result, entity, propertyName));
                Task.WaitAll(new Task[] { currentTask, nextTask });
                return nextTask.Result;
            }

            return response;
        }

        /// <summary
        /// >validate <paramref name="entitySetName"/> and trim leading and trailing forward slashes
        /// </summary>
        /// <param name="entitySetName">resource name to validate</param>
        /// <exception cref="ArgumentNullException">if entitySetName was null</exception>
        /// <exception cref="ArgumentException">if entitySetName was empty or contained only forward slash</exception>
        private static void ValidateEntitySetName(ref string entitySetName)
        {
            Util.CheckArgumentNullAndEmpty(entitySetName, "entitySetName");
            entitySetName = entitySetName.Trim(UriUtil.ForwardSlash);

            Util.CheckArgumentNullAndEmpty(entitySetName, "entitySetName");

            Uri tmp = UriUtil.CreateUri(entitySetName, UriKind.RelativeOrAbsolute);
            if (tmp.IsAbsoluteUri ||
                !String.IsNullOrEmpty(UriUtil.CreateUri(new Uri("http://ConstBaseUri/ConstService.svc/"), tmp)
                                     .GetComponents(UriComponents.Query | UriComponents.Fragment, UriFormat.SafeUnescaped)))
            {
                throw Error.Argument(Strings.Context_EntitySetName, "entitySetName");
            }
        }

        /// <summary>validate <paramref name="entity"/> is entity type</summary>
        /// <param name="entity">entity to validate</param>
        /// <param name="model">The client model.</param>
        /// <exception cref="ArgumentNullException">if entity was null</exception>
        /// <exception cref="ArgumentException">if entity does not have a key property</exception>
        private static void ValidateEntityType(object entity, ClientEdmModel model)
        {
            Util.CheckArgumentNull(entity, "entity");

            if (!ClientTypeUtil.TypeIsEntity(entity.GetType(), model))
            {
                throw Error.Argument(Strings.Content_EntityIsNotEntityType, "entity");
            }
        }

        /// <summary>
        /// Validates a given list of operation parameter and returns two seperated list of body operation parameter
        /// and uri operation parameter respectively.
        /// </summary>
        /// <param name="httpMethod">the http method used in the request. Only POST and GET http methods are supported with operation parameters.</param>
        /// <param name="parameters">The list of operation parameters to be validated.</param>
        /// <param name="bodyOperationParameters">The list of body operation parameters to be returned.</param>
        /// <param name="uriOperationParameters">The list of uri operation parameters to be returned.</param>
        private static void ValidateOperationParameters(
            string httpMethod,
            OperationParameter[] parameters,
            out List<BodyOperationParameter> bodyOperationParameters,
            out List<UriOperationParameter> uriOperationParameters)
        {
            Debug.Assert(parameters != null, "parameters != null");
            Debug.Assert(
                string.CompareOrdinal(XmlConstants.HttpMethodPost, httpMethod) == 0 ||
                string.CompareOrdinal(XmlConstants.HttpMethodGet, httpMethod) == 0,
                "HttpMethod was expected to be either GET or POST.");

            HashSet<string> uriParamNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> bodyParamNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            List<UriOperationParameter> uriParams = new List<UriOperationParameter>();
            List<BodyOperationParameter> bodyParams = new List<BodyOperationParameter>();

            foreach (OperationParameter operationParameter in parameters)
            {
                if (operationParameter == null)
                {
                    throw new ArgumentException(Strings.Context_NullElementInOperationParameterArray);
                }

                if (String.IsNullOrEmpty(operationParameter.Name))
                {
                    throw new ArgumentException(Strings.Context_MissingOperationParameterName);
                }

                String paramName = operationParameter.Name.Trim();

                BodyOperationParameter bodyOperationParameter = operationParameter as BodyOperationParameter;
                if (bodyOperationParameter != null)
                {
                    if (string.CompareOrdinal(XmlConstants.HttpMethodGet, httpMethod) == 0)
                    {
                        throw new ArgumentException(Strings.Context_BodyOperationParametersNotAllowedWithGet);
                    }

                    if (!bodyParamNames.Add(paramName))
                    {
                        throw new ArgumentException(Strings.Context_DuplicateBodyOperationParameterName);
                    }

                    bodyParams.Add(bodyOperationParameter);
                }
                else
                {
                    UriOperationParameter uriOperationParameter = operationParameter as UriOperationParameter;
                    if (uriOperationParameter != null)
                    {
                        if (!uriParamNames.Add(paramName))
                        {
                            throw new ArgumentException(Strings.Context_DuplicateUriOperationParameterName);
                        }

                        uriParams.Add(uriOperationParameter);
                    }
                }
            }

            uriOperationParameters = uriParams.Any() ? uriParams : null;
            bodyOperationParameters = bodyParams.Any() ? bodyParams : null;
        }

        /// <summary>
        /// Fires the BuildingRequest event so the user can add custom query parameters.
        /// </summary>
        /// <param name="buildingRequestEventArgs">Information about the request so they user can selectively add query parameters.</param>
        /// <returns>A new RequestMessageArgs object that contains any changes the user made to the query string.</returns>
        private BuildingRequestEventArgs FireBuildingRequest(BuildingRequestEventArgs buildingRequestEventArgs)
        {
            if (this.HasBuildingRequestEventHandlers)
            {
                this.InnerBuildingRequest(this, buildingRequestEventArgs);

                // The reason to clone it is so that users can change the
                // value after this event is fired.
                return buildingRequestEventArgs.Clone();
            }

            return buildingRequestEventArgs;
        }

        /// <summary>
        /// Validate the SaveChanges Option
        /// </summary>
        /// <param name="options">options as specified by the user.</param>
        private void ValidateSaveChangesOptions(SaveChangesOptions options)
        {
            const SaveChangesOptions All = SaveChangesOptions.ContinueOnError | SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.ReplaceOnUpdate | SaveChangesOptions.PostOnlySetProperties;

            // Make sure no higher order bits are set.
            if ((options | All) != All)
            {
                throw Error.ArgumentOutOfRange("options");
            }

            // SaveChangesOptions.BatchWithSingleChangeset and SaveChangesOptions.BatchWithIndependentOperations can't be set together.
            if (Util.IsFlagSet(options, SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.BatchWithIndependentOperations))
            {
                throw Error.ArgumentOutOfRange("options");
            }

            // BatchWithSingleChangeset and continueOnError can't be set together
            if (Util.IsFlagSet(options, SaveChangesOptions.BatchWithSingleChangeset | SaveChangesOptions.ContinueOnError))
            {
                throw Error.ArgumentOutOfRange("options");
            }

            // BatchWithIndependentOperations and continueOnError can't be set together
            if (Util.IsFlagSet(options, SaveChangesOptions.BatchWithIndependentOperations | SaveChangesOptions.ContinueOnError))
            {
                throw Error.ArgumentOutOfRange("options");
            }

            // OnlyPostExplicitProperties cannot be used without DataServiceCollection to track properties change
            if (Util.IsFlagSet(options, SaveChangesOptions.PostOnlySetProperties) && !this.UsingDataServiceCollection)
            {
                throw Error.InvalidOperation(Strings.Context_MustBeUsedWith("SaveChangesOptions.OnlyPostExplicitProperties", "DataServiceCollection"));
            }
        }

        /// <summary>
        /// Validate and process the input parameters to all the execute methods. Also seperates and returns
        /// the input operation parameters list into two seperate list - one of body operation parameters and the other
        /// for uri operation parameters.
        /// </summary>
        /// <typeparam name="TElement">element type. See Execute method for more details.</typeparam>
        /// <param name="requestUri">request to execute</param>
        /// <param name="httpMethod">HttpMethod to use. Only GET and POST are supported if operation parameters are not empty.</param>
        /// <param name="singleResult">If set to true, indicates that a single result is expected as a response.</param>
        /// <param name="bodyOperationParameters">The list of body operation parameters to be returned.</param>
        /// <param name="uriOperationParameters">The list of uri operation parameters to be returned.</param>
        /// <param name="operationParameters">The operation parameters associated with the service operation.</param>
        private void ValidateExecuteParameters<TElement>(
            ref Uri requestUri,
            string httpMethod,
            ref bool? singleResult,
            out List<BodyOperationParameter> bodyOperationParameters,
            out List<UriOperationParameter> uriOperationParameters,
            params OperationParameter[] operationParameters)
        {
            if (string.CompareOrdinal(XmlConstants.HttpMethodGet, httpMethod) != 0 &&
                string.CompareOrdinal(XmlConstants.HttpMethodPost, httpMethod) != 0 &&
                string.CompareOrdinal(XmlConstants.HttpMethodDelete, httpMethod) != 0)
            {
                throw new ArgumentException(Strings.Context_ExecuteExpectsGetOrPostOrDelete, "httpMethod");
            }

            if (ClientTypeUtil.TypeOrElementTypeIsStructured(typeof(TElement)))
            {
                singleResult = null;
            }

            if (operationParameters != null && operationParameters.Length > 0)
            {
                DataServiceContext.ValidateOperationParameters(httpMethod, operationParameters, out bodyOperationParameters, out uriOperationParameters);
            }
            else
            {
                uriOperationParameters = null;
                bodyOperationParameters = null;
            }

            requestUri = this.BaseUriResolver.GetOrCreateAbsoluteUri(requestUri);
        }

        /// <summary>
        /// create the load property request
        /// </summary>
        /// <param name="entity">entity</param>
        /// <param name="propertyName">name of collection or reference property to load</param>
        /// <param name="callback">The AsyncCallback delegate.</param>
        /// <param name="state">user state</param>
        /// <param name="requestUri">The request uri, or null if one is to be constructed</param>
        /// <param name="continuation">Continuation, if one is available.</param>
        /// <returns>a aync result that you can get a response from</returns>
        private LoadPropertyResult CreateLoadPropertyRequest(object entity, string propertyName, AsyncCallback callback, object state, Uri requestUri, DataServiceQueryContinuation continuation)
        {
            Debug.Assert(continuation == null || requestUri == null, "continuation == null || requestUri == null -- only one or the either (or neither) may be passed in");
            Util.CheckArgumentNull(entity, "entity");
            EntityDescriptor box = this.entityTracker.GetEntityDescriptor(entity);
            Util.CheckArgumentNullAndEmpty(propertyName, "propertyName");

            ClientTypeAnnotation type = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(entity.GetType()));
            Debug.Assert(type.IsEntityType, "must be entity type to be contained");

            if (EntityStates.Added == box.State)
            {
                throw Error.InvalidOperation(Strings.Context_NoLoadWithInsertEnd);
            }

            ClientPropertyAnnotation property = type.GetProperty(propertyName, UndeclaredPropertyBehavior.ThrowException);
            Debug.Assert(null != property, "should have thrown if propertyName didn't exist");

            bool isContinuation = requestUri != null || continuation != null;

            ProjectionPlan plan;
            if (continuation == null)
            {
                plan = null;
            }
            else
            {
                plan = continuation.Plan;
                requestUri = continuation.NextLinkUri;
            }

            bool mediaLink = (type.MediaDataMember != null && propertyName == type.MediaDataMember.PropertyName);
            Version requestVersion;
            if (requestUri == null)
            {
                if (mediaLink)
                {
                    // special case for requesting the "media" value of an ATOM media link entry
                    Uri relativeUri = UriUtil.CreateUri(XmlConstants.UriValueSegment, UriKind.Relative);
                    requestUri = UriUtil.CreateUri(box.GetResourceUri(this.BaseUriResolver, true /*queryLink*/), relativeUri);
                }
                else
                {
                    requestUri = box.GetNestedResourceInfo(this.baseUriResolver, property);
                }
            }

            requestVersion = Util.ODataVersion4;

            HeaderCollection headers = new HeaderCollection();

            // Validate and set the request DSV header
            headers.SetRequestVersion(requestVersion, this.MaxProtocolVersionAsVersion);

            if (mediaLink)
            {
                this.Format.SetRequestAcceptHeaderForStream(headers);
            }
            else
            {
                this.formatTracker.SetRequestAcceptHeader(headers);
            }

            ODataRequestMessageWrapper request = this.CreateODataRequestMessage(
                this.CreateRequestArgsAndFireBuildingRequest(XmlConstants.HttpMethodGet, requestUri, headers, this.HttpStack, null /*descriptor*/),
                null /*descriptor*/);

            DataServiceRequest dataServiceRequest = DataServiceRequest.GetInstance(property.PropertyType, requestUri);
            dataServiceRequest.PayloadKind = ODataPayloadKind.IndividualProperty;
            return new LoadPropertyResult(entity, propertyName, this, request, callback, state, dataServiceRequest, plan, isContinuation);
        }

        /// <summary>
        /// verify the source and target are relatable
        /// </summary>
        /// <param name="source">source Resource</param>
        /// <param name="sourceProperty">source Property</param>
        /// <param name="target">target Resource</param>
        /// <param name="state">destination state of relationship to evaluate for</param>
        /// <returns>true if DeletedState and one of the ends is in the added state</returns>
        /// <exception cref="ArgumentNullException">if source or target are null</exception>
        /// <exception cref="ArgumentException">if source or target are not contained</exception>
        /// <exception cref="ArgumentNullException">if source property is null</exception>
        /// <exception cref="ArgumentException">if source property empty</exception>
        /// <exception cref="InvalidOperationException">Can only relate ends with keys.</exception>
        /// <exception cref="ArgumentException">If target doesn't match property type.</exception>
        /// <exception cref="InvalidOperationException">If adding relationship where one of the ends is in the deleted state.</exception>
        /// <exception cref="InvalidOperationException">If attaching relationship where one of the ends is in the added or deleted state.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Pending")]
        private bool EnsureRelatable(object source, string sourceProperty, object target, EntityStates state)
        {
            Util.CheckArgumentNull(source, "source");
            EntityDescriptor sourceResource = this.entityTracker.GetEntityDescriptor(source);

            EntityDescriptor targetResource = null;
            if ((null != target) || ((EntityStates.Modified != state) && (EntityStates.Unchanged != state)))
            {
                Util.CheckArgumentNull(target, "target");
                targetResource = this.entityTracker.GetEntityDescriptor(target);
            }

            Util.CheckArgumentNullAndEmpty(sourceProperty, "sourceProperty");

            ClientTypeAnnotation type = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(source.GetType()));
            Debug.Assert(type.IsEntityType, "should be enforced by just adding an object");

            // will throw InvalidOperationException if property doesn't exist
            ClientPropertyAnnotation property = type.GetProperty(sourceProperty, UndeclaredPropertyBehavior.ThrowException);

            if (property.IsKnownType)
            {
                throw Error.InvalidOperation(Strings.Context_RelationNotRefOrCollection);
            }

            if (EntityStates.Unchanged == state && null == target && property.IsEntityCollection)
            {
                Util.CheckArgumentNull(target, "target");
                targetResource = this.entityTracker.GetEntityDescriptor(target);
            }

            if (((EntityStates.Added == state) || (EntityStates.Deleted == state)) && !property.IsEntityCollection)
            {
                throw Error.InvalidOperation(Strings.Context_AddLinkCollectionOnly);
            }
            else if (EntityStates.Modified == state && property.IsEntityCollection)
            {
                throw Error.InvalidOperation(Strings.Context_SetLinkReferenceOnly);
            }

            // if (property.IsEntityCollection) then property.PropertyType is the collection elementType
            // either way you can only have a relation ship between keyed objects
            type = this.model.GetClientTypeAnnotation(this.model.GetOrCreateEdmType(property.EntityCollectionItemType ?? property.PropertyType));
            Debug.Assert(type.IsEntityType, "should be enforced by just adding an object");

            if ((null != target) && !type.ElementType.IsInstanceOfType(target))
            {
                // target is not of the correct type
                throw Error.Argument(Strings.Context_RelationNotRefOrCollection, "target");
            }

            if ((EntityStates.Added == state) || (EntityStates.Unchanged == state))
            {
                if ((sourceResource.State == EntityStates.Deleted) ||
                    ((targetResource != null) && (targetResource.State == EntityStates.Deleted)))
                {
                    // can't add/attach new relationship when source or target in deleted state
                    throw Error.InvalidOperation(Strings.Context_NoRelationWithDeleteEnd);
                }
            }

            if ((EntityStates.Deleted == state) || (EntityStates.Unchanged == state))
            {
                if ((sourceResource.State == EntityStates.Added) ||
                    ((targetResource != null) && (targetResource.State == EntityStates.Added)))
                {
                    // can't have non-added relationship when source or target is in added state
                    if (EntityStates.Deleted == state)
                    {
                        return true;
                    }

                    throw Error.InvalidOperation(Strings.Context_NoRelationWithInsertEnd);
                }
            }

            return false;
        }

        /// <summary>
        /// This method creates an async result object around a request to get the read stream for a Media Resource
        /// associated with the Media Link Entry represented by the entity object.
        /// </summary>
        /// <param name="entity">The entity which is the Media Link Entry for the requested Media Resource. Thist must specify
        /// a tracked entity in a non-added state.</param>
        /// <param name="args">Instance of <see cref="DataServiceRequestArgs"/> class with additional metadata for the request.
        /// Must not be null.</param>
        /// <param name="callback">User defined callback to be called when results are available. Can be null.</param>
        /// <param name="state">User state in IAsyncResult. Can be null.</param>
        /// <param name="name">name of the stream.</param>
        /// <returns>The async result object for the request, the request hasn't been started yet.</returns>
        /// <exception cref="ArgumentNullException">Either entity or args parameters are null.</exception>
        /// <exception cref="ArgumentException">The specified entity is either not tracked,
        /// is in the added state or it's not an MLE.</exception>
        private GetReadStreamResult CreateGetReadStreamResult(
            object entity,
            DataServiceRequestArgs args,
            AsyncCallback callback,
            object state,
            string name)
        {
            Util.CheckArgumentNull(entity, "entity");
            Util.CheckArgumentNull(args, "args");

            EntityDescriptor entityDescriptor = this.entityTracker.GetEntityDescriptor(entity);
            StreamDescriptor streamDescriptor;
            Uri requestUri;
            Version version;
            if (name == null)
            {
                version = null;
                requestUri = entityDescriptor.ReadStreamUri;
                if (requestUri == null)
                {
                    throw new ArgumentException(Strings.Context_EntityNotMediaLinkEntry, "entity");
                }

                streamDescriptor = entityDescriptor.DefaultStreamDescriptor;
            }
            else
            {
                version = Util.ODataVersion4;
                if (!entityDescriptor.TryGetNamedStreamInfo(name, out streamDescriptor))
                {
                    throw new ArgumentException(Strings.Context_EntityDoesNotContainNamedStream(name), "name");
                }

                // use the edit link, if self link is not specified.
                requestUri = streamDescriptor.SelfLink ?? streamDescriptor.EditLink;
                if (requestUri == null)
                {
                    throw new ArgumentException(Strings.Context_MissingSelfAndEditLinkForNamedStream(name), "name");
                }
            }

            // Because the user could be re-using the args class, make a copy of the headers they have set so far before adding any more.
            HeaderCollection headers = args.HeaderCollection.Copy();

            // Validate and set the request DSV header
            headers.SetRequestVersion(version, this.MaxProtocolVersionAsVersion);

            this.Format.SetRequestAcceptHeaderForStream(headers);

            BuildingRequestEventArgs requestMessageArgs = this.CreateRequestArgsAndFireBuildingRequest(XmlConstants.HttpMethodGet, requestUri, headers, HttpStack.Auto, streamDescriptor);

            ODataRequestMessageWrapper requestMessage = this.CreateODataRequestMessage(requestMessageArgs, streamDescriptor);

            return new GetReadStreamResult(this, "GetReadStream", requestMessage, callback, state, streamDescriptor);
        }

        /// <summary>
        /// Ensures that the required OData version is lesser than the maxprotocolversion  on this instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">throws an invalidoperationexception if the max protocolversion is lesser than the required protocol version</exception>
        private void EnsureMinimumProtocolVersionV3()
        {
            if (this.MaxProtocolVersionAsVersion < Util.ODataVersion4)
            {
                throw Error.InvalidOperation(Strings.Context_RequestVersionIsBiggerThanProtocolVersion(Util.ODataVersion4, this.MaxProtocolVersionAsVersion));
            }
        }

        /// <summary>
        /// Helper method for calling the overridable GetEntityMetadataBuilder API and performing common logic/verification.
        /// </summary>
        /// <param name="descriptor">The entity descriptor tracking the entity.</param>
        /// <returns>A metadata builder for the entity tracked by the given descriptor.</returns>
        private ODataResourceMetadataBuilder GetEntityMetadataBuilderInternal(EntityDescriptor descriptor)
        {
            Debug.Assert(descriptor != null, "descriptor != null");

            // TODO: Should things with slashes still be passed down? We will need to make a decision one way or the other.
            // For now we will pass them down.
            ODataResourceMetadataBuilder entityMetadataBuilder = this.GetEntityMetadataBuilder(descriptor.EntitySetName, descriptor.EdmValue);
            if (entityMetadataBuilder == null)
            {
                throw new InvalidOperationException(Strings.Context_EntityMetadataBuilderIsRequired);
            }

            return entityMetadataBuilder;
        }

        /// <summary>
        /// This method wraps the HttpWebRequest.GetSyncronousResponse method call. It fires the ReceivingResponse event.
        /// It also gives us a place to invoke internal test hook callbacks that can validate the response headers, and also so that we can do
        /// debug validation to make sure that the headers have not changed since they were originally configured on the request.
        /// </summary>
        /// <param name="request">HttpWebRequest instance</param>
        /// <param name="asyncResult">IAsyncResult for EndGetResponse if this is an async call.</param>
        /// <param name="handleWebException">If set to true, this method will only re-throw the WebException that was caught if
        /// the response in the exception is null. If set to false, this method will always re-throw in case of a WebException.</param>
        /// <returns>Returns the HttpWebResponse from the wrapped GetSyncronousResponse method.</returns>
        private IODataResponseMessage GetResponseHelper(ODataRequestMessageWrapper request, IAsyncResult asyncResult, bool handleWebException)
        {
            Debug.Assert(request != null, "Expected a non-null request for all scenarios calling GetSyncronousResponse");

            IODataResponseMessage response = null;
            try
            {
#if !PORTABLELIB
                if (asyncResult == null)
                {
                    response = request.GetResponse();
                }
                else
                {
                    response = request.EndGetResponse(asyncResult);
                }
#else
                response = request.EndGetResponse(asyncResult);
#endif
                this.FireReceivingResponseEvent(new ReceivingResponseEventArgs(response, request.Descriptor));
            }
            catch (DataServiceTransportException e)
            {
                response = e.Response;

                this.FireReceivingResponseEvent(new ReceivingResponseEventArgs(response, request.Descriptor));

                if (!handleWebException || response == null)
                {
                    throw;
                }
            }

            return response;
        }

        /// <summary>
        /// Mark an existing object for update in the context.
        /// </summary>
        /// <param name="entity">entity to be mark for update</param>
        /// <param name="failIfNotUnchanged">If true, then an exception should be thrown if the entity is in neither the unchanged nor modified states.</param>
        /// <exception cref="ArgumentNullException">if entity is null</exception>
        /// <exception cref="ArgumentException">if entity is detached</exception>
        /// <exception cref="InvalidOperationException">if entity is not unchanged or modified and <paramref name="failIfNotUnchanged"/> is true.</exception>
        private void UpdateObjectInternal(object entity, bool failIfNotUnchanged)
        {
            Util.CheckArgumentNull(entity, "entity");

            EntityDescriptor resource = this.entityTracker.TryGetEntityDescriptor(entity);
            if (resource == null)
            {
                throw Error.Argument(Strings.Context_EntityNotContained, "entity");
            }

            if (resource.State == EntityStates.Modified)
            {
                return;
            }

            if (resource.State != EntityStates.Unchanged)
            {
                if (failIfNotUnchanged)
                {
                    throw Error.InvalidOperation(ClientStrings.Context_CannotChangeStateToModifiedIfNotUnchanged);
                }

                return;
            }

            resource.State = EntityStates.Modified;
            this.entityTracker.IncrementChange(resource);
        }

        /// <summary>
        /// Mark an existing object being tracked by the context for deletion.
        /// </summary>
        /// <param name="entity">entity to be mark deleted</param>
        /// <param name="failIfInAddedState">If true, then an exception will be thrown if the entity is in the added state.</param>
        /// <exception cref="ArgumentNullException">if entity is null</exception>
        /// <exception cref="InvalidOperationException">if entity is not being tracked by the context, or if the entity is in the added state and <paramref name="failIfInAddedState"/> is true.</exception>
        /// <remarks>
        /// Existings objects in the Added state become detached if <paramref name="failIfInAddedState"/> is false.
        /// </remarks>
        private void DeleteObjectInternal(object entity, bool failIfInAddedState)
        {
            Util.CheckArgumentNull(entity, "entity");

            EntityDescriptor resource = this.entityTracker.GetEntityDescriptor(entity);
            EntityStates state = resource.State;
            if (EntityStates.Added == state)
            {
                if (failIfInAddedState)
                {
                    throw Error.InvalidOperation(ClientStrings.Context_CannotChangeStateIfAdded(EntityStates.Deleted));
                }

                // added -> detach
                this.entityTracker.DetachResource(resource);
            }
            else if (EntityStates.Deleted != state)
            {
                Debug.Assert(
                    Util.IncludeLinkState(state),
                    "bad state transition to deleted");

                // Leave related links alone which means we can have a link in the Added
                // or Modified state referencing a source/target entity in the Deleted state.
                resource.State = EntityStates.Deleted;
                this.entityTracker.IncrementChange(resource);
            }
        }

        /// <summary>
        /// Sets the entity's state to unchanged.
        /// </summary>
        /// <param name="entity">The entity to set back to unchanged.</param>
        private void SetStateToUnchanged(object entity)
        {
            Util.CheckArgumentNull(entity, "entity");
            EntityDescriptor descriptor = this.entityTracker.GetEntityDescriptor(entity);
            if (descriptor.State == EntityStates.Added)
            {
                throw Error.InvalidOperation(ClientStrings.Context_CannotChangeStateIfAdded(EntityStates.Unchanged));
            }

            descriptor.State = EntityStates.Unchanged;
        }

        /// <summary>
        /// Cache for client edm models by version.
        /// </summary>
        private static class ClientEdmModelCache
        {
            /// <summary>A cache that maps a data service protocol version to its corresponding <see cref="ClientEdmModel"/>.</summary>
            /// <remarks>Note that it is initialized in a static ctor and must not be changed later to avoid threading issues.</remarks>
            private static readonly Dictionary<ODataProtocolVersion, ClientEdmModel> modelCache = CreateClientEdmModelCache();

            /// <summary>
            /// Get the cached model for the specified max protocol version.
            /// </summary>
            /// <param name="maxProtocolVersion">The <see cref="ODataProtocolVersion"/> to get the cached model for.</param>
            /// <returns>The cached model for the <paramref name="maxProtocolVersion"/>.</returns>
            internal static ClientEdmModel GetModel(ODataProtocolVersion maxProtocolVersion)
            {
                Util.CheckEnumerationValue(maxProtocolVersion, "maxProtocolVersion");
                Debug.Assert(modelCache.ContainsKey(maxProtocolVersion), "modelCache should be pre-initialized");

                return modelCache[maxProtocolVersion];
            }

            /// <summary>
            /// Initialize modelCache.
            /// </summary>
            /// <returns>The model cache built.</returns>
            private static Dictionary<ODataProtocolVersion, ClientEdmModel> CreateClientEdmModelCache()
            {
                Dictionary<ODataProtocolVersion, ClientEdmModel> cache =
                new Dictionary<ODataProtocolVersion, ClientEdmModel>(EqualityComparer<ODataProtocolVersion>.Default);

                IEnumerable<ODataProtocolVersion> protocolVersions =
#if PORTABLELIB // Portable lib does not support Enum.GetValues()
 typeof(ODataProtocolVersion).GetFields().Where(f => f.IsLiteral).Select(f => f.GetValue(typeof(ODataProtocolVersion))).Cast<ODataProtocolVersion>();
#else
 Enum.GetValues(typeof(ODataProtocolVersion)).Cast<ODataProtocolVersion>();
#endif

                foreach (var protocolVersion in protocolVersions)
                {
                    ClientEdmModel model = new ClientEdmModel(protocolVersion);
                    model.SetEdmVersion(protocolVersion.ToVersion());
                    cache.Add(protocolVersion, model);
                }

                return cache;
            }
        }
    }
}
