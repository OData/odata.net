﻿//---------------------------------------------------------------------
// <copyright file="ODataJsonPropertyAndValueDeserializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using Microsoft.VisualBasic;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    #endregion Namespaces

    /// <summary>
    /// OData Json deserializer for properties and value types.
    /// </summary>
    internal class ODataJsonPropertyAndValueDeserializer : ODataJsonDeserializer
    {
        /// <summary>A sentinel value indicating a missing property value.</summary>
        private static readonly object missingPropertyValue = new object();

        /// <summary>
        /// The current recursion depth of values read by this deserializer, measured by the number of resource, collection, JSON object and JSON array values read so far.
        /// </summary>
        private int recursionDepth;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The Json input context to read from.</param>
        internal ODataJsonPropertyAndValueDeserializer(ODataJsonInputContext jsonInputContext)
            : base(jsonInputContext)
        {
        }

        /// <summary>
        /// This method creates an reads the property from the input and
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        internal ODataProperty ReadTopLevelProperty(IEdmTypeReference expectedPropertyTypeReference)
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            this.ReadPayloadStart(
                ODataPayloadKind.Property,
                propertyAndAnnotationCollector,
                /*isReadingNestedPayload*/false,
                /*allowEmptyPayload*/false);

            ODataProperty resultProperty = this.ReadTopLevelPropertyImplementation(expectedPropertyTypeReference, propertyAndAnnotationCollector);

            this.ReadPayloadEnd(/*isReadingNestedPayload*/ false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return resultProperty;
        }

        /// <summary>
        /// This method creates an reads the property from the input and
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>A task which returns an <see cref="ODataProperty"/> representing the read property.</returns>
        internal async Task<ODataProperty> ReadTopLevelPropertyAsync(IEdmTypeReference expectedPropertyTypeReference)
        {
            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.None, "Pre-Condition: expected JsonNodeType.None, the reader must not have been used yet.");
            this.JsonReader.AssertNotBuffering();

            // We use this to store annotations and check for duplicate annotation names, but we don't really store properties in it.
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

            await this.ReadPayloadStartAsync(ODataPayloadKind.Property, propertyAndAnnotationCollector, isReadingNestedPayload: false, allowEmptyPayload: false)
                .ConfigureAwait(false);

            ODataProperty resultProperty = await this.ReadTopLevelPropertyImplementationAsync(expectedPropertyTypeReference, propertyAndAnnotationCollector)
                .ConfigureAwait(false);

            await this.ReadPayloadEndAsync(isReadingNestedPayload: false)
                .ConfigureAwait(false);

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndOfInput, "Post-Condition: expected JsonNodeType.EndOfInput");
            this.JsonReader.AssertNotBuffering();

            return resultProperty;
        }

        /// <summary>
        /// Reads a primitive value, enum, resource (complex or entity) value or collection.
        /// </summary>
        /// <param name="payloadTypeName">The type name read from the payload as a property annotation, or null if none is available.</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the property value.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="isTopLevelPropertyValue">true if we are reading a top-level property value; otherwise false.</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        /// <returns>The value of the property read.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value
        ///                 JsonNodeType.StartObject      - the value of the property is an object
        ///                 JsonNodeType.StartArray       - the value of the property is an array - method will fail in this case.
        /// Post-Condition: almost anything - the node after the property value.
        ///
        /// Returns the value of the property read, which can be one of:
        /// - null
        /// - primitive value
        /// - <see cref="ODataEnumValue"/>
        /// - <see cref="ODataResourceValue"/>
        /// - <see cref="ODataCollectionValue"/>
        /// </remarks>
        internal object ReadNonEntityValue(
            string payloadTypeName,
            IEdmTypeReference expectedValueTypeReference,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            bool isTopLevelPropertyValue,
            bool insideResourceValue,
            string propertyName,
            bool? isDynamicProperty = null)
        {
            this.AssertRecursionDepthIsZero();
            object nonEntityValue = this.ReadNonEntityValueImplementation(
                payloadTypeName,
                expectedValueTypeReference,
                propertyAndAnnotationCollector,
                collectionValidator,
                validateNullValue,
                isTopLevelPropertyValue,
                insideResourceValue,
                propertyName,
                isDynamicProperty);
            this.AssertRecursionDepthIsZero();

            return nonEntityValue;
        }

        /// <summary>
        /// Reads the value of the instance annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker instance.</param>
        /// <param name="name">The name of the instance annotation.</param>
        /// <returns>Returns the value of the instance annotation.</returns>
        internal object ReadCustomInstanceAnnotationValue(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string name)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            object propertyAnnotation;
            string odataType = null;
            if (propertyAndAnnotationCollector.GetODataPropertyAnnotations(name)
                .TryGetValue(ODataAnnotationNames.ODataType, out propertyAnnotation))
            {
                odataType = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)propertyAnnotation));
            }

            return ReadODataOrCustomInstanceAnnotationValue(name, odataType);
        }

        /// <summary>
        /// Reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="odataType">the odata.type value if exists.</param>
        /// <returns>The annotation value.</returns>
        internal object ReadODataOrCustomInstanceAnnotationValue(string annotationName, string odataType)
        {
            // If this term is defined in the model, look up its type. If the term is not in the model, this will be null.
            IEdmTypeReference expectedTypeFromTerm = MetadataUtils.LookupTypeOfTerm(annotationName, this.Model);
            object customInstanceAnnotationValue = this.ReadNonEntityValueImplementation(
                odataType,
                expectedTypeFromTerm,
                null, /*propertyAndAnnotationCollector*/
                null, /*collectionValidator*/
                false, /*validateNullValue*/
                false, /*isTopLevelPropertyValue*/
                false, /*insideResourceValue Always pass false here to try read @odata.type annotation in custom instance annotations*/
                annotationName);
            return customInstanceAnnotationValue;
        }

        /// <summary>
        /// Resolve the IEdmTypeReference of the current untyped value to be read.
        /// </summary>
        /// <param name="jsonReaderNodeType">The current JsonReader NodeType.</param>
        /// <param name="jsonReaderValue">The current JsonReader Value</param>
        /// <param name="payloadTypeName">The 'odata.type' annotation in payload.</param>
        /// <param name="payloadTypeReference">The payloadTypeReference of 'odata.type'.</param>
        /// <param name="primitiveTypeResolver">Function that takes a primitive value and returns an <see cref="IEdmTypeReference"/>.</param>
        /// <param name="readUntypedAsString">Whether unknown properties should be read as a raw string value.</param>
        /// <param name="generateTypeIfMissing">Whether to generate a type if not already part of the model.</param>
        /// <returns>The <see cref="IEdmTypeReference"/> of the current value to be read.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Each code path casts to bool at most one time, and only if needed.")]
        internal static IEdmTypeReference ResolveUntypedType(
                JsonNodeType jsonReaderNodeType,
                object jsonReaderValue,
                string payloadTypeName,
                IEdmTypeReference payloadTypeReference,
                Func<object, string, IEdmTypeReference> primitiveTypeResolver,
                bool readUntypedAsString,
                bool generateTypeIfMissing)
        {
            if (payloadTypeReference != null && (payloadTypeReference.TypeKind() != EdmTypeKind.Untyped || readUntypedAsString))
            {
                return payloadTypeReference;
            }

            if (readUntypedAsString)
            {
                if (jsonReaderNodeType == JsonNodeType.PrimitiveValue && jsonReaderValue is bool)
                {
                    return EdmCoreModel.Instance.GetBoolean(true);
                }

                return EdmCoreModel.Instance.GetUntyped();
            }

            string namespaceName;
            string name;
            bool isCollection;
            IEdmTypeReference typeReference;
            switch (jsonReaderNodeType)
            {
                case JsonNodeType.PrimitiveValue:
                    if (primitiveTypeResolver != null)
                    {
                        typeReference = primitiveTypeResolver(jsonReaderValue, payloadTypeName);
                        if (typeReference != null)
                        {
                            return typeReference;
                        }
                    }

                    if (jsonReaderValue == null)
                    {
                        if (payloadTypeName != null)
                        {
                            TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName, out name, out isCollection);
                            Debug.Assert(namespaceName != Metadata.EdmConstants.EdmNamespace, "If type was in the edm namespace it should already have been resolved");

                            typeReference = new EdmUntypedStructuredType(namespaceName, name).ToTypeReference(/*isNullable*/ true);
                            return isCollection ? new EdmCollectionType(typeReference).ToTypeReference(/*isNullable*/ true) : typeReference;
                        }

                        typeReference = EdmCoreModel.Instance.GetString(/*isNullable*/ true);
                    }
                    else if (jsonReaderValue is bool)
                    {
                        typeReference = EdmCoreModel.Instance.GetBoolean(/*isNullable*/ true);
                    }
                    else if (jsonReaderValue is string)
                    {
                        typeReference = EdmCoreModel.Instance.GetString(/*isNullable*/ true);
                    }
                    else
                    {
                        typeReference = EdmCoreModel.Instance.GetDecimal(/*isNullable*/ true);
                    }

                    if (payloadTypeName != null)
                    {
                        TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName, out name, out isCollection);
                        if (isCollection)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_CollectionTypeNotExpected, payloadTypeName));
                        }

                        typeReference = new EdmTypeDefinition(namespaceName, name, typeReference.PrimitiveKind()).ToTypeReference(/*isNullable*/ true);
                    }

                    return typeReference;

                case JsonNodeType.StartObject:
                    if (payloadTypeName != null && generateTypeIfMissing)
                    {
                        TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName, out name, out isCollection);
                        if (isCollection)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_CollectionTypeNotExpected, payloadTypeName));
                        }

                        return new EdmUntypedStructuredType(namespaceName, name).ToTypeReference(/*isNullable*/ true);
                    }

                    return new EdmUntypedStructuredType().ToTypeReference(/*isNullable*/ true);

                case JsonNodeType.StartArray:
                    if (payloadTypeName != null && generateTypeIfMissing)
                    {
                        TypeUtils.ParseQualifiedTypeName(payloadTypeName, out namespaceName, out name, out isCollection);
                        if (!isCollection)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_CollectionTypeExpected, payloadTypeName));
                        }

                        return new EdmCollectionType(new EdmUntypedStructuredType(namespaceName, name).ToTypeReference(/*isNullable*/ true)).ToTypeReference(/*isNullable*/true);
                    }

                    return new EdmCollectionType(new EdmUntypedStructuredType().ToTypeReference(/*isNullable*/ true)).ToTypeReference(/*isNullable*/true);

                default:
                    return EdmCoreModel.Instance.GetUntyped();
            }
        }

        /// <summary>
        /// Asynchronously reads a primitive value, enum, resource (complex or entity) value or collection.
        /// </summary>
        /// <param name="payloadTypeName">The type name read from the payload as a property annotation, or null if none is available.</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the property value.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        /// <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="isTopLevelPropertyValue">true if we are reading a top-level property value; otherwise false.</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the property read, which can be one of:
        /// 1). null
        /// 2). Primitive value
        /// 3). <see cref="ODataEnumValue"/>
        /// 4). <see cref="ODataResourceValue"/>
        /// 5). <see cref="ODataCollectionValue"/>
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value,
        ///                 JsonNodeType.StartObject      - the value of the property is an object,
        ///                 JsonNodeType.StartArray       - the value of the property is an array - method will fail in this case.
        /// Post-Condition: almost anything - the node after the property value.
        /// </remarks>
        internal async Task<object> ReadNonEntityValueAsync(
            string payloadTypeName,
            IEdmTypeReference expectedValueTypeReference,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            bool isTopLevelPropertyValue,
            bool insideResourceValue,
            string propertyName,
            bool? isDynamicProperty = null)
        {
            this.AssertRecursionDepthIsZero();
            object nonEntityValue = await this.ReadNonEntityValueImplementationAsync(
                payloadTypeName,
                expectedValueTypeReference,
                propertyAndAnnotationCollector,
                collectionValidator,
                validateNullValue,
                isTopLevelPropertyValue,
                insideResourceValue,
                propertyName,
                isDynamicProperty).ConfigureAwait(false);
            this.AssertRecursionDepthIsZero();

            return nonEntityValue;
        }

        /// <summary>
        /// Asynchronously reads the value of the instance annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker instance.</param>
        /// <param name="name">The name of the instance annotation.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the instance annotation.
        /// </returns>
        internal Task<object> ReadCustomInstanceAnnotationValueAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string name)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(!string.IsNullOrEmpty(name), "!string.IsNullOrEmpty(name)");

            object propertyAnnotation;
            string odataType = null;
            if (propertyAndAnnotationCollector.GetODataPropertyAnnotations(name)
                .TryGetValue(ODataAnnotationNames.ODataType, out propertyAnnotation))
            {
                odataType = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)propertyAnnotation));
            }

            return ReadODataOrCustomInstanceAnnotationValueAsync(name, odataType);
        }

        /// <summary>
        /// Asynchronously reads built-in "odata." or custom instance annotation's value.
        /// </summary>
        /// <param name="annotationName">The annotation name.</param>
        /// <param name="odataType">the odata.type value if exists.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the annotation value.
        /// </returns>
        internal Task<object> ReadODataOrCustomInstanceAnnotationValueAsync(string annotationName, string odataType)
        {
            // If this term is defined in the model, look up its type. If the term is not in the model, this will be null.
            IEdmTypeReference expectedTypeFromTerm = MetadataUtils.LookupTypeOfTerm(annotationName, this.Model);
            return this.ReadNonEntityValueImplementationAsync(
                odataType,
                expectedTypeFromTerm,
                propertyAndAnnotationCollector: null,
                collectionValidator: null,
                validateNullValue: false,
                isTopLevelPropertyValue: false,
                insideResourceValue: false, // Always pass false here to try read @odata.type annotation in custom instance annotations
                propertyName: annotationName);
        }

        /// <summary>
        /// Try to read or peek the odata.type annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The current level's PropertyAndAnnotationCollector.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="insideResourceValue">If inside complex value.</param>
        /// <returns>The odata.type value or null.</returns>
        protected string TryReadOrPeekPayloadType(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string propertyName, bool insideResourceValue)
        {
            string payloadTypeName = ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName);
            bool valueIsJsonObject = this.JsonReader.NodeType == JsonNodeType.StartObject;
            if (string.IsNullOrEmpty(payloadTypeName) && valueIsJsonObject)
            {
                try
                {
                    this.JsonReader.StartBuffering();

                    // If we have an object value initialize the duplicate property names checker
                    propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                    // Read the payload type name
                    string typeName;
                    bool typeNameFoundInPayload = this.TryReadPayloadTypeFromObject(
                        propertyAndAnnotationCollector,
                        insideResourceValue,
                        out typeName);
                    if (typeNameFoundInPayload)
                    {
                        payloadTypeName = typeName;
                    }
                }
                finally
                {
                    this.JsonReader.StopBuffering();
                }
            }

            return payloadTypeName;
        }

        /// <summary>
        /// Reads an entity or complex type's undeclared property.
        /// </summary>
        /// <param name="resourceState">The IODataJsonReaderResourceState.</param>
        /// <param name="propertyName">Now this name can't be found in model.</param>
        /// <param name="isTopLevelPropertyValue">bool</param>
        /// <returns>The read result.</returns>
        protected ODataJsonReaderNestedResourceInfo InnerReadUndeclaredProperty(IODataJsonReaderResourceState resourceState, string propertyName, bool isTopLevelPropertyValue)
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = resourceState.PropertyAndAnnotationCollector;
            bool insideResourceValue = false;
            string outerPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName);
            string payloadTypeName = this.TryReadOrPeekPayloadType(propertyAndAnnotationCollector, propertyName, insideResourceValue);
            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ReaderValidationUtils.ResolvePayloadTypeName(
                this.Model,
                null, // expectedTypeReference
                payloadTypeName,
                EdmTypeKind.Complex,
                this.MessageReaderSettings.ClientCustomTypeResolver,
                out payloadTypeKind);
            IEdmTypeReference payloadTypeReference = null;
            if (!string.IsNullOrEmpty(payloadTypeName) && payloadType != null)
            {
                // only try resolving for known type (the below will throw on unknown type name) :
                ODataTypeAnnotation typeAnnotation;
                EdmTypeKind targetTypeKind;
                payloadTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    /*expectStructuredType*/ null,
                    /*defaultPrimitivePayloadType*/ null,
                    null, // expectedTypeReference
                    payloadTypeName,
                    this.Model,
                    this.GetNonEntityValueKind,
                    out targetTypeKind,
                    out typeAnnotation);
            }

            object propertyValue = null;
            payloadTypeReference = ResolveUntypedType(
                this.JsonReader.NodeType,
                this.JsonReader.GetValue(),
                payloadTypeName,
                payloadTypeReference,
                this.MessageReaderSettings.PrimitiveTypeResolver,
                this.MessageReaderSettings.ReadUntypedAsString,
                !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

            if (payloadTypeReference.ToStructuredType() != null)
            {
                ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                // Complex property or collection of complex property.
                bool isCollection = payloadTypeReference.IsCollection();
                ValidateExpandedNestedResourceInfoPropertyValue(this.JsonReader, isCollection, propertyName, payloadTypeReference);
                if (isCollection)
                {
                    readerNestedResourceInfo =
                        ReadExpandedResourceSetNestedResourceInfo(resourceState, null, payloadTypeReference.ToStructuredType(), propertyName, /*isDeltaResourceSet*/ false);
                }
                else
                {
                    readerNestedResourceInfo = ReadExpandedResourceNestedResourceInfo(resourceState, null, propertyName, payloadTypeReference.ToStructuredType(), this.MessageReaderSettings);
                }

                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);

                return readerNestedResourceInfo;
            }

            if (!(payloadTypeReference is IEdmUntypedTypeReference))
            {
                this.JsonReader.AssertNotBuffering();
                propertyValue = this.ReadNonEntityValueImplementation(
                    outerPayloadTypeName,
                    payloadTypeReference,
                    /*propertyAndAnnotationCollector*/ null,
                    /*collectionValidator*/ null,
                    false, // validateNullValue
                    isTopLevelPropertyValue,
                    insideResourceValue,
                    propertyName);
            }
            else
            {
                propertyValue = this.JsonReader.ReadAsUntypedOrNullValue();
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
            AddResourceProperty(resourceState, propertyName, propertyValue);
            return null;
        }

        /// <summary>
        /// Validates that the value of a JSON property can represent expanded nested resource info.
        /// </summary>
        /// <param name="jsonReader">The IJsonReader.</param>
        /// <param name="isCollection">true if the property is entity set reference property;
        /// false for a resource reference property, null if unknown.</param>
        /// <param name="propertyName">Name for the navigation property, used in error message only.</param>
        /// <param name="typeReference">Type of navigation property</param>
        protected static void ValidateExpandedNestedResourceInfoPropertyValue(
            IJsonReader jsonReader,
            bool? isCollection,
            string propertyName,
            IEdmTypeReference typeReference)
        {
            // an expanded link with resource requires a StartObject node here;
            // an expanded link with resource set requires a StartArray node here;
            // an expanded link with null resource requires a primitive null node here;
            JsonNodeType nodeType = jsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                if (isCollection == false)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadSingletonNestedResource, nodeType, propertyName));
                }
            }
            else if ((nodeType == JsonNodeType.PrimitiveValue && jsonReader.GetValue() == null) || nodeType == JsonNodeType.StartObject)
            {
                // Expanded resource (null or non-null)
                if (isCollection == true)
                {
                    if (typeReference.IsNonEntityCollectionType())
                    {
                        ReaderValidationUtils.ValidateNullValue(typeReference, true,
                                                  true, propertyName, false);
                    }
                    else
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadCollectionNestedResource, nodeType, propertyName));
                    }

                }
            }
            else
            {
                Debug.Assert(nodeType == JsonNodeType.PrimitiveValue, "nodeType == JsonNodeType.PrimitiveValue");
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadNestedResource, propertyName));
            }
        }

        /// <summary>
        /// Reads non-expanded nested resource set.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="collectionProperty">The collection of complex property for which to read the nested resource info. null for undeclared property.</param>
        /// <param name="nestedResourceType">The item type of the resource set, which should be provided when the collectionProperty is undeclared.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadNonExpandedResourceSetNestedResourceInfo(IODataJsonReaderResourceState resourceState, IEdmStructuralProperty collectionProperty, IEdmStructuredType nestedResourceType, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert((propertyName != null) || (collectionProperty != null && collectionProperty.Type.ToStructuredType().IsODataComplexTypeKind()) || (nestedResourceType != null && nestedResourceType.IsODataComplexTypeKind()),
                "The property name shouldn't be null or the item in the collection property should be complex instance");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = true,
                IsComplex = true
            };

            ODataResourceSet expandedResourceSet = CreateCollectionResourceSet(resourceState, propertyName);
            return ODataJsonReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, collectionProperty, nestedResourceType, expandedResourceSet);
        }

        /// <summary>
        /// Reads non-expanded resource nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="complexProperty">The complex property for which to read the nested resource info. null for undeclared property.</param>
        /// <param name="nestedResourceType">The nested resource type which should be provided for undeclared property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The nested resource info for the complex property to read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadNonExpandedResourceNestedResourceInfo(IODataJsonReaderResourceState resourceState, IEdmStructuralProperty complexProperty, IEdmStructuredType nestedResourceType, string propertyName)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(complexProperty != null || nestedResourceType != null || propertyName != null, "complexProperty != null || nestedResourceType != null || propertyName != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = false,
                IsComplex = true
            };

            // Check the odata.type annotation for the complex property, it should show inside the complex object.
            if (ValidateDataPropertyTypeNameAnnotation(resourceState.PropertyAndAnnotationCollector, nestedResourceInfo.Name) != null)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ComplexValueWithPropertyTypeAnnotation, ODataAnnotationNames.ODataType));
            }

            return ODataJsonReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, complexProperty, nestedResourceType);
        }

        /// <summary>
        /// Reads expanded resource nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link. null for undeclared property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="messageReaderSettings">The ODataMessageReaderSettings.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadExpandedResourceNestedResourceInfo(IODataJsonReaderResourceState resourceState, IEdmNavigationProperty navigationProperty, string propertyName, IEdmStructuredType propertyType, ODataMessageReaderSettings messageReaderSettings)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null || propertyName != null, "navigationProperty != null || propertyName != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = false
            };

            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataNavigationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataAssociationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataContext:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.ContextUrl = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataType:
                        Debug.Assert(propertyAnnotation.Value is String && propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a non-null string.");
                        nestedResourceInfo.TypeAnnotation = new ODataTypeAnnotation((string)propertyAnnotation.Value);
                        break;

                    default:
                        if (messageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedExpandedSingletonNavigationLinkPropertyAnnotation, nestedResourceInfo.Name, propertyAnnotation.Key));
                        }

                        break;
                }
            }

            return ODataJsonReaderNestedResourceInfo.CreateResourceReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, propertyType);
        }

        /// <summary>
        /// Reads expanded resource set nested resource info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the expanded link. null for undeclared property.</param>
        /// <param name="propertyType">The type of the collection.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isDeltaResourceSet">The property being read represents a nested delta resource set.</param>
        /// <returns>The nested resource info for the expanded link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadExpandedResourceSetNestedResourceInfo(IODataJsonReaderResourceState resourceState, IEdmNavigationProperty navigationProperty, IEdmStructuredType propertyType, string propertyName, bool isDeltaResourceSet)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null || propertyName != null, "navigationProperty != null || propertyName != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = true
            };

            ODataResourceSetBase expandedResourceSet;
            if (isDeltaResourceSet)
            {
                expandedResourceSet = new ODataDeltaResourceSet();
            }
            else
            {
                expandedResourceSet = new ODataResourceSet();
            }

            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataNavigationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.navigationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.Url = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataAssociationLinkUrl:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.associationLinkUrl annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.AssociationLinkUrl = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataNextLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.nextLink annotation should have been parsed as a non-null Uri.");
                        expandedResourceSet.NextPageLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataCount:
                        Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                        expandedResourceSet.Count = (long?)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataContext:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.context annotation should have been parsed as a non-null Uri.");
                        nestedResourceInfo.ContextUrl = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataType:
                        Debug.Assert(propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a string.");
                        expandedResourceSet.TypeName = (string)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataDeltaLink:   // Delta links are not supported on expanded resource sets.
                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedExpandedCollectionNavigationLinkPropertyAnnotation, nestedResourceInfo.Name, propertyAnnotation.Key));
                }
            }

            return ODataJsonReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, navigationProperty, propertyType, expandedResourceSet);
        }

        /// <summary>
        /// Reads a nested stream collection as nested resource set info.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="collectionProperty">The collection of stream property for which to read the nested resource info. null for undeclared property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="elementType">They primitive type of the collection element</param>
        /// <returns>The nested resource info for the stream collection.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadStreamCollectionNestedResourceInfo(IODataJsonReaderResourceState resourceState, IEdmStructuralProperty collectionProperty, string propertyName, IEdmType elementType)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert((propertyName != null) || (collectionProperty != null),
                "The collection property and property name shouldn't both be null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = true,
                IsComplex = false
            };

            ODataResourceSet expandedResourceSet = CreateCollectionResourceSet(resourceState, propertyName);

            ODataJsonReaderNestedResourceInfo nestedInfo = ODataJsonReaderNestedResourceInfo.CreateResourceSetReaderNestedResourceInfo(nestedResourceInfo, collectionProperty, elementType, expandedResourceSet);
            return nestedInfo;
        }

        /// <summary>
        /// Reads entity reference link for a singleton navigation link in request.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference link.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference link read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadEntityReferenceLinkForSingletonNavigationLinkInRequest(
            IODataJsonReaderResourceState resourceState,
            IEdmNavigationProperty navigationProperty,
            string propertyName,
            bool isExpanded)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(navigationProperty != null || propertyName != null, "navigationProperty != null || propertyName != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = false
            };

            ODataEntityReferenceLink entityReferenceLink = null;
            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataBind:
                        LinkedList<ODataEntityReferenceLink> entityReferenceLinksList = propertyAnnotation.Value as LinkedList<ODataEntityReferenceLink>;
                        if (entityReferenceLinksList != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_ArrayValueForSingletonBindPropertyAnnotation, nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                        }

                        if (isExpanded)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_SingletonNavigationPropertyWithBindingAndValue, nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                        }

                        Debug.Assert(
                            propertyAnnotation.Value is ODataEntityReferenceLink && propertyAnnotation.Value != null,
                            "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                        entityReferenceLink = (ODataEntityReferenceLink)propertyAnnotation.Value;
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation,
                            nestedResourceInfo.Name,
                            propertyAnnotation.Key,
                            ODataAnnotationNames.ODataBind));
                }
            }

            return ODataJsonReaderNestedResourceInfo.CreateSingletonEntityReferenceLinkInfo(nestedResourceInfo, navigationProperty, entityReferenceLink, isExpanded);
        }

        /// <summary>
        /// Reads entity reference links for a collection navigation link in request.
        /// </summary>
        /// <param name="resourceState">The state of the reader for resource to read.</param>
        /// <param name="navigationProperty">The navigation property for which to read the entity reference links. null for undeclared property.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="isExpanded">true if the navigation link is expanded.</param>
        /// <returns>The navigation link info for the entity reference links read.</returns>
        /// <remarks>
        /// This method doesn't move the reader.
        /// </remarks>
        protected static ODataJsonReaderNestedResourceInfo ReadEntityReferenceLinksForCollectionNavigationLinkInRequest(
            IODataJsonReaderResourceState resourceState,
            IEdmNavigationProperty navigationProperty,
            string propertyName,
            bool isExpanded)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(propertyName != null, "propertyName != null");

            ODataNestedResourceInfo nestedResourceInfo = new ODataNestedResourceInfo()
            {
                Name = propertyName,
                IsCollection = true
            };

            LinkedList<ODataEntityReferenceLink> entityReferenceLinksList = null;
            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(nestedResourceInfo.Name))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataBind:
                        ODataEntityReferenceLink entityReferenceLink = propertyAnnotation.Value as ODataEntityReferenceLink;
                        if (entityReferenceLink != null)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_StringValueForCollectionBindPropertyAnnotation, nestedResourceInfo.Name, ODataAnnotationNames.ODataBind));
                        }

                        Debug.Assert(
                            propertyAnnotation.Value is LinkedList<ODataEntityReferenceLink> && propertyAnnotation.Value != null,
                            "The value of odata.bind property annotation must be either ODataEntityReferenceLink or List<ODataEntityReferenceLink>");
                        entityReferenceLinksList = (LinkedList<ODataEntityReferenceLink>)propertyAnnotation.Value;
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedNavigationLinkInRequestPropertyAnnotation,
                            nestedResourceInfo.Name,
                            propertyAnnotation.Key,
                            ODataAnnotationNames.ODataBind));
                }
            }

            return ODataJsonReaderNestedResourceInfo.CreateCollectionEntityReferenceLinksInfo(nestedResourceInfo, navigationProperty, entityReferenceLinksList, isExpanded);
        }

        /// <summary>
        /// Gets and validates the type name annotation for the specified property.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker in use for the resource content.</param>
        /// <param name="propertyName">The name of the property to get the type name for.</param>
        /// <returns>The type name for the property or null if no type name was found.</returns>
        protected static string ValidateDataPropertyTypeNameAnnotation(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string propertyName)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            string propertyTypeName = null;
            foreach (KeyValuePair<string, object> propertyAnnotation
                     in propertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
            {
                if (!string.Equals(propertyAnnotation.Key, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
                {
                    // here allow other annotation name than odata.type, instead of throwing:
                    // ODataErrorStrings.ODataJsonPropertyAndValueDeserializer_UnexpectedDataPropertyAnnotation
                    continue;
                }

                Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a non-null string.");
                propertyTypeName = (string)propertyAnnotation.Value;
            }

            if (propertyTypeName != null)
            {
                DerivedTypeValidator validator = propertyAndAnnotationCollector.GetDerivedTypeValidator(propertyName);
                if (validator != null)
                {
                    validator.ValidateResourceType(propertyTypeName);
                }
            }

            return propertyTypeName;
        }

        /// <summary>
        /// Adds a new property to a resource.
        /// </summary>
        /// <param name="resourceState">The resource state for the resource to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="propertyValue">The value of the property to add.</param>
        /// <returns>The added ODataProperty.</returns>
        protected static ODataProperty AddResourceProperty(IODataJsonReaderResourceState resourceState, string propertyName, object propertyValue)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            ODataProperty property = new ODataProperty { Name = propertyName, Value = propertyValue };
            AddResourceProperty(resourceState, propertyName, property);

            return property;
        }

        /// <summary>
        /// Adds a new property to a resource.
        /// </summary>
        /// <param name="resourceState">The resource state for the resource to add the property to.</param>
        /// <param name="propertyName">The name of the property to add.</param>
        /// <param name="property">The property to add.</param>
        protected static void AddResourceProperty(IODataJsonReaderResourceState resourceState, string propertyName, ODataPropertyInfo property)
        {
            Debug.Assert(resourceState != null, "resourceState != null");
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            AttachODataAnnotations(resourceState.PropertyAndAnnotationCollector, propertyName, property);
            AttachCustomAnnotations(resourceState.PropertyAndAnnotationCollector, propertyName, property);

            resourceState.PropertyAndAnnotationCollector.CheckForDuplicatePropertyNames(property);
            ODataResourceBase resource = resourceState.Resource;
            Debug.Assert(resource != null, "resource != null");

            // To avoid repeated computations for the property verification logic inside the
            // resource.Properties setter, we update the resource.Properties in-place
            // without invoking the setter each time.
            // Since the resource and properties are created internally by the reader,
            // we don't need to verify the properties if we can guarantee that we are only
            // adding properties with acceptable values (i.e. not ODataResourceValue).
            // We use debug-time assertions instead to catch such bugs in tests.

            // We should rethink the current approach to property verification.
            // See: https://github.com/OData/odata.net/issues/3263

            Debug.Assert(!(property is ODataProperty propertyWithValue && propertyWithValue.Value is ODataResourceValue),
                Error.Format(SRResources.ODataResource_PropertyValueCannotBeODataResourceValue, property.Name));
            Debug.Assert(
                !(property is ODataProperty collectionProp && collectionProp.Value is ODataCollectionValue collectionValue && collectionValue.Items.Any(item => item is ODataResourceValue)),
                Error.Format(SRResources.ODataResource_PropertyValueCannotBeODataResourceValue, property.Name));

            if (resource.Properties.IsEmptyReadOnlyEnumerable())
            {
                resource.Properties = new ReadOnlyEnumerable<ODataPropertyInfo>();
            }

            // Despite the name the resource.Properties type here is not actually
            // readonly, it supports appending in-place.
            resource.Properties.ConcatToReadOnlyEnumerable("Properties", property);
        }

        /// <summary>
        /// Attaches OData annotations to the property.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="property">The property.</param>
        protected static void AttachODataAnnotations(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string propertyName, ODataPropertyInfo property)
        {
            foreach (KeyValuePair<string, object> annotation
                     in propertyName.Length == 0
                        ? propertyAndAnnotationCollector.GetODataScopeAnnotation()
                        : propertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
            {
                Debug.Assert(annotation.Value != null);
                if (String.Equals(annotation.Key, ODataAnnotationNames.ODataType, StringComparison.Ordinal)
                    || String.Equals(annotation.Key, ODataJsonConstants.SimplifiedODataTypePropertyName, StringComparison.Ordinal))
                {
                    property.TypeAnnotation = new ODataTypeAnnotation(
                        ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName((string)annotation.Value)));
                }
                else
                {
                    Uri uri;
                    ODataValue val = (uri = annotation.Value as Uri) != null
                                        ? new ODataPrimitiveValue(uri.OriginalString)
                                        : annotation.Value.ToODataValue();
                    property.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Key, val, true));
                }
            }
        }

        /// <summary>
        /// Attaches custom annotations to the property.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="property">The property.</param>
        protected static void AttachCustomAnnotations(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string propertyName, ODataPropertyInfo property)
        {
            foreach (KeyValuePair<string, object> annotation in propertyAndAnnotationCollector.GetCustomPropertyAnnotations(propertyName))
            {
                if (annotation.Value != null)
                {
                    // annotation.Value == null indicates that this annotation should be skipped.
                    property.InstanceAnnotations.Add(new ODataInstanceAnnotation(annotation.Key, annotation.Value.ToODataValue()));
                }
            }
        }

        /// <summary>
        /// Tries to read an annotation as OData type name annotation.
        /// </summary>
        /// <param name="annotationName">The annotation name on which value the reader is positioned on.</param>
        /// <param name="value">The read value of the annotation (string).</param>
        /// <returns>true if the annotation is an OData type name annotation, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue - the value of the annotation
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.Property       - the next property after the annotation
        ///                 JsonNodeType.EndObject      - end of the parent object
        ///                 JsonNodeType.PrimitiveValue - the reader didn't move
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        ///
        /// If the method returns true, it consumed the value of the annotation from the reader.
        /// If it returns false, it didn't move the reader.
        /// </remarks>
        protected bool TryReadODataTypeAnnotationValue(string annotationName, out string value)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (string.Equals(annotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                value = this.ReadODataTypeAnnotationValue();
                return true;
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Reads the value of the odata.type annotation.
        /// </summary>
        /// <returns>The type name read from the annotation.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue - the value of the annotation, will fail if it's not PrimitiveValue
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.Property    - the next property after the annotation
        ///                 JsonNodeType.EndObject   - end of the parent object
        /// </remarks>
        protected string ReadODataTypeAnnotationValue()
        {
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            string typeName = ReaderUtils.AddEdmPrefixOfTypeName(ReaderUtils.RemovePrefixOfTypeName(this.JsonReader.ReadStringValue()));
            if (typeName == null)
            {
                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTypeName, typeName));
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return typeName;
        }

        /// <summary>
        /// Reads top-level property payload property annotation value.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation.</param>
        /// <returns>The value of the annotation read.</returns>
        protected object ReadTypePropertyAnnotationValue(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");

            string typeName;
            if (this.TryReadODataTypeAnnotationValue(propertyAnnotationName, out typeName))
            {
                return typeName;
            }

            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyAnnotationName));
        }

        /// <summary>
        /// Determines the value kind for a non-entity value (that is top-level property value, property value on a complex type, item in a collection)
        /// </summary>
        /// <returns>The type kind of the property value.</returns>
        /// <remarks>
        /// Doesn't move the JSON reader.
        /// </remarks>
        protected EdmTypeKind GetNonEntityValueKind()
        {
            // If we get here, we did not find a type name in the payload and don't have an expected type.
            // This can only happen for error cases when using open properties (for declared properties we always
            // have an expected type and for open properties we always require a type). We then decide based on
            // the node type of the reader.
            // PrimitiveValue       - we know that it is a primitive value;
            // StartArray           - we know that we have a collection;
            // Other                - for a JSON object value (and if we did not already find a payload type name)
            //                        we have already started reading the object to find a type name (and have failed)
            //                        and might thus be on a Property or EndObject node.
            //                        Also note that in this case we can't distinguish whether what we are looking at is
            //                        a complex value or a spatial value (both are JSON objects). We will report
            //                        'Complex' in that case which will fail we an appropriate error message
            //                        also for spatial ('value without type name found').
            switch (this.JsonReader.NodeType)
            {
                case JsonNodeType.PrimitiveValue: return EdmTypeKind.Primitive;
                case JsonNodeType.StartArray: return EdmTypeKind.Collection;
                default: return EdmTypeKind.Complex;
            }
        }

        /// <summary>
        /// Creates an instance of an ODataResourceSet that represents a nested collection
        /// </summary>
        /// <param name="resourceState">The current resource state</param>
        /// <param name="propertyName">The name of the collection property being read</param>
        /// <returns>An ODataResourceSet with properties set from any instance annotations</returns>
        private static ODataResourceSet CreateCollectionResourceSet(IODataJsonReaderResourceState resourceState, string propertyName)
        {
            ODataResourceSet collectionResourceSet = new ODataResourceSet();

            foreach (KeyValuePair<string, object> propertyAnnotation
                     in resourceState.PropertyAndAnnotationCollector.GetODataPropertyAnnotations(propertyName))
            {
                switch (propertyAnnotation.Key)
                {
                    case ODataAnnotationNames.ODataNextLink:
                        Debug.Assert(propertyAnnotation.Value is Uri && propertyAnnotation.Value != null, "The odata.nextLink annotation should have been parsed as a non-null Uri.");
                        collectionResourceSet.NextPageLink = (Uri)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataCount:
                        Debug.Assert(propertyAnnotation.Value is long && propertyAnnotation.Value != null, "The odata.count annotation should have been parsed as a non-null long.");
                        collectionResourceSet.Count = (long?)propertyAnnotation.Value;
                        break;

                    case ODataAnnotationNames.ODataType:
                        collectionResourceSet.TypeName = (string)propertyAnnotation.Value;
                        Debug.Assert(propertyAnnotation.Value is string && propertyAnnotation.Value != null, "The odata.type annotation should have been parsed as a non-null string.");
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_UnexpectedComplexCollectionPropertyAnnotation, propertyName, propertyAnnotation.Key));
                }
            }

            return collectionResourceSet;
        }

        /// <summary>
        /// Tries to read an annotation as OData type name annotation.
        /// </summary>
        /// <param name="payloadTypeName">The read value of the annotation (string).</param>
        /// <returns>true if the annotation is an OData type name annotation, false otherwise.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property       - the property that possibly is an odata.type instance annotation
        /// Post-Condition: JsonNodeType.Property       - the next property after the annotation or if the reader did not move
        ///                 JsonNodeType.EndObject      - end of the parent object
        /// If the method returns true, it consumed the value of the annotation from the reader.
        /// If it returns false, it didn't move the reader.
        /// </remarks>
        private bool TryReadODataTypeAnnotation(out string payloadTypeName)
        {
            this.AssertJsonCondition(JsonNodeType.Property);
            payloadTypeName = null;

            bool result = false;
            string propertyName = this.JsonReader.GetPropertyName();
            if (string.Equals(propertyName, ODataJsonConstants.PrefixedODataTypePropertyName, StringComparison.Ordinal)
                || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataTypePropertyName, propertyName))
            {
                // Read over the property name
                this.JsonReader.ReadNext();
                payloadTypeName = this.ReadODataTypeAnnotationValue();
                result = true;
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return result;
        }

        /// <summary>
        /// This method creates an reads the property from the input and
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use.</param>
        /// <returns>An <see cref="ODataProperty"/> representing the read property.</returns>
        /// <remarks>
        /// The method assumes that the ReadPayloadStart has already been called and it will not call ReadPayloadEnd.
        /// </remarks>
        private ODataProperty ReadTopLevelPropertyImplementation(IEdmTypeReference expectedPropertyTypeReference, PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            expectedPropertyTypeReference = this.UpdateExpectedTypeBasedOnContextUri(expectedPropertyTypeReference);

            object propertyValue = missingPropertyValue;
            Collection<ODataInstanceAnnotation> customInstanceAnnotations = new Collection<ODataInstanceAnnotation>();

            // Check for the special top-level OData 3.0 null marker in order to accommodate
            // the responses written by 6.x version of this library.
            if (this.IsTopLevel6xNullValue())
            {
                // NOTE: when reading a null value we will never ask the type resolver (if present) to resolve the
                //       type; we always fall back to the expected type.
                this.ReaderValidator.ValidateNullValue(
                    expectedPropertyTypeReference,
                    /*validateNullValue*/ true,
                    /*propertyName*/ null,
                    null);

                // We don't allow properties or non-custom annotations in the null payload.
                this.ValidateNoPropertyInNullPayload(propertyAndAnnotationCollector);

                propertyValue = null;
            }
            else
            {
                string payloadTypeName = null;
                if (this.ReadingResourceProperty(propertyAndAnnotationCollector, expectedPropertyTypeReference, out payloadTypeName))
                {
                    // Figure out whether we are reading a resource property or not; resource properties are not wrapped while all others are.
                    // Since we don't have metadata in all cases (open properties), we have to detect the type in some cases.
                    this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

                    // Now read the property value
                    propertyValue = this.ReadNonEntityValue(
                        payloadTypeName,
                        expectedPropertyTypeReference,
                        propertyAndAnnotationCollector,
                        /*collectionValidator*/ null,
                        /*validateNullValue*/ true,
                        /*isTopLevelPropertyValue*/ true,
                        /*insideResourceValue*/ true,
                        /*propertyName*/ null);
                }
                else
                {
                    bool isReordering = this.JsonReader is ReorderingJsonReader;

                    Func<string, object> propertyAnnotationReaderForTopLevelProperty =
                        annotationName => { throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation, annotationName)); };

                    // Read through all top-level properties, ignore the ones with reserved names (i.e., reserved
                    // characters in their name) and throw if we find none or more than one properties without reserved name.
                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        this.ProcessProperty(
                            propertyAndAnnotationCollector,
                            propertyAnnotationReaderForTopLevelProperty,
                            (propertyParsingResult, propertyName) =>
                            {
                                if (this.JsonReader.NodeType == JsonNodeType.Property)
                                {
                                    // Read over property name
                                    this.JsonReader.Read();
                                }

                                switch (propertyParsingResult)
                                {
                                    case PropertyParsingResult.ODataInstanceAnnotation:
                                        if (string.Equals(ODataAnnotationNames.ODataType, propertyName, StringComparison.Ordinal))
                                        {
                                            // When we are not using the reordering reader we have to ensure that the 'odata.type' property appears before
                                            // the 'value' property; otherwise we already scanned ahead and read the type name and have to now
                                            // ignore it (even if it is after the 'value' property).
                                            if (isReordering)
                                            {
                                                this.JsonReader.SkipValue();
                                            }
                                            else
                                            {
                                                if (!object.ReferenceEquals(missingPropertyValue, propertyValue))
                                                {
                                                    throw new ODataException(
                                                        Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TypePropertyAfterValueProperty, ODataAnnotationNames.ODataType, ODataJsonConstants.ODataValuePropertyName));
                                                }

                                                payloadTypeName = this.ReadODataTypeAnnotationValue();
                                            }
                                        }
                                        else
                                        {
                                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));
                                        }

                                        break;
                                    case PropertyParsingResult.CustomInstanceAnnotation:
                                        ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                        Debug.Assert(
                                            !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                            "!this.MessageReaderSettings.ShouldSkipAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                        object customInstanceAnnotationValue = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
                                        customInstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, customInstanceAnnotationValue.ToODataValue()));
                                        break;

                                    case PropertyParsingResult.PropertyWithoutValue:
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty, propertyName));

                                    case PropertyParsingResult.PropertyWithValue:
                                        if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                        {
                                            // Now read the property value
                                            propertyValue = this.ReadNonEntityValue(
                                                    payloadTypeName,
                                                    expectedPropertyTypeReference,
                                                    /*propertyAndAnnotationCollector*/ null,
                                                    /*collectionValidator*/ null,
                                                    /*validateNullValue*/ true,
                                                    /*isTopLevelPropertyValue*/ true,
                                                    /*insideResourceValue*/ false,
                                                    /*propertyName*/ propertyName);
                                        }
                                        else
                                        {
                                            throw new ODataException(
                                                Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName, propertyName, ODataJsonConstants.ODataValuePropertyName));
                                        }

                                        break;

                                    case PropertyParsingResult.EndOfObject:
                                        break;

                                    case PropertyParsingResult.MetadataReferenceProperty:
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                                }
                            });
                    }

                    if (object.ReferenceEquals(missingPropertyValue, propertyValue))
                    {
                        // No property found; there should be exactly one property in the top-level property wrapper that does not have a reserved name.
                        throw new ODataException(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
                    }
                }
            }

            Debug.Assert(!object.ReferenceEquals(missingPropertyValue, propertyValue), "!object.ReferenceEquals(missingPropertyValue, propertyValue)");
            ODataProperty resultProperty = new ODataProperty()
            {
                // The property name is not on the context URI or the payload, we report null.
                Name = null,
                Value = propertyValue,
                InstanceAnnotations = customInstanceAnnotations
            };

            // Read over the end object - note that this might be the last node in the input (in case there's no response wrapper)
            this.JsonReader.Read();
            return resultProperty;
        }

        /// <summary>
        /// Updates the expected type based on the context URI if there is one.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected property type reference provided by the user through public APIs, or null if one was not provided.</param>
        /// <returns>The expected type reference updated based on the context uri, if there is one.</returns>
        private IEdmTypeReference UpdateExpectedTypeBasedOnContextUri(IEdmTypeReference expectedPropertyTypeReference)
        {
            Debug.Assert(!this.JsonInputContext.ReadingResponse || this.ContextUriParseResult != null, "Responses should always have a context uri, and that should already have been validated.");
            if (this.ContextUriParseResult?.EdmType == null)
            {
                return expectedPropertyTypeReference;
            }

            IEdmType typeFromContextUri = this.ContextUriParseResult.EdmType;
            if (expectedPropertyTypeReference != null && !expectedPropertyTypeReference.Definition.IsAssignableFrom(typeFromContextUri))
            {
                throw new ODataException(Error.Format(SRResources.ReaderValidationUtils_TypeInContextUriDoesNotMatchExpectedType,
                        UriUtils.UriToString(this.ContextUriParseResult.ContextUri),
                        typeFromContextUri.FullTypeName(),
                        expectedPropertyTypeReference.FullName()));
            }

            // Assume the value is nullable as its the looser option and the value may come from an open property.
            bool isNullable = true;
            if (expectedPropertyTypeReference != null)
            {
                // if there is a user-provided expected type, then flow nullability information from it.
                isNullable = expectedPropertyTypeReference.IsNullable;
            }

            return typeFromContextUri.ToTypeReference(isNullable);
        }

        /// <summary>
        /// Reads a collection value.
        /// </summary>
        /// <param name="collectionValueTypeReference">The collection type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="typeAnnotation">The serialization type name for the collection value (possibly null).</param>
        /// <returns>The value of the collection.</returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartArray
        /// Post-Condition: almost anything - the node after the collection value (after the EndArray)
        /// </remarks>
        private ODataCollectionValue ReadCollectionValue(
            IEdmCollectionTypeReference collectionValueTypeReference,
            string payloadTypeName,
            ODataTypeAnnotation typeAnnotation)
        {
            Debug.Assert(
                collectionValueTypeReference == null || collectionValueTypeReference.IsNonEntityCollectionType(),
                "If the metadata is specified it must denote a Collection for this method to work.");

            this.IncreaseRecursionDepth();

            // Read over the start array
            this.JsonReader.ReadStartArray();

            ODataCollectionValue collectionValue = new ODataCollectionValue();
            collectionValue.TypeName = collectionValueTypeReference != null ? collectionValueTypeReference.FullName() : payloadTypeName;
            if (typeAnnotation != null)
            {
                collectionValue.TypeAnnotation = typeAnnotation;
            }

            List<object> items = new List<object>();
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
            IEdmTypeReference itemType = null;
            if (collectionValueTypeReference != null)
            {
                itemType = collectionValueTypeReference.CollectionDefinition().ElementType;
            }

            // NOTE: we do not support reading Json without metadata right now so we always have an expected item type;
            //       The collection validator is always null.
            CollectionWithoutExpectedTypeValidator collectionValidator = null;

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                object itemValue = this.ReadNonEntityValueImplementation(
                    /*payloadTypeName*/ null,
                    itemType,
                    propertyAndAnnotationCollector,
                    collectionValidator,
                    /*validateNullValue*/ true,
                    /*isTopLevelPropertyValue*/ false,
                    /*insideResourceValue*/ false,
                    /*propertyName*/ null);

                // Validate the item (for example that it's not null)
                ValidationUtils.ValidateCollectionItem(itemValue, itemType.IsNullable());

                // Note that the ReadNonEntityValue already validated that the actual type of the value matches
                // the expected type (the itemType).
                items.Add(itemValue);
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "The results value must end with an end array.");
            this.JsonReader.ReadEndArray();

            collectionValue.Items = new ReadOnlyEnumerable<object>(items);

            this.DecreaseRecursionDepth();

            return collectionValue;
        }

        /// <summary>
        /// Reads a type definition value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type definition reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The value of the primitive value.</returns>
        private object ReadTypeDefinitionValue(bool insideJsonObjectValue, IEdmTypeDefinitionReference expectedValueTypeReference, bool validateNullValue, string propertyName)
        {
            object result = this.ReadPrimitiveValue(insideJsonObjectValue, expectedValueTypeReference.AsPrimitive(), validateNullValue, propertyName);

            // Try convert to the expected CLR types from their underlying CLR types.
            try
            {
                return this.Model.GetPrimitiveValueConverter(expectedValueTypeReference).ConvertFromUnderlyingType(result);
            }
            catch (OverflowException)
            {
                throw new ODataException(Error.Format(SRResources.EdmLibraryExtensions_ValueOverflowForUnderlyingType, result, expectedValueTypeReference.FullName()));
            }
        }

        /// <summary>
        /// Reads a primitive value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The value of the primitive value.</returns>
        /// <remarks>
        /// Pre-Condition:  insideJsonObjectValue == false -> none - Fails if the current node is not a JsonNodeType.PrimitiveValue
        ///                 insideJsonObjectValue == true -> JsonNodeType.Property or JsonNodeType.EndObject - the first property of the value object,
        ///                     or the second property if first was odata.type, or the end-object.
        /// Post-Condition: almost anything - the node after the primitive value.
        /// </remarks>
        private object ReadPrimitiveValue(bool insideJsonObjectValue, IEdmPrimitiveTypeReference expectedValueTypeReference, bool validateNullValue, string propertyName)
        {
            object result;

            if (expectedValueTypeReference != null && expectedValueTypeReference.IsSpatial())
            {
                result = ODataJsonReaderCoreUtils.ReadSpatialValue(
                    this.JsonReader,
                    insideJsonObjectValue,
                    this.JsonInputContext,
                    expectedValueTypeReference,
                    validateNullValue,
                    this.recursionDepth,
                    propertyName);
            }
            else
            {
                if (insideJsonObjectValue)
                {
                    // We manually throw JSON exception here to get a nicer error message (we expect primitive value and got object).
                    // Otherwise the ReadPrimitiveValue would fail with something like "expected primitive value but found property/end object" which is rather confusing.
                    throw new ODataException(Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, propertyName));
                }

                result = this.JsonReader.ReadPrimitiveValue();

                if (expectedValueTypeReference != null)
                {
                    if ((expectedValueTypeReference.IsDecimal() || expectedValueTypeReference.IsInt64())
                        && result != null)
                    {
                        if ((result is string) ^ this.JsonReader.IsIeee754Compatible)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter, expectedValueTypeReference.FullName()));
                        }
                    }

                    result = ODataJsonReaderUtils.ConvertValue(
                        result,
                        expectedValueTypeReference,
                        this.MessageReaderSettings,
                        validateNullValue,
                        propertyName,
                        this.JsonInputContext.PayloadValueConverter);
                }
                else
                {
                    if (result is Decimal)
                    {
                        // convert decimal back to double to follow legacy logic when target type is not specified and IEEE754Compatible=false.
                        // we may lose precision for some range of int64 and decimal.
                        return Convert.ToDouble((Decimal)result);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Reads a primitive string as enum value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>The Enum value from the primitive string.</returns>
        private object ReadEnumValue(bool insideJsonObjectValue, IEdmEnumTypeReference expectedValueTypeReference, bool validateNullValue, string propertyName)
        {
            if (insideJsonObjectValue)
            {
                // We manually throw JSON exception here to get a nicer error message (we expect primitive value and got object).
                // Otherwise the ReadPrimitiveValue would fail with something like "expected primitive value but found property/end object" which is rather confusing.
                throw new ODataException(Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName, JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, propertyName));
            }

            string enumStr = this.JsonReader.ReadStringValue();
            return new ODataEnumValue(enumStr, expectedValueTypeReference.FullName());
        }

        /// <summary>
        /// Reads a resource value, <see cref="ODataResourceValue"/>
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="structuredTypeReference">The expected type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <returns>The value of the resource value.</returns>
        private ODataResourceValue ReadResourceValue(
            bool insideJsonObjectValue,
            bool insideResourceValue,
            string propertyName,
            IEdmStructuredTypeReference structuredTypeReference,
            string payloadTypeName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            if (!insideJsonObjectValue && !insideResourceValue)
            {
                if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    string typeName = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;
                    throw new ODataException(
                        Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ODataResourceExpectedForProperty, propertyName, this.JsonReader.NodeType, typeName));
                }

                this.JsonReader.Read();
            }

            return this.ReadResourceValue(structuredTypeReference, payloadTypeName, propertyAndAnnotationCollector);
        }

        /// <summary>
        /// Reads a resource value.
        /// </summary>
        /// <param name="structuredTypeReference">The expected type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <returns>The value of the resource value.</returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property - the first property of the resource value object, or the second one if the first one was odata.type.
        ///                 JsonNodeType.EndObject - the end object of the resource value object.
        /// Post-Condition: almost anything - the node after the resource value (after the EndObject)
        /// </remarks>
        private ODataResourceValue ReadResourceValue(
            IEdmStructuredTypeReference structuredTypeReference,
            string payloadTypeName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            this.IncreaseRecursionDepth();

            ODataResourceValue resourceValue = new ODataResourceValue();
            resourceValue.TypeName = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;

            if (structuredTypeReference != null)
            {
                resourceValue.TypeAnnotation = new ODataTypeAnnotation(resourceValue.TypeName);
            }

            List<ODataProperty> properties = new List<ODataProperty>();
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValue = this.ReadCustomInstanceAnnotationValue;
                this.ProcessProperty(
                    propertyAndAnnotationCollector,
                    this.ReadTypePropertyAnnotationValue,
                    (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            this.JsonReader.Read();
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation: // odata.*
                                if (string.Equals(ODataAnnotationNames.ODataType, propertyName, StringComparison.Ordinal))
                                {
                                    throw new ODataException(SRResources.ODataJsonPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst);
                                }
                                else
                                {
                                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));
                                }

                            case PropertyParsingResult.CustomInstanceAnnotation: // custom instance annotation
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldSkipAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object customInstanceAnnotationValue = this.ReadCustomInstanceAnnotationValue(propertyAndAnnotationCollector, propertyName);
                                resourceValue.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, customInstanceAnnotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty, propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                // Any other property is data
                                ODataProperty property = new ODataProperty();
                                property.Name = propertyName;

                                // Lookup the property in metadata
                                IEdmProperty edmProperty = null;
                                if (structuredTypeReference != null)
                                {
                                    edmProperty = ReaderValidationUtils.ValidatePropertyDefined(propertyName,
                                        structuredTypeReference.StructuredDefinition(), this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType,
                                        this.MessageReaderSettings.EnablePropertyNameCaseInsensitive);
                                }

                                // EdmLib bridge marks all key properties as non-nullable, but Astoria allows them to be nullable.
                                // If the property has an annotation to ignore null values, we need to omit the property in requests.
                                ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse || edmProperty == null
                                    ? ODataNullValueBehaviorKind.Default
                                    : this.Model.NullValueReadBehaviorKind(edmProperty);

                                // Read the property value
                                object propertyValue = this.ReadNonEntityValueImplementation(
                                    ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName),
                                    edmProperty == null ? null : edmProperty.Type,
                                    /*propertyAndAnnotationCollector*/ null,
                                    /*collectionValidator*/ null,
                                    nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                                    /*isTopLevelPropertyValue*/ false,
                                    /*insideResourceValue*/ false,
                                    propertyName,
                                    edmProperty == null);

                                if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.IgnoreValue || propertyValue != null)
                                {
                                    propertyAndAnnotationCollector.CheckForDuplicatePropertyNames(property);
                                    property.Value = propertyValue;
                                    AttachCustomAnnotations(propertyAndAnnotationCollector, propertyName, property);

                                    properties.Add(property);
                                }

                                break;

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                        }
                    });
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all the properties of a resource value are read the EndObject node is expected.");
            this.JsonReader.ReadEndObject();

            resourceValue.Properties = new ReadOnlyEnumerable<ODataProperty>(properties);

            this.DecreaseRecursionDepth();

            return resourceValue;
        }

        /// <summary>
        ///  Reads a primitive, enum, resource (complex or entity) or collection value.
        /// </summary>
        ///  <param name="payloadTypeName">The type name read from the payload as a property annotation, or null if none is available.</param>
        ///  <param name="expectedTypeReference">The expected type reference of the property value.</param>
        ///  <param name="propertyAndAnnotationCollector">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        ///  <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        ///  <param name="validateNullValue">true to validate null values; otherwise false.</param>
        ///  <param name="isTopLevelPropertyValue">true if we are reading a top-level property value; otherwise false.</param>
        ///  <param name="insideResourceValue">true if we are reading a complex value and the reader is already positioned inside the complex value; otherwise false.</param>
        ///  <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        ///  <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        ///  <returns>The value of the property read.</returns>
        ///  <remarks>
        ///  Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value
        ///                  JsonNodeType.StartArray       - the value of the property is an array
        ///  Post-Condition: almost anything - the node after the property value.
        ///
        ///  Returns the value of the property read, which can be one of:
        ///  - null
        ///  - primitive value
        ///  - <see cref="ODataCollectionValue"/>
        /// </remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No easy way to refactor.")]
        private object ReadNonEntityValueImplementation(
            string payloadTypeName,
            IEdmTypeReference expectedTypeReference,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            bool isTopLevelPropertyValue,
            bool insideResourceValue,
            string propertyName,
            bool? isDynamicProperty = null)
        {
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue || this.JsonReader.NodeType == JsonNodeType.StartObject ||
                this.JsonReader.NodeType == JsonNodeType.StartArray || (insideResourceValue && (this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject)),
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeTypeProperty (when inside complex value).");
            Debug.Assert(
                expectedTypeReference == null || collectionValidator == null,
                "If an expected value type reference is specified, no collection validator must be provided.");

            bool valueIsJsonObject = this.JsonReader.NodeType == JsonNodeType.StartObject;

            bool typeNameFoundInPayload = false;
            if (valueIsJsonObject || insideResourceValue)
            {
                // If we have an object value initialize the duplicate property names checker
                if (propertyAndAnnotationCollector == null)
                {
                    propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
                }
                else
                {
                    propertyAndAnnotationCollector.Reset();
                }

                // Read the payload type name
                if (!insideResourceValue)
                {
                    string typeName;
                    typeNameFoundInPayload = this.TryReadPayloadTypeFromObject(
                        propertyAndAnnotationCollector,
                        insideResourceValue,
                        out typeName);
                    if (typeNameFoundInPayload)
                    {
                        payloadTypeName = typeName;
                    }
                }
            }

            expectedTypeReference = expectedTypeReference != null && expectedTypeReference.IsUntyped() ? null : expectedTypeReference;
            ODataTypeAnnotation typeAnnotation;
            EdmTypeKind targetTypeKind;
            IEdmTypeReference targetTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                EdmTypeKind.None,
                /*expectStructuredType*/ null,
                /*defaultPrimitivePayloadType*/ null,
                expectedTypeReference,
                payloadTypeName,
                this.Model,
                this.GetNonEntityValueKind,
                out targetTypeKind,
                out typeAnnotation);

            object result;

            if (targetTypeKind == EdmTypeKind.Untyped || targetTypeKind == EdmTypeKind.None)
            {
                targetTypeReference = ResolveUntypedType(
                    this.JsonReader.NodeType,
                    this.JsonReader.GetValue(),
                    payloadTypeName,
                    expectedTypeReference,
                    this.MessageReaderSettings.PrimitiveTypeResolver,
                    this.MessageReaderSettings.ReadUntypedAsString,
                    !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

                targetTypeKind = targetTypeReference.TypeKind();
            }

            // Try to read a null value
            if (ODataJsonReaderCoreUtils.TryReadNullValue(this.JsonReader, this.JsonInputContext, targetTypeReference, validateNullValue, propertyName, isDynamicProperty))
            {
                if (this.JsonInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata && validateNullValue && targetTypeReference != null && !targetTypeReference.IsNullable)
                {
                    // For dynamic collection property, we should allow null value to be assigned to it.
                    if (targetTypeKind != EdmTypeKind.Collection || isDynamicProperty != true)
                    {
                        // A null value was found for the property named '{0}', which has the expected type '{1}[Nullable=False]'. The expected type '{1}[Nullable=False]' does not allow null values.
                        throw new ODataException(Error.Format(SRResources.ReaderValidationUtils_NullNamedValueForNonNullableType, propertyName, targetTypeReference.FullName()));
                    }
                }

                result = null;
            }
            else
            {
                Debug.Assert(
                    !valueIsJsonObject || this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "If the value was an object the reader must be on either property or end object.");
                switch (targetTypeKind)
                {
                    case EdmTypeKind.Primitive:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                        IEdmPrimitiveTypeReference primitiveTargetTypeReference = targetTypeReference == null ? null : targetTypeReference.AsPrimitive();

                        // If we found an odata.type annotation inside a primitive value, we have to fail; type annotations
                        // for primitive values are property annotations, not instance annotations inside the value.
                        if (typeNameFoundInPayload)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue, ODataAnnotationNames.ODataType));
                        }

                        result = this.ReadPrimitiveValue(
                            valueIsJsonObject,
                            primitiveTargetTypeReference,
                            validateNullValue,
                            propertyName);
                        break;

                    case EdmTypeKind.Enum:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataEnumTypeKind(), "Expected an OData enum type.");
                        IEdmEnumTypeReference enumTargetTypeReference = targetTypeReference == null ? null : targetTypeReference.AsEnum();
                        result = this.ReadEnumValue(
                            valueIsJsonObject,
                            enumTargetTypeReference,
                            validateNullValue,
                            propertyName);
                        break;

                    case EdmTypeKind.Complex: // nested complex
                    case EdmTypeKind.Entity: // nested entity
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataComplexTypeKind() || targetTypeReference.IsODataEntityTypeKind(), "Expected an OData complex or entity type.");
                        IEdmStructuredTypeReference structuredTypeTypeReference = targetTypeReference == null ? null : targetTypeReference.AsStructured();
                        result = ReadResourceValue(valueIsJsonObject,
                            insideResourceValue,
                            propertyName,
                            structuredTypeTypeReference,
                            payloadTypeName,
                            propertyAndAnnotationCollector);

                        break;

                    case EdmTypeKind.Collection:
                        IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(targetTypeReference);
                        if (valueIsJsonObject)
                        {
                            // We manually throw JSON exception here to get a nicer error message (we expect array value and got object).
                            // Otherwise the ReadCollectionValue would fail with something like "expected array value but found property/end object" which is rather confusing.
                            throw new ODataException(Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName, JsonNodeType.StartArray, JsonNodeType.StartObject, propertyName));
                        }

                        result = this.ReadCollectionValue(
                            collectionTypeReference,
                            payloadTypeName,
                            typeAnnotation);
                        break;

                    case EdmTypeKind.TypeDefinition:
                        result = this.ReadTypeDefinitionValue(
                            valueIsJsonObject,
                            expectedTypeReference.AsTypeDefinition(),
                            validateNullValue,
                            propertyName);
                        break;

                    case EdmTypeKind.Untyped:
                        result = this.JsonReader.ReadAsUntypedOrNullValue();
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.ODataJsonPropertyAndValueDeserializer_ReadPropertyValue));
                }

                // If we have no expected type make sure the collection items are of the same kind and specify the same name.
                if (collectionValidator != null && targetTypeKind != EdmTypeKind.None)
                {
                    string payloadTypeNameFromResult = ODataJsonReaderUtils.GetPayloadTypeName(result);
                    Debug.Assert(expectedTypeReference == null, "If a collection validator is specified there must not be an expected value type reference.");
                    collectionValidator.ValidateCollectionItem(payloadTypeNameFromResult, targetTypeKind);
                }
            }

            return result;
        }

        /// <summary>
        /// Reads the payload type name from a JSON object (if it exists).
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to track the detected 'odata.type' annotation (if any).</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <param name="payloadTypeName">The value of the odata.type annotation or null if no such annotation exists.</param>
        /// <returns>true if a type name was read from the payload; otherwise false.</returns>
        /// <remarks>
        /// Precondition:   StartObject     the start of a JSON object
        /// Postcondition:  Property        the first property of the object if no 'odata.type' annotation exists as first property
        ///                                 or the first property after the 'odata.type' annotation.
        ///                 EndObject       for an empty JSON object or an object with only the 'odata.type' annotation
        /// </remarks>
        private bool TryReadPayloadTypeFromObject(PropertyAndAnnotationCollector propertyAndAnnotationCollector, bool insideResourceValue, out string payloadTypeName)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(
                (this.JsonReader.NodeType == JsonNodeType.StartObject && !insideResourceValue) ||
                ((this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject) && insideResourceValue),
                "Pre-Condition: JsonNodeType.StartObject when not inside complex value; JsonNodeType.Property or JsonNodeType.EndObject otherwise.");
            bool readTypeName = false;
            payloadTypeName = null;

            // If not already positioned inside the JSON object, read over the object start
            if (!insideResourceValue)
            {
                this.JsonReader.ReadStartObject();
            }

            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                readTypeName = this.TryReadODataTypeAnnotation(out payloadTypeName);
                if (readTypeName)
                {
                    // Register the odata.type annotation we just found with the duplicate property names checker.
                    propertyAndAnnotationCollector.MarkPropertyAsProcessed(ODataAnnotationNames.ODataType);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            return readTypeName;
        }

        /// <summary>
        /// Detects whether we are currently reading a resource property or not. This can be determined from metadata (if we have it)
        /// or from the presence of the odata.type instance annotation in the payload.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker in use for the resource content.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <param name="payloadTypeName">The type name of the resource value if found in the payload; otherwise null.</param>
        /// <returns>true if we are reading a resource property; otherwise false.</returns>
        /// <remarks>
        /// This method does not move the reader.
        /// </remarks>
        private bool ReadingResourceProperty(PropertyAndAnnotationCollector propertyAndAnnotationCollector, IEdmTypeReference expectedPropertyTypeReference, out string payloadTypeName)
        {
            payloadTypeName = null;
            bool readingResourceProperty = false;

            // First try to use the metadata if is available
            if (expectedPropertyTypeReference != null)
            {
                readingResourceProperty = expectedPropertyTypeReference.IsStructured();
            }

            // Then check whether the first property in the JSON object is the 'odata.type'
            // annotation; if we don't have an expected property type reference, the 'odata.type'
            // annotation has to exist for resource properties. (This can happen for top-level open
            // properties).
            if (this.JsonReader.NodeType == JsonNodeType.Property && this.TryReadODataTypeAnnotation(out payloadTypeName))
            {
                // Register the odata.type annotation we just found with the duplicate property names checker.
                propertyAndAnnotationCollector.MarkPropertyAsProcessed(ODataAnnotationNames.ODataType);

                IEdmType expectedPropertyType = null;
                if (expectedPropertyTypeReference != null)
                {
                    expectedPropertyType = expectedPropertyTypeReference.Definition;
                }

                EdmTypeKind typeKind = EdmTypeKind.None;
                IEdmType actualWirePropertyTypeReference = MetadataUtils.ResolveTypeNameForRead(
                    this.Model,
                    expectedPropertyType,
                    payloadTypeName,
                    this.MessageReaderSettings.ClientCustomTypeResolver,
                    out typeKind);

                if (actualWirePropertyTypeReference != null)
                {
                    readingResourceProperty = actualWirePropertyTypeReference.IsODataComplexTypeKind() || actualWirePropertyTypeReference.IsODataEntityTypeKind();
                }
            }

            return readingResourceProperty;
        }

        /// <summary>
        /// Tries to read a top-level null value from the JSON reader.
        /// </summary>
        /// <returns>true if a null value could be read from the JSON reader; otherwise false.</returns>
        /// <remarks>If the method detects the odata.null annotation, it will read it; otherwise the reader does not move.</remarks>
        private bool IsTopLevel6xNullValue()
        {
            bool odataNullAnnotationInPayload = this.JsonReader.NodeType == JsonNodeType.Property && string.Equals(ODataJsonConstants.PrefixedODataNullPropertyName, JsonReader.GetPropertyName(), StringComparison.Ordinal);
            if (odataNullAnnotationInPayload)
            {
                // If we found the expected annotation read over the property name
                this.JsonReader.ReadNext();

                // Now check the value of the annotation
                object nullAnnotationValue = this.JsonReader.ReadPrimitiveValue();
                if (!(nullAnnotationValue is bool) || (bool)nullAnnotationValue == false)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonReaderUtils_InvalidValueForODataNullAnnotation, ODataAnnotationNames.ODataNull, ODataJsonConstants.ODataNullAnnotationTrueValue));
                }
            }

            return odataNullAnnotationInPayload;
        }

        /// <summary>
        /// Make sure that we don't find any other odata.* annotations or properties after reading a payload with the odata.null annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use.</param>
        private void ValidateNoPropertyInNullPayload(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            // we use the ParseProperty method to ignore custom annotations.
            Func<string, object> propertyAnnotationReaderForTopLevelNull = annotationName => { throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation, annotationName)); };
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ProcessProperty(
                propertyAndAnnotationCollector,
                propertyAnnotationReaderForTopLevelNull,
                (propertyParsingResult, propertyName) =>
                {
                    if (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        // Read over property name
                        this.JsonReader.Read();
                    }

                    switch (propertyParsingResult)
                    {
                        case PropertyParsingResult.ODataInstanceAnnotation:
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));

                        case PropertyParsingResult.CustomInstanceAnnotation:
                            this.JsonReader.SkipValue();
                            break;

                        case PropertyParsingResult.PropertyWithoutValue:
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty, propertyName));

                        case PropertyParsingResult.PropertyWithValue:
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload, propertyName));

                        case PropertyParsingResult.EndOfObject:
                            break;

                        case PropertyParsingResult.MetadataReferenceProperty:
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                    }
                });
            }
        }

        /// <summary>
        /// Try to asynchronously read or peek the odata.type annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use in current level.</param>
        /// <param name="propertyName">The property name.</param>
        /// <param name="insideResourceValue">If inside complex value.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the odata.type value or null.
        /// </returns>
        protected async Task<string> TryReadOrPeekPayloadTypeAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector, string propertyName, bool insideResourceValue)
        {
            string payloadTypeName = ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName);
            bool valueIsJsonObject = this.JsonReader.NodeType == JsonNodeType.StartObject;
            if (string.IsNullOrEmpty(payloadTypeName) && valueIsJsonObject)
            {
                try
                {
                    await this.JsonReader.StartBufferingAsync()
                        .ConfigureAwait(false);

                    // If we have an object value initialize the duplicate property names checker
                    propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();

                    // Read the payload type name
                    Tuple<bool, string> readPayloadTypeFromObjectResult = await this.TryReadPayloadTypeFromObjectAsync(
                        propertyAndAnnotationCollector,
                        insideResourceValue).ConfigureAwait(false);
                    bool typeNameFoundInPayload = readPayloadTypeFromObjectResult.Item1;
                    if (typeNameFoundInPayload)
                    {
                        payloadTypeName = readPayloadTypeFromObjectResult.Item2;
                    }
                }
                finally
                {
                    this.JsonReader.StopBuffering();
                }
            }

            return payloadTypeName;
        }

        /// <summary>
        /// Asynchronously reads an entity or complex type's undeclared property.
        /// </summary>
        /// <param name="resourceState">The IODataJsonReaderResourceState.</param>
        /// <param name="propertyName">Now this name can't be found in model.</param>
        /// <param name="isTopLevelPropertyValue">bool</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the read result.
        /// </returns>
        protected async Task<ODataJsonReaderNestedResourceInfo> InnerReadUndeclaredPropertyAsync(
            IODataJsonReaderResourceState resourceState,
            string propertyName,
            bool isTopLevelPropertyValue)
        {
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = resourceState.PropertyAndAnnotationCollector;
            bool insideResourceValue = false;
            string outerPayloadTypeName = ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName);
            string payloadTypeName = await this.TryReadOrPeekPayloadTypeAsync(propertyAndAnnotationCollector, propertyName, insideResourceValue)
                .ConfigureAwait(false);
            EdmTypeKind payloadTypeKind;
            IEdmType payloadType = ReaderValidationUtils.ResolvePayloadTypeName(
                this.Model,
                expectedTypeReference: null,
                payloadTypeName: payloadTypeName,
                expectedTypeKind: EdmTypeKind.Complex,
                clientCustomTypeResolver: this.MessageReaderSettings.ClientCustomTypeResolver,
                payloadTypeKind: out payloadTypeKind);
            IEdmTypeReference payloadTypeReference = null;
            if (!string.IsNullOrEmpty(payloadTypeName) && payloadType != null)
            {
                // only try resolving for known type (the below will throw on unknown type name) :
                ODataTypeAnnotation typeAnnotation;
                EdmTypeKind targetTypeKind;
                payloadTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                    EdmTypeKind.None,
                    expectStructuredType: null,
                    defaultPrimitivePayloadType: null,
                    expectedTypeReference: null,
                    payloadTypeName: payloadTypeName,
                    model: this.Model,
                    typeKindFromPayloadFunc: this.GetNonEntityValueKind,
                    targetTypeKind: out targetTypeKind,
                    typeAnnotation: out typeAnnotation);
            }

            object propertyValue = null;
            payloadTypeReference = ResolveUntypedType(
                this.JsonReader.NodeType,
                await this.JsonReader.GetValueAsync().ConfigureAwait(false),
                payloadTypeName,
                payloadTypeReference,
                this.MessageReaderSettings.PrimitiveTypeResolver,
                this.MessageReaderSettings.ReadUntypedAsString,
                !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

            if (payloadTypeReference.ToStructuredType() != null)
            {
                ODataJsonReaderNestedResourceInfo readerNestedResourceInfo = null;

                // Complex property or collection of complex property.
                bool isCollection = payloadTypeReference.IsCollection();
                await ValidateExpandedNestedResourceInfoPropertyValueAsync(
                    this.JsonReader,
                    isCollection,
                    propertyName,
                    payloadTypeReference).ConfigureAwait(false);

                if (isCollection)
                {
                    readerNestedResourceInfo =
                        ReadExpandedResourceSetNestedResourceInfo(resourceState, null, payloadTypeReference.ToStructuredType(), propertyName, isDeltaResourceSet: false);
                }
                else
                {
                    readerNestedResourceInfo = ReadExpandedResourceNestedResourceInfo(resourceState, null, propertyName, payloadTypeReference.ToStructuredType(), this.MessageReaderSettings);
                }

                resourceState.PropertyAndAnnotationCollector.ValidatePropertyUniquenessOnNestedResourceInfoStart(readerNestedResourceInfo.NestedResourceInfo);

                return readerNestedResourceInfo;
            }

            if (!(payloadTypeReference is IEdmUntypedTypeReference))
            {
                this.JsonReader.AssertNotBuffering();
                propertyValue = await this.ReadNonEntityValueImplementationAsync(
                    outerPayloadTypeName,
                    payloadTypeReference,
                    propertyAndAnnotationCollector: null,
                    collectionValidator: null,
                    validateNullValue: false,
                    isTopLevelPropertyValue: isTopLevelPropertyValue,
                    insideResourceValue: insideResourceValue,
                    propertyName: propertyName).ConfigureAwait(false);
            }
            else
            {
                propertyValue = await this.JsonReader.ReadAsUntypedOrNullValueAsync()
                    .ConfigureAwait(false);
            }

            this.JsonReader.AssertNotBuffering();
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                "Post-Condition: expected JsonNodeType.Property or JsonNodeType.EndObject");
            AddResourceProperty(resourceState, propertyName, propertyValue);
            return null;
        }

        /// <summary>
        /// Asynchronously validates that the value of a JSON property can represent expanded nested resource info.
        /// </summary>
        /// <param name="asyncJsonReader">The IJsonReaderAsync instance.</param>
        /// <param name="isCollection">true if the property is entity set reference property;
        /// false for a resource reference property, null if unknown.</param>
        /// <param name="propertyName">Name for the navigation property, used in error message only.</param>
        /// <param name="typeReference">Type of navigation property</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected static async Task ValidateExpandedNestedResourceInfoPropertyValueAsync(
            IJsonReader asyncJsonReader,
            bool? isCollection,
            string propertyName,
            IEdmTypeReference typeReference)
        {
            // An expanded link with resource requires a StartObject node here;
            // An expanded link with resource set requires a StartArray node here;
            // An expanded link with null resource requires a primitive null node here;
            JsonNodeType nodeType = asyncJsonReader.NodeType;
            if (nodeType == JsonNodeType.StartArray)
            {
                if (isCollection == false)
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadSingletonNestedResource, nodeType, propertyName));
                }
            }
            else if (
                (nodeType == JsonNodeType.PrimitiveValue && (await asyncJsonReader.GetValueAsync().ConfigureAwait(false)) == null)
                || nodeType == JsonNodeType.StartObject)
            {
                // Expanded resource (null or non-null)
                if (isCollection == true)
                {
                    if (typeReference.IsNonEntityCollectionType())
                    {
                        ReaderValidationUtils.ValidateNullValue(
                            expectedTypeReference: typeReference,
                            enablePrimitiveTypeConversion: true,
                            validateNullValue: true,
                            propertyName: propertyName,
                            isDynamicProperty: false);
                    }
                    else
                    {
                        throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadCollectionNestedResource, nodeType, propertyName));
                    }

                }
            }
            else
            {
                Debug.Assert(nodeType == JsonNodeType.PrimitiveValue, "nodeType == JsonNodeType.PrimitiveValue");
                throw new ODataException(Error.Format(SRResources.ODataJsonResourceDeserializer_CannotReadNestedResource, propertyName));
            }
        }

        /// <summary>
        /// Asynchronously reads the value of the OData type annotation.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the OData type annotation.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue - the value of the annotation, will fail if it's not PrimitiveValue
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.Property    - the next property after the annotation
        ///                 JsonNodeType.EndObject   - end of the parent object
        /// </remarks>
        protected async Task<string> ReadODataTypeAnnotationValueAsync()
        {
            this.AssertJsonCondition(JsonNodeType.PrimitiveValue, JsonNodeType.StartObject, JsonNodeType.StartArray);

            string typeName = ReaderUtils.AddEdmPrefixOfTypeName(
                ReaderUtils.RemovePrefixOfTypeName(
                    await this.JsonReader.ReadStringValueAsync().ConfigureAwait(false)));
            if (typeName == null)
            {
                // TODO: It's meaningless to output an error message using "typeName == null". Should use the original JSON value to construct the error message.
                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTypeName, typeName));
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return typeName;
        }

        /// <summary>
        /// Tries to asynchronously read an OData type annotation value.
        /// </summary>
        /// <param name="annotationName">The annotation name on which value the reader is positioned on.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). true if an OData type annotation was consumed from the reader; otherwise false - the reader was not advanced.
        /// 2). The annotation value (string) that was read.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.PrimitiveValue - the value of the annotation
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// Post-Condition: JsonNodeType.Property       - the next property after the annotation
        ///                 JsonNodeType.EndObject      - end of the parent object
        ///                 JsonNodeType.PrimitiveValue - the reader didn't move
        ///                 JsonNodeType.StartObject
        ///                 JsonNodeType.StartArray
        /// </remarks>
        protected async Task<Tuple<bool, string>> TryReadODataTypeAnnotationValueAsync(string annotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(annotationName), "!string.IsNullOrEmpty(annotationName)");

            if (string.Equals(annotationName, ODataAnnotationNames.ODataType, StringComparison.Ordinal))
            {
                return Tuple.Create(true, await this.ReadODataTypeAnnotationValueAsync().ConfigureAwait(false));
            }

            return Tuple.Create(false, (string)null);
        }

        /// <summary>
        /// Asynchronously reads top-level property payload property annotation value.
        /// </summary>
        /// <param name="propertyAnnotationName">The name of the property annotation.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the annotation.
        /// </returns>
        protected async Task<object> ReadTypePropertyAnnotationValueAsync(string propertyAnnotationName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyAnnotationName), "!string.IsNullOrEmpty(propertyAnnotationName)");
            Debug.Assert(
                propertyAnnotationName.StartsWith(ODataJsonConstants.ODataAnnotationNamespacePrefix, StringComparison.Ordinal),
                "The method should only be called with OData. annotations");

            Tuple<bool, string> readODataTypeAnnotationResult = await this.TryReadODataTypeAnnotationValueAsync(propertyAnnotationName)
                .ConfigureAwait(false);
            if (readODataTypeAnnotationResult.Item1)
            {
                string typeName = readODataTypeAnnotationResult.Item2;
                return typeName;
            }

            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyAnnotationName));
        }

        /// <summary>
        /// Tries to asynchronously read an OData type annotation.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). true if an OData type annotation was consumed from the reader; otherwise false - the reader was not advanced.
        /// 2). The annotation value (string) that was read.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property       - the property that possibly is an odata.type instance annotation
        /// Post-Condition: JsonNodeType.Property       - the next property after the annotation or if the reader did not move
        ///                 JsonNodeType.EndObject      - end of the parent object
        /// </remarks>
        private async Task<Tuple<bool, string>> TryReadODataTypeAnnotationAsync()
        {
            this.AssertJsonCondition(JsonNodeType.Property);
            string payloadTypeName = null;

            bool result = false;
            string propertyName = await this.JsonReader.GetPropertyNameAsync()
                .ConfigureAwait(false);
            if (string.Equals(propertyName, ODataJsonConstants.PrefixedODataTypePropertyName, StringComparison.Ordinal)
                || this.CompareSimplifiedODataAnnotation(ODataJsonConstants.SimplifiedODataTypePropertyName, propertyName))
            {
                // Read over the property name
                await this.JsonReader.ReadNextAsync()
                    .ConfigureAwait(false);
                payloadTypeName = await this.ReadODataTypeAnnotationValueAsync()
                    .ConfigureAwait(false);
                result = true;
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return Tuple.Create(result, payloadTypeName);
        }

        /// <summary>
        /// Asynchronously reads the property from the input and
        /// returns an <see cref="ODataProperty"/> representing the read property.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains an <see cref="ODataProperty"/> representing the read property.
        /// </returns>
        /// <remarks>
        /// The method assumes that the ReadPayloadStartAsync has already been called and it will not call ReadPayloadEndAsync.
        /// </remarks>
        private async Task<ODataProperty> ReadTopLevelPropertyImplementationAsync(
            IEdmTypeReference expectedPropertyTypeReference,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(
                expectedPropertyTypeReference == null || !expectedPropertyTypeReference.IsODataEntityTypeKind(),
                "If the expected type is specified it must not be an entity type.");
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            expectedPropertyTypeReference = this.UpdateExpectedTypeBasedOnContextUri(expectedPropertyTypeReference);

            object propertyValue = missingPropertyValue;
            Collection<ODataInstanceAnnotation> customInstanceAnnotations = new Collection<ODataInstanceAnnotation>();

            // Check for the special top-level OData 3.0 null marker in order to accommodate
            // the responses written by 6.x version of this library.
            if (await this.IsTopLevel6xNullValueAsync().ConfigureAwait(false))
            {
                // NOTE: When reading a null value we will never ask the type resolver (if present) to resolve the
                //       type; we always fall back to the expected type.
                this.ReaderValidator.ValidateNullValue(
                    expectedPropertyTypeReference,
                    validateNullValue: true,
                    propertyName: null,
                    isDynamicProperty: null);

                // We don't allow properties or non-custom annotations in the null payload.
                await this.ValidateNoPropertyInNullPayloadAsync(propertyAndAnnotationCollector)
                    .ConfigureAwait(false);

                propertyValue = null;
            }
            else
            {
                string payloadTypeName = null;
                Tuple<bool, string> readingResourcePropertyResult = await this.ReadingResourcePropertyAsync(
                    propertyAndAnnotationCollector,
                    expectedPropertyTypeReference).ConfigureAwait(false);
                bool isReadingResourceProperty = readingResourcePropertyResult.Item1;
                payloadTypeName = readingResourcePropertyResult.Item2;

                if (isReadingResourceProperty)
                {
                    // Figure out whether we are reading a resource property or not; resource properties are not wrapped while all others are.
                    // Since we don't have metadata in all cases (open properties), we have to detect the type in some cases.
                    this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

                    // Now read the property value
                    propertyValue = await this.ReadNonEntityValueAsync(
                        payloadTypeName,
                        expectedPropertyTypeReference,
                        propertyAndAnnotationCollector,
                        collectionValidator: null,
                        validateNullValue: true,
                        isTopLevelPropertyValue: true,
                        insideResourceValue: true,
                        propertyName: null).ConfigureAwait(false);
                }
                else
                {
                    bool isReordering = this.JsonReader is ReorderingJsonReader;

                    Func<string, Task<object>> readTopLevelPropertyAnnotationValueDelegate =
                        (annotationName) =>
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation, annotationName));
                        };

                    // Read through all top-level properties, ignore the ones with reserved names (i.e., reserved
                    // characters in their name) and throw if we find none or more than one properties without reserved name.
                    while (this.JsonReader.NodeType == JsonNodeType.Property)
                    {
                        await this.ProcessPropertyAsync(
                            propertyAndAnnotationCollector,
                            readTopLevelPropertyAnnotationValueDelegate,
                            async (propertyParsingResult, propertyName) =>
                            {
                                if (this.JsonReader.NodeType == JsonNodeType.Property)
                                {
                                    // Read over property name
                                    await this.JsonReader.ReadAsync()
                                        .ConfigureAwait(false);
                                }

                                switch (propertyParsingResult)
                                {
                                    case PropertyParsingResult.ODataInstanceAnnotation:
                                        if (string.Equals(ODataAnnotationNames.ODataType, propertyName, StringComparison.Ordinal))
                                        {
                                            // When we are not using the reordering reader we have to ensure that the 'odata.type' property appears before
                                            // the 'value' property; otherwise we already scanned ahead and read the type name and have to now
                                            // ignore it (even if it is after the 'value' property).
                                            if (isReordering)
                                            {
                                                await this.JsonReader.SkipValueAsync()
                                                    .ConfigureAwait(false);
                                            }
                                            else
                                            {
                                                if (!object.ReferenceEquals(missingPropertyValue, propertyValue))
                                                {
                                                    throw new ODataException(
                                                        Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TypePropertyAfterValueProperty, ODataAnnotationNames.ODataType, ODataJsonConstants.ODataValuePropertyName));
                                                }

                                                payloadTypeName = await this.ReadODataTypeAnnotationValueAsync()
                                                    .ConfigureAwait(false);
                                            }
                                        }
                                        else
                                        {
                                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));
                                        }

                                        break;
                                    case PropertyParsingResult.CustomInstanceAnnotation:
                                        ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                        Debug.Assert(
                                            !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                            "!this.MessageReaderSettings.ShouldSkipAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                        object customInstanceAnnotationValue = await this.ReadCustomInstanceAnnotationValueAsync(
                                            propertyAndAnnotationCollector,
                                            propertyName).ConfigureAwait(false);
                                        customInstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, customInstanceAnnotationValue.ToODataValue()));
                                        break;

                                    case PropertyParsingResult.PropertyWithoutValue:
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty, propertyName));

                                    case PropertyParsingResult.PropertyWithValue:
                                        if (string.Equals(ODataJsonConstants.ODataValuePropertyName, propertyName, StringComparison.Ordinal))
                                        {
                                            // Now read the property value
                                            propertyValue = await this.ReadNonEntityValueAsync(
                                                    payloadTypeName,
                                                    expectedPropertyTypeReference,
                                                    propertyAndAnnotationCollector: null,
                                                    collectionValidator: null,
                                                    validateNullValue: true,
                                                    isTopLevelPropertyValue: true,
                                                    insideResourceValue: false,
                                                    propertyName: propertyName).ConfigureAwait(false);
                                        }
                                        else
                                        {
                                            throw new ODataException(
                                                Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyName, propertyName, ODataJsonConstants.ODataValuePropertyName));
                                        }

                                        break;

                                    case PropertyParsingResult.EndOfObject:
                                        break;

                                    case PropertyParsingResult.MetadataReferenceProperty:
                                        throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                                }
                            }).ConfigureAwait(false);
                    }

                    if (object.ReferenceEquals(missingPropertyValue, propertyValue))
                    {
                        // No property found; there should be exactly one property in the top-level property wrapper that does not have a reserved name.
                        throw new ODataException(SRResources.ODataJsonPropertyAndValueDeserializer_InvalidTopLevelPropertyPayload);
                    }
                }
            }

            Debug.Assert(!object.ReferenceEquals(missingPropertyValue, propertyValue), "!object.ReferenceEquals(missingPropertyValue, propertyValue)");
            ODataProperty resultProperty = new ODataProperty
            {
                // The property name is not on the context URI or the payload, we report null.
                Name = null,
                Value = propertyValue,
                InstanceAnnotations = customInstanceAnnotations
            };

            // Read over the end object - note that this might be the last node in the input (in case there's no response wrapper)
            await this.JsonReader.ReadAsync()
                .ConfigureAwait(false);

            return resultProperty;
        }

        /// <summary>
        /// Asynchronously reads a collection value.
        /// </summary>
        /// <param name="collectionValueTypeReference">The collection type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="typeAnnotation">The serialization type name for the collection value (possibly null).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the collection value.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  Fails if the current node is not a JsonNodeType.StartArray
        /// Post-Condition: almost anything - the node after the collection value (after the EndArray)
        /// </remarks>
        private async Task<ODataCollectionValue> ReadCollectionValueAsync(
            IEdmCollectionTypeReference collectionValueTypeReference,
            string payloadTypeName,
            ODataTypeAnnotation typeAnnotation)
        {
            Debug.Assert(
                collectionValueTypeReference == null || collectionValueTypeReference.IsNonEntityCollectionType(),
                "If the metadata is specified it must denote a Collection for this method to work.");

            this.IncreaseRecursionDepth();

            // Read over the start array
            await this.JsonReader.ReadStartArrayAsync()
                .ConfigureAwait(false);

            ODataCollectionValue collectionValue = new ODataCollectionValue();
            collectionValue.TypeName = collectionValueTypeReference != null ? collectionValueTypeReference.FullName() : payloadTypeName;
            if (typeAnnotation != null)
            {
                collectionValue.TypeAnnotation = typeAnnotation;
            }

            List<object> items = new List<object>();
            PropertyAndAnnotationCollector propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
            IEdmTypeReference itemType = null;
            if (collectionValueTypeReference != null)
            {
                itemType = collectionValueTypeReference.CollectionDefinition().ElementType;
            }

            // NOTE: we do not support reading Json without metadata right now so we always have an expected item type;
            //       The collection validator is always null.
            CollectionWithoutExpectedTypeValidator collectionValidator = null;

            while (this.JsonReader.NodeType != JsonNodeType.EndArray)
            {
                object itemValue = await this.ReadNonEntityValueImplementationAsync(
                    payloadTypeName: null,
                    expectedTypeReference: itemType,
                    propertyAndAnnotationCollector: propertyAndAnnotationCollector,
                    collectionValidator: collectionValidator,
                    validateNullValue: true,
                    isTopLevelPropertyValue: false,
                    insideResourceValue: false,
                    propertyName: null).ConfigureAwait(false);

                // Validate the item (for example that it's not null)
                ValidationUtils.ValidateCollectionItem(itemValue, itemType.IsNullable());

                // Note that the ReadNonEntityValue already validated that the actual type of the value matches
                // the expected type (the itemType).
                items.Add(itemValue);
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndArray, "The results value must end with an end array.");
            await this.JsonReader.ReadEndArrayAsync()
                .ConfigureAwait(false);

            collectionValue.Items = new ReadOnlyEnumerable<object>(items);

            this.DecreaseRecursionDepth();

            return collectionValue;
        }

        /// <summary>
        /// Asynchronously reads a type definition value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type definition reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the type definition value.
        /// </returns>
        private async Task<object> ReadTypeDefinitionValueAsync(
            bool insideJsonObjectValue,
            IEdmTypeDefinitionReference expectedValueTypeReference,
            bool validateNullValue,
            string propertyName)
        {
            object result = await this.ReadPrimitiveValueAsync(
                insideJsonObjectValue,
                expectedValueTypeReference.AsPrimitive(),
                validateNullValue,
                propertyName).ConfigureAwait(false);

            // Try convert to the expected CLR types from their underlying CLR types.
            try
            {
                return this.Model.GetPrimitiveValueConverter(expectedValueTypeReference).ConvertFromUnderlyingType(result);
            }
            catch (OverflowException)
            {
                throw new ODataException(Error.Format(SRResources.EdmLibraryExtensions_ValueOverflowForUnderlyingType, result, expectedValueTypeReference.FullName()));
            }
        }

        /// <summary>
        /// Asynchronously reads a primitive value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the primitive value.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  insideJsonObjectValue == false -> none - Fails if the current node is not a JsonNodeType.PrimitiveValue
        ///                 insideJsonObjectValue == true -> JsonNodeType.Property or JsonNodeType.EndObject - the first property of the value object,
        ///                     or the second property if first was odata.type, or the end-object.
        /// Post-Condition: almost anything - the node after the primitive value.
        /// </remarks>
        private async Task<object> ReadPrimitiveValueAsync(
            bool insideJsonObjectValue,
            IEdmPrimitiveTypeReference expectedValueTypeReference,
            bool validateNullValue,
            string propertyName)
        {
            object result;

            if (expectedValueTypeReference != null && expectedValueTypeReference.IsSpatial())
            {
                result = await ODataJsonReaderCoreUtils.ReadSpatialValueAsync(
                    this.JsonReader,
                    insideJsonObjectValue,
                    this.JsonInputContext,
                    expectedValueTypeReference,
                    validateNullValue,
                    this.recursionDepth,
                    propertyName).ConfigureAwait(false);
            }
            else
            {
                if (insideJsonObjectValue)
                {
                    // We manually throw JSON exception here to get a nicer error message
                    // (we expected primitive value and got object).
                    // Otherwise the ReadPrimitiveValueAsync would fail with something like
                    // "expected primitive value but found property/end object" which is rather confusing.
                    throw new ODataException(
                        Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName,
                            JsonNodeType.PrimitiveValue,
                            JsonNodeType.StartObject,
                            propertyName));
                }

                result = await this.JsonReader.ReadPrimitiveValueAsync()
                    .ConfigureAwait(false);

                if (expectedValueTypeReference != null)
                {
                    if ((expectedValueTypeReference.IsDecimal() || expectedValueTypeReference.IsInt64())
                        && result != null)
                    {
                        if ((result is string) ^ this.JsonReader.IsIeee754Compatible)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonReaderUtils_ConflictBetweenInputFormatAndParameter, expectedValueTypeReference.FullName()));
                        }
                    }

                    result = ODataJsonReaderUtils.ConvertValue(
                        result,
                        expectedValueTypeReference,
                        this.MessageReaderSettings,
                        validateNullValue,
                        propertyName,
                        this.JsonInputContext.PayloadValueConverter);
                }
                else
                {
                    if (result is decimal decimalResult)
                    {
                        // convert decimal back to double to follow legacy logic when target type is not specified and IEEE754Compatible=false.
                        // we may lose precision for some range of int64 and decimal.
                        return Convert.ToDouble(decimalResult);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Asynchronously reads a primitive string as enum value.
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="expectedValueTypeReference">The expected type reference of the value, or null if none is available.</param>
        /// <param name="validateNullValue">true to validate null values; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the enum value of the primitive string.
        /// </returns>
        private async Task<object> ReadEnumValueAsync(
            bool insideJsonObjectValue,
            IEdmEnumTypeReference expectedValueTypeReference,
            bool validateNullValue,
            string propertyName)
        {
            if (insideJsonObjectValue)
            {
                // We manually throw JSON exception here to get a nicer error message
                // (we expect primitive value and got object).
                // Otherwise the ReadPrimitiveValue would fail with something like
                // "expected primitive value but found property/end object" which is rather confusing.
                throw new ODataException(
                    Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName,
                        JsonNodeType.PrimitiveValue,
                        JsonNodeType.StartObject,
                        propertyName));
            }

            string enumAsString = await this.JsonReader.ReadStringValueAsync()
                .ConfigureAwait(false);

            return new ODataEnumValue(enumAsString, expectedValueTypeReference.FullName());
        }

        /// <summary>
        /// Asynchronously reads a resource value, <see cref="ODataResourceValue"/>
        /// </summary>
        /// <param name="insideJsonObjectValue">true if the reader is positioned on the first property of the value which is a JSON Object
        ///     (or the second property if the first one was odata.type).</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        /// <param name="structuredTypeReference">The expected type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the read resource value.
        /// </returns>
        private async Task<ODataResourceValue> ReadResourceValueAsync(
            bool insideJsonObjectValue,
            bool insideResourceValue,
            string propertyName,
            IEdmStructuredTypeReference structuredTypeReference,
            string payloadTypeName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            if (!insideJsonObjectValue && !insideResourceValue)
            {
                if (this.JsonReader.NodeType != JsonNodeType.StartObject)
                {
                    string typeName = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;
                    throw new ODataException(
                        Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ODataResourceExpectedForProperty, propertyName, this.JsonReader.NodeType, typeName));
                }

                await this.JsonReader.ReadAsync()
                    .ConfigureAwait(false);
            }

            return await this.ReadResourceValueAsync(structuredTypeReference, payloadTypeName, propertyAndAnnotationCollector)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Asynchronously reads a resource value.
        /// </summary>
        /// <param name="structuredTypeReference">The expected type reference of the value.</param>
        /// <param name="payloadTypeName">The type name read from the payload.</param>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the read resource value.
        /// </returns>
        /// <remarks>
        /// Pre-Condition:  JsonNodeType.Property - the first property of the resource value object, or the second one if the first one was odata.type.
        ///                 JsonNodeType.EndObject - the end object of the resource value object.
        /// Post-Condition: almost anything - the node after the resource value (after the EndObject)
        /// </remarks>
        private async Task<ODataResourceValue> ReadResourceValueAsync(
            IEdmStructuredTypeReference structuredTypeReference,
            string payloadTypeName,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            this.IncreaseRecursionDepth();

            ODataResourceValue resourceValue = new ODataResourceValue();
            resourceValue.TypeName = structuredTypeReference != null ? structuredTypeReference.FullName() : payloadTypeName;

            if (structuredTypeReference != null)
            {
                resourceValue.TypeAnnotation = new ODataTypeAnnotation(resourceValue.TypeName);
            }

            List<ODataProperty> properties = new List<ODataProperty>();
            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                this.ReadPropertyCustomAnnotationValueAsync = this.ReadCustomInstanceAnnotationValueAsync;
                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    this.ReadTypePropertyAnnotationValueAsync,
                    async (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            await this.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation: // odata.*
                                if (string.Equals(ODataAnnotationNames.ODataType, propertyName, StringComparison.Ordinal))
                                {
                                    throw new ODataException(SRResources.ODataJsonPropertyAndValueDeserializer_ResourceTypeAnnotationNotFirst);
                                }
                                else
                                {
                                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));
                                }

                            case PropertyParsingResult.CustomInstanceAnnotation: // custom instance annotation
                                ODataAnnotationNames.ValidateIsCustomAnnotationName(propertyName);
                                Debug.Assert(
                                    !this.MessageReaderSettings.ShouldSkipAnnotation(propertyName),
                                    "!this.MessageReaderSettings.ShouldSkipAnnotation(annotationName) -- otherwise we should have already skipped the custom annotation and won't see it here.");
                                object customInstanceAnnotationValue = await this.ReadCustomInstanceAnnotationValueAsync(
                                    propertyAndAnnotationCollector,
                                    propertyName).ConfigureAwait(false);
                                resourceValue.InstanceAnnotations.Add(new ODataInstanceAnnotation(propertyName, customInstanceAnnotationValue.ToODataValue()));
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(
                                    Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ResourceValuePropertyAnnotationWithoutProperty, propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                // Any other property is data
                                ODataProperty property = new ODataProperty { Name = propertyName };

                                // Lookup the property in metadata
                                IEdmProperty edmProperty = null;
                                if (structuredTypeReference != null)
                                {
                                    edmProperty = ReaderValidationUtils.ValidatePropertyDefined(
                                        propertyName,
                                        structuredTypeReference.StructuredDefinition(),
                                        this.MessageReaderSettings.ThrowOnUndeclaredPropertyForNonOpenType,
                                        this.MessageReaderSettings.EnablePropertyNameCaseInsensitive);
                                }

                                // EdmLib bridge marks all key properties as non-nullable, but Astoria allows them to be nullable.
                                // If the property has an annotation to ignore null values, we need to omit the property in requests.
                                ODataNullValueBehaviorKind nullValueReadBehaviorKind = this.ReadingResponse || edmProperty == null
                                    ? ODataNullValueBehaviorKind.Default
                                    : this.Model.NullValueReadBehaviorKind(edmProperty);

                                // Read the property value
                                object propertyValue = await this.ReadNonEntityValueImplementationAsync(
                                    payloadTypeName: ValidateDataPropertyTypeNameAnnotation(propertyAndAnnotationCollector, propertyName),
                                    expectedTypeReference: edmProperty == null ? null : edmProperty.Type,
                                    propertyAndAnnotationCollector: null,
                                    collectionValidator: null,
                                    validateNullValue: nullValueReadBehaviorKind == ODataNullValueBehaviorKind.Default,
                                    isTopLevelPropertyValue: false,
                                    insideResourceValue: false,
                                    propertyName: propertyName,
                                    isDynamicProperty: edmProperty == null).ConfigureAwait(false);

                                if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.IgnoreValue || propertyValue != null)
                                {
                                    propertyAndAnnotationCollector.CheckForDuplicatePropertyNames(property);
                                    property.Value = propertyValue;
                                    AttachCustomAnnotations(propertyAndAnnotationCollector, propertyName, property);

                                    properties.Add(property);
                                }

                                break;

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                        }
                    }).ConfigureAwait(false);
            }

            Debug.Assert(this.JsonReader.NodeType == JsonNodeType.EndObject, "After all the properties of a resource value are read the EndObject node is expected.");
            await this.JsonReader.ReadEndObjectAsync()
                .ConfigureAwait(false);

            resourceValue.Properties = new ReadOnlyEnumerable<ODataProperty>(properties);

            this.DecreaseRecursionDepth();

            return resourceValue;
        }

        /// <summary>
        ///  Asynchronously reads a primitive, enum, resource (complex or entity) or collection value.
        /// </summary>
        ///  <param name="payloadTypeName">The type name read from the payload as a property annotation, or null if none is available.</param>
        ///  <param name="expectedTypeReference">The expected type reference of the property value.</param>
        ///  <param name="propertyAndAnnotationCollector">The duplicate property names checker to use - if null the method should create a new one if necessary.</param>
        ///  <param name="collectionValidator">The collection validator instance if no expected item type has been specified; otherwise null.</param>
        ///  <param name="validateNullValue">true to validate null values; otherwise false.</param>
        ///  <param name="isTopLevelPropertyValue">true if we are reading a top-level property value; otherwise false.</param>
        ///  <param name="insideResourceValue">true if we are reading a complex value and the reader is already positioned inside the complex value; otherwise false.</param>
        ///  <param name="propertyName">The name of the property whose value is being read, if applicable (used for error reporting).</param>
        ///  <param name="isDynamicProperty">Indicates whether the property is dynamic or unknown.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains the value of the property read, which can be one of:
        /// 1). null
        /// 2). Primitive value
        /// 3). <see cref="ODataEnumValue"/>
        /// 4). <see cref="ODataResourceValue"/>
        /// 5). <see cref="ODataCollectionValue"/>
        /// </returns>
        ///  <remarks>
        ///  Pre-Condition:  JsonNodeType.PrimitiveValue   - the value of the property is a primitive value
        ///                  JsonNodeType.StartArray       - the value of the property is an array
        ///  Post-Condition: almost anything - the node after the property value.
        /// </remarks>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "No easy way to refactor.")]
        private async Task<object> ReadNonEntityValueImplementationAsync(
            string payloadTypeName,
            IEdmTypeReference expectedTypeReference,
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            CollectionWithoutExpectedTypeValidator collectionValidator,
            bool validateNullValue,
            bool isTopLevelPropertyValue,
            bool insideResourceValue,
            string propertyName,
            bool? isDynamicProperty = null)
        {
            Debug.Assert(
                this.JsonReader.NodeType == JsonNodeType.PrimitiveValue
                || this.JsonReader.NodeType == JsonNodeType.StartObject
                || this.JsonReader.NodeType == JsonNodeType.StartArray
                || (insideResourceValue && (this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject)),
                "Pre-Condition: expected JsonNodeType.PrimitiveValue or JsonNodeType.StartObject or JsonNodeType.StartArray or JsonNodeTypeProperty (when inside complex value).");
            Debug.Assert(
                expectedTypeReference == null || collectionValidator == null,
                "If an expected value type reference is specified, no collection validator must be provided.");

            bool valueIsJsonObject = this.JsonReader.NodeType == JsonNodeType.StartObject;

            bool typeNameFoundInPayload = false;
            if (valueIsJsonObject || insideResourceValue)
            {
                // If we have an object value initialize the duplicate property names checker
                if (propertyAndAnnotationCollector == null)
                {
                    propertyAndAnnotationCollector = this.CreatePropertyAndAnnotationCollector();
                }
                else
                {
                    propertyAndAnnotationCollector.Reset();
                }

                // Read the payload type name
                if (!insideResourceValue)
                {
                    Tuple<bool, string> readPayloadTypeFromObjectResult = await this.TryReadPayloadTypeFromObjectAsync(
                        propertyAndAnnotationCollector,
                        insideResourceValue).ConfigureAwait(false);
                    typeNameFoundInPayload = readPayloadTypeFromObjectResult.Item1;
                    if (typeNameFoundInPayload)
                    {
                        payloadTypeName = readPayloadTypeFromObjectResult.Item2;
                    }
                }
            }

            expectedTypeReference = expectedTypeReference != null && expectedTypeReference.IsUntyped() ? null : expectedTypeReference;
            ODataTypeAnnotation typeAnnotation;
            EdmTypeKind targetTypeKind;
            IEdmTypeReference targetTypeReference = this.ReaderValidator.ResolvePayloadTypeNameAndComputeTargetType(
                expectedTypeKind: EdmTypeKind.None,
                expectStructuredType: null,
                defaultPrimitivePayloadType: null,
                expectedTypeReference: expectedTypeReference,
                payloadTypeName: payloadTypeName,
                model: this.Model,
                typeKindFromPayloadFunc: this.GetNonEntityValueKind,
                targetTypeKind: out targetTypeKind,
                typeAnnotation: out typeAnnotation);

            object result;

            if (targetTypeKind == EdmTypeKind.Untyped || targetTypeKind == EdmTypeKind.None)
            {
                targetTypeReference = ResolveUntypedType(
                    this.JsonReader.NodeType,
                    await this.JsonReader.GetValueAsync().ConfigureAwait(false),
                    payloadTypeName,
                    expectedTypeReference,
                    this.MessageReaderSettings.PrimitiveTypeResolver,
                    this.MessageReaderSettings.ReadUntypedAsString,
                    !this.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata);

                targetTypeKind = targetTypeReference.TypeKind();
            }

            // Try to read a null value
            if (await ODataJsonReaderCoreUtils.TryReadNullValueAsync(
                this.JsonReader,
                this.JsonInputContext,
                targetTypeReference,
                validateNullValue,
                propertyName,
                isDynamicProperty).ConfigureAwait(false))
            {
                if (this.JsonInputContext.MessageReaderSettings.ThrowIfTypeConflictsWithMetadata && validateNullValue && targetTypeReference != null && !targetTypeReference.IsNullable)
                {
                    // For dynamic collection property, we should allow null value to be assigned to it.
                    if (targetTypeKind != EdmTypeKind.Collection || isDynamicProperty != true)
                    {
                        // A null value was found for the property named '{0}', which has the expected type '{1}[Nullable=False]'. The expected type '{1}[Nullable=False]' does not allow null values.
                        throw new ODataException(Error.Format(SRResources.ReaderValidationUtils_NullNamedValueForNonNullableType, propertyName, targetTypeReference.FullName()));
                    }
                }

                result = null;
            }
            else
            {
                Debug.Assert(
                    !valueIsJsonObject || this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject,
                    "If the value was an object the reader must be on either property or end object.");
                switch (targetTypeKind)
                {
                    case EdmTypeKind.Primitive:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataPrimitiveTypeKind(), "Expected an OData primitive type.");
                        IEdmPrimitiveTypeReference primitiveTargetTypeReference = targetTypeReference == null ? null : targetTypeReference.AsPrimitive();

                        // If we found an odata.type annotation inside a primitive value, we have to fail; type annotations
                        // for primitive values are property annotations, not instance annotations inside the value.
                        if (typeNameFoundInPayload)
                        {
                            throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_ODataTypeAnnotationInPrimitiveValue, ODataAnnotationNames.ODataType));
                        }

                        result = await this.ReadPrimitiveValueAsync(
                            valueIsJsonObject,
                            primitiveTargetTypeReference,
                            validateNullValue,
                            propertyName).ConfigureAwait(false);
                        break;

                    case EdmTypeKind.Enum:
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataEnumTypeKind(), "Expected an OData enum type.");
                        IEdmEnumTypeReference enumTargetTypeReference = targetTypeReference == null ? null : targetTypeReference.AsEnum();
                        result = await this.ReadEnumValueAsync(
                            valueIsJsonObject,
                            enumTargetTypeReference,
                            validateNullValue,
                            propertyName).ConfigureAwait(false);
                        break;

                    case EdmTypeKind.Complex: // nested complex
                    case EdmTypeKind.Entity: // nested entity
                        Debug.Assert(targetTypeReference == null || targetTypeReference.IsODataComplexTypeKind() || targetTypeReference.IsODataEntityTypeKind(), "Expected an OData complex or entity type.");
                        IEdmStructuredTypeReference structuredTypeTypeReference = targetTypeReference == null ? null : targetTypeReference.AsStructured();
                        result = await this.ReadResourceValueAsync(
                            valueIsJsonObject,
                            insideResourceValue,
                            propertyName,
                            structuredTypeTypeReference,
                            payloadTypeName,
                            propertyAndAnnotationCollector).ConfigureAwait(false);

                        break;

                    case EdmTypeKind.Collection:
                        IEdmCollectionTypeReference collectionTypeReference = ValidationUtils.ValidateCollectionType(targetTypeReference);
                        if (valueIsJsonObject)
                        {
                            // We manually throw JSON exception here to get a nicer error message
                            // (we expect array value and got object).
                            // Otherwise the ReadCollectionValue would fail with something like
                            // "expected array value but found property/end object" which is rather confusing.
                            throw new ODataException(
                                Error.Format(SRResources.JsonReaderExtensions_UnexpectedNodeDetectedWithPropertyName,
                                    JsonNodeType.StartArray,
                                    JsonNodeType.StartObject,
                                    propertyName));
                        }

                        result = await this.ReadCollectionValueAsync(
                            collectionTypeReference,
                            payloadTypeName,
                            typeAnnotation).ConfigureAwait(false);
                        break;

                    case EdmTypeKind.TypeDefinition:
                        result = await this.ReadTypeDefinitionValueAsync(
                            valueIsJsonObject,
                            expectedTypeReference.AsTypeDefinition(),
                            validateNullValue,
                            propertyName).ConfigureAwait(false);
                        break;

                    case EdmTypeKind.Untyped:
                        result = await this.JsonReader.ReadAsUntypedOrNullValueAsync()
                            .ConfigureAwait(false);
                        break;

                    default:
                        throw new ODataException(Error.Format(SRResources.General_InternalError, InternalErrorCodes.ODataJsonPropertyAndValueDeserializer_ReadPropertyValue));
                }

                // If we have no expected type make sure the collection items are of the same kind and specify the same name.
                if (collectionValidator != null && targetTypeKind != EdmTypeKind.None)
                {
                    string payloadTypeNameFromResult = ODataJsonReaderUtils.GetPayloadTypeName(result);
                    Debug.Assert(expectedTypeReference == null, "If a collection validator is specified there must not be an expected value type reference.");
                    collectionValidator.ValidateCollectionItem(payloadTypeNameFromResult, targetTypeKind);
                }
            }

            return result;
        }

        /// <summary>
        /// Asynchronously reads the payload type name from a JSON object (if it exists).
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to track the detected 'odata.type' annotation (if any).</param>
        /// <param name="insideResourceValue">true if we are reading a resource value and the reader is already positioned inside the resource value; otherwise false.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). true if a type name was read from the payload; otherwise false.
        /// 2). The value of the odata.type annotation or null if no such annotation exists.
        /// </returns>
        /// <remarks>
        /// Precondition:   StartObject     the start of a JSON object
        /// Postcondition:  Property        the first property of the object if no 'odata.type' annotation exists as first property
        ///                                 or the first property after the 'odata.type' annotation.
        ///                 EndObject       for an empty JSON object or an object with only the 'odata.type' annotation
        /// </remarks>
        private async Task<Tuple<bool, string>> TryReadPayloadTypeFromObjectAsync(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            bool insideResourceValue)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");
            Debug.Assert(
                (this.JsonReader.NodeType == JsonNodeType.StartObject && !insideResourceValue) ||
                ((this.JsonReader.NodeType == JsonNodeType.Property || this.JsonReader.NodeType == JsonNodeType.EndObject) && insideResourceValue),
                "Pre-Condition: JsonNodeType.StartObject when not inside complex value; JsonNodeType.Property or JsonNodeType.EndObject otherwise.");
            bool readTypeName = false;
            string payloadTypeName = null;

            // If not already positioned inside the JSON object, read over the object start
            if (!insideResourceValue)
            {
                await this.JsonReader.ReadStartObjectAsync()
                    .ConfigureAwait(false);
            }

            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                Tuple<bool, string> readODataTypeAnnotationResult = await this.TryReadODataTypeAnnotationAsync()
                    .ConfigureAwait(false);
                readTypeName = readODataTypeAnnotationResult.Item1;
                if (readTypeName)
                {
                    payloadTypeName = readODataTypeAnnotationResult.Item2;
                    // Register the odata.type annotation we just found with the duplicate property names checker.
                    propertyAndAnnotationCollector.MarkPropertyAsProcessed(ODataAnnotationNames.ODataType);
                }
            }

            this.AssertJsonCondition(JsonNodeType.Property, JsonNodeType.EndObject);

            return Tuple.Create(readTypeName, payloadTypeName);
        }

        /// <summary>
        /// Asynchronously detects whether we are currently reading a resource property or not.
        /// This can be determined from metadata (if we have it)
        /// or from the presence of the odata.type instance annotation in the payload.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker in use for the resource content.</param>
        /// <param name="expectedPropertyTypeReference">The expected type reference of the property to read.</param>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains a tuple comprising of:
        /// 1). true if we are reading a resource property; otherwise false.
        /// 2). The type name of the resource value if found in the payload; otherwise null.
        /// </returns>
        /// <remarks>
        /// This method does not move the reader.
        /// </remarks>
        private async Task<Tuple<bool, string>> ReadingResourcePropertyAsync(
            PropertyAndAnnotationCollector propertyAndAnnotationCollector,
            IEdmTypeReference expectedPropertyTypeReference)
        {
            string payloadTypeName = null;
            bool readingResourceProperty = false;

            // First try to use the metadata if is available
            if (expectedPropertyTypeReference != null)
            {
                readingResourceProperty = expectedPropertyTypeReference.IsStructured();
            }

            // Then check whether the first property in the JSON object is the 'odata.type'
            // annotation; if we don't have an expected property type reference, the 'odata.type'
            // annotation has to exist for resource properties. (This can happen for top-level open
            // properties).
            if (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                Tuple<bool, string> readODataTypeAnnotationResult = await this.TryReadODataTypeAnnotationAsync()
                    .ConfigureAwait(false);
                if (readODataTypeAnnotationResult.Item1)
                {
                    payloadTypeName = readODataTypeAnnotationResult.Item2;
                    // Register the odata.type annotation we just found with the duplicate property names checker.
                    propertyAndAnnotationCollector.MarkPropertyAsProcessed(ODataAnnotationNames.ODataType);

                    IEdmType expectedPropertyType = null;
                    if (expectedPropertyTypeReference != null)
                    {
                        expectedPropertyType = expectedPropertyTypeReference.Definition;
                    }

                    IEdmType actualWirePropertyTypeReference = MetadataUtils.ResolveTypeNameForRead(
                        this.Model,
                        expectedPropertyType,
                        payloadTypeName,
                        this.MessageReaderSettings.ClientCustomTypeResolver,
                        out _);

                    if (actualWirePropertyTypeReference != null)
                    {
                        readingResourceProperty = actualWirePropertyTypeReference.IsODataComplexTypeKind() || actualWirePropertyTypeReference.IsODataEntityTypeKind();
                    }
                }
            }

            return Tuple.Create(readingResourceProperty, payloadTypeName);
        }

        /// <summary>
        /// Asynchronously tries to read a top-level null value from the JSON reader.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true
        /// if a null value could be read from the JSON reader; otherwise false.
        /// </returns>
        /// <remarks>If the method detects the odata.null annotation, it will read it; otherwise the reader does not move.</remarks>
        private async Task<bool> IsTopLevel6xNullValueAsync()
        {
            bool odataNullAnnotationInPayload = this.JsonReader.NodeType == JsonNodeType.Property
                && string.Equals(
                    ODataJsonConstants.PrefixedODataNullPropertyName,
                    await this.JsonReader.GetPropertyNameAsync().ConfigureAwait(false), StringComparison.Ordinal);

            if (odataNullAnnotationInPayload)
            {
                // If we found the expected annotation read over the property name
                await this.JsonReader.ReadNextAsync()
                    .ConfigureAwait(false);

                // Now check the value of the annotation
                object nullAnnotationValue = await this.JsonReader.ReadPrimitiveValueAsync()
                    .ConfigureAwait(false);
                if (!(nullAnnotationValue is bool) || (bool)nullAnnotationValue == false)
                {
                    throw new ODataException(
                        Error.Format(SRResources.ODataJsonReaderUtils_InvalidValueForODataNullAnnotation,
                            ODataAnnotationNames.ODataNull,
                            ODataJsonConstants.ODataNullAnnotationTrueValue));
                }
            }

            return odataNullAnnotationInPayload;
        }

        /// <summary>
        /// Asynchronously make sure that we don't find any other odata.* annotations or properties after reading a payload with the odata.null annotation.
        /// </summary>
        /// <param name="propertyAndAnnotationCollector">The duplicate property names checker to use.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task ValidateNoPropertyInNullPayloadAsync(PropertyAndAnnotationCollector propertyAndAnnotationCollector)
        {
            Debug.Assert(propertyAndAnnotationCollector != null, "propertyAndAnnotationCollector != null");

            // We use the ParsePropertyAsync method to ignore custom annotations.
            Func<string, Task<object>> readTopLevelPropertyAnnotationValueDelegate =
                (annotationName) =>
                {
                    throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedODataPropertyAnnotation, annotationName));
                };

            while (this.JsonReader.NodeType == JsonNodeType.Property)
            {
                await this.ProcessPropertyAsync(
                    propertyAndAnnotationCollector,
                    readTopLevelPropertyAnnotationValueDelegate,
                    async (propertyParsingResult, propertyName) =>
                    {
                        if (this.JsonReader.NodeType == JsonNodeType.Property)
                        {
                            // Read over property name
                            await this.JsonReader.ReadAsync()
                                .ConfigureAwait(false);
                        }

                        switch (propertyParsingResult)
                        {
                            case PropertyParsingResult.ODataInstanceAnnotation:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedAnnotationProperties, propertyName));

                            case PropertyParsingResult.CustomInstanceAnnotation:
                                await this.JsonReader.SkipValueAsync()
                                    .ConfigureAwait(false);
                                break;

                            case PropertyParsingResult.PropertyWithoutValue:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_TopLevelPropertyAnnotationWithoutProperty, propertyName));

                            case PropertyParsingResult.PropertyWithValue:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_NoPropertyAndAnnotationAllowedInNullPayload, propertyName));

                            case PropertyParsingResult.EndOfObject:
                                break;

                            case PropertyParsingResult.MetadataReferenceProperty:
                                throw new ODataException(Error.Format(SRResources.ODataJsonPropertyAndValueDeserializer_UnexpectedMetadataReferenceProperty, propertyName));
                        }
                    }).ConfigureAwait(false);
            }
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
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This is needed in DEBUG build.")]
        private void AssertRecursionDepthIsZero()
        {
            Debug.Assert(this.recursionDepth == 0, "The current recursion depth must be 0.");
        }
    }
}
