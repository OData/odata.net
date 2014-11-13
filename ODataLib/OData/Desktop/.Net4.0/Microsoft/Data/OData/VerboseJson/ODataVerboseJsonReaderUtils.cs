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

namespace Microsoft.Data.OData.VerboseJson
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Json;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    using ODataPlatformHelper = Microsoft.Data.OData.PlatformHelper;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the Verbose JSON format.
    /// </summary>
    internal static class ODataVerboseJsonReaderUtils
    {
        /// <summary>
        /// An enumeration of the various kinds of properties on a feed wrapper object.
        /// </summary>
        internal enum FeedPropertyKind
        {
            /// <summary>An unsupported property at the feed level.</summary>
            Unsupported, 

            /// <summary>The inline count property of a feed.</summary>
            Count,

            /// <summary>The results property of a feed.</summary>
            Results,

            /// <summary>The next page link property of a feed.</summary>
            NextPageLink,
        }

        /// <summary>
        /// An enumeration of the various kinds of properties on an entity reference link collection.
        /// </summary>
        [Flags]
        internal enum EntityReferenceLinksWrapperPropertyBitMask
        {
            /// <summary>An unsupported property at the wrapper level.</summary>
            None = 0,

            /// <summary>The inline count property of an entity reference links wrapper.</summary>
            Count = 1,

            /// <summary>The results property of an entity reference links wrapper.</summary>
            Results = 2,

            /// <summary>The next page link property of an entity reference links wrapper.</summary>
            NextPageLink = 4,
        }

        /// <summary>
        /// Enumeration of all properties in error payloads, the value of the enum is the bitmask which identifies
        /// a bit per property.
        /// </summary>
        /// <remarks>
        /// We only use a single enumeration for both top-level as well as inner errors. 
        /// This means that some bits are never set for top-level (or inner errors).
        /// </remarks>
        [Flags]
        internal enum ErrorPropertyBitMask
        {
            /// <summary>No property found yet.</summary>
            None = 0,

            /// <summary>The "error" of the top-level object.</summary>
            Error = 1,

            /// <summary>The "code" property.</summary>
            Code = 2,

            /// <summary>The "message" property of either the error object or the inner error object.</summary>
            Message = 4,

            /// <summary>The "lang" property of the message object.</summary>
            MessageLanguage = 8,

            /// <summary>The "value" property of the message object.</summary>
            MessageValue = 16,

            /// <summary>The "innererror" or "internalexception" property of the error object or an inner error object.</summary>
            InnerError = 32,

            /// <summary>The "type" property of an inner error object.</summary>
            TypeName = 64,

            /// <summary>The "stacktrace" property of an inner error object.</summary>
            StackTrace = 128,
        }

        /// <summary>
        /// Enumeration of all properties in __metadata, the value of the enum is the bitmask which identifies
        /// a bit per property.
        /// </summary>
        [Flags]
        internal enum MetadataPropertyBitMask
        {
            /// <summary>No property found yet.</summary>
            None = 0,

            /// <summary>The "uri" property.</summary>
            Uri = 1,

            /// <summary>The "type" property.</summary>
            Type = 2,

            /// <summary>The "etag" property.</summary>
            ETag = 4,

            /// <summary>The "media_src" property.</summary>
            MediaUri = 8,

            /// <summary>The "edit_media" property.</summary>
            EditMedia = 16,

            /// <summary>The "content_type" property.</summary>
            ContentType = 32,

            /// <summary>The "media_etag" property.</summary>
            MediaETag = 64,

            /// <summary>The "properties" property.</summary>
            Properties = 128,

            /// <summary>The "id" property.</summary>
            Id = 256,

            /// <summary>The "actions" property.</summary>
            Actions = 512,

            /// <summary>The "functions" property.</summary>
            Functions = 1024,
        }

        /// <summary>
        /// Compares the <paramref name="propertyName"/> against the list of supported feed-level properties and
        /// returns the kind of property.
        /// </summary>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>The kind of feed-level property of the property with name <paramref name="propertyName"/>.</returns>
        internal static FeedPropertyKind DetermineFeedPropertyKind(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "Property names must not be null or empty.");

            if (string.CompareOrdinal(JsonConstants.ODataCountName, propertyName) == 0)
            {
                return FeedPropertyKind.Count;
            }
            else if (string.CompareOrdinal(JsonConstants.ODataNextLinkName, propertyName) == 0)
            {
                return FeedPropertyKind.NextPageLink;
            }
            else if (string.CompareOrdinal(JsonConstants.ODataResultsName, propertyName) == 0)
            {
                return FeedPropertyKind.Results;
            }

            return FeedPropertyKind.Unsupported;
        }

        /// <summary>
        /// Converts the given JSON value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="value">Value to the converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <param name="messageReaderSettings">The message reader settings used for reading.</param>
        /// <param name="version">The version of the OData protocol used for reading.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>Object which is in sync with the property type (modulo the V1 exception of converting numbers to non-compatible target types).</returns>
        internal static object ConvertValue(
            object value,
            IEdmPrimitiveTypeReference primitiveTypeReference,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version,
            bool validateNullValue,
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            //// NOTE: this method was copied from WCF DS (and changed to take a type reference instead of a CLR target type)

            if (value == null)
            {
                // Only primitive type references are validated. Core model is sufficient.
                ReaderValidationUtils.ValidateNullValue(EdmCoreModel.Instance, primitiveTypeReference, messageReaderSettings, validateNullValue, version, propertyName);
                return null;
            }

            try
            {
                Type targetType = EdmLibraryExtensions.GetPrimitiveClrType(primitiveTypeReference.PrimitiveDefinition(), false);
                ODataReaderBehavior readerBehavior = messageReaderSettings.ReaderBehavior;

                string stringValue = value as string;
                if (stringValue != null)
                {
                    return ConvertStringValue(stringValue, targetType, version);
                }
                else if (value is Int32)
                {
                    return ConvertInt32Value((int)value, targetType, primitiveTypeReference, readerBehavior == null ? false : readerBehavior.UseV1ProviderBehavior);
                }
                else if (value is Double)
                {
                    Double doubleValue = (Double)value;
                    if (targetType == typeof(Single))
                    {
                        return Convert.ToSingle(doubleValue);
                    }

                    if (!IsV1PrimitiveType(targetType) || (targetType != typeof(Double) && (readerBehavior == null || !readerBehavior.UseV1ProviderBehavior)))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDouble(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is bool)
                {
                    if (targetType != typeof(bool) && (readerBehavior == null || readerBehavior.FormatBehaviorKind != ODataBehaviorKind.WcfDataServicesServer))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertBoolean(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is DateTime)
                {
                    return ConvertDateTimeValue((DateTime)value, targetType, primitiveTypeReference, readerBehavior);
                }
                else if (value is DateTimeOffset)
                {
                    // Currently, we do not support any conversion for DateTimeOffset date type. Hence failing if the target
                    // type is not DateTimeOffset.
                    if (targetType != typeof(DateTimeOffset))
                    {
                        throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTimeOffset(primitiveTypeReference.ODataFullName()));
                    }
                }
            }
            catch (Exception e)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(e))
                {
                    throw;
                }

                throw ReaderValidationUtils.GetPrimitiveTypeConversionException(primitiveTypeReference, e);
            }

            // otherwise just return the value without doing any conversion
            return value;
        }

        /// <summary>
        /// Ensure that the <paramref name="instance"/> is not null; if so create a new instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to check.</typeparam>
        /// <param name="instance">The instance to check for null.</param>
        internal static void EnsureInstance<T>(ref T instance)
            where T : class, new()
        {
            DebugUtils.CheckNoExternalCallers();

            if (instance == null)
            {
                instance = new T();
            }
        }

        /// <summary>
        /// Checks whether the specified property has already been found before.
        /// </summary>
        /// <param name="propertiesFoundBitField">
        /// The bit field which stores which properties of an error or inner error were found so far.
        /// </param>
        /// <param name="propertyFoundBitMask">The bit mask for the property to check.</param>
        /// <returns>true if the property has not been read before; otherwise false.</returns>
        internal static bool ErrorPropertyNotFound(
            ref ODataVerboseJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitField,
            ODataVerboseJsonReaderUtils.ErrorPropertyBitMask propertyFoundBitMask)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");

            if ((propertiesFoundBitField & propertyFoundBitMask) == propertyFoundBitMask)
            {
                return false;
            }

            propertiesFoundBitField |= propertyFoundBitMask;
            return true;
        }

        /// <summary>
        /// Validates that the string property in __metadata is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyName">The name of the property (used for error reporting).</param>
        internal static void ValidateMetadataStringProperty(string propertyValue, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_MetadataPropertyWithNullValue(propertyName));
            }
        }

        /// <summary>
        /// Verifies that the specified property was not yet found.
        /// </summary>
        /// <param name="propertiesFoundBitField">The bit field which stores which metadata properties were found so far.</param>
        /// <param name="propertyFoundBitMask">The bit mask for the property to check.</param>
        /// <param name="propertyName">The name of the property to check (used for error reporting).</param>
        internal static void VerifyMetadataPropertyNotFound(ref MetadataPropertyBitMask propertiesFoundBitField, MetadataPropertyBitMask propertyFoundBitMask, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if ((propertiesFoundBitField & propertyFoundBitMask) != 0)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_MultipleMetadataPropertiesWithSameName(propertyName));
            }

            propertiesFoundBitField |= propertyFoundBitMask;
        }

        /// <summary>
        /// Validates that the string property in an entity reference links collection is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyName">The name of the property (used for error reporting).</param>
        internal static void ValidateEntityReferenceLinksStringProperty(string propertyValue, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_EntityReferenceLinksPropertyWithNullValue(propertyName));
            }
        }

        /// <summary>
        /// Validates that the count property in an OData-owned object wrapper is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        internal static void ValidateCountPropertyInEntityReferenceLinks(long? propertyValue)
        {
            DebugUtils.CheckNoExternalCallers();

            if (!propertyValue.HasValue)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_EntityReferenceLinksInlineCountWithNullValue(JsonConstants.ODataCountName));
            }
        }

        /// <summary>
        /// Verifies that the specified property was not yet found.
        /// </summary>
        /// <param name="propertiesFoundBitField">
        /// The bit field which stores which properties of an entity reference link collection were found so far.
        /// </param>
        /// <param name="propertyFoundBitMask">The bit mask for the property to check.</param>
        /// <param name="propertyName">The name of the property to check (used for error reporting).</param>
        internal static void VerifyEntityReferenceLinksWrapperPropertyNotFound(
            ref EntityReferenceLinksWrapperPropertyBitMask propertiesFoundBitField, 
            EntityReferenceLinksWrapperPropertyBitMask propertyFoundBitMask, 
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if ((propertiesFoundBitField & propertyFoundBitMask) == propertyFoundBitMask)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_MultipleEntityReferenceLinksWrapperPropertiesWithSameName(propertyName));
            }

            propertiesFoundBitField |= propertyFoundBitMask;
        }

        /// <summary>
        /// Verifies that the specified property was not yet found.
        /// </summary>
        /// <param name="propertiesFoundBitField">
        /// The bit field which stores which properties of an error or inner error were found so far.
        /// </param>
        /// <param name="propertyFoundBitMask">The bit mask for the property to check.</param>
        /// <param name="propertyName">The name of the property to check (used for error reporting).</param>
        internal static void VerifyErrorPropertyNotFound(
            ref ErrorPropertyBitMask propertiesFoundBitField,
            ErrorPropertyBitMask propertyFoundBitMask,
            string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (!ErrorPropertyNotFound(ref propertiesFoundBitField, propertyFoundBitMask))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(propertyName));
            }
        }

        /// <summary>
        /// Validates that the string property in __mediaresource is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyName">The name of the property (used for error reporting).</param>
        internal static void ValidateMediaResourceStringProperty(string propertyValue, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_MediaResourcePropertyWithNullValue(propertyName));
            }
        }

        /// <summary>
        /// Validates that the property in feed wrapper is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the property.</param>
        /// <param name="propertyName">The name of the property (used for error reporting).</param>
        internal static void ValidateFeedProperty(object propertyValue, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_FeedPropertyWithNullValue(propertyName));
            }
        }

        /// <summary>
        /// Gets the payload type name for an OData OM instance for JSON.
        /// </summary>
        /// <param name="payloadItem">The payload item to get the type name for.</param>
        /// <returns>The type name as read from the payload item (or constructed for primitive items).</returns>
        internal static string GetPayloadTypeName(object payloadItem)
        {
            DebugUtils.CheckNoExternalCallers();

            if (payloadItem == null)
            {
                return null;
            }

            TypeCode typeCode = ODataPlatformHelper.GetTypeCode(payloadItem.GetType());
            switch (typeCode)
            {
                // In JSON only boolean, DateTime, String, Int32 and Double are recognized as primitive types
                // (without additional type conversion). So only check for those; if not one of these primitive
                // types it must be a complex, entity or collection value.
                case TypeCode.Boolean: return Metadata.EdmConstants.EdmBooleanTypeName;
                case TypeCode.DateTime: return Metadata.EdmConstants.EdmDateTimeTypeName;
                case TypeCode.String: return Metadata.EdmConstants.EdmStringTypeName;
                case TypeCode.Int32: return Metadata.EdmConstants.EdmInt32TypeName;
                case TypeCode.Double: return Metadata.EdmConstants.EdmDoubleTypeName;
                default:
                    Debug.Assert(typeCode == TypeCode.Object, "If not one of the primitive types above, it must be an object in JSON.");
                    break;
            }

            ODataComplexValue complexValue = payloadItem as ODataComplexValue;
            if (complexValue != null)
            {
                return complexValue.TypeName;
            }

            ODataCollectionValue collectionValue = payloadItem as ODataCollectionValue;
            if (collectionValue != null)
            {
                return collectionValue.TypeName;
            }

            ODataEntry entry = payloadItem as ODataEntry;
            if (entry != null)
            {
                return entry.TypeName;
            }

            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataVerboseJsonReader_ReadEntryStart));
        }

        /// <summary>
        /// Converts the given JSON string value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="stringValue">String value to the converted.</param>
        /// <param name="targetType">Target type to which the string value needs to be converted.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <returns>Object which is in sync with the target type.</returns>
        private static object ConvertStringValue(string stringValue, Type targetType, ODataVersion version)
        {
            if (targetType == typeof(byte[]))
            {
                return Convert.FromBase64String(stringValue);
            }

            if (targetType == typeof(Guid))
            {
                return new Guid(stringValue);
            }

            // Convert.ChangeType does not support TimeSpan.
            if (targetType == typeof(TimeSpan))
            {
                return XmlConvert.ToTimeSpan(stringValue);
            }

            // Convert.ChangeType does not support DateTimeOffset.
            // Convert.ChangeType does support DateTime, and hence the ChangeType
            // call below should handle the DateTime case.
            if (targetType == typeof(DateTimeOffset))
            {
                return PlatformHelper.ConvertStringToDateTimeOffset(stringValue);
            }

            // In Verbose JSON V3 the DateTime fomat is the ISO one, so we need to call XmlConvert instead of Convert
            // Convert doesn't read that value correctly (converts the result to Local kind always).
            if (targetType == typeof(DateTime) && version >= ODataVersion.V3)
            {
                try
                {
                    return PlatformHelper.ConvertStringToDateTime(stringValue);
                }
                catch (FormatException)
                {
                    // If the XmlConvert fails to convert we need to try Convert.ChangeType on the value anyway
                    // so that we can still read values like MM/DD/YYYY which we supported just fine in V1/V2.
                }
            }

            // For string types, we support conversion to all possible primitive types
            return Convert.ChangeType(stringValue, targetType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the given JSON int value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="intValue">Int32 value to the converted.</param>
        /// <param name="targetType">Target type to which the int value needs to be converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <param name="usesV1ProviderBehavior">true if the conversion should use the V1 provider behavior, false if the default behavior should be used.</param>
        /// <returns>Object which is in sync with the property type (modulo the V1 exception of converting numbers to non-compatible target types).</returns>
        private static object ConvertInt32Value(int intValue, Type targetType, IEdmPrimitiveTypeReference primitiveTypeReference, bool usesV1ProviderBehavior)
        {
            if (targetType == typeof(Int16))
            {
                return Convert.ToInt16(intValue);
            }

            if (targetType == typeof(Byte))
            {
                return Convert.ToByte(intValue);
            }

            if (targetType == typeof(SByte))
            {
                return Convert.ToSByte(intValue);
            }

            if (targetType == typeof(Single))
            {
                return Convert.ToSingle(intValue);
            }

            if (targetType == typeof(Double))
            {
                return Convert.ToDouble(intValue);
            }

            if (targetType == typeof(Decimal) ||
                targetType == typeof(Int64))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertInt64OrDecimal);
            }

            if (!IsV1PrimitiveType(targetType) || (targetType != typeof(Int32) && !usesV1ProviderBehavior))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertInt32(primitiveTypeReference.ODataFullName()));
            }

            return intValue;
        }

        /// <summary>
        /// Converts the given datetime value into the allowed target types.
        /// </summary>
        /// <param name="datetimeValue">DateTime value as read by the JsonReader.</param>
        /// <param name="targetType">Target type to which the datetime value needs to be converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <param name="readerBehavior">ODataReaderBehavior instance.</param>
        /// <returns>Object which is in sync with the target type.</returns>
        private static object ConvertDateTimeValue(DateTime datetimeValue, Type targetType, IEdmPrimitiveTypeReference primitiveTypeReference, ODataReaderBehavior readerBehavior)
        {
            if (targetType == typeof(DateTimeOffset))
            {
                // If the kind of DateTime is local or UTC, that means we know the timezone information. Hence we should allow
                // conversion to DateTimeOffset - there is no dataloss or ambiguity in that conversion.
                if (datetimeValue.Kind == DateTimeKind.Local || datetimeValue.Kind == DateTimeKind.Utc)
                {
                    return new DateTimeOffset(datetimeValue);
                }
            }

            if (targetType != typeof(DateTime) && (readerBehavior == null || readerBehavior.FormatBehaviorKind != ODataBehaviorKind.WcfDataServicesServer))
            {
                throw new ODataException(ODataErrorStrings.ODataJsonReaderUtils_CannotConvertDateTime(primitiveTypeReference.ODataFullName()));
            }

            return datetimeValue;
        }

        /// <summary>
        /// Checks if the given type is a V1 primitive type or not.
        /// </summary>
        /// <param name="targetType">Type instance.</param>
        /// <returns>True if the given target type is a V1 primitive type otherwise returns false.</returns>
        private static bool IsV1PrimitiveType(Type targetType)
        {
            if (targetType == typeof(DateTimeOffset) ||
                targetType == typeof(TimeSpan))
            {
                return false;
            }

            return true;
        }
    }
}
