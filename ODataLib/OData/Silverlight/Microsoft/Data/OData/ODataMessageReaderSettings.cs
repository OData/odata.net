//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Xml;
    using Microsoft.Data.Edm;
    #endregion Namespaces

    /// <summary>
    /// Configuration settings for OData message readers.
    /// </summary>
    public sealed class ODataMessageReaderSettings
    {
        /// <summary>
        /// A instance representing any knobs that control the behavior of the readers
        /// inside and outside of WCF Data Services.
        /// </summary>
        private ODataReaderBehavior readerBehavior;

        /// <summary>
        /// Quotas to use for limiting resource consumption when reading an OData message.
        /// </summary>
        private ODataMessageQuotas messageQuotas;

        /// <summary>
        /// ATOM entry XML customization callback.
        /// </summary>
        private Func<ODataEntry, XmlReader, Uri, XmlReader> atomFormatEntryXmlCustomizationCallback;

        /// <summary>
        /// Constructor to create default settings for OData readers.
        /// </summary>
        public ODataMessageReaderSettings()
        {
            this.DisablePrimitiveTypeConversion = false;
            this.DisableMessageStreamDisposal = false;
            this.UndeclaredPropertyBehaviorKinds = ODataUndeclaredPropertyBehaviorKinds.None;

            // On reading the default value for 'CheckCharacters' is set to false so that we 
            // can consume valid and invalid Xml documents per default.
            this.CheckCharacters = false;

            // ATOM metadata reading is disabled by default for performance reasons and because 
            // few clients will need the ATOM metadata.
            this.EnableAtomMetadataReading = false;

            // Create the default reader behavior
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;

            this.MaxProtocolVersion = ODataConstants.ODataDefaultProtocolVersion;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        public ODataMessageReaderSettings(ODataMessageReaderSettings other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.BaseUri = other.BaseUri;
            this.CheckCharacters = other.CheckCharacters;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.DisablePrimitiveTypeConversion = other.DisablePrimitiveTypeConversion;
            this.EnableAtomMetadataReading = other.EnableAtomMetadataReading;
            this.messageQuotas = new ODataMessageQuotas(other.MessageQuotas);
            this.UndeclaredPropertyBehaviorKinds = other.UndeclaredPropertyBehaviorKinds;
            this.MaxProtocolVersion = other.MaxProtocolVersion;
            this.atomFormatEntryXmlCustomizationCallback = other.atomFormatEntryXmlCustomizationCallback;

            // NOTE: reader behavior is immutable; copy by reference is ok.
            this.readerBehavior = other.ReaderBehavior;
        }

        /// <summary>
        /// Document base Uri (used as base for all relative Uris). If this is set, it must be an absolute URI.
        /// </summary>
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

        /// <summary>
        /// false (default) - convert all primitive values to the type specified in metadata.
        /// true - do not convert primitive values, report values with the type specified in the payload.
        /// </summary>
        public bool DisablePrimitiveTypeConversion
        {
            get;
            set;
        }

        /// <summary>
        /// The behavior the reader should use when it finds undeclared property.
        /// </summary>
        /// <remarks>
        /// This setting has no effect if there's no model specified for the reader.
        /// This setting must be set to Default when reading request payloads.
        /// 
        /// Detailed behavior description:
        /// ODataUndeclaredPropertyBehaviorKind.Default
        ///   If an undeclared property is found reading fails.
        ///   
        /// ODataUndeclaredPropertyBehaviorKind.DisableReferencePropertyValidation
        ///   ATOM 
        ///     - Undeclared deferred navigation link will be read and reported.
        ///     - Undeclared expanded navigation link will fail.
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties fail.
        ///   JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - the link will be read and reported as a deferred navigation link.
        ///       - __mediaresource value is found - the link will be read and reported as a stream property
        ///       - If nothing from the above matches the reading fails.
        ///     - Undeclared association links inside __metadata/properties will be read and reported.
        ///     
        /// ODataUndeclaredPropertyBehaviorKind.IgnoreValueProperty
        ///   ATOM
        ///     - Undeclared property inside m:properties is ignored (not even read).
        ///     - Undeclared navigation link, stream property link or association link fail.
        ///   JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - fail as undeclared deferred nav. link.
        ///       - __mediaresource value is found - fail as undeclared stream property.
        ///       - All other properties are ignored and not read.
        ///     - Undeclared association links inside __metadata/properties fail.
        ///     
        /// ODataUndeclaredPropertyBehaviorKind.DisableReferencePropertyValidation | ODataUndeclaredPropertyBehaviorKind.IgnoreValueProperty
        ///   ATOM
        ///     - Undeclared deferred navigation link will be read and reported.
        ///     - Undeclared expanded navigation link will be read and the navigation link part will be reported,
        ///       the expanded content will be ignored and not read or reported. (Same goes for entity reference links).
        ///     - Undeclared stream property link (both read and edit) will be read and reported.
        ///     - Undeclared association link will be read and reported.
        ///     - Undeclared properties inside m:properties will be ignored and not read.
        ///   JSON
        ///     - If an undeclared property is found a detection logic will run:
        ///       - __deferred value is found - read and report a deferred navigation link.
        ///       - __mediaresource value is found - read and report stream property.
        ///       - All other properties are ignore and not read.
        ///     - Undeclared association links inside __metadata/properties are read and reported.
        ///     
        ///   Note that there's one difference between ATOM and JSON. In ATOM expanded links are treated as both 
        ///   reference property and value property. The link itself is the reference part, the expanded content is the value part. 
        ///   In JSON expanded links are treated as a value property as a whole. Since in JSON expanded links don't actually have 
        ///   the reference part (the payload doesn't contain the "href") this is not such a big difference.
        /// </remarks>
        public ODataUndeclaredPropertyBehaviorKinds UndeclaredPropertyBehaviorKinds
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the reader should check for valid Xml characters or not.
        /// </summary>
        public bool CheckCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// false (default) - dispose the message stream after finish writing the message.
        /// true - do not dispose the message stream after finish writing the message.
        /// </summary>
        public bool DisableMessageStreamDisposal
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether ATOM metadata is read in ATOM payloads.
        /// </summary>
        public bool EnableAtomMetadataReading
        {
            get;
            set;
        }

        /// <summary>
        /// The maximum OData protocol version the reader should accept and understand.
        /// </summary>
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
        /// Sets ATOM entry XML customization.
        /// This method only affects ATOM format payloads. For payloads of other formats this method has no effect.
        /// </summary>
        /// <param name="atomEntryXmlCustomizationCallback">
        /// If non-null this func will be called when an entry start is found and the entry is to be read and provides a possiblity to the caller to inspect the raw XML of the entry.
        /// </param>
        /// <remarks>
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

        /// <summary>
        /// Enables the default behavior of the OData library.
        /// </summary>
        public void EnableDefaultBehavior()
        {
            // We have to reset the ATOM entry XML customization since in the default behavior no atom entry customization is used.
            this.SetAtomEntryXmlCustomizationCallback(null);
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;
        }

        /// <summary>
        /// Enables the same behavior that the WCF Data Services server has.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
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
    }
}
