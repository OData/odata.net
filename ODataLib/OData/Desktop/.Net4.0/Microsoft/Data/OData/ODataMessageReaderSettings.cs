//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Microsoft.Data.OData
{
    using System;
    using System.Xml;
    using Microsoft.Data.Edm;

    /// <summary>
    /// Configuration settings for OData message readers.
    /// </summary>
    public sealed class ODataMessageReaderSettings : ODataMessageReaderSettingsBase
    {
        /// <summary>
        /// A instance representing any knobs that control the behavior of the readers
        /// inside and outside of WCF Data Services.
        /// </summary>
        private ODataReaderBehavior readerBehavior;

        /// <summary>
        /// ATOM entry XML customization callback.
        /// </summary>
        private Func<ODataEntry, XmlReader, Uri, XmlReader> atomFormatEntryXmlCustomizationCallback;

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageReaderSettings" /> class with default values.</summary>
        public ODataMessageReaderSettings() 
            : base()
        {
            this.DisablePrimitiveTypeConversion = false;
            this.DisableMessageStreamDisposal = false;
            this.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.None;

            // Create the default reader behavior
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;

            this.MaxProtocolVersion = ODataConstants.ODataDefaultProtocolVersion;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageReaderSettings" /> class.</summary>
        /// <param name="other">The other message reader settings.</param>
        public ODataMessageReaderSettings(ODataMessageReaderSettings other)
            : base(other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.BaseUri = other.BaseUri;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.DisablePrimitiveTypeConversion = other.DisablePrimitiveTypeConversion;
            this.UndeclaredPropertyBehaviorKinds = other.UndeclaredPropertyBehaviorKinds;
            this.MaxProtocolVersion = other.MaxProtocolVersion;
            this.atomFormatEntryXmlCustomizationCallback = other.atomFormatEntryXmlCustomizationCallback;

            // NOTE: reader behavior is immutable; copy by reference is ok.
            this.readerBehavior = other.ReaderBehavior;
        }

        /// <summary>Gets or sets the document base URI (used as base for all relative URIs). If this is set, it must be an absolute URI.</summary>
        /// <returns>The base URI.</returns>
        /// <remarks>
        /// Note that for typical OData services this should end with a slash character. For example "http://services.odata.org/OData/OData.svc/" will work as expected,
        /// that is a relative URI "Products(0)" will correctly combine with the base to produce "http://services.odata.org/OData/OData.svc/Products(0)".
        /// If the URI would not end with a slash, the last segment is not considered when base and relative URIs are combined.
        /// So for example this base URI "http://services.odata.org/OData/OData.svc" combined with relative URI "Products(0)" would produce
        /// "http://services.odata.org/OData/Products(0)", which is typically not the desired result.
        /// </remarks>
        public Uri BaseUri
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value that indicates whether not to convert all primitive values to the type specified in the model or provided as an expected type. Note that values will still be converted to the type specified in the payload itself.</summary>
        /// <returns>true if primitive values and report values are not converted; false if all primitive values are converted to the type specified in the model or provided as an expected type. The default value is false.</returns>
        public bool DisablePrimitiveTypeConversion
        {
            get;
            set;
        }

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
        ///     - Undeclared deferred navigation link will be read and reported.
        ///     - Undeclared expanded navigation link will fail.
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties fail.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - the link will be read and reported as a deferred navigation link.
        ///       - __mediaresource value is found - the link will be read and reported as a stream property
        ///       - If nothing from the above matches the reading fails.
        ///     - Undeclared association links inside __metadata/properties will be read and reported.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and no value - it will be read as navigation/association link
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaETag' on it and no value
        ///             - it will be read as a stream property.
        ///       - Any other property (that is property with a value or property with no annotation mentioned above) will fail.
        ///
        /// ODataUndeclaredPropertyBehaviorKind.IgnoreUndeclaredValueProperty
        ///   ATOM
        ///     - Undeclared property inside m:properties is ignored (not even read).
        ///     - Undeclared navigation link, stream property link or association link fail.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - fail as undeclared deferred nav. link.
        ///       - __mediaresource value is found - fail as undeclared stream property.
        ///       - All other properties are ignored and not read.
        ///     - Undeclared association links inside __metadata/properties fail.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it (deferred or expanded navigation link)
        ///             - fail as undeclared navigation property
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaETag' on it and no value
        ///             - fail as undeclared stream property.
        ///       - The property has a value and no annotation mentioned above - the property is ignored and not read.
        ///
        /// ODataUndeclaredPropertyBehaviorKind.ReportUndeclaredLinkProperty | ODataUndeclaredPropertyBehaviorKind.IgnoreUndeclaredValueProperty
        ///   ATOM
        ///     - Undeclared deferred navigation link will be read and reported.
        ///     - Undeclared expanded navigation link will be read and the navigation link part will be reported,
        ///       the expanded content will be ignored and not read or reported.
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties will be ignored and not read.
        ///   Verbose JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - read and report a deferred navigation link.
        ///       - __mediaresource value is found - read and report stream property.
        ///       - All other properties are ignore and not read.
        ///     - Undeclared association links inside __metadata/properties are read and reported.
        ///   JSON Light
        ///     - If an undeclared property is found a detection logic will run:
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and no value (deferred navigation link)
        ///             - it will be read as navigation/association link
        ///       - The property has 'odata.navigationLink' or 'odata.associationLink' annotation on it and with value (expanded navigation link)
        ///             - it will be read, the navigation and association link will be reported and the content will be ignored.
        ///       - The property has 'odata.mediaEditLink', 'odata.mediaReadLink', 'odata.mediaContentType' or 'odata.mediaETag' on it and no value
        ///             - it will be read as a stream property.
        ///       - The property has a value and no annotation mentioned above - the property is ignored and not read.
        ///
        ///   Note that there's one difference between ATOM/JSON Light and Verbose JSON. In ATOM and JSON Light expanded links are treated as both
        ///   undeclared link and a value property. The URLs are the link part, the expanded content is the value part.
        ///   In Verbose JSON expanded links are treated as a value property as a whole. Since in JSON expanded links don't actually have
        ///   the link part (the payload doesn't contain the "href") this is not such a big difference.
        /// </remarks>
        public ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds
        {
            get;
            set;
        }

        /// <summary>Gets or sets a value that indicates whether the message stream will not be disposed after finishing writing with the message.</summary>
        /// <returns>true if the message stream will not be disposed after finishing writing with the message; otherwise false. The default value is false.</returns>
        public bool DisableMessageStreamDisposal
        {
            get; 
            set;
        }

        /// <summary>Gets or sets the maximum OData protocol version the reader should accept and understand.</summary>
        /// <returns>The maximum OData protocol version the reader should accept and understand.</returns>
        /// <remarks>
        /// If the payload to be read has higher DataServiceVersion than the value specified for this property
        /// the reader will fail.
        /// Reader will also not report features which require higher version than specified for this property.
        /// It may either ignore such features in the payload or fail on them.
        /// </remarks>
        public ODataVersion MaxProtocolVersion
        {
            get;
            set;
        }

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
        internal bool DisableStrictMetadataValidation
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.ReaderBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesServer || this.ReaderBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesClient;
            }
        }

        /// <summary>
        /// The reader behavior that holds all the knobs needed to make the reader
        /// behave differently inside and outside of WCF Data Services.
        /// </summary>
        internal ODataReaderBehavior ReaderBehavior
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.readerBehavior;
            }
        }

        /// <summary>
        /// ATOM entry XML customization callback.
        /// </summary>
        internal Func<ODataEntry, XmlReader, Uri, XmlReader> AtomEntryXmlCustomizationCallback
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.atomFormatEntryXmlCustomizationCallback;
            }
        }

        /// <summary>
        /// True if OdataMessgeReaderSettings contain corresponding undeclaredPropertyBehaviorKinds.
        /// </summary>
        /// <param name="undeclaredPropertyBehaviorKinds">The enum value of ODataUndeclaredPropertyBehaviorKinds.</param>
        /// <returns>True if OdataMessgeReaderSettings contain corresponding undeclaredPropertyBehaviorKinds.</returns>
        public bool ContainUndeclaredPropertyBehavior(ODataUndeclaredPropertyBehaviorKinds undeclaredPropertyBehaviorKinds)
        {
            DebugUtils.CheckNoExternalCallers();
            if (undeclaredPropertyBehaviorKinds == ODataUndeclaredPropertyBehaviorKinds.None)
            {
                return this.UndeclaredPropertyBehaviorKinds == ODataUndeclaredPropertyBehaviorKinds.None;
            }

            return this.UndeclaredPropertyBehaviorKinds.HasFlag(undeclaredPropertyBehaviorKinds);
        }

        /// <summary>Sets the atom entry XML customization callback.</summary>
        /// <param name="atomEntryXmlCustomizationCallback">The atom entry XML customization callback.</param>
        /// <remarks>
        /// This method only affects ATOM format payloads. For payloads of other formats this method has no effect.
        /// It takes three parameters:
        ///  - ODataEntry entry - which is the entry to be read.
        ///  - XmlReader reader - which is the current XmlReader used by the ODataReader to read the entry. The reader is positioned on the atom:entry start element tag.
        ///     Note that the reader might not be the exact instance of the reader create by the parent entry customization or passed in by other means to the ODataReader,
        ///     the ODataReader sometimes needs to wrap the readers and the wrapped XmlReader might be passed in here.
        ///  - Uri - the current xml:base URI value for the reader. If there is no active xml:base this parameter is passed a null value.
        /// It returns XmlReader:
        ///  - null - means there's no need for customization and the original XmlReader will be used to read the entry.
        ///  - non-null XmlReader - an XmlReader which the ODataReader will use to read the entry. This reader must be positioned on the atom:entry start element tag.
        ///     The ODataReader will not close or dispose the reader. It will read from it and leave the reader positioned on the atom:entry end element tag
        ///     (or the empty atom:entry start tag).
        ///     Once the ODataReader reports the ODataReaderState.EntryEnd for the entry, it will not use this XmlReader anymore.
        ///     After the ODataReaderState.EntryEnd is reported the parent reader (the parameter to the func) is expected to be positioned on the node AFTER
        ///     the atom:entry end element tag (or after the atom:entry empty start tag).
        ///     Note that this means that the ODataReader will only read till the end tag on the inner reader, but it expects the parent reader to move after the end tag.
        ///     It's the resposibility of the caller to move the parent read after the end tag manually if necessary.
        ///     The func must NOT return the same XmlReader instance as the XmlReader passed to it.
        /// </remarks>
        public void SetAtomEntryXmlCustomizationCallback(Func<ODataEntry, XmlReader, Uri, XmlReader> atomEntryXmlCustomizationCallback)
        {
            this.atomFormatEntryXmlCustomizationCallback = atomEntryXmlCustomizationCallback;
        }

        /// <summary>Enables the default behavior.</summary>
        public void EnableDefaultBehavior()
        {
            // We have to reset the ATOM entry XML customization since in the default behavior no atom entry customization is used.
            this.SetAtomEntryXmlCustomizationCallback(null);
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;
        }

        /// <summary>Specifies whether the WCF data services server behavior is enabled.</summary>
        /// <param name="usesV1Provider">true to use V1 provider; otherwise, false.</param>
        public void EnableWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            // We have to reset the ATOM entry XML customization since in the server behavior no atom entry customization is used.
            this.SetAtomEntryXmlCustomizationCallback(null);
            this.readerBehavior = ODataReaderBehavior.CreateWcfDataServicesServerBehavior(usesV1Provider);
        }

        /// <summary>
        /// Enables the same behavior that the WCF Data Services client has. Also, lets the user set the values for custom data namespace and type scheme.
        /// </summary>
        /// <param name="typeResolver">Custom type resolver which takes both expected type and type name.
        /// This function is used instead of the IEdmModel.FindType if it's specified.
        /// The first parameter to the function is the expected type (the type inferred from the parent property or specified by the external caller).
        /// The second parameter is the type name from the payload.
        /// The function should return the resolved type, or null if no such type was found.</param>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types. This should be validated to be a valid URI, this method will not check that.</param>
        /// <param name="entryXmlCustomizationCallback">
        /// If non-null this func will be called when an entry start is found and the entry is to be read.
        /// It takes three parameters:
        ///  - ODataEntry entry - which is the entry to be read.
        ///  - XmlReader reader - which is the current XmlReader used by the ODataReader to read the entry. The reader is positioned on the atom:entry start element tag.
        ///     Note that the reader might not be the exact instance of the reader create by the parent entry customization or passed in by other means to the ODataReader,
        ///     the ODataReader sometimes needs to wrap the readers and the wrapped XmlReader might be passed in here.
        ///  - Uri - the current xml:base URI value for the reader. If there is no active xml:base this parameter is passed a null value.
        /// It returns XmlReader:
        ///  - null - means there's no need for customization and the original XmlReader will be used to read the entry.
        ///  - non-null XmlReader - an XmlReader which the ODataReader will use to read the entry. This reader must be positioned on the atom:entry start element tag.
        ///     The ODataReader will not close or dispose the reader. It will read from it and leave the reader positioned on the atom:entry end element tag
        ///     (or the empty atom:entry start tag).
        ///     Once the ODataReader reports the ODataReaderState.EntryEnd for the entry, it will not use this XmlReader anymore.
        ///     After the ODataReaderState.EntryEnd is reported the parent reader (the parameter to the func) is expected to be positioned on the node AFTER
        ///     the atom:entry end element tag (or after the atom:entry empty start tag).
        ///     Note that this means that the ODataReader will only read till the end tag on the inner reader, but it expects the parent reader to move after the end tag.
        ///     It's the resposibility of the caller to move the parent read after the end tag manually if necessary.
        ///     The func must NOT return the same XmlReader instance as the XmlReader passed to it.
        /// </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "odataNamespace is valid")]
        public void EnableWcfDataServicesClientBehavior(
            Func<IEdmType, string, IEdmType> typeResolver,
            string odataNamespace,
            string typeScheme,
            Func<ODataEntry, XmlReader, Uri, XmlReader> entryXmlCustomizationCallback)
        {
            ExceptionUtils.CheckArgumentNotNull(odataNamespace, "odataNamespace");
            ExceptionUtils.CheckArgumentNotNull(typeScheme, "typeScheme");

            this.SetAtomEntryXmlCustomizationCallback(entryXmlCustomizationCallback);
            this.readerBehavior = ODataReaderBehavior.CreateWcfDataServicesClientBehavior(typeResolver, odataNamespace, typeScheme);
        }

        /// <summary>
        /// Enables the same behavior that the WCF Data Services client has. Also, lets the user set the values for custom data namespace and type scheme.
        /// </summary>
        /// <param name="typeResolver">Custom type resolver which takes both expected type and type name.
        /// This function is used instead of the IEdmModel.FindType if it's specified.
        /// The first parameter to the function is the expected type (the type inferred from the parent property or specified by the external caller).
        /// The second parameter is the type name from the payload.
        /// The function should return the resolved type, or null if no such type was found.</param>
        /// <param name="odataNamespace">Custom data namespace.</param>
        /// <param name="typeScheme">Custom type scheme to use when resolving types. This should be validated to be a valid URI, this method will not check that.</param>
        /// <param name="entryXmlCustomizationCallback">
        /// If non-null this func will be called when an entry start is found and the entry is to be read.
        /// It takes three parameters:
        ///  - ODataEntry entry - which is the entry to be read.
        ///  - XmlReader reader - which is the current XmlReader used by the ODataReader to read the entry. The reader is positioned on the atom:entry start element tag.
        ///     Note that the reader might not be the exact instance of the reader create by the parent entry customization or passed in by other means to the ODataReader,
        ///     the ODataReader sometimes needs to wrap the readers and the wrapped XmlReader might be passed in here.
        ///  - Uri - the current xml:base URI value for the reader. If there is no active xml:base this parameter is passed a null value.
        /// It returns XmlReader:
        ///  - null - means there's no need for customization and the original XmlReader will be used to read the entry.
        ///  - non-null XmlReader - an XmlReader which the ODataReader will use to read the entry. This reader must be positioned on the atom:entry start element tag.
        ///     The ODataReader will not close or dispose the reader. It will read from it and leave the reader positioned on the atom:entry end element tag
        ///     (or the empty atom:entry start tag).
        ///     Once the ODataReader reports the ODataReaderState.EntryEnd for the entry, it will not use this XmlReader anymore.
        ///     After the ODataReaderState.EntryEnd is reported the parent reader (the parameter to the func) is expected to be positioned on the node AFTER
        ///     the atom:entry end element tag (or after the atom:entry empty start tag).
        ///     Note that this means that the ODataReader will only read till the end tag on the inner reader, but it expects the parent reader to move after the end tag.
        ///     It's the resposibility of the caller to move the parent read after the end tag manually if necessary.
        ///     The func must NOT return the same XmlReader instance as the XmlReader passed to it.
        /// </param>
        /// <param name="shouldQualifyOperations">Callback to determine whether operations bound to a particular entity type must be qualified with a container name when appearing in a $select clause.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704", Justification = "odataNamespace is valid")]
        [Obsolete("The 'shouldQualifyOperations' parameter is no longer needed and will be removed. Use the overload which does not take it.")]
        public void EnableWcfDataServicesClientBehavior(
            Func<IEdmType, string, IEdmType> typeResolver,
            string odataNamespace,
            string typeScheme,
            Func<ODataEntry, XmlReader, Uri, XmlReader> entryXmlCustomizationCallback,
            Func<IEdmEntityType, bool> shouldQualifyOperations)
        {
            this.EnableWcfDataServicesClientBehavior(typeResolver, odataNamespace, typeScheme, entryXmlCustomizationCallback);
            this.readerBehavior.OperationsBoundToEntityTypeMustBeContainerQualified = shouldQualifyOperations;
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be skipped, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should be skipped, false otherwise.</returns>
        internal bool ShouldSkipAnnotation(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            return this.MaxProtocolVersion < ODataVersion.V3 || this.ShouldIncludeAnnotation == null || !this.ShouldIncludeAnnotation(annotationName);
        }
    }
}
