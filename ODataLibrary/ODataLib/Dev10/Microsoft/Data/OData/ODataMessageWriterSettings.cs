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
        /// The format to use when writing the payload; this replaces the 'AcceptHeader' and 'AcceptCharSetHeader' 
        /// fields and uses the default values for the respective format. If ODataFormat.Default is specified
        /// the default format and the default media type will be picked depending on the writer these settings are used with.
        /// </summary>
        private ODataFormat format = ODataFormat.Default;

        /// <summary>Maximum number of query operations and changesets allowed in a single batch./// </summary>
        private int maxBatchSize;

        /// <summary>Maximum number of requests allowed in a single changeset.</summary>
        private int maxChangeSetSize;

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
        /// Constructor to create default settings for OData writers.
        /// </summary>
        public ODataMessageWriterSettings()
        {
            this.maxBatchSize = int.MaxValue;
            this.maxChangeSetSize = int.MaxValue;

            // on writing the default value for 'CheckCharacters' is set to true so that we 
            // produce valid Xml documents per default.
            this.CheckCharacters = true;

            // Create the default writer behavior
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
        }

        /// <summary>
        /// Copy constructor to create a copy of the settings for OData writers.
        /// </summary>
        /// <param name="settings">Settings to create a copy from</param>
        public ODataMessageWriterSettings(ODataMessageWriterSettings settings)
        {
            ExceptionUtils.CheckArgumentNotNull(settings, "settings");

            this.acceptCharSets = settings.acceptCharSets;
            this.acceptMediaTypes = settings.acceptMediaTypes;
            this.BaseUri = settings.BaseUri;
            this.CheckCharacters = settings.CheckCharacters;
            this.DisableMessageStreamDisposal = settings.DisableMessageStreamDisposal;
            this.format = settings.format;
            this.Indent = settings.Indent;
            this.MaxBatchSize = settings.MaxBatchSize;
            this.MaxChangesetSize = settings.MaxChangesetSize;
            this.useFormat = settings.useFormat;
            this.Version = settings.Version;

            // NOTE: writer behavior is immutable; copy by reference is ok.
            this.writerBehavior = settings.writerBehavior;
        }

        /// <summary>
        /// The OData protocol version to be used for writing payloads.
        /// </summary>
        public ODataVersion? Version
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the writer should use indentation or not.
        /// </summary>
        public bool Indent
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to control whether the writer should check for valid Xml characters or not.
        /// </summary>
        public bool CheckCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// Document base Uri (used as base for all relative Uris).
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
        /// Maximum number of query operations and changesets allowed in a single batch.
        /// </summary>
        public int MaxBatchSize
        {
            get
            {
                return this.maxBatchSize;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "value");
                this.maxBatchSize = value;
            }
        }

        /// <summary>
        /// Maximum number of requests allowed in a single changeset.
        /// </summary>
        public int MaxChangesetSize
        {
            get
            {
                return this.maxChangeSetSize;
            }

            set
            {
                ExceptionUtils.CheckIntegerNotNegative(value, "value");
                this.maxChangeSetSize = value;
            }
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
        /// properties and uses the default values for the respective format. If ODataFormat.Default is specified
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
        /// Sets the acceptable media types and character sets from which the content type will be computed when writing the payload.
        /// </summary>
        /// <param name="acceptableMediaTypes">
        /// The acceptable media types used to determine the content type of the message.
        /// This is a comma separated list of content types as specified in RFC 2616, Section 14.1
        /// </param>
        /// <param name="acceptableCharSets">
        /// The acceptable charsets to use to the determine the encoding of the message.
        /// This is a comma separated list of charsets as specified in RFC 2616, Section 14.2
        /// </param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(string acceptableMediaTypes, string acceptableCharSets)
        {
            this.acceptMediaTypes = acceptableMediaTypes;
            this.acceptCharSets = acceptableCharSets;
            this.format = ODataFormat.Default;
            this.useFormat = false;
        }

        /// <summary>
        /// Sets the format to be used when writing the payload. This will automatically set a compatible 
        /// content type header.
        /// </summary>
        /// <param name="payloadFormat">The format to use for writing the payload.</param>
        /// <remarks>Calling this method replaces any previously set content-type settings.</remarks>
        public void SetContentType(ODataFormat payloadFormat)
        {
            this.acceptCharSets = null;
            this.acceptMediaTypes = null;
            this.format = payloadFormat;
            this.useFormat = true;
        }

        /// <summary>
        /// Enables the default behavior of the OData library.
        /// </summary>
        public void EnableDefaultBehavior()
        {
            this.writerBehavior = ODataWriterBehavior.DefaultBehavior;
        }

        /// <summary>
        /// Enables the behavior of the WCF Data Services server.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        public void EnableWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            this.writerBehavior = ODataWriterBehavior.CreateWcfDataServicesServerBehavior(usesV1Provider);
        }

        /// <summary>
        /// Enables the behavior of the WCF Data Services client.
        /// </summary>
        /// <param name="startEntryXmlCustomizationCallback">
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
        /// </param>
        /// <param name="endEntryXmlCustomizationCallback">
        /// If non-null this action will be called when a (non-null) entry has been written.
        /// The action takes three parameters:
        ///  - ODataEntry entry - which is the entry which was written.
        ///  - XmlWriter entryWriter - the XmlWriter used to write the entry. The action must dispose or otherwise clear this writer.
        ///  - XmlWriter parentWriter - the XmlWriter used to write the parent scope of the entry. This is the writer which will be used to write further
        ///      payload once this action returns.
        /// </param>
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
        public void EnableWcfDataServicesClientBehavior(
            Func<ODataEntry, XmlWriter, XmlWriter> startEntryXmlCustomizationCallback,
            Action<ODataEntry, XmlWriter, XmlWriter> endEntryXmlCustomizationCallback)
        {
            if ((startEntryXmlCustomizationCallback == null && endEntryXmlCustomizationCallback != null) ||
                (startEntryXmlCustomizationCallback != null && endEntryXmlCustomizationCallback == null))
            {
                throw new ODataException(Strings.ODataMessageWriterSettings_MessageWriterSettingsXmlCustomizationCallbacksMustBeSpecifiedBoth);
            }

            this.writerBehavior = ODataWriterBehavior.CreateWcfDataServicesClientBehavior(
                startEntryXmlCustomizationCallback,
                endEntryXmlCustomizationCallback);
        }
    }
}
