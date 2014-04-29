//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Atom
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Class responsible for storing and manipulating instance annotation data in ATOM payloads.
    /// </summary>
    internal sealed class AtomInstanceAnnotation
    {
        /// <summary>
        /// Backing field of the Target property.
        /// </summary>
        private readonly string target;

        /// <summary>
        /// Backing field of the Term property.
        /// </summary>
        private readonly string term;

        /// <summary>
        /// Backing field of the Value property.
        /// </summary>
        private readonly ODataValue value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target">The target of the annotation.</param>
        /// <param name="term">The term whose value is being expressed through this annotation.</param>
        /// <param name="value">The annotation's value.</param>
        private AtomInstanceAnnotation(string target, string term, ODataValue value)
        {
            this.target = target;
            this.term = term;
            this.value = value;
        }

        /// <summary>
        /// The target of this annotation, as specified in the m:annotation/@target attribute.
        /// </summary>
        internal string Target
        {
            get
            {
                return this.target;
            }
        }

        /// <summary>
        /// The term of this annotation's value, as specified in the m:annotation/@term attribute.
        /// </summary>
        internal string TermName
        {
            get
            {
                return this.term;
            }
        }

        /// <summary>
        /// The value of this annotation.
        /// </summary>
        internal ODataValue Value
        {
            get
            {
                return this.value;
            }
        }

        /// <summary>
        /// True if the annotation is targeting the xml element in which the annotation was found; false if the annotation is targeting a different element.
        /// </summary>
        internal bool IsTargetingCurrentElement
        {
            get
            {
                return string.IsNullOrEmpty(this.Target) || string.Equals(this.Target, ".", StringComparison.Ordinal);
            }
        }

        /// <summary>
        /// Creates a new instance of this class by consuming xml from the given reader.
        /// Creates an Atom-specific instance annotation from the format-independent representation of an annotation.
        /// </summary>
        /// <param name="odataInstanceAnnotation">The format-independent represetnation of an instance annotation.</param>
        /// <param name="target">The value of the target attribute on the m:annotation element, or null if the attribute should be omitted.</param>
        /// <returns>The created AtomInstanceAnnotation.</returns>
        internal static AtomInstanceAnnotation CreateFrom(ODataInstanceAnnotation odataInstanceAnnotation, string target)
        {
            Debug.Assert(odataInstanceAnnotation != null, "odataInstanceAnnotation != null");

            return new AtomInstanceAnnotation(target, odataInstanceAnnotation.Name, odataInstanceAnnotation.Value);
        }

        /// <summary>
        /// Creates a new instance of this class by consuming xml from the given input context.
        /// </summary>
        /// <param name="inputContext">The input context to use to create the annotation.</param>
        /// <param name="propertyAndValueDeserializer">The property and value deserializer to use when reading values in the annotation element content.</param>
        /// <returns>The <see cref="AtomInstanceAnnotation"/> populated with the information from the 'm:annotation' XML element, as long as the value is a string. Returns null otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The annotation element to read.
        /// Post-Condition:  XmlNodeType.Any        - The node after the end of the annotation element, or the same element as in the pre-condition if the annotation was skipped.
        /// </remarks>
        internal static AtomInstanceAnnotation CreateFrom(ODataAtomInputContext inputContext, ODataAtomPropertyAndValueDeserializer propertyAndValueDeserializer)
        {
            var xmlReader = inputContext.XmlReader;
            Debug.Assert(xmlReader != null, "xmlReader != null");
            Debug.Assert(xmlReader.NodeType == XmlNodeType.Element, "xmlReader must be positioned on an Element");
            Debug.Assert(xmlReader.NameTable != null, "xmlReader.NameTable != null");
            Debug.Assert(xmlReader.LocalName == "annotation", "Must be positioned on an annotation element");

            string termAttributeValue = null;
            string targetAttributeValue = null;
            string typeAttributeValue = null;
            bool nullAttributePresentAndTrue = false;
            bool sawMultipleAttributeValueNotations = false;

            // Notes on "attribute value notation":
            // Empty elements may have the annotation value specified via an attribute on the annotation element.
            // Exactly one of the following attributes must be present if this notation is being used: "string", "int", "bool", "float", "decimal".
            // The value of the annotation is the value of this value-specifying attribute.
            string attributeValueNotationAttributeName = null;
            string attributeValueNotationAttributeValue = null;
            IEdmPrimitiveTypeReference attributeValueNotationTypeReference = null;
            
            XmlNameTable xmlNameTable = xmlReader.NameTable;
            string metadataNamespace = xmlNameTable.Get(AtomConstants.ODataMetadataNamespace);
            string nullAttributeName = xmlNameTable.Get(AtomConstants.ODataNullAttributeName);
            string typeAttributeName = xmlNameTable.Get(AtomConstants.AtomTypeAttributeName);
            string emptyNamespace = xmlNameTable.Get(string.Empty);
            string termAttributeName = xmlNameTable.Get(AtomConstants.ODataAnnotationTermAttribute);
            string targetAttributeName = xmlNameTable.Get(AtomConstants.ODataAnnotationTargetAttribute);

            // Loop through all the attributes and remember the ones specific to annotations.
            while (xmlReader.MoveToNextAttribute())
            {
                if (xmlReader.NamespaceEquals(metadataNamespace))
                {
                    if (xmlReader.LocalNameEquals(typeAttributeName))
                    {
                        typeAttributeValue = xmlReader.Value;
                    }
                    else if (xmlReader.LocalNameEquals(nullAttributeName))
                    {
                        nullAttributePresentAndTrue = ODataAtomReaderUtils.ReadMetadataNullAttributeValue(xmlReader.Value);
                    }

                    // Ignore all other attributes in the metadata namespace.
                    // In general, we only fail on reading if we can't make sense of the document any more. Reader should be loose.
                    // If we choose to start recognizing an additional attribute in the metadata namespace later, be careful not to
                    // fail if it doesn't parse correctly (so that we don't cause a breaking change).
                }
                else if (xmlReader.NamespaceEquals(emptyNamespace))
                {
                    if (xmlReader.LocalNameEquals(termAttributeName))
                    {
                        termAttributeValue = xmlReader.Value;

                        // Before doing any other validation or further reading, check whether or not to read this annotation according to the filter.
                        if (propertyAndValueDeserializer.MessageReaderSettings.ShouldSkipAnnotation(termAttributeValue))
                        {
                            xmlReader.MoveToElement();
                            return null;
                        }
                    }
                    else if (xmlReader.LocalNameEquals(targetAttributeName))
                    {
                        targetAttributeValue = xmlReader.Value;
                    }
                    else
                    {
                        // Check if this attribute is one used by attribute value notation.
                        IEdmPrimitiveTypeReference potentialTypeFromAttributeValueNotation = LookupEdmTypeByAttributeValueNotationName(xmlReader.LocalName);
                        if (potentialTypeFromAttributeValueNotation != null)
                        {
                            // If we've already seen an attribute used for attribute value notation, 
                            // throw since we don't know which type to use (even if the values are the same).
                            // But don't throw yet, because we might not have encountered the term name yet,
                            // and the annotation filter might say to ignore this annotation (and so we shouldn't throw).
                            if (attributeValueNotationTypeReference != null)
                            {
                                sawMultipleAttributeValueNotations = true;
                            }

                            attributeValueNotationTypeReference = potentialTypeFromAttributeValueNotation;
                            attributeValueNotationAttributeName = xmlReader.LocalName;
                            attributeValueNotationAttributeValue = xmlReader.Value;
                        }
                    }

                    // Ignore all other attributes in the empty namespace.
                }

                // Ignore all other attributes in all other namespaces.
            }

            xmlReader.MoveToElement();

            // The term attribute is required.
            if (termAttributeValue == null)
            {
                throw new ODataException(ODataErrorStrings.AtomInstanceAnnotation_MissingTermAttributeOnAnnotationElement);
            }

            if (sawMultipleAttributeValueNotations)
            {
                throw new ODataException(ODataErrorStrings.AtomInstanceAnnotation_MultipleAttributeValueNotationAttributes);
            }
            
            // If this term is defined in the model, look up its type. If the term is not in the model, this will be null.
            IEdmTypeReference expectedTypeReference = MetadataUtils.LookupTypeOfValueTerm(termAttributeValue, propertyAndValueDeserializer.Model);

            ODataValue annotationValue;
            if (nullAttributePresentAndTrue)
            {
                // The m:null attribute has precedence over the content of the element, thus if we find m:null='true' we ignore the content of the element.
                ReaderValidationUtils.ValidateNullValue(
                    propertyAndValueDeserializer.Model, 
                    expectedTypeReference, 
                    propertyAndValueDeserializer.MessageReaderSettings, 
                    /*validateNullValue*/ true,
                    propertyAndValueDeserializer.Version,
                    termAttributeValue);
                annotationValue = new ODataNullValue();
            }
            else if (attributeValueNotationTypeReference != null)
            {
                annotationValue = GetValueFromAttributeValueNotation(
                    expectedTypeReference,
                    attributeValueNotationTypeReference,
                    attributeValueNotationAttributeName,
                    attributeValueNotationAttributeValue,
                    typeAttributeValue,
                    xmlReader.IsEmptyElement,
                    propertyAndValueDeserializer.Model,
                    propertyAndValueDeserializer.MessageReaderSettings,
                    propertyAndValueDeserializer.Version);
            }
            else
            {
                annotationValue = ReadValueFromElementContent(propertyAndValueDeserializer, expectedTypeReference);
            }

            // Read the end tag (or the start tag if it was an empty element).
            xmlReader.Read();

            return new AtomInstanceAnnotation(
                targetAttributeValue,
                termAttributeValue,
                annotationValue);
        }

        /// <summary>
        /// Retrieves the name of the attribute used in attribute value notation to indicate the given primitive type kind.
        /// </summary>
        /// <param name="typeKind">The primitive type kind to look up.</param>
        /// <returns>The name of the corresponding attribute.</returns>
        internal static string LookupAttributeValueNotationNameByEdmTypeKind(EdmPrimitiveTypeKind typeKind)
        {
            switch (typeKind)
            {
                case EdmPrimitiveTypeKind.Int32:
                    return AtomConstants.ODataAnnotationIntAttribute;
                case EdmPrimitiveTypeKind.String:
                    return AtomConstants.ODataAnnotationStringAttribute;
                case EdmPrimitiveTypeKind.Double:
                    return AtomConstants.ODataAnnotationFloatAttribute;
                case EdmPrimitiveTypeKind.Boolean:
                    return AtomConstants.ODataAnnotationBoolAttribute;
                case EdmPrimitiveTypeKind.Decimal:
                    return AtomConstants.ODataAnnotationDecimalAttribute;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the Edm type represented by the given attribute name when using attribute value notation.
        /// </summary>
        /// <param name="attributeName">The name of the attribute (must be one of "string", "int", "bool", "decimal", "float")</param>
        /// <returns>A nullable reference to the type represented by the attribute name, or null if the given name is not a valid attribute value notation name.</returns>
        internal static IEdmPrimitiveTypeReference LookupEdmTypeByAttributeValueNotationName(string attributeName)
        {
            switch (attributeName)
            {
                // Note that we should return non-nullable type references from this method. This is so that the type reference we return here is compatible
                // with the expected type from the model whether the expected type is nullable or not.
                case AtomConstants.ODataAnnotationIntAttribute:
                    return EdmCoreModel.Instance.GetInt32(false);
                case AtomConstants.ODataAnnotationStringAttribute:
                    return EdmCoreModel.Instance.GetString(false);
                case AtomConstants.ODataAnnotationFloatAttribute:
                    return EdmCoreModel.Instance.GetDouble(false);
                case AtomConstants.ODataAnnotationBoolAttribute:
                    return EdmCoreModel.Instance.GetBoolean(false);
                case AtomConstants.ODataAnnotationDecimalAttribute:
                    return EdmCoreModel.Instance.GetDecimal(false);
            }

            return null;
        }

        /// <summary>
        /// Reads the current element's content as an ODataValue.
        /// </summary>
        /// <param name="propertyAndValueDeserializer">The property and value deserializer to use to read values in ATOM.</param>
        /// <param name="expectedType">The expected type of the annotation, may be null if the term is not defined in the model.</param>
        /// <returns>The deserialized value.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The XML element containing the value to read (also the attributes will be read from it)
        /// Post-Condition:  XmlNodeType.EndElement - The end tag of the element.
        ///                  XmlNodeType.Element    - The empty element node.
        /// </remarks>
        private static ODataValue ReadValueFromElementContent(ODataAtomPropertyAndValueDeserializer propertyAndValueDeserializer, IEdmTypeReference expectedType)
        {
            object result = propertyAndValueDeserializer.ReadNonEntityValue(
                expectedType,
                null /*duplicatePropertyNamesChecker*/,
                null /*collectionValidator*/,
                true /*validateNullValue*/);

            return result.ToODataValue();
        }

        /// <summary>
        /// Reads an annotation's value from the annotation value notation specified on the current element. 
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference of the vocabulary term from the metadata.</param>
        /// <param name="attributeValueNotationTypeReference">The type reference indicated by the name of the attribute used in attribute value notation. 
        ///   For example, if the attribute was called "string", this will be a reference to the string type.</param>
        /// <param name="attributeValueNotationAttributeName">The name of the attribute used by attribute avalue notation.</param>
        /// <param name="attributeValueNotationAttributeValue">The value of the attribute used by attribute value notation.</param>
        /// <param name="typeAttributeValue">The value of the "m:type" attribute on the annotation element.</param>
        /// <param name="positionedOnEmptyElement">true if the annotation element is empty, false otherwise.</param>
        /// <param name="model">The edm model instance.</param>
        /// <param name="messageReaderSettings">The message reader settings instance.</param>
        /// <param name="version">The payload version to read.</param>
        /// <returns>The primitive value represented on this element via attribute value notation.</returns>
        private static ODataPrimitiveValue GetValueFromAttributeValueNotation(
            IEdmTypeReference expectedTypeReference,
            IEdmPrimitiveTypeReference attributeValueNotationTypeReference,
            string attributeValueNotationAttributeName,
            string attributeValueNotationAttributeValue,
            string typeAttributeValue,
            bool positionedOnEmptyElement,
            IEdmModel model,
            ODataMessageReaderSettings messageReaderSettings,
            ODataVersion version)
        {
            Debug.Assert(attributeValueNotationTypeReference != null, "attributeValueNotationTypeReference != null");

            if (!positionedOnEmptyElement)
            {
                // If there is content in the body of the element, throw since it's ambiguous whether we should use the value from the attribute or the element content.
                throw new ODataException(ODataErrorStrings.AtomInstanceAnnotation_AttributeValueNotationUsedOnNonEmptyElement(attributeValueNotationAttributeName));
            }

            // If both m:type is present and attribute value notation is being used, they must match.
            // For example, if m:type is "Edm.Int32", but the "string" attribute is also present, we should throw.
            if (typeAttributeValue != null && !string.Equals(attributeValueNotationTypeReference.Definition.ODataFullName(), typeAttributeValue, StringComparison.Ordinal))
            {
                throw new ODataException(ODataErrorStrings.AtomInstanceAnnotation_AttributeValueNotationUsedWithIncompatibleType(typeAttributeValue, attributeValueNotationAttributeName));
            }
            
            IEdmTypeReference targetTypeReference = ReaderValidationUtils.ResolveAndValidatePrimitiveTargetType(
                expectedTypeReference,
                EdmTypeKind.Primitive,
                attributeValueNotationTypeReference.Definition,
                attributeValueNotationTypeReference.ODataFullName(),
                attributeValueNotationTypeReference.Definition,
                model,
                messageReaderSettings,
                version);
            return new ODataPrimitiveValue(AtomValueUtils.ConvertStringToPrimitive(attributeValueNotationAttributeValue, targetTypeReference.AsPrimitive()));
        }
    }
}
