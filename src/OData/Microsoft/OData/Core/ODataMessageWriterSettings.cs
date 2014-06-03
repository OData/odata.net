//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.OData.Core.UriParser.Semantic;
    using Microsoft.OData.Edm;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for OData message writers.
    /// </summary>
    public sealed class ODataMessageWriterSettings : ODataMessageWriterSettingsBase
    {
        /// <summary>
        /// The acceptable charsets used to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </summary>
        private string acceptCharSets;

        /// <summary>
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </summary>
        private string acceptMediaTypes;

        /// <summary>
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// fields and uses the default values for the respective format. If null is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        private ODataFormat format;

        /// <summary>
        /// true if the Format property should be used to compute the media type; 
        /// false if AcceptableMediaTypes and AcceptableCharsets should be used.
        /// null if neither the format nor the acceptable media types/charsets have been set.
        /// </summary>
        private bool? useFormat;

        /// <summary>
        /// An instance representing any knobs that control the behavior of the writers
        /// inside and outside of WCF Data Services.
        /// </summary>
        private ODataWriterBehavior writerBehavior;

        /// <summary>
        /// Func to evaluate whether an annotation should be writen by the writer. The func should return true if the annotation should
        /// be writen and false if the annotation should be skipped.
        /// </summary>
        private Func<string, bool> shouldIncludeAnnotation;

        /// <summary>
        /// If set to true, then the root element of each payload will be written in the default (non-prefix-qualified) namespace of the document. 
        /// All other elements in the same namespace will also not have prefixes.
        /// </summary>
        private bool alwaysUseDefaultXmlNamespaceForRootElement;

        /// <summary>
        /// The parse result of request Uri
        /// </summary>
        private ODataUri odataUri;

        /// <summary>
        /// The base uri used in payload.
        /// </summary>
        private Uri payloadBaseUri;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataMessageWriterSettings" /> class with default settings. </summary>
        public ODataMessageWriterSettings()
            : base()
        {
            // Create the default writer behavior
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
            this.EnableAtom = false;
            this.EnableFullValidation = true;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.Core.ODataMessageWriterSettings" /> class with specified settings.</summary>
        /// <param name="other">The specified settings.</param>
        public ODataMessageWriterSettings(ODataMessageWriterSettings other)
            : base(other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.acceptCharSets = other.acceptCharSets;
            this.acceptMediaTypes = other.acceptMediaTypes;
            this.PayloadBaseUri = other.PayloadBaseUri;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.format = other.format;
            this.useFormat = other.useFormat;
            this.Version = other.Version;
            this.JsonPCallback = other.JsonPCallback;
            this.shouldIncludeAnnotation = other.shouldIncludeAnnotation;
            this.AutoComputePayloadMetadataInJson = other.AutoComputePayloadMetadataInJson;
            this.UseKeyAsSegment = other.UseKeyAsSegment;
            this.alwaysUseDefaultXmlNamespaceForRootElement = other.alwaysUseDefaultXmlNamespaceForRootElement;
            this.ODataUri = other.ODataUri;

            // NOTE: writer behavior is immutable; copy by reference is ok.
            this.writerBehavior = other.writerBehavior;
            this.EnableAtom = other.EnableAtom;
            this.EnableFullValidation = other.EnableFullValidation;
        }

        /// <summary>Gets or sets the OData protocol version to be used for writing payloads. </summary>
        /// <returns>The OData protocol version to be used for writing payloads.</returns>
        public ODataVersion? Version
        {
            get;
            set;
        }

        /// <summary>Gets or sets the document base URI which is used as base for all relative URIs. </summary>
        /// <returns>The document base URI which is used as base for all relative URIs.</returns>
        /// <remarks>
        /// This URI will be used in ATOM format only, it would be shown in &lt;xml:base /&gt; element, for JSON payload, base URI is context URI
        /// If the URI does not end with a slash, a slash would be appended automatically.
        /// </remarks>
        public Uri PayloadBaseUri
        {
            get
            {
                return this.payloadBaseUri;
            }

            set
            {
                this.payloadBaseUri = UriUtils.EnsureTaillingSlash(value);
            }
        }

        /// <summary>
        /// The OData Uri of an incoming request.  Call <see cref="ODataUriParser"/>'s methods,
        /// and assign properties (e.g., <see cref="ODataPath"/>) to <see cref="ODataUri"/>.
        /// </summary>
        public ODataUri ODataUri
        {
            get { return this.odataUri ?? (this.odataUri = new ODataUri()); }
            set { this.odataUri = value; }
        }

        /// <summary>Gets or sets a value that indicates whether the message stream will not be disposed after finishing writing with the message.</summary>
        /// <returns>true if the message stream will not be disposed after finishing writing with the message; otherwise false. The default value is false.</returns>
        public bool DisableMessageStreamDisposal
        {
            get;
            set;
        }

        /// <summary>Gets or sets a callback function use to wrap the response from server.</summary>
        /// <returns>The callback function used to wrap the response from server.</returns>
        /// <remarks>If it has a value and we are writing a JSON response, then we will wrap the entirety of the response in
        /// the provided function name and parenthesis for JSONP. Otherwise this value is ignored.</remarks>
        public string JsonPCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the writer should automatically generate or omit metadata in JSON payloads based on the metadata level.
        /// </summary>
        /// <remarks>
        /// Payload metadata includes the type names of entries and property values as well as any information that may be computed automatically, such as edit links.
        /// If, for example, ODataEntry.EditLink is not specified, then it will be automatically computed and written out in full metadata mode.
        /// If ODataEntry.EditLink is specified, then that value will be considered an "override" of the default computed edit link, and will be written out in full and minimal metadata modes. It will not be written in no metadata mode.
        /// </remarks>
        public bool AutoComputePayloadMetadataInJson
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the writer should put key values in their own URI segment when automatically building URIs.
        /// If this value is false, automatically-generated URLs will take the form "../EntitySet('KeyValue')/..".
        /// If this value is true, automatically-generated URLs will take the form "../EntitySet/KeyValue/..".
        /// If this value is not set (null), decision will be made based on the "Com.Microsoft.OData.Service.Conventions.V1.UrlConventions" vocabulary
        /// annotation on the IEdmEntityContainer, if available. The default behavior is to put key values inside parentheses and not a distinct URL segments.
        /// This setting only applies to URLs that are automatically generated by the <see cref="ODataMessageWriter" /> and does not modify URLs explicitly provided by the user.
        /// </summary>
        public bool? UseKeyAsSegment
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, all the validation would be enabled. Else some validation will be skipped.
        /// Default to true.
        /// </summary>
        internal bool EnableFullValidation { get; set; }

        /// <summary>
        /// If set to true, then the root element of each payload will be written in the default (non-prefix-qualified) namespace of the document. 
        /// All other elements in the same namespace will also not have prefixes.
        /// </summary>
        internal bool AlwaysUseDefaultXmlNamespaceForRootElement
        {
            get
            {
                return this.alwaysUseDefaultXmlNamespaceForRootElement;
            }
        }

        /// <summary>
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </summary>
        /// <remarks>A null or empty accept header means that all content types are acceptable.</remarks>
        /// <remarks>For response messages this is usually the 'Accept' header of the request message.</remarks>
        internal string AcceptableMediaTypes
        {
            get
            {
                return this.acceptMediaTypes;
            }
        }

        /// <summary>
        /// The acceptable charsets used to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </summary>
        /// <remarks>A null or empty accept charset header means that all charsets are acceptable.</remarks>
        /// <remarks>For response messages this is usually the 'Accept-Charset' header of the request message.</remarks>
        internal string AcceptableCharsets
        {
            get
            {
                return this.acceptCharSets;
            }
        }

        /// <summary>
        /// The writer behavior that holds all the knobs needed to make the writer
        /// behave differently inside and outside of WCF Data Services.
        /// </summary>
        internal ODataWriterBehavior WriterBehavior
        {
            get
            {
                return this.writerBehavior;
            }
        }

        /// <summary>
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// properties and uses the default values for the respective format. If null is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        internal ODataFormat Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// true if the Format property should be used to compute the media type; 
        /// false if AcceptableMediaTypes and AcceptableCharsets should be used.
        /// null if neither the format nor the acceptable media types/charsets have been set.
        /// </summary>
        internal bool? UseFormat
        {
            get
            {
                return this.useFormat;
            }
        }

        /// <summary>
        /// Gets the metadata document URI that has been set on the settings, or null if it has not been set.
        /// </summary>
        internal Uri MetadataDocumentUri
        {
            get
            {
                return this.ODataUri.MetadataDocumentUri;
            }
        }

        /// <summary>
        /// Gets the SelectExpand clause used when generating metadata links.
        /// </summary>
        internal SelectExpandClause SelectExpandClause
        {
            get
            {
                return this.ODataUri.SelectAndExpand;
            }
        }

        /// <summary>
        /// Gets the SelectedPropertiesNode clause generated from SelectExpandClause.
        /// </summary>
        internal SelectedPropertiesNode SelectedProperties
        {
            get
            {
                return this.SelectExpandClause != null
                    ? SelectedPropertiesNode.Create(this.SelectExpandClause)
                    : SelectedPropertiesNode.EntireSubtree;
            }
        }

        /// <summary>
        /// Gets the value indicating whether the payload represents an individual property
        /// </summary>
        internal bool IsIndividualProperty
        {
            get
            {
                return this.ODataUri.Path != null && this.ODataUri.Path.IsIndividualProperty();
            }
        }

        /// <summary>
        /// Func to evaluate whether an annotation should be writen by the writer. The func should return true if the annotation should
        /// be writen and false if the annotation should be skipped.
        /// </summary>
        internal Func<string, bool> ShouldIncludeAnnotation
        {
            get
            {
                return this.shouldIncludeAnnotation;
            }

            set
            {
                this.shouldIncludeAnnotation = value;
            }
        }

        /// <summary>
        /// Whether ATOM support is enabled.
        /// </summary>
        internal bool EnableAtom { get; set; }

        /// <summary>Sets the acceptable media types and character sets from which the content type will be computed when writing the payload.</summary>
        /// <param name="acceptableMediaTypes">The acceptable media types used to determine the content type of the message. This is a comma separated list of content types as specified in RFC 2616, Section 14.1.</param>
        /// <param name="acceptableCharSets"> The acceptable charsets to use to determine the encoding of the message. This is a comma separated list of charsets as specified in RFC 2616, Section 14.2 </param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(string acceptableMediaTypes, string acceptableCharSets)
        {
            this.acceptMediaTypes = acceptableMediaTypes;
            this.acceptCharSets = acceptableCharSets;
            this.format = null;
            this.useFormat = false;
        }

        /// <summary>Sets the format to be used when writing the payload. This will automatically set a compatible content type header.</summary>
        /// <param name="payloadFormat">The format to use for writing the payload.</param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(ODataFormat payloadFormat)
        {
            this.acceptCharSets = null;
            this.acceptMediaTypes = null;
            this.format = payloadFormat;
            this.useFormat = true;
        }

        /// <summary>Enables the <see cref="T:Microsoft.OData.Core.ODataMessageWriterSettings" /> default behavior.</summary>
        public void EnableDefaultBehavior()
        {
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
        }

        /// <summary>Specifies whether the WCF data services server behavior is enabled.</summary>
        public void EnableODataServerBehavior()
        {
            // We have to reset the ATOM entry XML customization since in the server behavior no atom entry customization is used.
            this.writerBehavior = ODataWriterBehavior.CreateODataServerBehavior();
        }

        /// <summary>Specifies whether the OData services server behavior is enabled.</summary>
        /// <param name="alwaysUseDefaultXmlNamespaceForRootElement">true if the server is configured to leave prefixes off all root elements and anything else in the same namespace, otherwise, false.</param>
        public void EnableODataServerBehavior(bool alwaysUseDefaultXmlNamespaceForRootElement)
        {
            this.EnableODataServerBehavior();
            this.alwaysUseDefaultXmlNamespaceForRootElement = alwaysUseDefaultXmlNamespaceForRootElement;
        }

        /// <summary>Enables the WCF data services client behavior.</summary>
        public void EnableWcfDataServicesClientBehavior()
        {
            this.writerBehavior = ODataWriterBehavior.CreateWcfDataServicesClientBehavior();
        }

        /// <summary>Sets the URI of the metadata document.</summary>
        /// <param name="serviceDocumentUri">The URI of the service document.</param>
        internal void SetServiceDocumentUri(Uri serviceDocumentUri)
        {
            this.ODataUri.ServiceRoot = serviceDocumentUri;
        }

        /// <summary>
        /// Determines if there is a JSON padding function defined.
        /// </summary>
        /// <returns>True if the JsonPCallback property is not null or emtpy.</returns>
        internal bool HasJsonPaddingFunction()
        {
            return !string.IsNullOrEmpty(this.JsonPCallback);
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be writen, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be writen, false otherwise.</returns>
        internal bool ShouldSkipAnnotation(string annotationName)
        {
            return this.ShouldIncludeAnnotation == null || !this.ShouldIncludeAnnotation(annotationName);
        }
    }
}
