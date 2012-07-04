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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Values;
    using Microsoft.Data.Edm.Values;
    using Microsoft.Data.OData.Atom;
    using Strings = Microsoft.Data.OData.Strings;  
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for dealing with OData metadata.
    /// </summary>
    internal static class MetadataUtils
    {
        /// <summary>
        /// Returns the annotation in the OData metadata namespace with the specified <paramref name="localName" />.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotation.</param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to get the annotation from.</param>
        /// <param name="localName">The local name of the annotation to find.</param>
        /// <param name="value">The value of the annotation in the OData metadata namespace and with the specified <paramref name="localName"/>.</param>
        /// <returns>true if an annotation with the specified local name was found; otherwise false.</returns>
        internal static bool TryGetODataAnnotation(this IEdmModel model, IEdmElement annotatable, string localName, out string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null"); 
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            object annotationValue = model.GetAnnotationValue(annotatable, AtomConstants.ODataMetadataNamespace, localName);
            if (annotationValue == null)
            {
                value = null;
                return false;
            }

            IEdmStringValue annotationStringValue = annotationValue as IEdmStringValue;
            if (annotationStringValue == null)
            {
                // invalid annotation type found
                throw new ODataException(Strings.ODataAtomWriterMetadataUtils_InvalidAnnotationValue(localName, annotationValue.GetType().FullName));
            }

            value = annotationStringValue.Value;
            return true;
        }

        /// <summary>
        /// Sets the annotation with the OData metadata namespace and the specified <paramref name="localName" /> on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations."/></param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to set the annotation on.</param>
        /// <param name="localName">The local name of the annotation to set.</param>
        /// <param name="value">The value of the annotation to set.</param>
        internal static void SetODataAnnotation(this IEdmModel model, IEdmElement annotatable, string localName, string value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null"); 
            Debug.Assert(annotatable != null, "annotatable != null");
            Debug.Assert(!string.IsNullOrEmpty(localName), "!string.IsNullOrEmpty(localName)");

            IEdmStringValue stringValue = null;
            if (value != null)
            {
                IEdmStringTypeReference typeReference = EdmCoreModel.Instance.GetString(/*nullable*/true);
                stringValue = new EdmStringConstant(typeReference, value);
            }

            model.SetAnnotationValue(annotatable, AtomConstants.ODataMetadataNamespace, localName, stringValue);
        }

        /// <summary>
        /// Gets all the serializable annotations in the OData metadata namespace on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations."/></param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to get the annotations from.</param>
        /// <returns>All annotations in the OData metadata namespace; or null if no annotations are found.</returns>
        internal static IEnumerable<IEdmDirectValueAnnotation> GetODataAnnotations(this IEdmModel model, IEdmElement annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(annotatable != null, "annotatable != null");
            
            IEnumerable<IEdmDirectValueAnnotation> annotations = model.DirectValueAnnotations(annotatable);
            if (annotations == null)
            {
                return null;
            }

            return annotations.Where(a => a.NamespaceUri == AtomConstants.ODataMetadataNamespace);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex, entity or collection type to the respective type. Uses the semantics used by writers.
        /// Thus it implements the strict speced behavior.
        /// </summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmType ResolveTypeNameForWrite(IEdmModel model, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            EdmTypeKind typeKind;

            // Writers should use the highest recognized version for type resolution since they need to verify
            // that the type being used is allowed in the given version. So pass the max here
            // so that we recognize all types and writers can fail later on if the type doesn't fit into the payload.
            return ResolveTypeName(model, /*expectedType*/ null, typeName, /*customTypeResolved*/ null, /*version*/ ODataConstants.MaxODataVersion, out typeKind);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex, entity or collection type to the respective type. Uses the semantics used be readers.
        /// Thus it can be a bit looser.
        /// </summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="expectedType">The expected type for the type name being resolved, or null if none is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <param name="readerBehavior">Reader behavior if the caller is a reader, null if no reader behavior is available.</param>
        /// <param name="version">The version of the payload being read.</param>
        /// <param name="typeKind">The type kind of the type, if it could be determined. This will be None if we couldn't tell. It might be filled
        /// even if the method returns null, for example for Collection types with item types which are not recognized.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmType ResolveTypeNameForRead(
            IEdmModel model,
            IEdmType expectedType,
            string typeName,
            ODataReaderBehavior readerBehavior,
            ODataVersion version,
            out EdmTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Func<IEdmType, string, IEdmType> customTypeResolver = readerBehavior == null ? null : readerBehavior.TypeResolver;
            Debug.Assert(
                customTypeResolver == null || readerBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesClient,
                "Custom type resolver can only be specified in WCF DS Client behavior.");

            return ResolveTypeName(model, expectedType, typeName, customTypeResolver, version, out typeKind);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex, entity or collection type to the respective type.
        /// </summary>
        /// <param name="model">The model to use or null if no model is available.</param>
        /// <param name="expectedType">The expected type for the type name being resolved, or null if none is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <param name="customTypeResolver">Custom type resolver to use, if null the model is used directly.</param>
        /// <param name="version">The version to use when resolving the type name.</param>
        /// <param name="typeKind">The type kind of the type, if it could be determined. This will be None if we couldn't tell. It might be filled
        /// even if the method returns null, for example for Collection types with item types which are not recognized.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        private static IEdmType ResolveTypeName(
            IEdmModel model,
            IEdmType expectedType,
            string typeName,
            Func<IEdmType, string, IEdmType> customTypeResolver,
            ODataVersion version,
            out EdmTypeKind typeKind)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(typeName != null, "typeName != null");
            IEdmType resolvedType = null;

            // Collection types should only be recognized in V3 and higher.
            string itemTypeName = version >= ODataVersion.V3 ? EdmLibraryExtensions.GetCollectionItemTypeName(typeName) : null;
            if (itemTypeName == null)
            {
                // Note: we require the type resolver or the model to also resolve
                //       primitive types.
                if (customTypeResolver != null && model.IsUserModel())
                {
                    resolvedType = customTypeResolver(expectedType, typeName);
                    if (resolvedType == null)
                    {
                        // If a type resolver is specified it must never return null.
                        throw new ODataException(Strings.MetadataUtils_ResolveTypeName(typeName));
                    }
                }
                else
                {
                    resolvedType = model.FindType(typeName);
                }

                // Spatial types are only recognized in V3 and higher.
                if (version < ODataVersion.V3 && resolvedType != null && resolvedType.IsSpatial())
                {
                    resolvedType = null;
                }

                typeKind = resolvedType == null ? EdmTypeKind.None : resolvedType.TypeKind;
            }
            else
            {
                // Collection
                typeKind = EdmTypeKind.Collection;
                EdmTypeKind itemTypeKind;

                IEdmType expectedItemType = null;
                if (customTypeResolver != null && expectedType != null && expectedType.TypeKind == EdmTypeKind.Collection)
                {
                    expectedItemType = ((IEdmCollectionType)expectedType).ElementType.Definition;
                }

                IEdmType itemType = ResolveTypeName(model, expectedItemType, itemTypeName, customTypeResolver, version, out itemTypeKind);
                if (itemType != null)
                {
                    resolvedType = EdmLibraryExtensions.GetCollectionType(itemType);
                }
            }

            return resolvedType;
        }
    }
}
