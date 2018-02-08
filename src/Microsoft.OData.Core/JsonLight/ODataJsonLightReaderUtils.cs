//---------------------------------------------------------------------
// <copyright file="ODataJsonLightReaderUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Metadata;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    using ODataPlatformHelper = Microsoft.OData.PlatformHelper;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the JsonLight format.
    /// </summary>
    internal static class ODataJsonLightReaderUtils
    {
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

            /// <summary>The "value" property of the message object.</summary>
            MessageValue = 16,

            /// <summary>The "innererror" or "internalexception" property of the error object or an inner error object.</summary>
            InnerError = 32,

            /// <summary>The "type" property of an inner error object.</summary>
            TypeName = 64,

            /// <summary>The "stacktrace" property of an inner error object.</summary>
            StackTrace = 128,

            /// <summary>The "target" property.</summary>
            Target = 256,

            /// <summary>The "details" property.</summary>
            Details = 512
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
            ref ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField,
            ODataJsonLightReaderUtils.ErrorPropertyBitMask propertyFoundBitMask)
        {
            Debug.Assert(((int)propertyFoundBitMask & (((int)propertyFoundBitMask) - 1)) == 0, "propertyFoundBitMask is not a power of 2.");

            if ((propertiesFoundBitField & propertyFoundBitMask) == propertyFoundBitMask)
            {
                return false;
            }

            propertiesFoundBitField |= propertyFoundBitMask;
            return true;
        }



        /// <summary>
        /// Converts the given JSON value to the expected type as per OData conversion rules for JSON values.
        /// </summary>
        /// <param name="value">Value to the converted.</param>
        /// <param name="primitiveTypeReference">Type reference to which the value needs to be converted.</param>
        /// <param name="messageReaderSettings">The message reader settings used for reading.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="converter">The payload value converter to convert this value.</param>
        /// <returns>Object which is in sync with the property type (modulo the V1 exception of converting numbers to non-compatible target types).</returns>
        internal static object ConvertValue(
            object value,
            IEdmPrimitiveTypeReference primitiveTypeReference,
            ODataMessageReaderSettings messageReaderSettings,
            bool validateNullValue,
            string propertyName,
            ODataPayloadValueConverter converter)
        {
            Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

            if (value == null)
            {
                // Only primitive type references are validated. Core model is sufficient.
                messageReaderSettings.Validator.ValidateNullValue(
                    primitiveTypeReference,
                    validateNullValue,
                    propertyName,
                    null);
                return null;
            }

            value = converter.ConvertFromPayloadValue(value, primitiveTypeReference);

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
            if (instance == null)
            {
                instance = new T();
            }
        }

        /// <summary>
        /// Determines if the specified <paramref name="propertyName"/> is an OData annotation property name.
        /// </summary>
        /// <param name="propertyName">The property name to test.</param>
        /// <returns>true if the property name is an OData annotation property name, false otherwise.</returns>
        internal static bool IsODataAnnotationName(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.StartsWith(JsonLightConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines if the specified property name is a name of an annotation property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>true if <paramref name="propertyName"/> is a name of an annotation property, false otherwise.</returns>
        /// <remarks>
        /// This method returns true both for normal annotation as well as property annotations.
        /// </remarks>
        internal static bool IsAnnotationProperty(string propertyName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            return propertyName.IndexOf('.') >= 0;
        }

        /// <summary>
        /// Validates that the annotation value is valid.
        /// </summary>
        /// <param name="propertyValue">The value of the annotation.</param>
        /// <param name="annotationName">The name of the (instance or property) annotation (used for error reporting).</param>
        internal static void ValidateAnnotationValue(object propertyValue, string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (propertyValue == null)
            {
                throw new ODataException(ODataErrorStrings.ODataJsonLightReaderUtils_AnnotationWithNullValue(annotationName));
            }
        }

        /// <summary>
        /// Gets the payload type name for an OData OM instance for JsonLight.
        /// </summary>
        /// <param name="payloadItem">The payload item to get the type name for.</param>
        /// <returns>The type name as read from the payload item (or constructed for primitive items).</returns>
        internal static string GetPayloadTypeName(object payloadItem)
        {
            if (payloadItem == null)
            {
                return null;
            }

            // In JSON only boolean, String, Int32 and Double are recognized as primitive types
            // (without additional type conversion). So only check for those; if not one of these primitive
            // types it must be a complex, entity or collection value.
            if (payloadItem is Boolean)
            {
                return Metadata.EdmConstants.EdmBooleanTypeName;
            }

            if (payloadItem is String)
            {
                return Metadata.EdmConstants.EdmStringTypeName;
            }

            if (payloadItem is Int32)
            {
                return Metadata.EdmConstants.EdmInt32TypeName;
            }

            if (payloadItem is Double)
            {
                return Metadata.EdmConstants.EdmDoubleTypeName;
            }

            ODataCollectionValue collectionValue = payloadItem as ODataCollectionValue;
            if (collectionValue != null)
            {
                return EdmLibraryExtensions.GetCollectionTypeFullName(collectionValue.TypeName);
            }

            ODataResourceBase resource = payloadItem as ODataResourceBase;
            if (resource != null)
            {
                return resource.TypeName;
            }

            throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataJsonLightReader_ReadResourceStart));
        }
    }
}
