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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;

    using System.Xml;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Utility methods used with the OData library.
    /// </summary>
    public static class ODataUtils
    {
        /// <summary>String representation of the version 1.0 of the OData protocol.</summary>
        private const string Version1NumberString = "1.0";

        /// <summary>String representation of the version 2.0 of the OData protocol.</summary>
        private const string Version2NumberString = "2.0";

        /// <summary>String representation of the version 3.0 of the OData protocol.</summary>
        private const string Version3NumberString = "3.0";

        /// <summary>
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write methods on the <paramref name="messageWriter"/>.
        /// If it is sufficient to set the headers when the write methods on the <paramref name="messageWriter"/> 
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="messageWriter">The message writer to set the headers for.</param>
        /// <param name="payloadKind">The kind of payload to be written with the message writer.</param>
        public static void SetHeadersForPayload(ODataMessageWriter messageWriter, ODataPayloadKind payloadKind)
        {
            SetHeadersForPayload(messageWriter, payloadKind, null);
        }

        /// <summary>
        /// Sets the content-type and data service version headers on the message used by the message writer.
        /// This method can be called if it is important to set all the message headers before calling any of the
        /// write methods on the <paramref name="messageWriter"/>.
        /// If it is sufficient to set the headers when the write methods on the <paramref name="messageWriter"/> 
        /// are called, you don't have to call this method and setting the headers will happen automatically.
        /// </summary>
        /// <param name="messageWriter">The message writer to set the headers for.</param>
        /// <param name="payloadKind">The kind of payload to be written with the message writer.</param>
        /// <param name="mimeType">
        /// The MIME type to be used for writing the content of the message. Note that this is only supported for 
        /// top-level raw values.
        /// </param>
        public static void SetHeadersForPayload(ODataMessageWriter messageWriter, ODataPayloadKind payloadKind, string mimeType)
        {
            ExceptionUtils.CheckArgumentNotNull(messageWriter, "messageWriter");

            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                throw new ArgumentException(Strings.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(payloadKind), "payloadKind");
            }

            if (mimeType != null && payloadKind != ODataPayloadKind.Value)
            {
                throw new ArgumentException(Strings.ODataMessageWriter_CannotSetMimeTypeWithInvalidPayloadKind(payloadKind), "mimeType");
            }

            messageWriter.SetHeaders(payloadKind, mimeType);
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to process.</param>
        public static void LoadODataAnnotations(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            // NOTE: right now we only support annotations on entity types so we only iterate through them.
            foreach (IEdmEntityType entityType in model.EntityTypes())
            {
                LoadODataAnnotations(entityType);
            }
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        public static void LoadODataAnnotations(this IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            // PERF: should we be smarter and only update existing annotations as needed (and only drop the caches if needed)?

            // remove any existing in-memory EPM annotations and EPM caches on the entity type
            entityType.ClearInMemoryEpmAnnotations();

            string entityTypeName = entityType.ODataFullName();
            ODataEntityPropertyMappingCollection mappings = new ODataEntityPropertyMappingCollection();

            // Load the annotations from the type (these are the mappings for properties not explicitly declared on this type)
            entityType.LoadEpmAnnotations(mappings, entityTypeName, null /*property*/);

            IEnumerable<IEdmProperty> declaredProperties = entityType.DeclaredProperties;
            if (declaredProperties != null)
            {
                foreach (IEdmProperty property in declaredProperties)
                {
                    // Load the EPM annotations for all properties independent of their kind in order to fail on invalid property kinds.
                    property.LoadEpmAnnotations(mappings, entityTypeName, property);
                }
            }

            // Set the new EPM annotation on the entity type only at the very end to not leave a 
            // inconsistent annotation if building it fails.
            entityType.SetAnnotation(mappings);

            entityType.EnsureEpmCache();
        }

        /// <summary>
        /// Turns the in-memory representations of the supported, OData-specific annotations into their serializable form.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to process.</param>
        public static void SaveODataAnnotations(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            if (!model.IsUserModel())
            {
                throw new ODataException(Strings.ODataUtils_CannotSaveAnnotationsToBuiltInModel);
            }

            foreach (IEdmEntityType entityType in model.EntityTypes())
            {
                SaveODataAnnotationsImplementation(entityType);
            }
        }

        /// <summary>
        /// Turns the in-memory representations of the supported, OData-specific annotations into their serializable form.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        public static void SaveODataAnnotations(this IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            SaveODataAnnotationsImplementation(entityType);
        }

        /// <summary>
        /// Checks whether the <paramref name="entityType"/> has a default stream.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to check.</param>
        /// <returns>true if the entity type has a default stream; otherwise false.</returns>
        public static bool HasDefaultStream(this IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            return GetBooleanAnnotation(entityType, EdmConstants.HasStreamAttributeName);
        }

        /// <summary>
        /// Adds or removes a default stream to/from the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to modify.</param>
        /// <param name="hasStream">true to add a default stream to the entity type; false to remove an existing default stream (if any).</param>
        public static void SetHasDefaultStream(this IEdmEntityType entityType, bool hasStream)
        {
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            SetBooleanAnnotation(entityType, EdmConstants.HasStreamAttributeName, hasStream);
        }

        /// <summary>
        /// Checks whether the <paramref name="entityContainer"/> is the default entity container.
        /// </summary>
        /// <param name="entityContainer">The <see cref="IEdmEntityContainer"/> to check.</param>
        /// <returns>true if the <paramref name="entityContainer"/> is the default container; otherwise false.</returns>
        public static bool IsDefaultEntityContainer(this IEdmEntityContainer entityContainer)
        {
            ExceptionUtils.CheckArgumentNotNull(entityContainer, "entityContainer");
            return GetBooleanAnnotation(entityContainer, EdmConstants.IsDefaultEntityContainerAttributeName);
        }

        /// <summary>
        /// Adds or removes a default stream to/from the <paramref name="entityContainer"/>.
        /// </summary>
        /// <param name="entityContainer">The <see cref="IEdmEntityContainer"/> to modify.</param>
        /// <param name="isDefaultContainer">true to set the <paramref name="entityContainer"/> as the default container; false to remove an existing default container annotation (if any).</param>
        public static void SetIsDefaultEntityContainer(this IEdmEntityContainer entityContainer, bool isDefaultContainer)
        {
            ExceptionUtils.CheckArgumentNotNull(entityContainer, "entityType");
            SetBooleanAnnotation(entityContainer, EdmConstants.HasStreamAttributeName, isDefaultContainer);
        }

        /// <summary>
        /// Checks whether the <paramref name="annotatable"/> has a MIME type annotation.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to check.</param>
        /// <returns>The value of the MIME type annotation of the <paramref name="annotatable"/> or null if no MIME type annotation exists.</returns>
        public static string GetMimeType(this IEdmAnnotatable annotatable)
        {
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            return annotatable.GetODataAnnotation(EdmConstants.MimeTypeAttributeName);
        }

        /// <summary>
        /// Sets the MIME type annotation of the <paramref name="annotatable"/> to <paramref name="mimeType"/>.
        /// </summary>
        /// <param name="annotatable">The <see cref="IEdmAnnotatable"/> to modify.</param>
        /// <param name="mimeType">The MIME type value to set as annotation value; if null, an existing annotation will be removed.</param>
        /// <remarks>The MIME type annotation is only supported on service operations and primitive properties for serialization purposes.</remarks>
        public static void SetMimeType(this IEdmAnnotatable annotatable, string mimeType)
        {
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");

            annotatable.SetODataAnnotation(EdmConstants.MimeTypeAttributeName, mimeType);
        }

        /// <summary>
        /// Gets the result kind of the <paramref name="serviceOperation"/>.
        /// </summary>
        /// <param name="serviceOperation">The <see cref="IEdmFunctionImport"/> to check.</param>
        /// <returns>The result kind of the <paramref name="serviceOperation"/> or null if no result kind annotation exists.</returns>
        public static ODataServiceOperationResultKind? GetServiceOperationResultKind(this IEdmFunctionImport serviceOperation)
        {
            ExceptionUtils.CheckArgumentNotNull(serviceOperation, "functionImport");

            ODataEdmServiceOperationAnnotation annotation = serviceOperation.GetAnnotation<ODataEdmServiceOperationAnnotation>();
            return annotation == null ? null : (ODataServiceOperationResultKind?)annotation.ResultKind;
        }

        /// <summary>
        /// Sets the result kind of the <paramref name="serviceOperation"/>.
        /// </summary>
        /// <param name="serviceOperation">The <see cref="IEdmFunctionImport"/> to check.</param>
        /// <param name="resultKind">The result kind to set.</param>
        public static void SetServiceOperationResultKind(this IEdmFunctionImport serviceOperation, ODataServiceOperationResultKind resultKind)
        {
            ExceptionUtils.CheckArgumentNotNull(serviceOperation, "serviceOperation");

            ODataEdmServiceOperationAnnotation existingAnnotation = serviceOperation.GetAnnotation<ODataEdmServiceOperationAnnotation>();
            if (existingAnnotation == null)
            {
                ODataEdmServiceOperationAnnotation newAnnotation = 
                    new ODataEdmServiceOperationAnnotation
                    {
                        ResultKind = resultKind
                    };
                serviceOperation.SetAnnotation(newAnnotation);
            }
            else
            {
                existingAnnotation.ResultKind = resultKind;
            }
        }

        /// <summary>
        /// Returns the instance type for the specified <paramref name="typeReference"/> or null if none exists.
        /// </summary>
        /// <param name="typeReference">The type reference to get the instance type for.</param>
        /// <returns>The instance type for the <paramref name="typeReference"/> or null if no instance type exists.</returns>
        /// <remarks>All primitive type references are guaranteed to have an instance type.</remarks>
        public static Type GetInstanceType(this IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            if (typeReference.TypeKind() == EdmTypeKind.Primitive)
            {
                IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
                Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

                return primitiveTypeReference.GetInstanceType();
            }

            ODataEdmTypeAnnotation annotation = typeReference.Definition.GetAnnotation<ODataEdmTypeAnnotation>();
            return annotation == null ? null : annotation.InstanceType;
        }

        /// <summary>
        /// Returns the instance type for the specified <paramref name="type"/> or null if none exists.
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <returns>The instance type for the <paramref name="type"/> or null if no instance type exists.</returns>
        public static Type GetInstanceType(this IEdmType type)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                return ((IEdmPrimitiveTypeReference)type.ToTypeReference(false)).GetInstanceType();
            }

            ODataEdmTypeAnnotation annotation = type.GetAnnotation<ODataEdmTypeAnnotation>();
            return annotation == null ? null : annotation.InstanceType;
        }

        /// <summary>
        /// Sets the instance type for the specified <paramref name="type"/>; if null is specified an existing instance type will be removed.
        /// </summary>
        /// <param name="type">The type to get the instance type for.</param>
        /// <param name="instanceType">The instance type for the <paramref name="type"/> or null to remove an existing instance type.</param>
        public static void SetInstanceType(this IEdmType type, Type instanceType)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                throw new ODataException(Strings.ODataUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }

            ODataEdmTypeAnnotation existingAnnotation = type.GetAnnotation<ODataEdmTypeAnnotation>();
            if (existingAnnotation == null)
            {
                if (instanceType != null)
                {
                    ODataEdmTypeAnnotation newAnnotation = new ODataEdmTypeAnnotation
                    {
                        InstanceType = instanceType,
                    };

                    type.SetAnnotation(newAnnotation);
                }
            }
            else
            {
                existingAnnotation.InstanceType = instanceType;
            }
        }

        /// <summary>
        /// Checks whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="typeReference">The type reference to check.</param>
        /// <returns>true if reflection over the instance type is allowed; otherwise false.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "primitiveTypeReference", Justification = "Local used in debug assertion.")]
        public static bool GetCanReflectOnInstanceType(this IEdmTypeReference typeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            if (typeReference.TypeKind() == EdmTypeKind.Primitive)
            {
                IEdmPrimitiveTypeReference primitiveTypeReference = typeReference.AsPrimitiveOrNull();
                Debug.Assert(primitiveTypeReference != null, "primitiveTypeReference != null");

                // we can reflect over all primitive types
                return true;
            }

            ODataEdmTypeAnnotation annotation = typeReference.Definition.GetAnnotation<ODataEdmTypeAnnotation>();
            return annotation == null ? false : annotation.CanReflectOnInstanceType;
        }

        /// <summary>
        /// Sets whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="typeReference">The type reference to check.</param>
        /// <param name="canReflect">true if reflection over the instance type is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceType(this IEdmTypeReference typeReference, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(typeReference, "typeReference");

            SetCanReflectOnInstanceType(typeReference.Definition, canReflect);
        }

        /// <summary>
        /// Sets whether reflection over the instance type is allowed or not.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="canReflect">true if reflection over the instance type is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceType(this IEdmType type, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(type, "type");

            if (type.TypeKind == EdmTypeKind.Primitive)
            {
                throw new ODataException(Strings.ODataUtils_CannotSetMetadataAnnotationOnPrimitiveType);
            }

            ODataEdmTypeAnnotation annotation = type.GetAnnotation<ODataEdmTypeAnnotation>();
            if (annotation == null)
            {
                if (canReflect)
                {
                    annotation = new ODataEdmTypeAnnotation { CanReflectOnInstanceType = true };
                    type.SetAnnotation(annotation);
                }
            }
            else
            {
                annotation.CanReflectOnInstanceType = canReflect;
            }
        }

        /// <summary>
        /// Checks whether reflection over the property is allowed or not.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <returns>true if reflection over the property is allowed; otherwise false.</returns>
        public static bool GetCanReflectOnInstanceTypeProperty(this IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            ODataEdmPropertyAnnotation annotation = property.GetAnnotation<ODataEdmPropertyAnnotation>();
            return annotation == null ? false : annotation.CanReflectOnProperty;
        }

        /// <summary>
        /// Sets whether reflection over the property is allowed or not.
        /// </summary>
        /// <param name="property">The property to check.</param>
        /// <param name="canReflect">true if reflection over the property is allowed; otherwise false.</param>
        public static void SetCanReflectOnInstanceTypeProperty(this IEdmProperty property, bool canReflect)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");

            ODataEdmPropertyAnnotation annotation = property.GetAnnotation<ODataEdmPropertyAnnotation>();
            if (annotation == null)
            {
                if (canReflect)
                {
                    annotation = new ODataEdmPropertyAnnotation
                    {
                        CanReflectOnProperty = true
                    };
                    property.SetAnnotation(annotation);
                }
            }
            else
            {
                annotation.CanReflectOnProperty = canReflect;
            }
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmEntitySet"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="entitySetName">The name of the entity set to look up.</param>
        /// <returns>An <see cref="IEdmEntitySet"/> instance with the specified <paramref name="entitySetName"/>; if no such entity set exists the method throws.</returns>
        public static IEdmEntitySet ResolveEntitySet(this IEdmModel model, string entitySetName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, "entitySetName");

            IEdmEntitySet entitySet = model.TryResolveEntitySet(entitySetName);
            if (entitySet == null)
            {
                throw new ODataException(Strings.ODataUtils_DidNotFindEntitySet(entitySetName));
            }

            return entitySet;
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmEntitySet"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="entitySetName">The name of the entity set to look up.</param>
        /// <returns>An <see cref="IEdmEntitySet"/> instance with the specified <paramref name="entitySetName"/> or null if no such entity set exists.</returns>
        public static IEdmEntitySet TryResolveEntitySet(this IEdmModel model, string entitySetName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(entitySetName, "entitySetName");

            // we only support a single entity container right now
            IEdmEntityContainer container = model.EntityContainers.Single();
            return container.FindEntitySet(entitySetName);
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmFunctionImport"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="operationName">The name of the service operation to look up.</param>
        /// <returns>An <see cref="IEdmFunctionImport"/> instance with the specified <paramref name="operationName"/>; if no such service operation exists the method throws.</returns>
        public static IEdmFunctionImport ResolveServiceOperation(this IEdmModel model, string operationName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            IEdmFunctionImport functionImport = model.TryResolveServiceOperation(operationName);
            if (functionImport == null)
            {
                throw new ODataException(Strings.ODataUtils_DidNotFindServiceOperation(operationName));
            }

            return functionImport;
        }

        /// <summary>
        /// Resolves a name to an <see cref="IEdmFunctionImport"/> instance.
        /// </summary>
        /// <param name="model">The model to resolve the name against.</param>
        /// <param name="operationName">The name of the service operation to look up.</param>
        /// <returns>An <see cref="IEdmFunctionImport"/> instance with the specified <paramref name="operationName"/> or null if no such service operation exists.</returns>
        public static IEdmFunctionImport TryResolveServiceOperation(this IEdmModel model, string operationName)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");

            // we only support a single entity container right now
            IEdmEntityContainer container = model.EntityContainers.Single();

            IEnumerable<IEdmFunctionImport> functionImports = container.FindFunctionImports(operationName);
            if (functionImports == null)
            {
                return null;
            }

            IEdmFunctionImport functionImport = null;
            foreach (IEdmFunctionImport import in functionImports)
            {
                if (functionImport == null)
                {
                    functionImport = import;
                }
                else
                {
                    throw new ODataException(Strings.ODataUtils_FoundMultipleServiceOperations(operationName));
                }
            }

            // Ensure that we have an annotation on the function import that makes it a service operation
            if (functionImport != null)
            {
                ODataEdmServiceOperationAnnotation serviceOperationAnnotation =
                    functionImport.GetAnnotation<ODataEdmServiceOperationAnnotation>();
                if (serviceOperationAnnotation == null)
                {
                    return null;
                }
            }

            return functionImport;
        }

        /// <summary>
        /// Converts the version number enum into the string header value.
        /// </summary>
        /// <param name="version"> The <see cref="ODataVersion"/> to convert into a string.</param>
        /// <returns>A string representation of the version number.</returns>
        public static string ODataVersionToString(ODataVersion version)
        {
            switch (version)
            {
                case ODataVersion.V1:
                    return Version1NumberString;

                case ODataVersion.V2:
                    return Version2NumberString;

                case ODataVersion.V3:
                    return Version3NumberString;

                default:
                    // invalid enum value - unreachable.
                    throw new ODataException(Strings.ODataUtils_UnsupportedVersionNumber);
            }
        }

        /// <summary>
        /// Converts the version string into the internal enum.
        /// </summary>
        /// <param name="version"> The string to convert into a <see cref="ODataVersion"/>.</param>
        /// <returns>A <see cref="ODataVersion"/> representation of the version string.</returns>
        public static ODataVersion StringToODataVersion(string version)
        {
            // don't want to edit the string later.
            string modifiedVersion = version;

            // version must not be null or empty
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(version, "version");

            int ix = modifiedVersion.IndexOf(';');
            if (ix >= 0)
            {
                modifiedVersion = modifiedVersion.Substring(0, ix);
            }

            switch (modifiedVersion.Trim())
            {
                case Version1NumberString:
                    return ODataVersion.V1;

                case Version2NumberString:
                    return ODataVersion.V2;

                case Version3NumberString:
                    return ODataVersion.V3;

                default:
                    // invalid version string
                    throw new ODataException(Strings.ODataUtils_UnsupportedVersionHeader(version));
            }
        }

        /// <summary>
        /// Turns the in-memory representations of the supported, OData-specific annotations into their serializable form.
        /// Assumes that the entity type and the model have been validated.
        /// </summary>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        private static void SaveODataAnnotationsImplementation(IEdmEntityType entityType)
        {
            Debug.Assert(entityType != null, "entityType != null");

            // remove any existing serializable EPM annotations on the entity type
            entityType.ClearSerializableEpmAnnotations();

            // Get the cached EPM information for this entity type; if the mappings have changed this will update the cache;
            // if all the mappings have been removed, we will delete the cache.
            // Building the cache will also validate the EPM (since we don't want to save/write invalid EPM information).
            ODataEntityPropertyMappingCache epmCache = entityType.EnsureEpmCache();
            if (epmCache != null)
            {
                // write any mappings for properties that are not declared on this type on the entity type
                entityType.SaveEpmAnnotations(epmCache.MappingsForInheritedProperties, false, false);

                IEnumerable<IEdmProperty> declaredProperties = entityType.DeclaredProperties;
                if (declaredProperties != null)
                {
                    foreach (IEdmProperty property in declaredProperties)
                    {
                        if (property.Type.IsODataPrimitiveTypeKind() || property.Type.IsODataComplexTypeKind())
                        {
                            property.SaveEpmAnnotationsForProperty(epmCache);
                        }
                        else if (property.Type.IsODataMultiValueTypeKind())
                        {
                            property.SaveEpmAnnotationsForProperty(epmCache);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a boolean value for the <paramref name="annotationLocalName"/> OData metadata annotation on 
        /// the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The annotatable to get the annotation from.</param>
        /// <param name="annotationLocalName">The local name of the annotation to get.</param>
        /// <returns>true if the annotation exists and has the value 'true'; false if the annotation does not exist or has the value 'false'.</returns>
        /// <remarks>If the annotation exists but does not have a valid boolean value this method wil throw.</remarks>
        private static bool GetBooleanAnnotation(IEdmAnnotatable annotatable, string annotationLocalName)
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(annotationLocalName != null, "annotationLocalName != null");

            string stringValue = annotatable.GetODataAnnotation(annotationLocalName);

            // no annotation exists
            return stringValue != null && XmlConvert.ToBoolean(stringValue);
        }

        /// <summary>
        /// Sets the <paramref name="boolValue "/> as value of the <paramref name="annotationLocalName"/> annotation
        /// on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="annotatable">The annotatable to set the annotation on.</param>
        /// <param name="annotationLocalName">The local name of the annotation to set.</param>
        /// <param name="boolValue">The value of the annotation to set.</param>
        private static void SetBooleanAnnotation(IEdmAnnotatable annotatable, string annotationLocalName, bool boolValue)
        {
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(annotationLocalName != null, "annotationLocalName != null");

            // TODO: we should find a way to not require the model here! Talk to EdmLib!
            annotatable.SetODataAnnotation(annotationLocalName, boolValue ? EdmConstants.TrueLiteral : null);
        }

        /// <summary>
        /// Turns a <see cref="IEdmPrimitiveTypeReference"/> into the corresponding <see cref="Type"/>.
        /// </summary>
        /// <param name="primitiveTypeReference">The type reference to convert.</param>
        /// <returns>A <see cref="Type"/> for the <paramref name="primitiveTypeReference"/>.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Not overly complex.")]
        private static Type GetInstanceType(this IEdmPrimitiveTypeReference primitiveTypeReference)
        {
            ExceptionUtils.CheckArgumentNotNull(primitiveTypeReference, "primitiveTypeReference");

            bool isNullable = primitiveTypeReference.IsNullable;
            switch (primitiveTypeReference.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Binary: return typeof(byte[]);
                case EdmPrimitiveTypeKind.Boolean: return isNullable ? typeof(bool?) : typeof(bool);
                case EdmPrimitiveTypeKind.Byte: return isNullable ? typeof(byte?) : typeof(byte);
                case EdmPrimitiveTypeKind.DateTime: return isNullable ? typeof(DateTime?) : typeof(DateTime);
                case EdmPrimitiveTypeKind.Decimal: return isNullable ? typeof(Decimal?) : typeof(Decimal);
                case EdmPrimitiveTypeKind.Single: return isNullable ? typeof(Single?) : typeof(Single);
                case EdmPrimitiveTypeKind.Double: return isNullable ? typeof(Double?) : typeof(Double);
                case EdmPrimitiveTypeKind.Int16: return isNullable ? typeof(Int16?) : typeof(Int16);
                case EdmPrimitiveTypeKind.Int32: return isNullable ? typeof(Int32?) : typeof(Int32);
                case EdmPrimitiveTypeKind.Int64: return isNullable ? typeof(Int64?) : typeof(Int64);
                case EdmPrimitiveTypeKind.Guid: return isNullable ? typeof(Guid?) : typeof(Guid);
                case EdmPrimitiveTypeKind.SByte: return isNullable ? typeof(SByte?) : typeof(SByte);
                case EdmPrimitiveTypeKind.String: return typeof(string);
                case EdmPrimitiveTypeKind.Stream: return typeof(Stream);

                // The following types are all not yet supported in ODataLib
                case EdmPrimitiveTypeKind.DateTimeOffset:               // fall through
                case EdmPrimitiveTypeKind.Time:                         // fall through
                case EdmPrimitiveTypeKind.Polygon:                      // fall through
                case EdmPrimitiveTypeKind.GeometryCollection:               // fall through
                case EdmPrimitiveTypeKind.MultiPolygon:                 // fall through
                case EdmPrimitiveTypeKind.MultiLineString:              // fall through
                case EdmPrimitiveTypeKind.MultiPoint:                   // fall through
                case EdmPrimitiveTypeKind.Geometry:                     // fall through
                case EdmPrimitiveTypeKind.GeometricPoint:               // fall through
                case EdmPrimitiveTypeKind.GeometricLineString:          // fall through
                case EdmPrimitiveTypeKind.GeometricPolygon:             // fall through
                case EdmPrimitiveTypeKind.GeographyCollection:      // fall through
                case EdmPrimitiveTypeKind.GeometricMultiPolygon:        // fall through
                case EdmPrimitiveTypeKind.GeometricMultiLineString:     // fall through
                case EdmPrimitiveTypeKind.GeometricMultiPoint:
                    throw new ODataException(Strings.EdmLibraryExtensions_UnsupportedPrimitiveType(primitiveTypeReference.PrimitiveKind().ToString()));

                case EdmPrimitiveTypeKind.None:                         // fall through
                default:
                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.EdmLibraryExtensions_ToClrType));
            }
        }
    }
}
