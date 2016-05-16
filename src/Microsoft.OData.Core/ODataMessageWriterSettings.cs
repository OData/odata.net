//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using Microsoft.OData.UriParser;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for OData message writers.
    /// </summary>
    public sealed class ODataMessageWriterSettings : IMessageValidationSetting
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
        /// The base uri used in payload.
        /// </summary>
        private Uri baseUri;

        /// <summary>
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// fields and uses the default values for the respective format. If null is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        private ODataFormat format;

        /// <summary>Quotas to use for limiting resource consumption when writing an OData message.</summary>
        private ODataMessageQuotas messageQuotas;

        /// <summary>
        /// The parse result of request Uri
        /// </summary>
        private ODataUri odataUri;

        /// <summary>
        /// Func to evaluate whether an annotation should be writen by the writer. The func should return true if the annotation should
        /// be writen and false if the annotation should be skipped.
        /// </summary>
        private Func<string, bool> shouldIncludeAnnotation;

        /// <summary>
        /// true if the Format property should be used to compute the media type; 
        /// false if AcceptableMediaTypes and AcceptableCharsets should be used.
        /// null if neither the format nor the acceptable media types/charsets have been set.
        /// </summary>
        private bool? useFormat;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageWriterSettings" /> class with default settings. </summary>
        public ODataMessageWriterSettings()
        {
            this.AllowDuplicatePropertyNames = false;
            this.AllowNullValuesForNonNullablePrimitiveTypes = false;
            this.AutoComputePayloadMetadataInJson = false;
            this.DisableMessageStreamDisposal = false;
            this.EnableCharactersCheck = false;
            this.EnableFullValidation = true;
            this.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.SupportUndeclaredValueProperty;
            this.EnableIndentation = false;
            this.ODataSimplified = false;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageWriterSettings" /> class with specified settings.</summary>
        /// <param name="other">The specified settings.</param>
        public ODataMessageWriterSettings(ODataMessageWriterSettings other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.acceptCharSets = other.acceptCharSets;
            this.acceptMediaTypes = other.acceptMediaTypes;
            this.AllowDuplicatePropertyNames = other.AllowDuplicatePropertyNames;
            this.AllowNullValuesForNonNullablePrimitiveTypes = other.AllowNullValuesForNonNullablePrimitiveTypes;
            this.AutoComputePayloadMetadataInJson = other.AutoComputePayloadMetadataInJson;
            this.BaseUri = other.BaseUri;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.EnableCharactersCheck = other.EnableCharactersCheck;
            this.EnableFullValidation = other.EnableFullValidation;
            this.UndeclaredPropertyBehaviorKinds = other.UndeclaredPropertyBehaviorKinds;
            this.EnableIndentation = other.EnableIndentation;
            this.format = other.format;
            this.JsonPCallback = other.JsonPCallback;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.ODataUri = other.ODataUri;
            this.ODataSimplified = other.ODataSimplified;
            this.shouldIncludeAnnotation = other.shouldIncludeAnnotation;
            this.UseKeyAsSegment = other.UseKeyAsSegment;
            this.useFormat = other.useFormat;
            this.Version = other.Version;
        }

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        /// <remarks>
        /// Independently of this setting duplicate property names are never allowed if one of the duplicate property names refers to
        /// a named stream property, an association link or a collection.
        /// </remarks>
        public bool AllowDuplicatePropertyNames { get; set; }

        /// <summary>
        /// If set to true, the writers will allow writing null values even if the metadata specifies a non-nullable primitive type. Default to 'false'
        /// </summary>
        public bool AllowNullValuesForNonNullablePrimitiveTypes { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the writer should automatically generate or omit metadata in JSON payloads based on the metadata level.
        /// </summary>
        /// <remarks>
        /// Payload metadata includes the type names of entries and property values as well as any information that may be computed automatically, such as edit links.
        /// If, for example, ODataResource.EditLink is not specified, then it will be automatically computed and written out in full metadata mode.
        /// If ODataResource.EditLink is specified, then that value will be considered an "override" of the default computed edit link, and will be written out in full and minimal metadata modes. It will not be written in no metadata mode.
        /// </remarks>
        public bool AutoComputePayloadMetadataInJson { get; set; }

        /// <summary>Gets or sets the document base URI which is used as base for all relative URIs. </summary>
        /// <returns>The document base URI which is used as base for all relative URIs.</returns>
        /// <remarks>
        /// This URI will be used in ATOM format only, it would be shown in &lt;xml:base /&gt; element, for JSON payload, base URI is context URI
        /// If the URI does not end with a slash, a slash would be appended automatically.
        /// </remarks>
        public Uri BaseUri
        {
            get
            {
                return this.baseUri;
            }

            set
            {
                this.baseUri = UriUtils.EnsureTaillingSlash(value);
            }
        }

        /// <summary>Gets or sets a value that indicates whether the message stream will not be disposed after finishing writing with the message.</summary>
        /// <returns>true if the message stream will not be disposed after finishing writing with the message; otherwise false. The default value is false.</returns>
        public bool DisableMessageStreamDisposal { get; set; }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public bool EnableCharactersCheck { get; set; }

        /// <summary>
        /// If set to true, all the validation would be enabled. Else some validation will be skipped.
        /// Default to true.
        /// </summary>
        public bool EnableFullValidation { get; set; }

        /// <summary>
        /// Gets or sets UndeclaredPropertyBehaviorKinds.
        /// </summary>
        public ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds { get; set; }

        /// <summary>
        /// Flag to control whether the writer should use indentation or not.
        /// </summary>
        public bool EnableIndentation { get; set; }

        /// <summary>Gets or sets a callback function use to wrap the response from server.</summary>
        /// <returns>The callback function used to wrap the response from server.</returns>
        /// <remarks>If it has a value and we are writing a JSON response, then we will wrap the entirety of the response in
        /// the provided function name and parenthesis for JSONP. Otherwise this value is ignored.</remarks>
        public string JsonPCallback { get; set; }

        /// <summary>
        /// Quotas to use for limiting resource consumption when writing an OData message.
        /// </summary>
        public ODataMessageQuotas MessageQuotas
        {
            get
            {
                if (this.messageQuotas == null)
                {
                    this.messageQuotas = new ODataMessageQuotas();
                }

                return this.messageQuotas;
            }

            set
            {
                this.messageQuotas = value;
            }
        }

        /// <summary>
        /// Whether OData Simplified is enabled.
        /// </summary>
        public bool ODataSimplified { get; set; }

        /// <summary>
        /// The OData Uri of an incoming request.  Call <see cref="ODataUriParser"/>'s methods,
        /// and assign properties (e.g., <see cref="ODataPath"/>) to <see cref="ODataUri"/>.
        /// </summary>
        public ODataUri ODataUri
        {
            get { return this.odataUri ?? (this.odataUri = new ODataUri()); }
            set { this.odataUri = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the writer should put key values in their own URI segment when automatically building URIs.
        /// If this value is false, automatically-generated URLs will take the form "../EntitySet('KeyValue')/..".
        /// If this value is true, automatically-generated URLs will take the form "../EntitySet/KeyValue/..".
        /// If this value is not set (null), decision will be made based on the "Com.Microsoft.OData.Service.Conventions.V1.UrlConventions" vocabulary
        /// annotation on the IEdmEntityContainer, if available. The default behavior is to put key values inside parentheses and not a distinct URL segments.
        /// This setting only applies to URLs that are automatically generated by the <see cref="ODataMessageWriter" /> and does not modify URLs explicitly provided by the user.
        /// </summary>
        public bool? UseKeyAsSegment { get; set; }

        /// <summary>Gets or sets the OData protocol version to be used for writing payloads. </summary>
        /// <returns>The OData protocol version to be used for writing payloads.</returns>
        public ODataVersion? Version { get; set; }

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

        /// <summary>Sets the acceptable media types and character sets from which the content type will be computed when writing the payload.</summary>
        /// <param name="acceptableMediaTypes">The acceptable media types used to determine the content type of the message. This is a comma separated list of content types as specified in RFC 2616, Section 14.1.</param>
        /// <param name="acceptableCharSets"> The acceptable charsets to use to determine the encoding of the message. This is a comma separated list of charsets as specified in RFC 2616, Section 14.2 </param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(string acceptableMediaTypes, string acceptableCharSets)
        {
            // Should accept json as application/json
            this.acceptMediaTypes = string.Equals(acceptableMediaTypes, MimeConstants.MimeJsonSubType, StringComparison.OrdinalIgnoreCase) ? MimeConstants.MimeApplicationJson : acceptableMediaTypes;
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
