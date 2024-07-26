//---------------------------------------------------------------------
// <copyright file="ODataMessageWriterSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Evaluation;

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OData.Buffers;
    using Microsoft.OData.UriParser;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for OData message writers.
    /// </summary>
    public sealed class ODataMessageWriterSettings
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
        /// true if the Format property should be used to compute the media type;
        /// false if AcceptableMediaTypes and AcceptableCharsets should be used.
        /// null if neither the format nor the acceptable media types/charsets have been set.
        /// </summary>
        private bool? useFormat;

        /// <summary>
        /// Validation settings.
        /// </summary>
        private ValidationKinds validations;

        /// <summary>
        /// Default setting for writing control information without the 'odata' prefix.
        /// </summary>
        private bool enableWritingODataAnnotationWithoutPrefix;

        /// <summary>
        /// OData 4.0-specific setting for writing control information without the 'odata' prefix.
        /// </summary>
        private bool omitODataPrefix40 = false;

        /// <summary>
        /// OData 4.01 and greater setting for writing control information without the 'odata' prefix.
        /// </summary>
        private bool omitODataPrefix = true;

        /// <summary>Initializes a new instance of the <see cref="Microsoft.OData.ODataMessageWriterSettings" /> class with default settings.</summary>
        public ODataMessageWriterSettings()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Microsoft.OData.ODataMessageWriterSettings" /> class with default settings for the
        /// specified OData version.
        /// </summary>
        /// <param name="version">OData version for which to create default settings.</param>
        public ODataMessageWriterSettings(ODataVersion? version)
        {
            this.EnableMessageStreamDisposal = true;
            this.EnableCharactersCheck = false;
            this.Validations = ValidationKinds.All;
            this.Validator = new WriterValidator(this);
            this.LibraryCompatibility = ODataLibraryCompatibility.None;
            this.MultipartNewLine = "\r\n";
            this.AlwaysAddTypeAnnotationsForDerivedTypes = false;
            this.BufferSize = ODataConstants.DefaultOutputBufferSize;
            this.EnableWritingKeyAsSegment = false;
            this.Version = version;

            if (version == null || version < ODataVersion.V401)
            {
                this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix40;
            }
            else
            {
                this.enableWritingODataAnnotationWithoutPrefix = this.omitODataPrefix;
            }
        }

        /// <summary>
        /// Gets or sets library compatibility version. Default value is <see cref="Microsoft.OData.ODataLibraryCompatibility.None"/>,
        /// </summary>
        public ODataLibraryCompatibility LibraryCompatibility { get; set; }

        /// <summary>
        /// Gets or sets validations to perform. Default value is <see cref="Microsoft.OData.ValidationKinds.All"/>,
        /// </summary>
        public ValidationKinds Validations
        {
            get
            {
                return validations;
            }

            set
            {
                validations = value;
                ThrowIfTypeConflictsWithMetadata = (validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0;
                ThrowOnDuplicatePropertyNames = (validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0;
                ThrowOnUndeclaredPropertyForNonOpenType = (validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0;
            }
        }

        /// <summary>Gets or sets the document base URI which is used as base for all relative URIs. </summary>
        /// <returns>The document base URI which is used as base for all relative URIs.</returns>
        /// <remarks>
        /// Base URI is context URI; if the URI does not end with a slash, a slash would be appended automatically.
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

        /// <summary>Gets or sets a value that indicates whether the message stream will be disposed after finishing writing with the message.</summary>
        /// <returns>true if the message stream will be disposed after finishing writing with the message; otherwise false. The default value is true.</returns>
        public bool EnableMessageStreamDisposal { get; set; }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public bool EnableCharactersCheck { get; set; }

        /// <summary>Gets or sets a callback function use to wrap the response from server.</summary>
        /// <returns>The callback function used to wrap the response from server.</returns>
        /// <remarks>If it has a value and we are writing a JSON response, then we will wrap the entirety of the response in
        /// the provided function name and parenthesis for JSONP. Otherwise this value is ignored.</remarks>
        [Obsolete("This will be dropped in the 9.x release.")]
        public string JsonPCallback { get; set; }

        /// <summary>
        /// Get/sets the character buffer pool.
        /// </summary>
        public ICharArrayPool ArrayPool { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer used to buffer writes to the output
        /// stream. Note that this is a hint and may be disregarded where deemed fit.
        /// </summary>
        public int BufferSize { get; set; }

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
        /// The OData Uri of an incoming request.  Call <see cref="ODataUriParser"/>'s methods,
        /// and assign properties (e.g., <see cref="ODataPath"/>) to <see cref="ODataUri"/>.
        /// </summary>
        public ODataUri ODataUri
        {
            get { return this.odataUri ?? (this.odataUri = new ODataUri()); }
            set { this.odataUri = value; }
        }

        /// <summary>Gets or sets the OData protocol version to be used for writing payloads. </summary>
        /// <returns>The OData protocol version to be used for writing payloads.</returns>
        public ODataVersion? Version { get; set; }

        /// <summary>
        /// Informs the metadata builder which properties, functions, actions, links to omit.
        /// </summary>
        public ODataMetadataSelector MetadataSelector { get; set; }

        /// <summary>
        /// Gets or sets the new line character sequence used when writing multipart messages
        /// see https://tools.ietf.org/html/rfc2046#section-5.1.1
        /// A TextWriter uses OS specific newline but rfc2046 requires it to be CRLF.
        /// </summary>
        public string MultipartNewLine { get; set; }

        /// <summary>
        /// When set, type annotations will be added for derived types, even when the metadata level is set to "None".
        /// </summary>
        public bool AlwaysAddTypeAnnotationsForDerivedTypes { get; set; }

        /// <summary>
        /// Func to evaluate whether an annotation should be written by the writer. This is useful when you want to force the writer
        /// to write annotations that would have otherwise been skipped (e.g. writing annotations that are not part of the odata.include-annotations filter).
        /// </summary>
        /// <remarks>
        /// Note that this returning false does not guarantee that the annotation will not be written. For example, if an annotation was included in the preference
        /// header annotations filter, it may still be written even if this func returns false.
        /// </remarks>
        public Func<string, bool> ShouldIncludeAnnotation { get; set; }

        /// <summary>
        /// Gets the validator corresponding to the validation settings.
        /// </summary>
        internal IWriterValidator Validator { get; private set; }

        /// <summary>
        /// Returns whether ThrowIfTypeConflictsWithMetadata validation should be performed.
        /// </summary>
        internal bool ThrowIfTypeConflictsWithMetadata { get; private set; }

        /// <summary>
        /// Returns whether ThrowOnDuplicatePropertyNames validation setting is enabled.
        /// </summary>
        internal bool ThrowOnDuplicatePropertyNames { get; private set; }

        /// <summary>
        /// Returns whether ThrowOnUndeclaredPropertyForNonOpenType validation setting is enabled.
        /// </summary>
        internal bool ThrowOnUndeclaredPropertyForNonOpenType { get; private set; }

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
        /// isIEEE754Compatible is used to determine if the message follows IEEE 754 standard.
        /// If this is set to true then long and decimals will be serialized as strings.
        /// </summary>
        internal bool IsIeee754Compatible { get; set; }

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
                    : new SelectedPropertiesNode(SelectedPropertiesNode.SelectionType.EntireSubtree);
            }
        }

        /// <summary>
        /// Func to evaluate whether an annotation should be written by the writer based on the annotations filtered in the odata.include-annotations header and OData standard.
        /// The func should return true if the annotation should
        /// be written and false if the annotation should be skipped.
        /// </summary>
        /// <remarks>
        /// This property is internal and automatically set based on annotation filters. The developer can use the <see cref="ShouldIncludeAnnotation"/> property
        /// to tell the writer to write annotations even if they were not included in the annotations filters.
        /// </remarks>
        internal Func<string, bool> ShouldIncludeAnnotationInternal { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the writer should put key values in their own URI segment when automatically building URIs.
        /// If this value is false, automatically-generated URLs will take the form "../EntitySet('KeyValue')/..".
        /// If this value is true, automatically-generated URLs will take the form "../EntitySet/KeyValue/..".
        /// This setting only applies to URLs that are automatically generated by the <see cref="ODataMessageWriter" /> and does not modify URLs explicitly provided by the user.
        /// </summary>
        public bool EnableWritingKeyAsSegment { get; set; }

        /// <summary>
        /// Get whether to write OData control information without the 'odata' prefix.
        /// The default value is false for OData 4.0 and true for OData 4.01.
        /// </summary>
        /// <returns>true if control information should be written with the 'odata' prefix, otherwise false.</returns>
        public bool GetOmitODataPrefix()
        {
            return this.enableWritingODataAnnotationWithoutPrefix;
        }

        /// <summary>
        /// Returns a value indicating whether control information should be written with the 'odata' prefix for the specified OData version.
        /// The default value is false for OData 4.0 and true for OData 4.01.
        /// </summary>
        /// <param name="version">The OData version.</param>
        /// <returns>true if control information should be written with the 'odata' prefix, otherwise false</returns>
        public bool GetOmitODataPrefix(ODataVersion version)
        {
            if (version >= ODataVersion.V401)
            {
                return this.omitODataPrefix;
            }

            return this.omitODataPrefix40;
        }

        /// <summary>
        /// Sets a value indicating whether control information should be written with or without the 'odata' prefix.
        /// </summary>
        /// <remarks>
        /// This method updates the setting for both OData version 4.0 and 4.01. If you want to target a specific version instead,
        /// use the <see cref="SetOmitODataPrefix(bool, ODataVersion)"/> overload.
        /// </remarks>
        /// <param name="value">true to write control information with the 'odata' prefix, otherwise false.</param>
        public void SetOmitODataPrefix(bool value)
        {
            this.enableWritingODataAnnotationWithoutPrefix =
            this.omitODataPrefix =
            this.omitODataPrefix40 = value;
        }

        /// <summary>
        /// Sets a value indicating whether control information should be written with or without the 'odata' prefix for the specified OData version
        /// </summary>
        /// <param name="value">true to write control information with the 'odata' prefix, otherwise false.</param>
        /// <param name="version">The OData version for which to set the omit prefix behavior.</param>
        public void SetOmitODataPrefix(bool value, ODataVersion version)
        {
            if (version == ODataVersion.V4)
            {
                this.omitODataPrefix40 = value;
            }
            else
            {
                this.omitODataPrefix = value;
            }
        }

        /// <summary>
        /// Creates a shallow copy of this <see cref="ODataMessageWriterSettings"/>.
        /// </summary>
        /// <returns>A shallow copy of this <see cref="ODataMessageWriterSettings"/>.</returns>
        public ODataMessageWriterSettings Clone()
        {
            var copy = new ODataMessageWriterSettings();
            copy.CopyFrom(this);
            return copy;
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

        internal static ODataMessageWriterSettings CreateWriterSettings(
            IServiceProvider container,
            ODataMessageWriterSettings other)
        {
            ODataMessageWriterSettings writerSettings;
            if (container == null)
            {
                writerSettings = new ODataMessageWriterSettings();
            }
            else
            {
                writerSettings = container.GetRequiredService<ODataMessageWriterSettings>();
            }

            if (other != null)
            {
                writerSettings.CopyFrom(other);
            }

            return writerSettings;
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
        /// <returns>True if the JsonPCallback property is not null or empty.</returns>
        internal bool HasJsonPaddingFunction()
        {
            return !string.IsNullOrEmpty(this.JsonPCallback);
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be written, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be written, false otherwise.</returns>
        internal bool ShouldSkipAnnotation(string annotationName)
        {
            bool skipAnnotation = this.ShouldIncludeAnnotationInternal == null || !this.ShouldIncludeAnnotationInternal(annotationName);
            // if annotation is not included by default, the caller ensure it's written using ShouldIncludeAnnotation
            if (skipAnnotation && this.ShouldIncludeAnnotation != null)
            {
                return !this.ShouldIncludeAnnotation(annotationName);
            }

            return skipAnnotation;
        }

        private void CopyFrom(ODataMessageWriterSettings other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.acceptCharSets = other.acceptCharSets;
            this.acceptMediaTypes = other.acceptMediaTypes;
            this.BaseUri = other.BaseUri;
            this.EnableMessageStreamDisposal = other.EnableMessageStreamDisposal;
            this.EnableCharactersCheck = other.EnableCharactersCheck;
            this.format = other.format;
            this.JsonPCallback = other.JsonPCallback;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.ODataUri = other.ODataUri.Clone();
            this.ShouldIncludeAnnotationInternal = other.ShouldIncludeAnnotationInternal;
            this.ShouldIncludeAnnotation = other.ShouldIncludeAnnotation;
            this.useFormat = other.useFormat;
            this.Version = other.Version;
            this.LibraryCompatibility = other.LibraryCompatibility;
            this.AlwaysAddTypeAnnotationsForDerivedTypes = other.AlwaysAddTypeAnnotationsForDerivedTypes;
            this.MetadataSelector = other.MetadataSelector;
            this.IsIeee754Compatible = other.IsIeee754Compatible;

            this.validations = other.validations;
            this.ThrowIfTypeConflictsWithMetadata = other.ThrowIfTypeConflictsWithMetadata;
            this.ThrowOnDuplicatePropertyNames = other.ThrowOnDuplicatePropertyNames;
            this.ThrowOnUndeclaredPropertyForNonOpenType = other.ThrowOnUndeclaredPropertyForNonOpenType;
            this.ArrayPool = other.ArrayPool;
            this.BufferSize = other.BufferSize;
            this.EnableWritingKeyAsSegment = other.EnableWritingKeyAsSegment;
            this.enableWritingODataAnnotationWithoutPrefix = other.enableWritingODataAnnotationWithoutPrefix;
            this.omitODataPrefix40 = other.omitODataPrefix40;
            this.omitODataPrefix = other.omitODataPrefix;
        }
    }
}
