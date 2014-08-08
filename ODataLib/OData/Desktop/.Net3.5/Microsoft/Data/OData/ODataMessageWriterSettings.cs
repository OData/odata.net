//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
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
        /// The start ATOM entry callback for XML customization of entries.
        /// </summary>
        private Func<ODataEntry, XmlWriter, XmlWriter> atomFormatStartEntryXmlCustomizationCallback;

        /// <summary>
        /// The end ATOM entry callback for XML customization of entries.
        /// </summary>
        private Action<ODataEntry, XmlWriter, XmlWriter> atomFormatEndEntryXmlCustomizationCallback;

        /// <summary>
        /// An instance representing any knobs that control the behavior of the writers
        /// inside and outside of WCF Data Services.
        /// </summary>
        private ODataWriterBehavior writerBehavior;

        /// <summary>Stores the base uri for the metadata document along with a select clause.</summary>
        private ODataMetadataDocumentUri metadataDocumentUri;

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

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageWriterSettings" /> class with default settings. </summary>
        public ODataMessageWriterSettings()
            : base()
        {
            // Create the default writer behavior
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
        }

        /// <summary>Initializes a new instance of the <see cref="T:Microsoft.Data.OData.ODataMessageWriterSettings" /> class with specified settings.</summary>
        /// <param name="other">The specified settings.</param>
        public ODataMessageWriterSettings(ODataMessageWriterSettings other)
            : base(other)
        {
            ExceptionUtils.CheckArgumentNotNull(other, "other");

            this.acceptCharSets = other.acceptCharSets;
            this.acceptMediaTypes = other.acceptMediaTypes;
            this.BaseUri = other.BaseUri;
            this.DisableMessageStreamDisposal = other.DisableMessageStreamDisposal;
            this.format = other.format;
            this.useFormat = other.useFormat;
            this.Version = other.Version;
            this.metadataDocumentUri = other.metadataDocumentUri;
            this.atomFormatStartEntryXmlCustomizationCallback = other.atomFormatStartEntryXmlCustomizationCallback;
            this.atomFormatEndEntryXmlCustomizationCallback = other.atomFormatEndEntryXmlCustomizationCallback;
            this.JsonPCallback = other.JsonPCallback;
            this.shouldIncludeAnnotation = other.shouldIncludeAnnotation;
            this.AutoComputePayloadMetadataInJson = other.AutoComputePayloadMetadataInJson;
            this.AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment = other.AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment;
            this.alwaysUseDefaultXmlNamespaceForRootElement = other.alwaysUseDefaultXmlNamespaceForRootElement;

            // NOTE: writer behavior is immutable; copy by reference is ok.
            this.writerBehavior = other.writerBehavior;
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
        /// If this value is not set (null), decision will be made based on the "Com.Microsoft.Data.Services.Conventions.V1.UrlConventions" vocabulary
        /// annotation on the IEdmEntityContainer, if available. The default behavior is to put key values inside parentheses and not a distinct URL segments.
        /// This setting only applies to URLs that are automatically generated by the <see cref="ODataMessageWriter" /> and does not modify URLs explicitly provided by the user.
        /// </summary>
        public bool? AutoGeneratedUrlsShouldPutKeyValueInDedicatedSegment
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, then the root element of each payload will be written in the default (non-prefix-qualified) namespace of the document. 
        /// All other elements in the same namespace will also not have prefixes.
        /// </summary>
        internal bool AlwaysUseDefaultXmlNamespaceForRootElement
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
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
                DebugUtils.CheckNoExternalCallers();
                return this.useFormat;
            }
        }

        /// <summary>
        /// The start ATOM entry callback for XML customization of entries.
        /// </summary>
        internal Func<ODataEntry, XmlWriter, XmlWriter> AtomStartEntryXmlCustomizationCallback
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.atomFormatStartEntryXmlCustomizationCallback;
            }
        }

        /// <summary>
        /// The end ATOM entry callback for XML customization of entries.
        /// </summary>
        internal Action<ODataEntry, XmlWriter, XmlWriter> AtomEndEntryXmlCustomizationCallback
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.atomFormatEndEntryXmlCustomizationCallback;
            }
        }

        /// <summary>
        /// Gets the metadata document URI that has been set on the settings, or null if it has not been set.
        /// </summary>
        internal ODataMetadataDocumentUri MetadataDocumentUri
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();
                return this.metadataDocumentUri;
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
                DebugUtils.CheckNoExternalCallers();
                return this.shouldIncludeAnnotation;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();
                this.shouldIncludeAnnotation = value;
            }
        }

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

        /// <summary>Sets the customization XML format for Atom entry.</summary>
        /// <param name="atomStartEntryXmlCustomizationCallback">The start of the Atom entry XML customization callback.</param>
        /// <param name="atomEndEntryXmlCustomizationCallback">The end of the Atom entry XML customization callback.</param>
        /// <remarks>
        /// Either both <paramref name="atomStartEntryXmlCustomizationCallback"/> and <paramref name="atomEndEntryXmlCustomizationCallback"/> must be null, or both must be non-null.
        /// The XmlWriter returned by the startEntryXmlCustomizationCallback is not owned by the ODataWriter and it won't be Disposed or Flushed or cleared
        /// in any way by it. It's the responsibility of the caller to flush/dispose it inside the endEntryXmlCustomizationCallback and also in case of an exception.
        /// In case an exception is thrown while a new writer is used to write an entry, only the original XmlWriter will be disposed
        /// and no endEntryXmlCustomizationCallback will be called.
        /// It's the responsibility of this callback to write the entry payload written to the entryWriter into the parentWriter at this point.
        /// The ODataWriter assumes that once this callback returns the entry was already written and it will continue onward.
        /// If expanded entries are being written this callback may get called multiple times without the corresponding end callback in between.
        /// It's the responsibility of the callback and the caller to be able to handle nested entries.
        /// This method only applies when writing ATOM format payloads.
        /// When writing payloads in different formats, this method has no effect.
        /// param name="atomStartEntryXmlCustomizationCallback"
        /// If non-null this func will be called when a new (non-null) entry is to be written.
        /// It takes two parameters:
        ///  - ODataEntry entry - which is the entry to be written.
        ///  - XmlWriter writer - which is the current XmlWriter used by the ODataWriter to write the entry.
        /// It returns XmlWriter:
        ///  - null - means there's no need to customize the XML for this entry, and thus the original writer will be used to write the entry.
        ///  - non-null XmlWriter - the ODataWriter will use the new returned writer to write the entry.
        ///      Once the entry has been written the EndEntryXmlCustomizationCallback will be called and the writer will be passed to it.
        ///      Note that the ODataWriter will not dipose or otherwise clear the writer.
        ///      The callback must never return the same instance as the writer parameter!
        /// param name="atomEndEntryXmlCustomizationCallback"
        /// If non-null this action will be called when a (non-null) entry has been written.
        /// The action takes three parameters:
        ///  - ODataEntry entry - which is the entry which was written.
        ///  - XmlWriter entryWriter - the XmlWriter used to write the entry. The action must dispose or otherwise clear this writer.
        ///  - XmlWriter parentWriter - the XmlWriter used to write the parent scope of the entry. This is the writer which will be used to write further
        ///      payload once this action returns.
        /// </remarks>
        public void SetAtomEntryXmlCustomization(
            Func<ODataEntry, XmlWriter, XmlWriter> atomStartEntryXmlCustomizationCallback,
            Action<ODataEntry, XmlWriter, XmlWriter> atomEndEntryXmlCustomizationCallback)
        {
            if ((atomStartEntryXmlCustomizationCallback == null && atomEndEntryXmlCustomizationCallback != null) ||
                (atomStartEntryXmlCustomizationCallback != null && atomEndEntryXmlCustomizationCallback == null))
            {
                throw new ODataException(Strings.ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth);
            }

            this.atomFormatStartEntryXmlCustomizationCallback = atomStartEntryXmlCustomizationCallback;
            this.atomFormatEndEntryXmlCustomizationCallback = atomEndEntryXmlCustomizationCallback;
        }

        /// <summary>Enables the <see cref="T:Microsoft.Data.OData.ODataMessageWriterSettings" /> default behavior.</summary>
        public void EnableDefaultBehavior()
        {
            // We have to reset the ATOM entry XML customization since in the default behavior no atom entry customization is used.
            this.SetAtomEntryXmlCustomization(null, null);
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
        }

        /// <summary>Specifies whether the WCF data services server behavior is enabled.</summary>
        /// <param name="usesV1Provider">true if the server uses V1 provider, otherwise, false.</param>
        public void EnableWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            // We have to reset the ATOM entry XML customization since in the server behavior no atom entry customization is used.
            this.SetAtomEntryXmlCustomization(null, null);
            this.writerBehavior = ODataWriterBehavior.CreateWcfDataServicesServerBehavior(usesV1Provider);
        }

        /// <summary>Specifies whether the WCF data services server behavior is enabled.</summary>
        /// <param name="usesV1Provider">true if the server uses V1 provider, otherwise, false.</param>
        /// <param name="alwaysUseDefaultXmlNamespaceForRootElement">true if the server is configured to leave prefixes off all root elements and anything else in the same namespace, otherwise, false.</param>
        public void EnableWcfDataServicesServerBehavior(bool usesV1Provider, bool alwaysUseDefaultXmlNamespaceForRootElement)
        {
            DebugUtils.CheckNoExternalCallers();
            this.EnableWcfDataServicesServerBehavior(usesV1Provider);
            this.alwaysUseDefaultXmlNamespaceForRootElement = alwaysUseDefaultXmlNamespaceForRootElement;
        }

        /// <summary>Enables the WCF data services client behavior.</summary>
        /// <param name="startEntryXmlCustomizationCallback">The start of the entry XML customization callback.</param>
        /// <param name="endEntryXmlCustomizationCallback">The end of the entry XML customization callback.</param>
        /// <param name="odataNamespace">The OData namespace.</param>
        /// <param name="typeScheme">The type scheme.</param>
        /// <remarks>
        /// Either both <paramref name="startEntryXmlCustomizationCallback"/> and <paramref name="endEntryXmlCustomizationCallback"/> must be null, or both must be non-null.
        /// The XmlWriter returned by the startEntryXmlCustomizationCallback is not owned by the ODataWriter and it won't be Disposed or Flushed or cleared
        /// in any way by it. It's the responsibility of the caller to flush/dispose it inside the endEntryXmlCustomizationCallback and also in case of an exception.
        /// In case an exception is thrown while a new writer is used to write an entry, only the original XmlWriter will be disposed
        /// and no endEntryXmlCustomizationCallback will be called.
        /// It's the responsibility of this callback to write the entry payload written to the entryWriter into the parentWriter at this point.
        /// The ODataWriter assumes that once this callback returns the entry was already written and it will continue onward.
        /// If expanded entries are being written this callback may get called multiple times without the corresponding end callback in between.
        /// It's the responsibility of the callback and the caller to be able to handle nested entries.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "odataNamespace is valid")]
        public void EnableWcfDataServicesClientBehavior(
            Func<ODataEntry, XmlWriter, XmlWriter> startEntryXmlCustomizationCallback,
            Action<ODataEntry, XmlWriter, XmlWriter> endEntryXmlCustomizationCallback,
            string odataNamespace, 
            string typeScheme)
        {
            ExceptionUtils.CheckArgumentNotNull(odataNamespace, "odataNamespace");
            ExceptionUtils.CheckArgumentNotNull(typeScheme, "typeScheme");

            this.SetAtomEntryXmlCustomization(startEntryXmlCustomizationCallback, endEntryXmlCustomizationCallback);

            this.writerBehavior = ODataWriterBehavior.CreateWcfDataServicesClientBehavior(
                odataNamespace,
                typeScheme);
        }

        /// <summary>Sets the URI of the metadata document.</summary>
        /// <param name="value">The URI of the metadata document.</param>
        public void SetMetadataDocumentUri(Uri value)
        {
            this.metadataDocumentUri = value == null ? null : new ODataMetadataDocumentUri(value);
        }

        /// <summary>Sets the URI of the metadata document.</summary>
        /// <param name="value">The URI of the metadata document.</param>
        /// <param name="selectClause">The select clause.</param>
        public void SetMetadataDocumentUri(Uri value, string selectClause)
        {
            this.metadataDocumentUri = new ODataMetadataDocumentUri(value) { SelectClause = selectClause };
        }

        /// <summary>
        /// Determines if there is a JSON padding function defined.
        /// </summary>
        /// <returns>True if the JsonPCallback property is not null or emtpy.</returns>
        internal bool HasJsonPaddingFunction()
        {
            DebugUtils.CheckNoExternalCallers();
            return !string.IsNullOrEmpty(this.JsonPCallback);
        }

        /// <summary>
        /// Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be writen, false otherwise.
        /// </summary>
        /// <param name="annotationName">The name of the annotation in question.</param>
        /// <returns>Returns true to indicate that the annotation with the name <paramref name="annotationName"/> should not be writen, false otherwise.</returns>
        internal bool ShouldSkipAnnotation(string annotationName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(this.Version.HasValue, "The version should be set by now.");
            return this.Version.Value < ODataVersion.V3 || this.ShouldIncludeAnnotation != null && !this.ShouldIncludeAnnotation(annotationName);
        }
    }
}
