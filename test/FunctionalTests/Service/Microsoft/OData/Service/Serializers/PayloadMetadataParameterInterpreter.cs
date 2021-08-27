//---------------------------------------------------------------------
// <copyright file="PayloadMetadataParameterInterpreter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
{
    #region Namespaces

    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.OData;
    using Microsoft.OData.Json;
    using Microsoft.OData.Service;
    using Microsoft.OData.Service.Providers;


    #endregion

    /// <summary>
    /// Component for interpreting the media type parameter for controlling how much payload metadata is written on the wire.
    /// </summary>
    internal class PayloadMetadataParameterInterpreter
    {
        /// <summary>
        /// The name of the media type parameter to use.
        /// </summary>
        private const string MediaTypeParameterName = "odata.metadata";

        /// <summary>
        /// The interpreted representation of the parameter.
        /// </summary>
        private readonly MetadataParameterValue metadataParameterValue;

        /// <summary>
        /// The interpreted representation of the parameter, to used for controlling the type name serialization.
        /// </summary>
        private readonly MetadataParameterValue metadataParameterValueForTypeNames;

        /// <summary>
        /// true if the odata format is JSON Light, false otherwise.
        /// </summary>
        private readonly bool isJsonLight;

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadMetadataParameterInterpreter"/> class.
        /// NOTE: This constructor is for unit-testing only.
        /// </summary>
        /// <param name="format">The response format.</param>
        /// <param name="rawParameterValue">The parameter value.</param>
        internal PayloadMetadataParameterInterpreter(ODataFormat format, string rawParameterValue)
        {
            this.isJsonLight = format == ODataFormat.Json;

            // NOTE: we explicitly allow the format passed in to be null, because it implies that no response payload
            // can possibly be written. In this case, it is treated exactly the same as any other non-json-light format.
            if (this.isJsonLight)
            {
                this.metadataParameterValue = ParseMetadataParameterForJsonLight(rawParameterValue);
                this.metadataParameterValueForTypeNames = this.metadataParameterValue;
            }
            else
            {
                Debug.Assert(rawParameterValue == null || rawParameterValue == "verbose", "Only JSON-Light allows the the amount of metadata to be controlled.");
                this.metadataParameterValue = MetadataParameterValue.Full;
                this.metadataParameterValueForTypeNames = MetadataParameterValue.Minimal;
            }
        }

        /// <summary>
        /// Local enum for capturing which of the parameter values was specified.
        /// </summary>
        private enum MetadataParameterValue
        {
            /// <summary>
            /// The 'minimal' option
            /// </summary>
            Minimal = 0,

            /// <summary>
            /// The 'full' option
            /// </summary>
            Full,

            /// <summary>
            /// The 'none' option
            /// </summary>
            None,
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PayloadMetadataParameterInterpreter"/> class.
        /// </summary>
        /// <param name="format">The response format.</param>
        /// <returns>A new instance of the <see cref="PayloadMetadataParameterInterpreter"/> class.</returns>
        internal static PayloadMetadataParameterInterpreter Create(ODataFormatWithParameters format)
        {
            ODataFormat odataFormat = format == null ? null : format.Format;
            string rawParameterValue = format == null ? null : format.GetParameterValue(MediaTypeParameterName);
            return new PayloadMetadataParameterInterpreter(odataFormat, rawParameterValue);
        }

        /// <summary>
        /// Returns whether a specific kind of entry metadata should be included. Must not be called for type names, however.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <returns>True if the metadata should be included, otherwise false</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeEntryMetadata(PayloadMetadataKind.Entry kind)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Entry), kind), "Invalid enum value");
            Debug.Assert(kind != PayloadMetadataKind.Entry.TypeName, "Must use other helper method 'ShouldIncludeEntryTypeName' instead");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "Unexpected metadata parameter value: " + this.metadataParameterValue);
            Debug.Assert(
                kind == PayloadMetadataKind.Entry.Id
                || kind == PayloadMetadataKind.Entry.ETag
                || kind == PayloadMetadataKind.Entry.EditLink,
                "Unexpected metadata kind: " + kind);

            if (kind == PayloadMetadataKind.Entry.ETag)
            {
                return true;
            }

            // Default case:
            // Entry-metadata is generated based on conventions and should be left out by default.
            // This is specifically the case for the entity's Id and Edit-Link.
            return false;
        }

        /// <summary>
        /// Returns whether an entry type name should be included.
        /// </summary>
        /// <param name="entityTypeName">Name of the entity type.</param>
        /// <param name="entitySetBaseTypeName">Name of the entity set's base-type.</param>
        /// <returns>True if the type name should be included, otherwise false</returns>
        internal bool ShouldIncludeEntryTypeName(string entityTypeName, string entitySetBaseTypeName)
        {
            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "setting == MetadataParameterValue.Default");

            // Default case:
            // The type name should only be written if it is more-derived than the base type for the entity-set/feed.
            return !string.Equals(entitySetBaseTypeName, entityTypeName, StringComparison.Ordinal);
        }

        /// <summary>
        /// Returns whether a specific kind of stream metadata should be included.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <returns>True if the metadata should be included, otherwise false</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeStreamMetadata(PayloadMetadataKind.Stream kind)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Stream), kind), "Invalid enum value");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "Unexpected metadata parameter value: " + this.metadataParameterValue);
            Debug.Assert(
                kind == PayloadMetadataKind.Stream.ContentType
                || kind == PayloadMetadataKind.Stream.ETag
                || kind == PayloadMetadataKind.Stream.EditLink
                || kind == PayloadMetadataKind.Stream.ReadLink,
                "Unexpected metadata kind: " + kind);

            // Default case:
            // Most stream metadata is generated by the custom provider and cannot be computed client-side, and must be included by default.
            // This is specifically the case for the streams's Content-Type, Read-Link, and ETag.
            // However, the stream's edit-link is generated based on conventions and should be left out by default.
            return kind != PayloadMetadataKind.Stream.EditLink;
        }

        /// <summary>
        /// Returns whether a specific kind of navigation link metadata should be included.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <returns>True if the metadata should be included, otherwise false</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeNavigationMetadata(PayloadMetadataKind.Navigation kind)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Navigation), kind), "Invalid enum value");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "Unexpected metadata parameter value: " + this.metadataParameterValue);
            Debug.Assert(kind == PayloadMetadataKind.Navigation.AssociationLinkUrl || kind == PayloadMetadataKind.Navigation.Url, "Unexpected metadata kind: " + kind);

            // Default case:
            // Navigation link metadata is generated based on conventions and should be left out by default.
            // This is specifically the case for the links's Url and AssociationLinkUrl.
            return false;
        }

        /// <summary>
        /// Returns whether a specific kind of association link metadata should be included.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <returns>True if the metadata should be included, otherwise false</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeAssociationMetadata(PayloadMetadataKind.Association kind)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Association), kind), "Invalid enum value");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "Unexpected metadata parameter value: " + this.metadataParameterValue);
            Debug.Assert(kind == PayloadMetadataKind.Association.Url, "Unexpected metadata kind: " + kind);

            // Default case:
            // Association link metadata is generated based on conventions and should be left out by default.
            // This is specifically the case for the links's Url.
            return false;
        }

        /// <summary>
        /// Returns whether a specific kind of feed metadata should be included.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <returns>True if the metadata should be included, otherwise false</returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeFeedMetadata(PayloadMetadataKind.Feed kind)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Feed), kind), "Invalid enum value");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(this.metadataParameterValue == MetadataParameterValue.Minimal, "Unexpected metadata parameter value: " + this.metadataParameterValue);
            Debug.Assert(kind == PayloadMetadataKind.Feed.Id, "Unexpected metadata kind: " + kind);

            // Default case:
            // Feed Ids are generated based on conventions and should be left out by default.
            return false;
        }

        /// <summary>
        /// Returns whether a specific kind of operation metadata should be included.
        /// </summary>
        /// <param name="kind">The kind of metadata.</param>
        /// <param name="checkIfUserValue">Callback to determine if the value has been changed by the user.</param>
        /// <returns>
        /// True if the metadata should be included, otherwise false
        /// </returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "kind", Justification = "The parameter is used in debug builds.")]
        internal bool ShouldIncludeOperationMetadata(PayloadMetadataKind.Operation kind, Func<bool> checkIfUserValue)
        {
            Debug.Assert(Enum.IsDefined(typeof(PayloadMetadataKind.Operation), kind), "Invalid enum value");

            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            Debug.Assert(kind == PayloadMetadataKind.Operation.Title || kind == PayloadMetadataKind.Operation.Target, "Unexpected metadata kind: " + kind);

            // Default case:
            // Operation metadata is generated based on conventions and should be left out by default, unless it was changed by the user.
            // This is specifically the case for the operations's Title and Target.
            Debug.Assert(checkIfUserValue != null, "checkIfUserValue != null");
            return checkIfUserValue();
        }

        /// <summary>
        /// Returns whether an always available operations should be included in the entry.
        /// </summary>
        /// <returns>true if always available operations should be included, false otherwise.</returns>
        internal bool ShouldIncludeAlwaysAvailableOperation()
        {
            if (this.metadataParameterValue == MetadataParameterValue.Full)
            {
                return true;
            }

            if (this.metadataParameterValue == MetadataParameterValue.None)
            {
                return false;
            }

            // Default case:
            // Always available operations is generated based on conventions and should be left out be default.
            return false;
        }

        /// <summary>
        /// Returns whether ODataLib should be explicitly instructed to include or omit a type name on the wire.
        /// </summary>
        /// <param name="value">The value to be serialized.</param>
        /// <param name="actualType">The type to be potentially serialized.</param>
        /// <param name="typeNameToWrite">The type name which ODataLib should be told to serialize. A value of null indicates the type name should be omitted.</param>
        /// <returns>true if an annotation should be created to override ODataLib's default type name serialization behavior; false if the ODataLib default behavior should be used.</returns>
        internal bool ShouldSpecifyTypeNameAnnotation(ODataValue value, ResourceType actualType, out string typeNameToWrite)
        {
            if (this.metadataParameterValueForTypeNames == MetadataParameterValue.Full)
            {
                ODataPrimitiveValue primitiveValue = value as ODataPrimitiveValue;
                Debug.Assert(primitiveValue == null || actualType.ResourceTypeKind == ResourceTypeKind.Primitive, "If value is ODataPrimitiveValue, actualType must also be primitive.");
                if (primitiveValue != null && JsonSharedUtils.ValueTypeMatchesJsonType(primitiveValue, MetadataProviderUtils.CreatePrimitiveTypeReference(actualType)))
                {
                    // Don't set the annotation and use the default ODataLib type name serialization behavior for basic JSON types.
                    typeNameToWrite = null;
                    return false;
                }

                typeNameToWrite = actualType.FullName;
                return true;
            }

            if (this.metadataParameterValueForTypeNames == MetadataParameterValue.None)
            {
                // Setting the type name to null explicitly tells ODataLib to not write a type name annotation on the wire.
                typeNameToWrite = null;
                return true;
            }

            // Otherwise, don't set the annotation and use the default ODataLib type name serialization behavior.
            typeNameToWrite = null;
            return false;
        }

        /// <summary>
        /// Determins whether to use absolute or relative Uri for next link.
        /// </summary>
        /// <returns>true if the next link Uri should be absolute; false if the next link Uri should be relative.</returns>
        internal bool ShouldNextPageLinkBeAbsolute()
        {
            if (this.isJsonLight && this.metadataParameterValue != MetadataParameterValue.None)
            {
                // In JsonLight, we need to write relative links for next link except for nometadata since ODL will only
                // allow relative Uris when odata.metadata is on the payload.
                return false;
            }

            // In V2, Astoria used to write absoluteUri in the next link instead of the relative uri.
            // For backward compatible, we need to write the absolute uri for Atom and JSON Verbose.
            return true;
        }

        /// <summary>
        /// Unit test method for determining whether two facades are equivalent (ie: wrap the same server/client models).
        /// </summary>
        /// <param name="other">The other facade.</param>
        /// <returns>
        ///   <c>true</c> if the two facades wrap the same models; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEquivalentTo(PayloadMetadataParameterInterpreter other)
        {
            return other != null
                && other.isJsonLight == this.isJsonLight
                && other.metadataParameterValue == this.metadataParameterValue
                && other.metadataParameterValueForTypeNames == this.metadataParameterValueForTypeNames;
        }

        /// <summary>
        /// Parses the raw parameter value provided in the media type and returns a simplified representation, assuming the format is Json Light.
        /// </summary>
        /// <param name="rawParameterValue">The raw parameter value.</param>
        /// <returns>A representation of what option was specified.</returns>
        private static MetadataParameterValue ParseMetadataParameterForJsonLight(string rawParameterValue)
        {
            // If the parameter is not specified, it is equivalent to 'minimal'.
            if (rawParameterValue == null)
            {
                return MetadataParameterValue.Minimal;
            }

            // ignore case for media type parameters.
            return (MetadataParameterValue)Enum.Parse(typeof(MetadataParameterValue), rawParameterValue, true);
        }
    }
}