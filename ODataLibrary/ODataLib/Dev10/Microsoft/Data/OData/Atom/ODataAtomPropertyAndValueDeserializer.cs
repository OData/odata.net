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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for properties and value types.
    /// </summary>
    internal class ODataAtomPropertyAndValueDeserializer : ODataAtomDeserializer
    {
        #region Atomized strings
        /// <summary>OData attribute which indicates the null value for the element.</summary>
        protected readonly string ODataNullAttributeName;

        /// <summary>Element name for the items in a MultiValue.</summary>
        protected readonly string ODataMultiValueItemElementName;

        /// <summary>XML element name to mark type attribute in Atom.</summary>
        protected readonly string AtomTypeAttributeName;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomPropertyAndValueDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.ODataNullAttributeName = nameTable.Add(AtomConstants.ODataNullAttributeName);
            this.ODataMultiValueItemElementName = nameTable.Add(AtomConstants.ODataMultiValueItemElementName);
            this.AtomTypeAttributeName = nameTable.Add(AtomConstants.AtomTypeAttributeName);
        }

        /// <summary>
        /// This method creates and reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal ODataProperty ReadTopLevelProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            Debug.Assert(this.XmlReader != null, "this.xmlReader != null");

            this.ReadPayloadStart();
            Debug.Assert(this.XmlReader.NodeType == XmlNodeType.Element, "The XML reader must be positioned on an Element.");

            // For compatibility with WCF DS Server we need to be able to read the property element in any namespace, not just the OData namespace.
            if (this.MessageReaderSettings.ReaderBehavior.BehaviorKind != ODataBehaviorKind.WcfDataServicesServer && !this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
            {
                throw new ODataException(Strings.ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace(this.XmlReader.NamespaceURI));
            }

            ODataProperty property = this.ReadProperty(expectedPropertyTypeReference);

            this.ReadPayloadEnd();

            return property;
        }

        /// <summary>
        /// Reads the primitive, complex or multi value.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use (cached), or null if new one should be created.</param>
        /// <returns>The value read (null, primitive CLR value, ODataComplexValue or ODataMultiValue).</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The XML element containing the value to read (also the attributes will be read from it)
        /// Post-Condition:  XmlNodeType.EndElement - The end tag of the element.
        ///                  XmlNodeType.Element    - The empty element node.
        /// </remarks>
        internal object ReadNonEntityValue(IEdmTypeReference expectedValueTypeReference, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            DebugUtils.CheckNoExternalCallers();
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                expectedValueTypeReference == null || !expectedValueTypeReference.IsODataEntityTypeKind(),
                "Only primitive, complex or multivalue types can be read by this method.");
            this.XmlReader.AssertNotBuffering();

            // Read the attributes looking for m:type and m:null
            string payloadTypeName = null;
            bool isNull = false;
            while (this.XmlReader.MoveToNextAttribute())
            {
                if (!this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    continue;
                }

                if (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName))
                {
                    // m:type
                    payloadTypeName = this.XmlReader.Value;
                    ODataAtomReaderUtils.ValidateTypeName(payloadTypeName);
                }
                else if (this.XmlReader.LocalNameEquals(this.ODataNullAttributeName))
                {
                    // m:null
                    isNull = ODataAtomReaderUtils.ReadMetadataNullAttributeValue(this.XmlReader.Value);
                }
                else
                {
                    // Ignore all other attributes in the metadata namespace
                    continue;
                }
            }

            this.XmlReader.MoveToElement();

            // Resolve the payload type name.
            // If the payload type is not recognized then it means (in here) that we should assume complex (since MultiValue or Primitive is always recognized).
            EdmTypeKind payloadValueTypeKind;
            IEdmType payloadValueType = ReaderValidationUtils.ResolvePayloadTypeName(
                this.Model,
                payloadTypeName,
                expectedValueTypeReference == null ? EdmTypeKind.Complex : expectedValueTypeReference.TypeKind(),
                out payloadValueTypeKind);

            // Determine the type kind
            EdmTypeKind expectedValueTypeKind;
            if (expectedValueTypeReference != null)
            {
                // If we have an expected type, use that.
                expectedValueTypeKind = expectedValueTypeReference.TypeKind();
            }
            else if (payloadValueTypeKind != EdmTypeKind.None)
            {
                // If we have a type kind based on the type name, use it.
                ValidationUtils.ValidateValueTypeKind(payloadValueTypeKind, payloadTypeName);
                expectedValueTypeKind = payloadValueTypeKind;
            }
            else
            {
                // We don't have expected or payload type, so determine the type from the payload shape.
                expectedValueTypeKind = this.GetNonEntityValueKind(isNull);
            }

            Debug.Assert(expectedValueTypeKind != EdmTypeKind.None, "We should have determined the type kind by now.");

            // Resolve potential conflicts between payload and expected types and apply all the various behavior changing flags from settings
            IEdmTypeReference targetTypeReference;
            SerializationTypeNameAnnotation serializationTypeNameAnnotation = null;
            if (expectedValueTypeKind == EdmTypeKind.Primitive)
            {
                targetTypeReference = ODataAtomReaderUtils.ResolveAndValidatePrimitiveTargetType(
                    expectedValueTypeReference,
                    payloadValueTypeKind,
                    payloadValueType,
                    payloadTypeName,
                    this.Model,
                    this.MessageReaderSettings);
            }
            else
            {
                targetTypeReference = ReaderValidationUtils.ResolveAndValidateTargetType(
                    expectedValueTypeKind,
                    expectedValueTypeReference,
                    payloadValueTypeKind,
                    payloadValueType,
                    payloadTypeName,
                    this.Model,
                    this.MessageReaderSettings,
                    out serializationTypeNameAnnotation);
            }

            object result;
            if (isNull)
            {
                // The m:null attribute has a precedence over the content of the element, thus if we find m:null='true' we ignore the content of the element.
                this.XmlReader.SkipElementContent();

                // If we don't have an actualValueType it means that it was not specified in the payload
                // and the expected one was not available or should not be used. In that case we default to Edm.String
                // for null value since there's no other way to tell the difference between primitive and any other type.
                // For non-null payloads we will still try to figure out the kind from the payload shape.
                // Note that this applies to no-model parsing as well (that's why we have to do it again here, since the ResolveAndValidateTargetType
                // above doesn't solve the no-model case.
                if (targetTypeReference == null)
                {
                    targetTypeReference = EdmCoreModel.Instance.GetString(true);
                }

                ReaderValidationUtils.ValidateNullValue(targetTypeReference);
                result = null;
            }
            else
            {
                switch (expectedValueTypeKind)
                {
                    case EdmTypeKind.Primitive:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                        result = this.ReadPrimitiveValue(targetTypeReference.AsPrimitiveOrNull());
                        break;

                    case EdmTypeKind.Complex:
                        ODataComplexValue complexValue = this.ReadComplexValue(targetTypeReference.AsComplexOrNull(), payloadTypeName, duplicatePropertyNamesChecker);
                        if (serializationTypeNameAnnotation != null)
                        {
                            complexValue.SetAnnotation(serializationTypeNameAnnotation);
                        }

                        result = complexValue;
                        break;

                    case EdmTypeKind.Collection:
                        IEdmCollectionTypeReference multiValueTypeReference = ValidationUtils.ValidateMultiValueType(targetTypeReference);
                        ODataMultiValue multiValue = this.ReadMultiValue(multiValueTypeReference, payloadTypeName);
                        if (serializationTypeNameAnnotation != null)
                        {
                            multiValue.SetAnnotation(serializationTypeNameAnnotation);
                        }

                        result = multiValue;
                        break;

                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataAtomPropertyAndValueDeserializer_ReadNonEntityValue));
                }
            }

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            this.XmlReader.AssertNotBuffering();
            return result;
        }

        /// <summary>
        /// Reads the content of a properties in an element (complex value, m:properties, ...)
        /// </summary>
        /// <param name="structuredType">The type which should declare the properties to be read. Optional.</param>
        /// <param name="properties">The list of properties to add properties to.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element    - The element to read properties from.
        /// Post-Condition: XmlNodeType.Element    - The element to read properties from if it is an empty element.
        ///                 XmlNodeType.EndElement - The end element of the element to read properties from.
        /// </remarks>
        protected void ReadProperties(IEdmStructuredType structuredType, List<ODataProperty> properties, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            Debug.Assert(properties != null, "properties != null");
            Debug.Assert(duplicatePropertyNamesChecker != null, "duplicatePropertyNamesChecker != null");
            this.AssertXmlCondition(XmlNodeType.Element);

            // Empty values are valid - they have no properties
            if (!this.XmlReader.IsEmptyElement)
            {
                // Read over the complex value element to its first child node (or end-element)
                this.XmlReader.ReadStartElement();

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
                            {
                                // Found a property
                                IEdmProperty edmProperty;
                                bool isOpen = false;
                                if (structuredType != null)
                                {
                                    // Lookup the property in metadata
                                    edmProperty = ValidationUtils.ValidatePropertyDefined(this.XmlReader.LocalName, structuredType);

                                    // If the property was not declared, it must be open.
                                    isOpen = edmProperty == null;
                                }
                                else
                                {
                                    // XmlReader already validates that XML element names are not null or empty
                                    // so no need to validate the property name here.
                                    edmProperty = null;
                                }

                                ODataProperty property = this.ReadProperty(edmProperty == null ? null : edmProperty.Type);

                                if (isOpen)
                                {
                                    ValidationUtils.ValidateOpenPropertyValue(property.Name, property.Value);
                                }

                                duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
                                properties.Add(property);
                            }
                            else
                            {
                                this.XmlReader.Skip();
                            }

                            break;

                        case XmlNodeType.EndElement:
                            // End of the complex value.
                            break;

                        default:
                            // Non-element so for example a text node, just ignore
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }
        }

        /// <summary>
        /// Reads a property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property value.</param>
        /// <returns>The ODataProperty representing the property in question.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element - The XML element representing the property to read.
        ///                                        Note that the method does NOT check for the property name neither it resolves the property against metadata.
        /// Post-Condition:  Any                 - The node after the property.
        /// </remarks>
        private ODataProperty ReadProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            Debug.Assert(
                expectedPropertyTypeReference == null || expectedPropertyTypeReference.IsODataPrimitiveTypeKind() ||
                expectedPropertyTypeReference.IsODataComplexTypeKind() || expectedPropertyTypeReference.IsODataMultiValueTypeKind(),
                "Only primitive, complex and multivalue types can be read by this method.");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                this.MessageReaderSettings.ReaderBehavior.BehaviorKind == ODataBehaviorKind.WcfDataServicesServer || this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace),
                "Property elements must be in the OData namespace");
            this.XmlReader.AssertNotBuffering();

            ODataProperty property = new ODataProperty();
            property.Name = this.XmlReader.LocalName;
            property.Value = this.ReadNonEntityValue(expectedPropertyTypeReference, null);

            // Read past the end tag of the property or the start tag if the element is empty.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
            return property;
        }

        /// <summary>
        /// Determines the kind of value to read based on the payload shape.
        /// </summary>
        /// <param name="isNull">true if the value is null (has the m:null='true'); false otherwise.</param>
        /// <returns>The kind of type of the value to read.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element -   The XML element containing the value to get the kind for.
        /// Post-Condition:  XmlNodeType.Element -   The XML element containing the value to get the kind for.
        /// </remarks>
        private EdmTypeKind GetNonEntityValueKind(bool isNull)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();

            if (isNull)
            {
                // Null values without any type information should be assumed to be primitive values
                return EdmTypeKind.Primitive;
            }

            if (this.XmlReader.IsEmptyElement)
            {
                // Empty element is considered to be a primitive value (which means Edm.String since we don't have a payload type)
                return EdmTypeKind.Primitive;
            }

            this.XmlReader.StartBuffering();

            try
            {
                // Move to the first node of the content of the value element.
                this.XmlReader.Read();

                bool foundMultivalueItem = false;
                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.ODataMultiValueItemElementName))
                                {
                                    // Note that even if we've already seen another d:element element
                                    // it can still be a complex value since in some cases we allow duplicate properties
                                    // in complex values, and thus we have to keep looking and only if we se just d:element
                                    // ones then we can assume it's a multivalue.
                                    foundMultivalueItem = true;
                                }
                                else
                                {
                                    // Element in the "d" namespace but not called "element" -> must be a complex value
                                    return EdmTypeKind.Complex;
                                }
                            }

                            this.XmlReader.Skip();
                            break;

                        case XmlNodeType.EndElement:
                            break;

                        default:
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);

                // If we've found at least one d:element and no other elements in the "d" namespace then it's a multivalue.
                // If we didn't find any elements in the "d" namespace then we treat this as a string value -> primitive.
                return foundMultivalueItem ? EdmTypeKind.Collection : EdmTypeKind.Primitive;
            }
            finally
            {
                this.XmlReader.StopBuffering();
            }
        }

        /// <summary>
        /// Read a primitive value from the reader.
        /// </summary>
        /// <param name="actualValueTypeReference">The type of the value to read.</param>
        /// <returns>The value read from the payload and converted as appropriate to the target type.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        ///                  
        /// Note that this method will not read null values, those should be handled by the caller already.
        /// </remarks>
        private object ReadPrimitiveValue(IEdmPrimitiveTypeReference actualValueTypeReference)
        {
            Debug.Assert(actualValueTypeReference != null, "actualValueTypeReference != null");
            Debug.Assert(actualValueTypeReference.TypeKind() == EdmTypeKind.Primitive, "Only primitive values can be read by this method.");
            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.Attribute);

            object result = AtomValueUtils.ReadPrimitiveValue(this.XmlReader, actualValueTypeReference);

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(result != null, "The method should never return null since it doesn't handle null values.");

            return result;
        }

        /// <summary>
        /// Read a complex value from the reader.
        /// </summary>
        /// <param name="complexTypeReference">The type reference of the value to read (or null if no type is available).</param>
        /// <param name="payloadTypeName">The name of the type specified in the payload.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use (cached), or null if new one should be created.</param>
        /// <returns>The value read from the payload.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        ///                  
        /// Note that this method will not read null values, those should be handled by the caller already.
        /// </remarks>
        private ODataComplexValue ReadComplexValue(IEdmComplexTypeReference complexTypeReference, string payloadTypeName, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker)
        {
            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.Attribute);

            ODataComplexValue complexValue = new ODataComplexValue();
            IEdmComplexType complexType = complexTypeReference == null ? null : (IEdmComplexType)complexTypeReference.Definition;

            // If we have a metadata type for the complex value, use that type name
            // otherwise use the type name from the payload (if there was any).
            complexValue.TypeName = complexTypeReference == null ? payloadTypeName : complexType.ODataFullName();

            // Move to the element (so that if we were on an attribute we can test the element for being empty)
            this.XmlReader.MoveToElement();

            if (duplicatePropertyNamesChecker == null)
            {
                duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
            }
            else
            {
                duplicatePropertyNamesChecker.Clear();
            }

            List<ODataProperty> properties = new List<ODataProperty>();
            this.ReadProperties(complexType, properties, duplicatePropertyNamesChecker);
            complexValue.Properties = new ReadOnlyEnumerable<ODataProperty>(properties);

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(complexValue != null, "The method should never return null since it doesn't handle null values.");

            return complexValue;
        }

        /// <summary>
        /// Read a multivalue from the reader.
        /// </summary>
        /// <param name="multiValueTypeReference">The type of the multivalue to read (or null if no type is available).</param>
        /// <param name="payloadTypeName">The name of the multivalue type specified in the payload.</param>
        /// <returns>The value read from the payload.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        ///                  
        /// Note that this method will not read null values, those should be handled by the caller already.
        /// </remarks>
        private ODataMultiValue ReadMultiValue(IEdmCollectionTypeReference multiValueTypeReference, string payloadTypeName)
        {
            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.Attribute);
            Debug.Assert(
                multiValueTypeReference == null || multiValueTypeReference.IsODataMultiValueTypeKind(),
                "If the metadata is specified it must denote a MultiValue for this method to work.");

            ODataMultiValue multiValue = new ODataMultiValue();

            // If we have a metadata type for the multivalue, use that type name
            // otherwise use the type name from the payload (if there was any).
            multiValue.TypeName = multiValueTypeReference == null ? payloadTypeName : multiValueTypeReference.ODataFullName();

            // Move to the element (so that if we were on an attribute we can test the element for being empty)
            this.XmlReader.MoveToElement();

            List<object> items = new List<object>();

            // Empty multivalues are valid - they have no items
            if (!this.XmlReader.IsEmptyElement)
            {
                // Read over the multivalue element to its first child node (or end-element)
                this.XmlReader.ReadStartElement();

                IEdmTypeReference itemTypeReference = multiValueTypeReference == null ? null : multiValueTypeReference.ElementType();

                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
                            {
                                if (!this.XmlReader.LocalNameEquals(this.ODataMultiValueItemElementName))
                                {
                                    throw new ODataException(Strings.ODataAtomPropertyAndValueDeserializer_InvalidMultiValueElement(this.XmlReader.LocalName));
                                }

                                // Found an item
                                object itemValue = this.ReadNonEntityValue(itemTypeReference, duplicatePropertyNamesChecker);

                                // read over the end tag of the element or the start tag if the element was empty.
                                this.XmlReader.Read();

                                ValidationUtils.ValidateMultiValueItem(itemValue);

                                // Note that the ReadNonEntityValue already validated that the actual type of the value matches
                                // the expected type (the itemType).
                                items.Add(itemValue);
                            }
                            else
                            {
                                this.XmlReader.Skip();
                            }

                            break;

                        case XmlNodeType.EndElement:
                            // End of the multivalue.
                            break;

                        default:
                            // Non-element so for example a text node, just ignore
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }

            multiValue.Items = new ReadOnlyEnumerable(items);

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(multiValue != null, "The method should never return null since it doesn't handle null values.");

            return multiValue;
        }
    }
}
