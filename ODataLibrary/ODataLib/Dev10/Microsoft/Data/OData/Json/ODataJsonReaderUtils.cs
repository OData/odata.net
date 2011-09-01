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

namespace Microsoft.Data.OData.Json
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the JSON format.
    /// </summary>
    internal static class ODataJsonReaderUtils
    {
        /// <summary>
        /// An enumeration of the various kinds of properties on a feed wrapper object.
        /// </summary>
        internal enum FeedPropertyKind
        {
            /// <summary>An unsupported property at the feed level.</summary>
            Unsupported, 

            /// <summary>The inline count property of a feed.</summary>
            InlineCount,

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
            InlineCount = 1,

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
                return FeedPropertyKind.InlineCount;
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
        /// <param name="usesV1ProviderBehavior">true if the conversion should use the V1 provider behavior, false if the default behavior should be used.</param>
        /// <returns>Object which is in sync with the property type (modulo the V1 exception of converting numbers to non-compatible target types).</returns>
        internal static object ConvertValue(object value, IEdmPrimitiveTypeReference primitiveTypeReference, bool usesV1ProviderBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            //// NOTE: this method was copied from WCF DS (and changed to take a type reference instead of a CLR target type)

            if (value == null)
            {
                ReaderValidationUtils.ValidateNullValue(primitiveTypeReference);
                return null;
            }

            try
            {
                Type targetType = primitiveTypeReference.GetInstanceType();
                targetType = TypeUtils.GetNonNullableType(targetType);

                string stringValue = value as string;
                if (stringValue != null)
                {
                    if (targetType == typeof(byte[]))
                    {
                        return Convert.FromBase64String(stringValue);
                    }
                    
                    if (targetType == typeof(Guid))
                    {
                        return new Guid(stringValue);
                    }
                    
                    // For string types, we support conversion to all possible primitive types
                    return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                }
                else if (value is Int32)
                {
                    int intValue = (int)value;
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
                        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertInt64OrDecimal);
                    }

                    if (targetType != typeof(Int32) && !usesV1ProviderBehavior)
                    {
                        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertInt32(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is Double)
                {
                    Double doubleValue = (Double)value;
                    if (targetType == typeof(Single))
                    {
                        return Convert.ToSingle(doubleValue);
                    }

                    if (targetType != typeof(Double) && !usesV1ProviderBehavior)
                    {
                        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDouble(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is bool)
                {
                    if (targetType != typeof(bool))
                    {
                        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertBoolean(primitiveTypeReference.ODataFullName()));
                    }
                }
                else if (value is DateTime)
                {
                    if (targetType != typeof(DateTime))
                    {
                        throw new ODataException(Strings.ODataJsonReaderUtils_CannotConvertDateTime(primitiveTypeReference.ODataFullName()));
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
            ref ODataJsonReaderUtils.ErrorPropertyBitMask propertiesFoundBitField,
            ODataJsonReaderUtils.ErrorPropertyBitMask propertyFoundBitMask)
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
                throw new ODataException(Strings.ODataJsonReaderUtils_MetadataPropertyWithNullValue(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_MultipleMetadataPropertiesWithSameName(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_EntityReferenceLinksPropertyWithNullValue(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_EntityReferenceLinksInlineCountWithNullValue(JsonConstants.ODataCountName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_MultipleEntityReferenceLinksWrapperPropertiesWithSameName(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_MultipleErrorPropertiesWithSameName(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_MediaResourcePropertyWithNullValue(propertyName));
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
                throw new ODataException(Strings.ODataJsonReaderUtils_FeedPropertyWithNullValue(propertyName));
            }
        }

    }
}
