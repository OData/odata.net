//---------------------------------------------------------------------
// <copyright file="ODataMessageReaderSettings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Configuration settings for OData message readers.
    /// </summary>
    public sealed class ODataMessageReaderSettings
    {
        /// <summary>
        /// The base uri used in payload.
        /// </summary>
        private Uri baseUri;

        /// <summary> Quotas to use for limiting resource consumption when reading an OData message. </summary>
        private ODataMessageQuotas messageQuotas;

        /// <summary>
        /// Validation settings.
        /// </summary>
        private ValidationKinds validations;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageReaderSettings" /> class
        /// with default values for OData 4.0.</summary>
        public ODataMessageReaderSettings() : this(ODataConstants.ODataDefaultProtocolVersion)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageReaderSettings" /> class
        /// with default values for the specified OData version.</summary>
        /// <param name="odataVersion">OData Version for which to create default settings.</param>
        public ODataMessageReaderSettings(ODataVersion odataVersion)
        {
            this.ClientCustomTypeResolver = null;
            this.PrimitiveTypeResolver = null;
            this.EnablePrimitiveTypeConversion = true;
            this.EnableMessageStreamDisposal = true;
            this.EnableCharactersCheck = false;
            this.Version = odataVersion;

            Validator = new ReaderValidator(this);
            if (odataVersion < ODataVersion.V401)
            {
                Validations = ValidationKinds.All;
                this.ReadUntypedAsString = true;
                this.MaxProtocolVersion = ODataConstants.ODataDefaultProtocolVersion;
            }
            else
            {
                Validations = ValidationKinds.All & ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
                this.ReadUntypedAsString = false;
                this.MaxProtocolVersion = odataVersion;
            }
        }

        /// <summary>Gets or sets the OData protocol version to be used for reading payloads. </summary>
        /// <returns>The OData protocol version to be used for reading payloads.</returns>
        public ODataVersion? Version { get; set; }

        /// <summary>
        /// Gets or sets validation settings.
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
                ThrowOnDuplicatePropertyNames = (validations & ValidationKinds.ThrowOnDuplicatePropertyNames) != 0;
                ThrowIfTypeConflictsWithMetadata = (validations & ValidationKinds.ThrowIfTypeConflictsWithMetadata) != 0;
                ThrowOnUndeclaredPropertyForNonOpenType = (validations & ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType) != 0;
            }
        }

        /// <summary>
        /// Gets or sets the document base URI (used as base for all relative URIs). If this is set, it must be an absolute URI.
        /// ODataMessageReaderSettings.BaseUri may be deprecated in the future, please use ODataMessageReaderSettings.baseUri instead.
        /// </summary>
        /// <returns>The base URI used in payload.</returns>
        /// <remarks>
        /// This URI will be used in ATOM format only, it would be overridden by &lt;xml:base /&gt; element in ATOM payload.
        /// If the URI does not end with a slash, a slash would be appended automatically.
        /// </remarks>
        public Uri BaseUri
        {
            get
            {
                return baseUri;
            }

            set
            {
                this.baseUri = UriUtils.EnsureTaillingSlash(value);
            }
        }

        /// <summary>
        /// Gets or sets custom type resolver used by the Client.
        /// </summary>
        public Func<IEdmType, string, IEdmType> ClientCustomTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets a custom resolver for resolving untyped primitive values
        /// </summary>
        public Func<object, string, IEdmTypeReference> PrimitiveTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to convert all primitive values to the type specified in the model or provided as an expected type. Note that values will still be converted to the type specified in the payload itself.
        /// </summary>
        /// <returns>false if primitive values and report values are not converted; true if all primitive values are converted to the type specified in the model or provided as an expected type. The default value is true.</returns>
        public bool EnablePrimitiveTypeConversion { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the message stream will be disposed after finishing writing with the message.
        /// </summary>
        /// <returns>true if the message stream will be disposed after finishing writing with the message; otherwise false. The default value is true.</returns>
        public bool EnableMessageStreamDisposal { get; set; }

        /// <summary>
        /// Flag to control whether the reader should check for valid Xml characters or not.
        /// </summary>
        public bool EnableCharactersCheck { get; set; }

        /// <summary>
        /// Gets or sets the maximum OData protocol version the reader should accept and understand.
        /// </summary>
        /// <returns>The maximum OData protocol version the reader should accept and understand.</returns>
        /// <remarks>
        /// If the payload to be read has higher OData-Version than the value specified for this property
        /// the reader will fail.
        /// Reader will also not report features which require higher version than specified for this property.
        /// It may either ignore such features in the payload or fail on them.
        /// </remarks>
        public ODataVersion MaxProtocolVersion { get; set; }

        /// <summary>
        /// Quotas to use for limiting resource consumption when reading an OData message.
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
        /// Whether to read untyped values as a raw string.
        /// </summary>
        public bool ReadUntypedAsString { get; set; }

        /// <summary>
        /// Func to evaluate whether an annotation should be read or skipped by the reader. The func should return true if the annotation should
        /// be read and false if the annotation should be skipped. A null value indicates that all annotations should be skipped.
        /// </summary>
        public Func<string, bool> ShouldIncludeAnnotation { get; set; }

        /// <summary>
        /// Gets the bound validator.
        /// </summary>
        internal IReaderValidator Validator { get; private set; }

        /// <summary>
        /// Returns whether ThrowOnDuplicatePropertyNames validation setting is enabled.
        /// </summary>
        internal bool ThrowOnDuplicatePropertyNames { get; private set; }

        /// <summary>
        /// Returns whether ThrowIfTypeConflictsWithMetadata is enabled.
        /// </summary>
        internal bool ThrowIfTypeConflictsWithMetadata { get; private set; }

        /// <summary>
        /// Returns whether ThrowOnUndeclaredPropertyForNonOpenType validation setting is enabled.
        /// </summary>
        internal bool ThrowOnUndeclaredPropertyForNonOpenType { get; private set; }

        /// <summary>
        /// Creates a shallow copy of this <see cref="ODataMessageReaderSettings"/>.
        /// </summary>
        /// <returns>A shallow copy of this <see cref="ODataMessageReaderSettings"/>.</returns>
        public ODataMessageReaderSettings Clone()
        {
            var copy = new ODataMessageReaderSettings();
            copy.CopyFrom(this);
            return copy;
        }

        internal static ODataMessageReaderSettings CreateReaderSettings(
            IServiceProvider container,
            ODataMessageReaderSettings other)
        {
            ODataMessageReaderSettings readerSettings;
            if (container == null)
            {
                readerSettings = new ODataMessageReaderSettings();
            }
            else
            {
                readerSettings = container.GetRequiredService<ODataMessageReaderSettings>();
            }

            if (other != null)
            {
                readerSettings.CopyFrom(other);
            }

            return readerSettings;
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be skipped, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be skipped, false otherwise.</returns>
        internal bool ShouldSkipAnnotation(string annotationName)
        {
            return this.ShouldIncludeAnnotation == null || !this.ShouldIncludeAnnotation(annotationName);
        }

        private void CopyFrom(ODataMessageReaderSettings other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.BaseUri = other.BaseUri;
            this.ClientCustomTypeResolver = other.ClientCustomTypeResolver;
            this.PrimitiveTypeResolver = other.PrimitiveTypeResolver;
            this.EnableMessageStreamDisposal = other.EnableMessageStreamDisposal;
            this.EnablePrimitiveTypeConversion = other.EnablePrimitiveTypeConversion;
            this.EnableCharactersCheck = other.EnableCharactersCheck;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.MaxProtocolVersion = other.MaxProtocolVersion;
            this.ReadUntypedAsString = other.ReadUntypedAsString;
            this.ShouldIncludeAnnotation = other.ShouldIncludeAnnotation;
            this.validations = other.validations;
            this.ThrowOnDuplicatePropertyNames = other.ThrowOnDuplicatePropertyNames;
            this.ThrowIfTypeConflictsWithMetadata = other.ThrowIfTypeConflictsWithMetadata;
            this.ThrowOnUndeclaredPropertyForNonOpenType = other.ThrowOnUndeclaredPropertyForNonOpenType;
            this.Version = other.Version;
        }
    }
}
