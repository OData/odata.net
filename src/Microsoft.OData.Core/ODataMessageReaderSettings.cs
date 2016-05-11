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

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageReaderSettings" /> class with default values.</summary>
        public ODataMessageReaderSettings()
        {
            this.AllowDuplicatePropertyNames = false;
            this.ClientCustomTypeResolver = null;
            this.DisablePrimitiveTypeConversion = false;
            this.DisableMessageStreamDisposal = false;
            this.EnableCharactersCheck = false;
            this.EnableFullValidation = true;
            this.EnableLaxMetadataValidation = false;
            this.EnableReadingEntryContentInEntryStartState = true;
            this.ODataSimplified = false;
            this.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.None;
            this.MaxProtocolVersion = ODataConstants.ODataDefaultProtocolVersion;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.OData.ODataMessageReaderSettings" /> class.</summary>
        /// <param name="other">The other message reader settings.</param>
        public ODataMessageReaderSettings(ODataMessageReaderSettings other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.AllowDuplicatePropertyNames = other.AllowDuplicatePropertyNames;
            this.BaseUri = other.BaseUri;
            this.ClientCustomTypeResolver = other.ClientCustomTypeResolver;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.DisablePrimitiveTypeConversion = other.DisablePrimitiveTypeConversion;
            this.EnableCharactersCheck = other.EnableCharactersCheck;
            this.EnableFullValidation = other.EnableFullValidation;
            this.EnableLaxMetadataValidation = other.EnableLaxMetadataValidation;
            this.EnableReadingEntryContentInEntryStartState = other.EnableReadingEntryContentInEntryStartState;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.MaxProtocolVersion = other.MaxProtocolVersion;
            this.ODataSimplified = other.ODataSimplified;
            this.ShouldIncludeAnnotation = other.ShouldIncludeAnnotation;
            this.UndeclaredPropertyBehaviorKinds = other.UndeclaredPropertyBehaviorKinds;
            this.UseKeyAsSegment = other.UseKeyAsSegment;
            this.ODataSimplified = other.ODataSimplified;
        }

        /// <summary>
        /// If set to true, allows the writers to write duplicate properties of entries and 
        /// complex values (i.e., properties that have the same name). Defaults to 'false'.
        /// </summary>
        /// <remarks>
        /// Independently of this setting duplicate property names are never allowed if one 
        /// of the duplicate property names refers to a named stream property, 
        /// an association link or a collection.
        /// </remarks>
        public bool AllowDuplicatePropertyNames { get; set; }

        // TODO: [Breaking V7.0] Refactor validator: AllowDuplicatePropertyNames conflict with EnableFullValidation now.

        /// <summary>
        /// Gets or sets the document base URI (used as base for all relative URIs). If this is set, it must be an absolute URI.
        /// ODataMessageReaderSettings.BaseUri may be deprecated in the furture, please use ODataMessageReaderSettings.baseUri instead.
        /// </summary>
        /// <returns>The base URI used in payload.</returns>
        /// <remarks>
        /// This URI will be used in ATOM format only, it would overrided by &lt;xml:base /&gt; element in ATOM payload.
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
        /// Custom type resolver used by the Client.
        /// </summary>
        public Func<IEdmType, string, IEdmType> ClientCustomTypeResolver { get; set; }

        /// <summary>Gets or sets a value that indicates whether not to convert all primitive values to the type specified in the model or provided as an expected type. Note that values will still be converted to the type specified in the payload itself.</summary>
        /// <returns>true if primitive values and report values are not converted; false if all primitive values are converted to the type specified in the model or provided as an expected type. The default value is false.</returns>
        public bool DisablePrimitiveTypeConversion { get; set; }

        /// <summary>Gets or sets a value that indicates whether the message stream will not be disposed after finishing writing with the message.</summary>
        /// <returns>true if the message stream will not be disposed after finishing writing with the message; otherwise false. The default value is false.</returns>
        public bool DisableMessageStreamDisposal { get; set; }

        /// <summary>
        /// Flag to control whether the reader should check for valid Xml characters or not.
        /// </summary>
        public bool EnableCharactersCheck { get; set; }

        /// <summary>
        /// If set to true, all the validation would be enabled. Else some validation will be skipped.
        /// Default to true.
        /// </summary>
        public bool EnableFullValidation { get; set; }

        /// <summary>
        /// false - metadata validation is strict, the input must exactly match against the model.
        /// true - metadata validation is lax, the input doesn't have to match the model in all cases.
        /// This property has effect only if the metadata model is specified.
        /// </summary>
        /// <remarks>
        /// Strict metadata validation:
        ///   Primitive values: The wire type must be convertible to the expected type.
        ///   Complex values: The wire type must resolve against the model and it must exactly match the expected type.
        ///   Entities: The wire type must resolve against the model and it must be assignable to the expected type.
        ///   Collections: The wire type must exactly match the expected type.
        ///   If no expected type is available we use the payload type.
        /// Lax metadata validation:
        ///   Primitive values: If expected type is available, we ignore the wire type.
        ///   Complex values: The wire type is used if the model defines it. If the model doesn't define such a type, the expected type is used.
        ///     If the wire type is not equal to the expected type, but it's assignable, we fail because we don't support complex type inheritance.
        ///     If the wire type if not assignable we use the expected type.
        ///   Entities: same as complex values except that if the payload type is assignable we use the payload type. This allows derived entity types.
        ///   Collections: If expected type is available, we ignore the wire type, except we fail if the item type is a derived complex type.
        ///   If no expected type is available we use the payload type and it must resolve against the model.
        /// If DisablePrimitiveTypeConversion is on, the rules for primitive values don't apply
        ///   and the primitive values are always read with the type from the wire.
        /// </remarks>
        public bool EnableLaxMetadataValidation { get; set; }

        /// <summary>
        /// Whether reader read the entry content in entry start state. The default value is false
        /// </summary>
        public bool EnableReadingEntryContentInEntryStartState { get; set; }

        /// <summary>Gets or sets the maximum OData protocol version the reader should accept and understand.</summary>
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
        /// Whether OData Simplified is enabled.
        /// </summary>
        public bool ODataSimplified { get; set; }

        /// <summary>
        /// Func to evaluate whether an annotation should be read or skipped by the reader. The func should return true if the annotation should
        /// be read and false if the annotation should be skipped. A null value indicates that all annotations should be skipped.
        /// </summary>
        public Func<string, bool> ShouldIncludeAnnotation { get; set; }

        /// <summary>Gets or sets the behavior the reader should use when it finds undeclared property.</summary>
        /// <returns>The behavior the reader should use when it finds undeclared property.</returns>
        /// <remarks>
        /// This setting has no effect if there's no model specified for the reader.
        /// This setting must be set to Default when reading request payloads.
        ///
        /// Detailed behavior description:
        /// ODataUndeclaredPropertyBehaviorKind.Default
        ///   If an undeclared property is found reading fails.
        ///
        /// ODataUndeclaredPropertyBehaviorKind.ReportUndeclaredLinkProperty
        ///   ATOM
        ///     - Undeclared deferred nested resource info will be read and reported.
        ///     - Undeclared expanded nested resource info will fail.
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties fail.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - the link will be read and reported as a deferred nested resource info.
        ///       - __mediaresource value is found - the link will be read and reported as a stream property
        ///       - If nothing from the above matches the reading fails.
        ///     - Undeclared association links inside __metadata/properties will be read and reported.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and no value - it will be read as navigation/association link
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' on it and no value
        ///             - it will be read as a stream property.
        ///       - Any other property (that is property with a value or property with no annotation mentioned above) will fail.
        ///
        /// ODataUndeclaredPropertyBehaviorKind.IgnoreUndeclaredValueProperty
        ///   ATOM
        ///     - Undeclared property inside m:properties is ignored (not even read).
        ///     - Undeclared nested resource info, stream property link or association link fail.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - fail as undeclared deferred nav. link.
        ///       - __mediaresource value is found - fail as undeclared stream property.
        ///       - All other properties are ignored and not read.
        ///     - Undeclared association links inside __metadata/properties fail.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it (deferred or expanded nested resource info)
        ///             - fail as undeclared navigation property
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' on it and no value
        ///             - fail as undeclared stream property.
        ///       - The property has a value and no annotation mentioned above - the property is ignored and not read.
        ///
        /// ODataUndeclaredPropertyBehaviorKind.ReportUndeclaredLinkProperty | ODataUndeclaredPropertyBehaviorKind.IgnoreUndeclaredValueProperty
        ///   ATOM
        ///     - Undeclared deferred nested resource info will be read and reported.
        ///     - Undeclared expanded nested resource info will be read and the nested resource info part will be reported,
        ///       the expanded content will be ignored and not read or reported.
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties will be ignored and not read.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - read and report a deferred nested resource info.
        ///       - __mediaresource value is found - read and report stream property.
        ///       - All other properties are ignore and not read.
        ///     - Undeclared association links inside __metadata/properties are read and reported.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and no value (deferred nested resource info)
        ///             - it will be read as navigation/association link
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and with value (expanded nested resource info)
        ///             - it will be read, the navigation and association link will be reported and the content will be ignored.
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaEtag' on it and no value
        ///             - it will be read as a stream property.
        ///       - The property has a value and no annotation mentioned above - the property is ignored and not read.
        ///
        ///   Note that there's one difference between ATOM/JSON Light and Verbose JSON. In ATOM and JSON Light expanded links are treated as both
        ///   undeclared link and a value property. The URLs are the link part, the expanded content is the value part.
        ///   In Verbose JSON expanded links are treated as a value property as a whole. Since in JSON expanded links don't actually have
        ///   the link part (the payload doesn't contain the "href") this is not such a big difference.
        /// </remarks>
        public ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the reader should put key values in their own URI segment when automatically building URIs.
        /// If this value is false, automatically-generated URLs will take the form "../EntitySet('KeyValue')/..".
        /// If this value is true, automatically-generated URLs will take the form "../EntitySet/KeyValue/..".
        /// If this value is not set (null), decision will be made based on the "Com.Microsoft.OData.Service.Conventions.V1.UrlConventions" vocabulary
        /// annotation on the IEdmEntityContainer, if available. The default behavior is to put key values inside parentheses and not a distinct URL segments.
        /// This setting only applies to URLs that are automatically generated by the <see cref="ODataMessageReader" /> and the URLs explicitly provided by the server won't be modified.
        /// </summary>
        public bool? UseKeyAsSegment { get; set; }

        /// <summary>
        /// The reader behavior that holds all the knobs needed to make the reader
        /// behave differently inside and outside of WCF Data Services.
        /// Whether or not to ignore any undeclared value properties in the payload. Computed from the UndeclaredPropertyBehaviorKinds enum property.
        /// </summary>
        internal bool IgnoreUndeclaredValueProperties
        {
            get
            {
                return this.UndeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.IgnoreUndeclaredValueProperty);
            }
        }

        /// <summary>
        /// Whether or not to report any undeclared link properties in the payload. Computed from the UndeclaredPropertyBehaviorKinds enum property.
        /// </summary>
        internal bool ReportUndeclaredLinkProperties
        {
            get
            {
                return this.UndeclaredPropertyBehaviorKinds.HasFlag(ODataUndeclaredPropertyBehaviorKinds.ReportUndeclaredLinkProperty);
            }
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
    }
}
