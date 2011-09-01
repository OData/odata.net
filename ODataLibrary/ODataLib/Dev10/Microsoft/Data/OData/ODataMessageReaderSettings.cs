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
        /// Constructor to create default settings for OData readers.
        /// </summary>
        public ODataMessageReaderSettings()
        {
            this.DisablePrimitiveTypeConversion = false;

            // On reading the default value for 'CheckCharacters' is set to false so that we 
            // can consume valid and invalid Xml documents per default.
            this.CheckCharacters = false;

            // ATOM metadata reading is disabled by default for performance reasons and because 
            // few clients will need the ATOM metadata.
            this.EnableAtomMetadataReading = false;

            // Create the default reader behavior
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;
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
            this.DisableStrictMetadataValidation = other.DisableStrictMetadataValidation;
            this.EnableAtomMetadataReading = other.EnableAtomMetadataReading;

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
        /// false (default) - metadata validation is strict, the input must exactly match against the model.
        /// true - metadata validation is lax, the input doesn't have to match the model in all cases.
        /// This property has effect only if the metadata model is specified.
        /// </summary>
        /// <remarks>
        /// Strict metadata validation:
        ///   Primitive values: The wire type must be convertible to the expected type.
        ///   Complex values: The wire type must resolve against the model and it must exactly match the expected type.
        ///   Entities: The wire type must resolve against the model and it must be assignable to the expected type.
        ///   MultiValues: The wire type must exactly match the expected type.
        ///   If no expected type is available we use the payload type.
        /// Lax metadata validation:
        ///   Primitive values: If expected type is available, we ignore the wire type.
        ///   Complex values: The wire type is used if the model defines it. If the model doesn't define such a type, the expected type is used.
        ///     If the wire type is not equal to the expected type, but it's assignable, we fail because we don't support complex type inheritance.
        ///     If the wire type if not assignable we use the expected type.
        ///   Entities: same as complex values except that if the payload type is assignable we use the payload type. This allows derived entity types.
        ///   MultiValues: If expected type is available, we ignore the wire type, except we fail if the item type is a derived complex type.
        ///   If no expected type is available we use the payload type and it must resolve against the model.
        /// If DisablePrimitiveTypeConversion is on, the rules for primitive values don't apply
        ///   and the primitive values are always read with the type from the wire.
        /// </remarks>
        public bool DisableStrictMetadataValidation
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
        /// Enables the default behavior of the OData library.
        /// </summary>
        public void EnableDefaultBehavior()
        {
            this.readerBehavior = ODataReaderBehavior.DefaultBehavior;
        }

        /// <summary>
        /// Enables the same behavior that the WCF Data Services server has.
        /// </summary>
        /// <param name="usesV1Provider">true if the server uses a V1 provider; otherwise false.</param>
        public void EnableWcfDataServicesServerBehavior(bool usesV1Provider)
        {
            this.readerBehavior = ODataReaderBehavior.CreateWcfDataServicesServerBehavior(usesV1Provider);
        }

        /// <summary>
        /// Enables the same behavior that the WCF Data Services client has.
        /// </summary>
        public void EnableWcfDataServicesClientBehavior()
        {
            this.readerBehavior = ODataReaderBehavior.CreateWcfDataServicesClientBehavior();
        }
    }
}
