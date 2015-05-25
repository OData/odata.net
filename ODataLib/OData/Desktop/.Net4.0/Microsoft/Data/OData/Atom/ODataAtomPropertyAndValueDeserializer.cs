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

namespace Microsoft.Data.OData.Atom
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// OData ATOM deserializer for properties and value types.
    /// </summary>
    internal class ODataAtomPropertyAndValueDeserializer : ODataAtomDeserializer
    {
        #region Atomized strings
        /// <summary>The empty namespace used for attributes in no namespace.</summary>
        protected readonly string EmptyNamespace;

        /// <summary>OData attribute which indicates the null value for the element.</summary>
        protected readonly string ODataNullAttributeName;

        /// <summary>Element name for the items in a Collection.</summary>
        protected readonly string ODataCollectionItemElementName;

        /// <summary>XML element name to mark type attribute in Atom.</summary>
        protected readonly string AtomTypeAttributeName;
        #endregion

        /// <summary>The Edm.String type from the core model.</summary>
        private static readonly IEdmType edmStringType = EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.String);

        /// <summary>The current recursion depth of values read by this deserializer, measured by the number of complex and collection values read so far.</summary>
        private int recursionDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="atomInputContext">The ATOM input context to read from.</param>
        internal ODataAtomPropertyAndValueDeserializer(ODataAtomInputContext atomInputContext)
            : base(atomInputContext)
        {
            DebugUtils.CheckNoExternalCallers();

            XmlNameTable nameTable = this.XmlReader.NameTable;
            this.EmptyNamespace = nameTable.Add(string.Empty);
            this.ODataNullAttributeName = nameTable.Add(AtomConstants.ODataNullAttributeName);
            this.ODataCollectionItemElementName = nameTable.Add(AtomConstants.ODataCollectionItemElementName);
            this.AtomTypeAttributeName = nameTable.Add(AtomConstants.AtomTypeAttributeName);
        }

        /// <summary>
        /// This method creates and reads the property from the input and 
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedProperty">The <see cref="IEdmProperty"/> producing the property to be read.</param>
        /// <param name="expectedPropertyTypeReference">The expected type of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal ODataProperty ReadTopLevelProperty(IEdmStructuralProperty expectedProperty, IEdmTypeReference expectedPropertyTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            Debug.Assert(this.XmlReader != null, "this.xmlReader != null");

            this.ReadPayloadStart();
            Debug.Assert(this.XmlReader.NodeType == XmlNodeType.Element, "The XML reader must be positioned on an Element.");

            // For compatibility with WCF DS Server we need to be able to read the property element in any namespace, not just the OData namespace.
            if (!this.UseServerFormatBehavior && !this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
            {
                throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueDeserializer_TopLevelPropertyElementWrongNamespace(this.XmlReader.NamespaceURI, this.XmlReader.ODataNamespace));
            }

            // this is a top level property so EPM does not apply hence it is safe to say that EPM is not present
            this.AssertRecursionDepthIsZero();
            string expectedPropertyName = ReaderUtils.GetExpectedPropertyName(expectedProperty);
            ODataProperty property = this.ReadProperty(
                expectedPropertyName,
                expectedPropertyTypeReference,
                /*nullValueReadBehaviorKind*/ ODataNullValueBehaviorKind.Default,
                /* epmPresent */false);
            this.AssertRecursionDepthIsZero();

            Debug.Assert(property != null, "If we don't ignore null values the property must not be null.");

            this.ReadPayloadEnd();

            return property;
        }

        /// <summary>
        /// Reads the primitive, complex or collection value.
        /// </summary>
        /// <param name="expectedValueTypeReference">The expected type reference of the value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use (cached), or null if new one should be created.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate a null value (i.e., throw if a null value is being written for a non-nullable property); otherwise false.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <returns>The value read (null, primitive CLR value, ODataComplexValue or ODataCollectionValue).</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The XML element containing the value to read (also the attributes will be read from it)
        /// Post-Condition:  XmlNodeType.EndElement - The end tag of the element.
        ///                  XmlNodeType.Element    - The empty element node.
        /// </remarks>
        internal object ReadNonEntityValue(
            IEdmTypeReference expectedValueTypeReference,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            bool epmPresent)
        {
            DebugUtils.CheckNoExternalCallers();

            this.AssertRecursionDepthIsZero();
            object nonEntityValue = this.ReadNonEntityValueImplementation(
                expectedValueTypeReference,
                duplicatePropertyNamesChecker,
                collectionValidator,
                validateNullValue,
                epmPresent,
                /*propertyName*/ null);
            this.AssertRecursionDepthIsZero();

            return nonEntityValue;
        }

        /// <summary>
        /// Determines the kind of value to read based on the payload shape.
        /// </summary>
        /// <returns>The kind of type of the value to read.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element -   The XML element containing the value to get the kind for.
        /// Post-Condition:  XmlNodeType.Element -   The XML element containing the value to get the kind for.
        /// </remarks>
        protected EdmTypeKind GetNonEntityValueKind()
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();

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

                bool foundCollectionItem = false;
                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
                            {
                                if (this.XmlReader.LocalNameEquals(this.ODataCollectionItemElementName) && this.Version >= ODataVersion.V3)
                                {
                                    // Note that even if we've already seen another d:element element
                                    // it can still be a complex value since in some cases we allow duplicate properties
                                    // in complex values, and thus we have to keep looking and only if we se just d:element
                                    // ones then we can assume it's a collection.
                                    foundCollectionItem = true;
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

                // If we've found at least one d:element and no other elements in the "d" namespace then it's a collection.
                // If we didn't find any elements in the "d" namespace then we treat this as a string value -> primitive.
                return foundCollectionItem ? EdmTypeKind.Collection : EdmTypeKind.Primitive;
            }
            finally
            {
                this.XmlReader.StopBuffering();
            }
        }

        /// <summary>
        /// Reads the 'type' and 'isNull' attributes of a value.
        /// </summary>
        /// <param name="typeName">The value of the 'type' attribute or null if no 'type' attribute exists.</param>
        /// <param name="isNull">The value of the 'isNull' attribute or null if no 'isNull' attribute exists.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element    - The element to read attributes from.
        /// Post-Condition: XmlNodeType.Element    - The element to read attributes from.
        /// </remarks>
        protected void ReadNonEntityValueAttributes(out string typeName, out bool isNull)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();

            typeName = null;
            isNull = false;

            while (this.XmlReader.MoveToNextAttribute())
            {
                if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataMetadataNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName))
                    {
                        // m:type
                        typeName = this.XmlReader.Value;
                    }
                    else if (this.XmlReader.LocalNameEquals(this.ODataNullAttributeName))
                    {
                        // m:null
                        isNull = ODataAtomReaderUtils.ReadMetadataNullAttributeValue(this.XmlReader.Value);

                        // Once we find m:null we stop reading further since m:null trumps any other
                        // content (attributes or elements)
                        break;
                    }

                    //// Ignore all other attributes in the metadata namespace
                }
                else if (this.UseClientFormatBehavior && this.XmlReader.NamespaceEquals(this.EmptyNamespace))
                {
                    if (this.XmlReader.LocalNameEquals(this.AtomTypeAttributeName))
                    {
                        // type
                        typeName = typeName ?? this.XmlReader.Value;
                    }

                    //// Ignore all other attributes in the empty namespace
                }

                //// Ignore all other attributes in all other namespaces
            }

            this.XmlReader.MoveToElement();
            this.AssertXmlCondition(XmlNodeType.Element);
            this.XmlReader.AssertNotBuffering();
        }

        /// <summary>
        /// Reads the content of a properties in an element (complex value, m:properties, ...)
        /// </summary>
        /// <param name="structuredType">The type which should declare the properties to be read. Optional.</param>
        /// <param name="properties">The list of properties to add properties to.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element    - The element to read properties from.
        /// Post-Condition: XmlNodeType.Element    - The element to read properties from if it is an empty element.
        ///                 XmlNodeType.EndElement - The end element of the element to read properties from.
        /// </remarks>
        protected void ReadProperties(IEdmStructuredType structuredType, ReadOnlyEnumerable<ODataProperty> properties, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool epmPresent)
        {
            this.AssertRecursionDepthIsZero();
            this.ReadPropertiesImplementation(
                structuredType,
                properties,
                duplicatePropertyNamesChecker,
                epmPresent);
            this.AssertRecursionDepthIsZero();
        }

        /// <summary>
        /// Reads the primitive, complex or collection value.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference of the value.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use (cached), or null if new one should be created.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate a null value (i.e., throw if a null value is being written for a non-nullable property); otherwise false.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The value read (null, primitive CLR value, ODataComplexValue or ODataCollectionValue).</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element    - The XML element containing the value to read (also the attributes will be read from it)
        /// Post-Condition:  XmlNodeType.EndElement - The end tag of the element.
        ///                  XmlNodeType.Element    - The empty element node.
        /// </remarks>
        private object ReadNonEntityValueImplementation(IEdmTypeReference expectedTypeReference, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, CollectionWithoutExpectedTypeValidator collectionValidator, bool validateNullValue, bool epmPresent, string propertyName)
        {
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(
                expectedTypeReference == null || !expectedTypeReference.IsODataEntityTypeKind(),
                "Only primitive, complex or collection types can be read by this method.");
            Debug.Assert(
                expectedTypeReference == null || collectionValidator == null,
                "If an expected value type reference is specified, no collection validator must be provided.");
            this.XmlReader.AssertNotBuffering();

            // Read the attributes looking for m:type and m:null
            string payloadTypeName;
            bool isNull;
            this.ReadNonEntityValueAttributes(out payloadTypeName, out isNull);

            object result;
            if (isNull)
            {
                result = this.ReadNullValue(expectedTypeReference, validateNullValue, propertyName);
            }
            else
            {
                // If we could derive the item type name from the collection's type name and no type name was specified in the payload
                // fill it in now.
                EdmTypeKind payloadTypeKind;
                bool derivedItemTypeNameFromCollectionTypeName = false;
                if (collectionValidator != null && payloadTypeName == null)
                {
                    payloadTypeName = collectionValidator.ItemTypeNameFromCollection;
                    payloadTypeKind = collectionValidator.ItemTypeKindFromCollection;
                    derivedItemTypeNameFromCollectionTypeName = payloadTypeKind != EdmTypeKind.None;
                }

                // Resolve the payload type name and compute the target type kind and target type reference.
                SerializationTypeNameAnnotation serializationTypeNameAnnotation;
                EdmTypeKind targetTypeKind;
                IEdmTypeReference targetTypeReference = ReaderValidationUtils.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*defaultPrimitivePayloadType*/ edmStringType,
                    expectedTypeReference,
                    payloadTypeName,
                    this.Model,
                    this.MessageReaderSettings,
                    this.Version,
                    this.GetNonEntityValueKind,
                    out targetTypeKind,
                    out serializationTypeNameAnnotation);

                if (derivedItemTypeNameFromCollectionTypeName)
                {
                    Debug.Assert(
                        serializationTypeNameAnnotation == null,
                        "If we derived the item type name from the collection type name we must not have created a serialization type name annotation.");
                    serializationTypeNameAnnotation = new SerializationTypeNameAnnotation { TypeName = null };
                }

                // If we have no expected type make sure the collection items are of the same kind and specify the same name.
                if (collectionValidator != null)
                {
                    Debug.Assert(expectedTypeReference == null, "If a collection validator is specified there must not be an expected value type reference.");
                    collectionValidator.ValidateCollectionItem(payloadTypeName, targetTypeKind);
                }

                switch (targetTypeKind)
                {
                    case EdmTypeKind.Primitive:
                        Debug.Assert(targetTypeReference != null && targetTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                        result = this.ReadPrimitiveValue(targetTypeReference.AsPrimitive());
                        break;

                    case EdmTypeKind.Complex:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsComplex(), "Expected a complex type.");
                        result = this.ReadComplexValue(
                            targetTypeReference == null ? null : targetTypeReference.AsComplex(),
                            payloadTypeName,
                            serializationTypeNameAnnotation,
                            duplicatePropertyNamesChecker,
                            epmPresent);
                        break;

                    case EdmTypeKind.Collection:
                        IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(targetTypeReference);
                        result = this.ReadCollectionValue(
                            collectionTypeReference,
                            payloadTypeName,
                            serializationTypeNameAnnotation);
                        break;

                    default:
                        throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.ODataAtomPropertyAndValueDeserializer_ReadNonEntityValue));
                }
            }

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            this.XmlReader.AssertNotBuffering();
            return result;
        }

        /// <summary>
        /// Read a null value from the payload.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type reference (for validation purposes).</param>
        /// <param name="validateNullValue">true to validate the value against the <paramref name="expectedTypeReference"/>.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The null value.</returns>
        private object ReadNullValue(IEdmTypeReference expectedTypeReference, bool validateNullValue, string propertyName)
        {
            // The m:null attribute has a precedence over the content of the element, thus if we find m:null='true' we ignore the content of the element.
            this.XmlReader.SkipElementContent();

            // NOTE: when reading a null value we will never ask the type resolver (if present) to resolve the
            //       type; we always fall back to the expected type.
            ReaderValidationUtils.ValidateNullValue(this.Model, expectedTypeReference, this.MessageReaderSettings, validateNullValue, this.Version, propertyName);

            return null;
        }

        /// <summary>
        /// Reads the content of a properties in an element (complex value, m:properties, ...)
        /// </summary>
        /// <param name="structuredType">The type which should declare the properties to be read. Optional.</param>
        /// <param name="properties">The list of properties to add properties to.</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <remarks>
        /// Pre-Condition:  XmlNodeType.Element    - The element to read properties from.
        /// Post-Condition: XmlNodeType.Element    - The element to read properties from if it is an empty element.
        ///                 XmlNodeType.EndElement - The end element of the element to read properties from.
        /// </remarks>
        private void ReadPropertiesImplementation(IEdmStructuredType structuredType, ReadOnlyEnumerable<ODataProperty> properties, DuplicatePropertyNamesChecker duplicatePropertyNamesChecker, bool epmPresent)
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
                                IEdmProperty edmProperty = null;
                                bool isOpen = false;
                                bool ignoreProperty = false;

                                if (structuredType != null)
                                {
                                    // Lookup the property in metadata
                                    edmProperty = ReaderValidationUtils.ValidateValuePropertyDefined(this.XmlReader.LocalName, structuredType, this.MessageReaderSettings, out ignoreProperty);
                                    if (edmProperty != null && edmProperty.PropertyKind == EdmPropertyKind.Navigation)
                                    {
                                        throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueDeserializer_NavigationPropertyInProperties(edmProperty.Name, structuredType));
                                    }

                                    // If the property was not declared, it must be open.
                                    isOpen = edmProperty == null;
                                }

                                ODataProperty property = null;
                                if (ignoreProperty)
                                {
                                    property = this.ReadUndeclarePropertyOrSkip();
                                }
                                else
                                {
                                    ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse || edmProperty == null
                                        ? ODataNullValueBehaviorKind.Default
                                        : this.Model.NullValueReadBehaviorKind(edmProperty);
                                    property = this.ReadProperty(
                                        edmProperty == null ? null : edmProperty.Name,
                                        edmProperty == null ? null : edmProperty.Type,
                                        nullValueReadBehaviorKind,
                                        epmPresent);
                                    Debug.Assert(
                                        property != null || nullValueReadBehaviorKind == ODataNullValueBehaviorKind.IgnoreValue,
                                        "If we don't ignore null values the property must not be null.");
                                }

                                if (property != null)
                                {
                                    if (isOpen)
                                    {
                                        ValidationUtils.ValidateOpenPropertyValue(property.Name, property.Value);
                                    }

                                    duplicatePropertyNamesChecker.CheckForDuplicatePropertyNames(property);
                                    properties.AddToSourceList(property);
                                }
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
        /// Reads undeclared property in an element (primitive type value, or null value of complex type)
        /// </summary>
        /// <returns>The result ODataProperty.</returns>
        private ODataProperty ReadUndeclarePropertyOrSkip()
        {
            ODataProperty property = new ODataProperty();
            string propertyName = this.XmlReader.LocalName;
            ValidationUtils.ValidatePropertyName(propertyName);
            property.Name = propertyName;

            // Read the attributes looking for m:type and m:null
            string payloadTypeName;
            bool isNull;
            this.ReadNonEntityValueAttributes(out payloadTypeName, out isNull);

            // only read primitive (Edm.) type values, ignore collection/complex values.
            if ((string.IsNullOrEmpty(payloadTypeName) || payloadTypeName.StartsWith("Edm.", System.StringComparison.Ordinal)))
            {
                IEdmPrimitiveTypeReference targetTypeReference = null;
                if (string.IsNullOrEmpty(payloadTypeName))
                {
                    targetTypeReference = EdmCoreModel.Instance.GetString(true);
                }
                else
                {
                    EdmTypeKind payloadTypeKind;
                    IEdmType payloadType =
                        MetadataUtils.ResolveTypeNameForRead(
                            this.Model,
                            null,
                            payloadTypeName, // expectedType
                            this.MessageReaderSettings.ReaderBehavior,
                            this.Version,
                            out payloadTypeKind);
                    targetTypeReference = payloadType.ToTypeReference().AsPrimitive();
                }

                if (isNull)
                {
                    property.Value = this.ReadNullValue(targetTypeReference, /*validateNullValue*/ false, propertyName);
                }
                else
                {
                    property.Value = this.ReadPrimitiveValue(targetTypeReference);
                }

                // Read past the end tag of the property or the start tag if the element is empty.
                this.XmlReader.Read();
                this.XmlReader.AssertNotBuffering();

                return property;
            }
            else
            {
                this.XmlReader.Skip();
                return null;
            }
        }

        /// <summary>
        /// Reads a property.
        /// </summary>
        /// <param name="expectedPropertyName">The expected property name to be read from the payload (or null if no expected property name was specified).</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property value.</param>
        /// <param name="nullValueReadBehaviorKind">Behavior to use when reading null value for the property.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <returns>The ODataProperty representing the property in question; if null is returned from this method it means that the property is to be ignored.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element - The XML element representing the property to read.
        ///                                        Note that the method does NOT check for the property name neither it resolves the property against metadata.
        /// Post-Condition:  Any                 - The node after the property.
        /// </remarks>
        private ODataProperty ReadProperty(
            string expectedPropertyName,
            IEdmTypeReference expectedPropertyTypeReference,
            ODataNullValueBehaviorKind nullValueReadBehaviorKind,
            bool epmPresent)
        {
            Debug.Assert(
                expectedPropertyTypeReference == null || expectedPropertyTypeReference.IsODataPrimitiveTypeKind() ||
                expectedPropertyTypeReference.IsODataComplexTypeKind() || expectedPropertyTypeReference.IsNonEntityCollectionType(),
                "Only primitive, complex and collection types can be read by this method.");
            this.AssertXmlCondition(XmlNodeType.Element);
            Debug.Assert(this.UseServerFormatBehavior || this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace), "Property elements must be in the OData namespace (unless we are running in WCF DS server format mode).");
            this.XmlReader.AssertNotBuffering();

            ODataProperty property = new ODataProperty();
            string propertyName = this.XmlReader.LocalName;
            ValidationUtils.ValidatePropertyName(propertyName);
            ReaderValidationUtils.ValidateExpectedPropertyName(expectedPropertyName, propertyName);
            property.Name = propertyName;

            object propertyValue = this.ReadNonEntityValueImplementation(
                expectedPropertyTypeReference,
                /*duplicatePropertyNamesChecker*/ null,
                /*collectionValidator*/ null,
                nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                epmPresent,
                propertyName);

            if (nullValueReadBehaviorKind == ODataNullValueBehaviorKind.IgnoreValue && propertyValue == null)
            {
                property = null;
            }
            else
            {
                property.Value = propertyValue;
            }

            // Read past the end tag of the property or the start tag if the element is empty.
            this.XmlReader.Read();

            this.XmlReader.AssertNotBuffering();
            return property;
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
        /// <param name="serializationTypeNameAnnotation">The serialization type name for the complex value (possibly null).</param>
        /// <param name="duplicatePropertyNamesChecker">The duplicate property names checker to use (cached), or null if new one should be created.</param>
        /// <param name="epmPresent">Whether any EPM mappings exist.</param>
        /// <returns>The value read from the payload.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.EndElement - the element has been read.
        ///                  
        /// Note that this method will not read null values, those should be handled by the caller already.
        /// </remarks>
        private ODataComplexValue ReadComplexValue(
            IEdmComplexTypeReference complexTypeReference,
            string payloadTypeName,
            SerializationTypeNameAnnotation serializationTypeNameAnnotation,
            DuplicatePropertyNamesChecker duplicatePropertyNamesChecker,
            bool epmPresent)
        {
            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.Attribute);

            this.IncreaseRecursionDepth();

            ODataComplexValue complexValue = new ODataComplexValue();
            IEdmComplexType complexType = complexTypeReference == null ? null : (IEdmComplexType)complexTypeReference.Definition;

            // If we have a metadata type for the complex value, use that type name
            // otherwise use the type name from the payload (if there was any).
            complexValue.TypeName = complexType == null ? payloadTypeName : complexType.ODataFullName();
            if (serializationTypeNameAnnotation != null)
            {
                complexValue.SetAnnotation(serializationTypeNameAnnotation);
            }

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

            ReadOnlyEnumerable<ODataProperty> properties = new ReadOnlyEnumerable<ODataProperty>();
            this.ReadPropertiesImplementation(complexType, properties, duplicatePropertyNamesChecker, epmPresent);
            complexValue.Properties = properties;

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(complexValue != null, "The method should never return null since it doesn't handle null values.");

            this.DecreaseRecursionDepth();

            return complexValue;
        }

        /// <summary>
        /// Read a collection from the reader.
        /// </summary>
        /// <param name="collectionTypeReference">The type of the collection to read (or null if no type is available).</param>
        /// <param name="payloadTypeName">The name of the collection type specified in the payload.</param>
        /// <param name="serializationTypeNameAnnotation">The serialization type name for the collection value (possibly null).</param>
        /// <returns>The value read from the payload.</returns>
        /// <remarks>
        /// Pre-Condition:   XmlNodeType.Element   - the element to read the value for.
        ///                  XmlNodeType.Attribute - an attribute on the element to read the value for.
        /// Post-Condition:  XmlNodeType.Element    - the element was empty.
        ///                  XmlNodeType.EndElement - the element had some value.
        ///                  
        /// Note that this method will not read null values, those should be handled by the caller already.
        /// </remarks>
        private ODataCollectionValue ReadCollectionValue(
            IEdmCollectionTypeReference collectionTypeReference,
            string payloadTypeName,
            SerializationTypeNameAnnotation serializationTypeNameAnnotation)
        {
            this.AssertXmlCondition(XmlNodeType.Element, XmlNodeType.Attribute);
            Debug.Assert(
                collectionTypeReference == null || collectionTypeReference.IsNonEntityCollectionType(),
                "If the metadata is specified it must denote a collection for this method to work.");

            this.IncreaseRecursionDepth();

            ODataCollectionValue collectionValue = new ODataCollectionValue();

            // If we have a metadata type for the collection, use that type name
            // otherwise use the type name from the payload (if there was any).
            collectionValue.TypeName = collectionTypeReference == null ? payloadTypeName : collectionTypeReference.ODataFullName();
            if (serializationTypeNameAnnotation != null)
            {
                collectionValue.SetAnnotation(serializationTypeNameAnnotation);
            }

            // Move to the element (so that if we were on an attribute we can test the element for being empty)
            this.XmlReader.MoveToElement();

            List<object> items = new List<object>();

            // Empty collections are valid - they have no items
            if (!this.XmlReader.IsEmptyElement)
            {
                // Read over the collection element to its first child node (or end-element)
                this.XmlReader.ReadStartElement();

                IEdmTypeReference itemTypeReference = collectionTypeReference == null ? null : collectionTypeReference.ElementType();

                DuplicatePropertyNamesChecker duplicatePropertyNamesChecker = this.CreateDuplicatePropertyNamesChecker();
                CollectionWithoutExpectedTypeValidator collectionValidator = null;
                if (collectionTypeReference == null)
                {
                    // Parse the type name from the payload (if any), extract the item type name and construct a collection validator
                    string itemTypeName = payloadTypeName == null ? null : EdmLibraryExtensions.GetCollectionItemTypeName(payloadTypeName);
                    collectionValidator = new CollectionWithoutExpectedTypeValidator(itemTypeName);
                }

                do
                {
                    switch (this.XmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (this.XmlReader.NamespaceEquals(this.XmlReader.ODataNamespace))
                            {
                                if (!this.XmlReader.LocalNameEquals(this.ODataCollectionItemElementName))
                                {
                                    throw new ODataException(ODataErrorStrings.ODataAtomPropertyAndValueDeserializer_InvalidCollectionElement(this.XmlReader.LocalName, this.XmlReader.ODataNamespace));
                                }

                                // We don't support EPM for collections so it is fine to say that EPM is not present
                                object itemValue = this.ReadNonEntityValueImplementation(
                                    itemTypeReference,
                                    duplicatePropertyNamesChecker,
                                    collectionValidator,
                                    /*validateNullValue*/ true,
                                    /*epmPresent*/ false,
                                    /*propertyName*/ null);

                                // read over the end tag of the element or the start tag if the element was empty.
                                this.XmlReader.Read();

                                // Validate the item (for example that it's not null)
                                ValidationUtils.ValidateCollectionItem(itemValue, false /* isStreamable */);

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
                            // End of the collection.
                            break;

                        default:
                            // Non-element so for example a text node, just ignore
                            this.XmlReader.Skip();
                            break;
                    }
                }
                while (this.XmlReader.NodeType != XmlNodeType.EndElement);
            }

            collectionValue.Items = new ReadOnlyEnumerable(items);

            this.AssertXmlCondition(true, XmlNodeType.EndElement);
            Debug.Assert(collectionValue != null, "The method should never return null since it doesn't handle null values.");

            this.DecreaseRecursionDepth();

            return collectionValue;
        }

        /// <summary>
        /// Increases the recursion depth of values by 1. This will throw if the recursion depth exceeds the current limit.
        /// </summary>
        private void IncreaseRecursionDepth()
        {
            ValidationUtils.IncreaseAndValidateRecursionDepth(ref this.recursionDepth, this.MessageReaderSettings.MessageQuotas.MaxNestingDepth);
        }

        /// <summary>
        /// Decreases the recursion depth of values by 1.
        /// </summary>
        private void DecreaseRecursionDepth()
        {
            Debug.Assert(this.recursionDepth > 0, "Can't decrease recursion depth below 0.");

            this.recursionDepth--;
        }

        /// <summary>
        /// Asserts that the current recursion depth of values is zero. This should be true on all calls into this class from outside of this class.
        /// </summary>
        [Conditional("DEBUG")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "The this is needed in DEBUG build.")]
        private void AssertRecursionDepthIsZero()
        {
            Debug.Assert(this.recursionDepth == 0, "The current recursion depth must be 0.");
        }
    }
}
