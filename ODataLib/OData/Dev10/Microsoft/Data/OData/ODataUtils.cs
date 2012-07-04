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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
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
        /// <returns>The <see cref="ODataFormat"/> used for the specified <paramref name="payloadKind"/>.</returns>
        public static ODataFormat SetHeadersForPayload(ODataMessageWriter messageWriter, ODataPayloadKind payloadKind)
        {
            ExceptionUtils.CheckArgumentNotNull(messageWriter, "messageWriter");

            if (payloadKind == ODataPayloadKind.Unsupported)
            {
                throw new ArgumentException(Strings.ODataMessageWriter_CannotSetHeadersWithInvalidPayloadKind(payloadKind), "payloadKind");
            }

            return messageWriter.SetHeaders(payloadKind);
        }

        /// <summary>
        /// Returns the format used by the message reader for reading the payload.
        /// </summary>
        /// <param name="messageReader">The <see cref="ODataMessageReader"/> to get the read format from.</param>
        /// <returns>The format used by the <paramref name="messageReader"/> for reading the payload.</returns>
        /// <remarks>This method must only be called once reading has started. 
        /// This means that a read method has been called on the <paramref name="messageReader"/> or that a reader (for entries, feeds, collections, etc.) has been created.
        /// If the method is called prior to that it will throw.</remarks>
        public static ODataFormat GetReadFormat(ODataMessageReader messageReader)
        {
            ExceptionUtils.CheckArgumentNotNull(messageReader, "messageReader");
            return messageReader.GetFormat();
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to process.</param>
        public static void LoadODataAnnotations(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            LoadODataAnnotations(model, ODataConstants.DefaultMaxEntityPropertyMappingsPerType);
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to process.</param>
        /// <param name="maxEntityPropertyMappingsPerType">The maximum number of entity mapping attributes to be found 
        /// for an entity type (on the type itself and all its base types).</param>
        public static void LoadODataAnnotations(this IEdmModel model, int maxEntityPropertyMappingsPerType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            // NOTE: right now we only support annotations on entity types so we only iterate through them.
            foreach (IEdmEntityType entityType in model.EntityTypes())
            {
                LoadODataAnnotations(model, entityType, maxEntityPropertyMappingsPerType);
            }
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        public static void LoadODataAnnotations(this IEdmModel model, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            LoadODataAnnotations(model, entityType, ODataConstants.DefaultMaxEntityPropertyMappingsPerType);
        }

        /// <summary>
        /// Loads the supported, OData-specific serializable annotations into their in-memory representations.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        /// <param name="maxEntityPropertyMappingsPerType">The maximum number of entity mapping attributes to be found 
        /// for an entity type (on the type itself and all its base types).</param>
        public static void LoadODataAnnotations(this IEdmModel model, IEdmEntityType entityType, int maxEntityPropertyMappingsPerType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            // PERF: should we be smarter and only update existing annotations as needed (and only drop the caches if needed)?

            // remove any existing in-memory EPM annotations and EPM caches on the entity type
            model.ClearInMemoryEpmAnnotations(entityType);

            // NOTE: EnsureEpmCache will load the serializable EPM annotations if no in-memory annotations exist.
            model.EnsureEpmCache(entityType, maxEntityPropertyMappingsPerType);
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
                SaveODataAnnotationsImplementation(model, entityType);
            }
        }

        /// <summary>
        /// Turns the in-memory representations of the supported, OData-specific annotations into their serializable form.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        public static void SaveODataAnnotations(this IEdmModel model, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model"); 
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            SaveODataAnnotationsImplementation(model, entityType);
        }

        /// <summary>
        /// Checks whether the <paramref name="entityType"/> has a default stream.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to check.</param>
        /// <returns>true if the entity type has a default stream; otherwise false.</returns>
        public static bool HasDefaultStream(this IEdmModel model, IEdmEntityType entityType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model"); 
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");

            bool hasDefaultStream;
            if (TryGetBooleanAnnotation(model, entityType, EdmConstants.HasStreamAttributeName, /*recursive*/true, out hasDefaultStream))
            {
                return hasDefaultStream;
            }

            return false;
        }

        /// <summary>
        /// Adds or removes a default stream to/from the <paramref name="entityType"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to modify.</param>
        /// <param name="hasStream">true to add a default stream to the entity type; false to remove an existing default stream (if any).</param>
        public static void SetHasDefaultStream(this IEdmModel model, IEdmEntityType entityType, bool hasStream)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(entityType, "entityType");
            
            SetBooleanAnnotation(model, entityType, EdmConstants.HasStreamAttributeName, hasStream);
        }

        /// <summary>
        /// Checks whether the <paramref name="entityContainer"/> is the default entity container.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="entityContainer">The <see cref="IEdmEntityContainer"/> to check.</param>
        /// <returns>true if the <paramref name="entityContainer"/> is the default container; otherwise false.</returns>
        public static bool IsDefaultEntityContainer(this IEdmModel model, IEdmEntityContainer entityContainer)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model"); 
            ExceptionUtils.CheckArgumentNotNull(entityContainer, "entityContainer");

            bool isDefaultContainer;
            if (TryGetBooleanAnnotation(model, entityContainer, EdmConstants.IsDefaultEntityContainerAttributeName, out isDefaultContainer))
            {
                return isDefaultContainer;
            }

            return false;
        }

        /// <summary>
        /// Adds or removes a default stream to/from the <paramref name="entityContainer"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="entityContainer">The <see cref="IEdmEntityContainer"/> to modify.</param>
        /// <param name="isDefaultContainer">true to set the <paramref name="entityContainer"/> as the default container; false to remove an existing default container annotation (if any).</param>
        public static void SetIsDefaultEntityContainer(this IEdmModel model, IEdmEntityContainer entityContainer, bool isDefaultContainer)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(entityContainer, "entityContainer");
            
            SetBooleanAnnotation(model, entityContainer, EdmConstants.IsDefaultEntityContainerAttributeName, isDefaultContainer);
        }

        /// <summary>
        /// Checks whether the <paramref name="annotatable"/> has a MIME type annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to check.</param>
        /// <returns>The (non-null) value of the MIME type annotation of the <paramref name="annotatable"/> or null if no MIME type annotation exists.</returns>
        public static string GetMimeType(this IEdmModel model, IEdmElement annotatable)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            
            string mimeType;
            if (model.TryGetODataAnnotation(annotatable, EdmConstants.MimeTypeAttributeName, out mimeType))
            {
                if (mimeType == null)
                {
                    throw new ODataException(Strings.ODataUtils_NullValueForMimeTypeAnnotation);
                }

                return mimeType;
            }

            return null;
        }

        /// <summary>
        /// Sets the MIME type annotation of the <paramref name="annotatable"/> to <paramref name="mimeType"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to modify.</param>
        /// <param name="mimeType">The MIME type value to set as annotation value; if null, an existing annotation will be removed.</param>
        /// <remarks>The MIME type annotation is only supported on service operations and primitive properties for serialization purposes.</remarks>
        public static void SetMimeType(this IEdmModel model, IEdmElement annotatable, string mimeType)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            
            model.SetODataAnnotation(annotatable, EdmConstants.MimeTypeAttributeName, mimeType);
        }

        /// <summary>
        /// Checks whether the <paramref name="annotatable"/> has an HttpMethod annotation.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to check.</param>
        /// <returns>The (non-null) value of the HttpMethod annotation of the <paramref name="annotatable"/> or null if no such annotation exists.</returns>
        public static string GetHttpMethod(this IEdmModel model, IEdmElement annotatable)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            
            string httpMethod;
            if (model.TryGetODataAnnotation(annotatable, EdmConstants.HttpMethodAttributeName, out httpMethod))
            {
                if (httpMethod == null)
                {
                    throw new ODataException(Strings.ODataUtils_NullValueForHttpMethodAnnotation);
                }

                return httpMethod;
            }

            return null;
        }

        /// <summary>
        /// Sets the HttpMethod annotation of the <paramref name="annotatable"/> to <paramref name="httpMethod"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> contatining the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to modify.</param>
        /// <param name="httpMethod">The HttpMethod value to set as annotation value; if null, an existing annotation will be removed.</param>
        /// <remarks>The HttpMethod annotation is only supported on service operations for serialization purposes.</remarks>
        public static void SetHttpMethod(this IEdmModel model, IEdmElement annotatable, string httpMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(annotatable, "annotatable");
            
            model.SetODataAnnotation(annotatable, EdmConstants.HttpMethodAttributeName, httpMethod);
        }

        /// <summary>
        /// Gets the value of IsAlwaysBindable annotation on the <paramref name="functionImport"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="functionImport">The <see cref="IEdmFunctionImport"/> to get the annotation from.</param>
        /// <returns>The value of the annotation if it exists; false otherwise.</returns>
        /// <exception cref="ODataException">Thrown if the IsAlwaysBindable annotation is set to true for a non-bindable <paramref name="functionImport"/>.</exception>
        public static bool IsAlwaysBindable(this IEdmModel model, IEdmFunctionImport functionImport)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(functionImport, "functionImport");
            
            bool isAlwaysBindable;

            if (TryGetBooleanAnnotation(model, functionImport, EdmConstants.IsAlwaysBindableAttributeName, out isAlwaysBindable))
            {
                if (!functionImport.IsBindable && isAlwaysBindable)
                {
                    throw new ODataException(Strings.ODataUtils_UnexpectedIsAlwaysBindableAnnotationInANonBindableFunctionImport);
                }

                return isAlwaysBindable;
            }

            return false;
        }

        /// <summary>
        /// Sets the value of IsAlwaysBindable annotation of the <paramref name="functionImport"/> to <paramref name="isAlwaysBindable"/>
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="functionImport">The <see cref="IEdmFunctionImport"/> to set the annotation on.</param>
        /// <param name="isAlwaysBindable">The value of the annotation to set.</param>
        /// <exception cref="ODataException">Thrown if IsAlwaysBindable is set to true for a non-bindable <paramref name="functionImport"/>.</exception>
        public static void SetIsAlwaysBindable(this IEdmModel model, IEdmFunctionImport functionImport, bool isAlwaysBindable)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(functionImport, "functionImport");
            
            if (!functionImport.IsBindable && isAlwaysBindable)
            {
                throw new ODataException(Strings.ODataUtils_IsAlwaysBindableAnnotationSetForANonBindableFunctionImport);
            }

            SetBooleanAnnotation(model, functionImport, EdmConstants.IsAlwaysBindableAttributeName, isAlwaysBindable);
        }

        /// <summary>
        /// Gets the reader behavior for null property value on the specified property.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>The behavior to use when reading null value for this property.</returns>
        public static ODataNullValueBehaviorKind NullValueReadBehaviorKind(this IEdmModel model, IEdmProperty property)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            
            ODataEdmPropertyAnnotation annotation = model.GetAnnotationValue<ODataEdmPropertyAnnotation>(property);
            return annotation == null ? ODataNullValueBehaviorKind.Default : annotation.NullValueReadBehaviorKind;
        }

        /// <summary>
        /// Adds a transient annotation to indicate how null values for the specified property should be read.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="property">The <see cref="IEdmProperty"/> to modify.</param>
        /// <param name="nullValueReadBehaviorKind">The new behavior for reading null values for this property.</param>
        public static void SetNullValueReaderBehavior(this IEdmModel model, IEdmProperty property, ODataNullValueBehaviorKind nullValueReadBehaviorKind)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            
            ODataEdmPropertyAnnotation annotation = model.GetAnnotationValue<ODataEdmPropertyAnnotation>(property);
            if (annotation == null)
            {
                if (nullValueReadBehaviorKind != ODataNullValueBehaviorKind.Default)
                {
                    annotation = new ODataEdmPropertyAnnotation
                    {
                        NullValueReadBehaviorKind = nullValueReadBehaviorKind
                    };
                    model.SetAnnotationValue(property, annotation);
                }
            }
            else
            {
                annotation.NullValueReadBehaviorKind = nullValueReadBehaviorKind;
            }
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

            // removes the ";" and the user agent string from the version.
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
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations.</param>
        /// <param name="entityType">The <see cref="IEdmEntityType"/> to process.</param>
        private static void SaveODataAnnotationsImplementation(IEdmModel model, IEdmEntityType entityType)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(entityType != null, "entityType != null");
            
            // Get the cached EPM information for this entity type; if the mappings have changed this will update the cache;
            // if all the mappings have been removed, we will delete the cache.
            // Building the cache will also validate the EPM (since we don't want to save/write invalid EPM information).
            // NOTE: when saving annotations on a model we assume it is valid and thus pass int.MaxValue for the maxMappingCount.
            ODataEntityPropertyMappingCache epmCache = model.EnsureEpmCache(entityType, /*maxMappingCount*/ int.MaxValue);

            if (epmCache != null)
            {
                // write any mappings for properties that are not declared on this type on the entity type
                model.SaveEpmAnnotations(entityType, epmCache.MappingsForInheritedProperties, false, false);

                IEnumerable<IEdmProperty> declaredProperties = entityType.DeclaredProperties;
                if (declaredProperties != null)
                {
                    foreach (IEdmProperty property in declaredProperties)
                    {
                        if (property.Type.IsODataPrimitiveTypeKind() || property.Type.IsODataComplexTypeKind())
                        {
                            model.SaveEpmAnnotationsForProperty(property, epmCache);
                        }
                        else if (property.Type.IsNonEntityODataCollectionTypeKind())
                        {
                            model.SaveEpmAnnotationsForProperty(property, epmCache);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a boolean value for the <paramref name="annotationLocalName"/> OData metadata annotation on 
        /// the <paramref name="structuredType"/>.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="structuredType">The annotatable to get the annotation from.</param>
        /// <param name="annotationLocalName">The local name of the annotation to get.</param>
        /// <param name="recursive">true to search the base type hierarchy of the structured type for the annotation; otherwise false.</param>
        /// <param name="boolValue">true if the annotation exists and has the value 'true'; false if the annotation does not exist or has the value 'false'.</param>
        /// <returns>true if the annotation with the specified local names exists; otherwise false.</returns>
        /// <remarks>If the annotation exists but does not have a valid boolean value this method will throw.</remarks>
        private static bool TryGetBooleanAnnotation(
            IEdmModel model, 
            IEdmStructuredType structuredType,
            string annotationLocalName, 
            bool recursive, 
            out bool boolValue)
        {
            Debug.Assert(model != null, "model != null"); 
            Debug.Assert(structuredType != null, "structuredType != null");
            Debug.Assert(annotationLocalName != null, "annotationLocalName != null");

            string stringValue = null;
            bool found;
            do
            {
                found = model.TryGetODataAnnotation(structuredType, annotationLocalName, out stringValue);
                if (found)
                {
                    break;
                }

                // no annotation on this type; check the base type (if enabled)
                structuredType = structuredType.BaseType;
            }
            while (recursive && structuredType != null);

            // no annotation exists
            if (!found)
            {
                boolValue = false;
                return false;
            }

            boolValue = XmlConvert.ToBoolean(stringValue);
            return true;
        }

        /// <summary>
        /// Gets a boolean value for the <paramref name="annotationLocalName"/> OData metadata annotation on 
        /// the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="annotatable">The annotatable to get the annotation from.</param>
        /// <param name="annotationLocalName">The local name of the annotation to get.</param>
        /// <param name="boolValue">true if the annotation exists and has the value 'true'; false if the annotation does not exist or has the value 'false'.</param>
        /// <returns>true if the annotation with the specified local names exists; otherwise false.</returns>
        /// <remarks>If the annotation exists but does not have a valid boolean value this method wil throw.</remarks>
        private static bool TryGetBooleanAnnotation(IEdmModel model, IEdmElement annotatable, string annotationLocalName, out bool boolValue)
        {
            Debug.Assert(model != null, "model != null"); 
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(annotationLocalName != null, "annotationLocalName != null");

            string stringValue;
            if (model.TryGetODataAnnotation(annotatable, annotationLocalName, out stringValue))
            {
                boolValue = XmlConvert.ToBoolean(stringValue);
                return true;
            }

            // no annotation exists
            boolValue = false;
            return false;
        }

        /// <summary>
        /// Sets the <paramref name="boolValue "/> as value of the <paramref name="annotationLocalName"/> annotation
        /// on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The model containing the annotation.</param>
        /// <param name="annotatable">The annotatable to set the annotation on.</param>
        /// <param name="annotationLocalName">The local name of the annotation to set.</param>
        /// <param name="boolValue">The value of the annotation to set.</param>
        private static void SetBooleanAnnotation(IEdmModel model, IEdmElement annotatable, string annotationLocalName, bool boolValue)
        {
            Debug.Assert(model != null, "model != null"); 
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(annotationLocalName != null, "annotationLocalName != null");

            model.SetODataAnnotation(annotatable, annotationLocalName, boolValue ? EdmConstants.TrueLiteral : null);
        }
    }
}
