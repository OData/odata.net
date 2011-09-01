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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Helper methods used by the OData reader for the ATOM format.
    /// </summary>
    internal static class ODataAtomReaderUtils
    {
        /// <summary>
        /// Creates an Xml reader over the specified stream with the provided settings.
        /// </summary>
        /// <param name="stream">The stream to create the XmlReader over.</param>
        /// <param name="messageReaderSettings">The OData message reader settings used to control the settings of the Xml reader.</param>
        /// <returns>An <see cref="XmlReader"/> instance configured with the provided settings.</returns>
        internal static XmlReader CreateXmlReader(Stream stream, ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");

            XmlReaderSettings xmlReaderSettings = CreateXmlReaderSettings(messageReaderSettings);
            return XmlReader.Create(stream, xmlReaderSettings);
        }

        /// <summary>
        /// Parses the value of the m:null attribute and returns a boolean.
        /// </summary>
        /// <param name="attributeValue">The string value of the m:null attribute.</param>
        /// <returns>true if the value denotes that the element should be null; false otherwise.</returns>
        internal static bool ReadMetadataNullAttributeValue(string attributeValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(attributeValue != null, "attributeValue != null");

            return XmlConvert.ToBoolean(attributeValue);
        }

        /// <summary>
        /// Resolves the primitive payload type versus the expected type and validates that such combination is allowed.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference, if any.</param>
        /// <param name="payloadTypeKind">The kind of the payload type, or None if the detection was not possible.</param>
        /// <param name="payloadType">The resolved payload type, or null if no payload type was specified.</param>
        /// <param name="payloadTypeName">The name of the payload type, or null if no payload type was specified.</param>
        /// <param name="model">The model to use.</param>
        /// <param name="messageReaderSettings">The message reader settings to use.</param>
        /// <returns>The target type reference to use for parsing the value. This method never returns null.</returns>
        internal static IEdmTypeReference ResolveAndValidatePrimitiveTargetType(
            IEdmTypeReference expectedTypeReference,
            EdmTypeKind payloadTypeKind,
            IEdmType payloadType,
            string payloadTypeName,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageReaderSettings != null, "messageReaderSettings != null");
            Debug.Assert(
                payloadTypeKind == EdmTypeKind.Primitive || payloadTypeKind == EdmTypeKind.Complex ||
                payloadTypeKind == EdmTypeKind.Entity || payloadTypeKind == EdmTypeKind.Collection ||
                payloadTypeKind == EdmTypeKind.None,
                "The payload type kind must be one of None, Primitive, Complex, Entity or MultiValue.");
            Debug.Assert(
                expectedTypeReference == null || expectedTypeReference.TypeKind() == EdmTypeKind.Primitive,
                "This method only works for primitive expected type.");
            Debug.Assert(
                payloadType == null || payloadType.TypeKind == payloadTypeKind,
                "The payload type kind must match the payload type if that is available.");
            Debug.Assert(payloadType == null || payloadTypeName != null, "If we have a payload type, we must have its name as well.");

            if (payloadTypeKind != EdmTypeKind.None)
            {
                // Make sure that the type kinds match.
                ValidationUtils.ValidateTypeKind(payloadTypeKind, EdmTypeKind.Primitive, payloadTypeName);
            }

            if (!model.IsUserModel())
            {
                // If there's no model, it means we should not have the expected type either, and that there's no type to use,
                // no metadata validation to perform.
                Debug.Assert(expectedTypeReference == null, "If we don't have a model, we must not have expected type either.");

                return payloadType == null ? EdmCoreModel.Instance.GetString(true) : payloadType.ToTypeReference(true);
            }

            // If the primitive type conversion is off, use the payload type always.
            // If there's no expected type, use the payload type as well.
            if (expectedTypeReference == null || messageReaderSettings.DisablePrimitiveTypeConversion)
            {
                // If there's no payload type, it means Edm.String.
                // Note that in MultiValues the items without type should inherit the type name from the MultiValue, in that case the expectedTypeReference
                // is never null (assuming we do have a model), so we won't get here.

                // No expected type (for example an open property, but other scenarios are possible)
                // We need some type to go on. We do have a model, so we must perform metadata validation and for that we need a type.
                return payloadType == null ? EdmCoreModel.Instance.GetString(true) : payloadType.ToTypeReference(true);
            }

            if (messageReaderSettings.DisableStrictMetadataValidation)
            {
                // Lax validation logic
                // Always use the expected type, the payload type is ignored.
                return expectedTypeReference;
            }

            // Strict validation logic
            if (payloadType != null)
            {
                // The payload type must be convertible to the expected type.
                // Note that we compare the type definitions, since we want to ignore nullability (the payload type doesn't specify nullability).
                if (!MetadataUtilsCommon.CanConvertPrimitiveTypeTo(
                    (IEdmPrimitiveType)payloadType,
                    (IEdmPrimitiveType)(expectedTypeReference.Definition)))
                {
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(payloadTypeName, expectedTypeReference.ODataFullName()));
                }
            }

            // Read to the expected type.
            return expectedTypeReference;
        }

        /// <summary>
        /// Validates a type name comming from the m:type attribute.
        /// </summary>
        /// <param name="typeName">The type name value, must not be null.</param>
        internal static void ValidateTypeName(string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(typeName != null, "typeName != null");

            if (typeName.Length == 0)
            {
                throw new ODataException(Strings.ODataAtomReaderUtils_InvalidTypeName);
            }
        }

        /// <summary>
        /// Creates a new XmlReaderSettings instance using the encoding.
        /// </summary>
        /// <param name="messageReaderSettings">Configuration settings of the OData reader.</param>
        /// <returns>The Xml reader settings to use for this reader.</returns>
        private static XmlReaderSettings CreateXmlReaderSettings(ODataMessageReaderSettings messageReaderSettings)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.CheckCharacters = messageReaderSettings.CheckCharacters;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.CloseInput = true;

            // We do not allow DTDs - this is the default
#if ORCAS
            settings.ProhibitDtd = true;
#else
            settings.DtdProcessing = DtdProcessing.Prohibit;
#endif

            return settings;
        }
    }
}
