//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.Metadata
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Annotations;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Edm.Library.Values;
    using Microsoft.OData.Edm.Values;
    using Microsoft.OData.Core.Atom;

    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for dealing with OData metadata.
    /// </summary>
    internal static class MetadataUtils
    {
        /// <summary>
        /// Gets all the serializable annotations in the OData metadata namespace on the <paramref name="annotatable"/>.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> containing the annotations."/></param>
        /// <param name="annotatable">The <see cref="IEdmElement"/> to get the annotations from.</param>
        /// <returns>All annotations in the OData metadata namespace; or null if no annotations are found.</returns>
        internal static IEnumerable<IEdmDirectValueAnnotation> GetODataAnnotations(this IEdmModel model, IEdmElement annotatable)
        {
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
        /// Gets the EDM type of an OData instance from the <see cref="ODataTypeAnnotation"/> of the instance (if available).
        /// </summary>
        /// <param name="annotatable">The OData instance to get the EDM type for.</param>
        /// <returns>The EDM type of the <paramref name="annotatable"/> if available in the <see cref="ODataTypeAnnotation"/> annotation.</returns>
        internal static IEdmTypeReference GetEdmType(this ODataAnnotatable annotatable)
        {
            if (annotatable == null)
            {
                return null;
            }

            ODataTypeAnnotation typeAnnotation = annotatable.GetAnnotation<ODataTypeAnnotation>();
            return typeAnnotation == null ? null : typeAnnotation.Type;
        }

        /// <summary>
        /// Resolves the name of a primitive, complex, entity or collection type to the respective type. Uses the semantics used by writers.
        /// Thus it implements the strict speced behavior.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        internal static IEdmType ResolveTypeNameForWrite(IEdmModel model, string typeName)
        {
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
        /// <param name="model">The model to use.</param>
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
            Func<IEdmType, string, IEdmType> customTypeResolver = readerBehavior == null ? null : readerBehavior.TypeResolver;
            Debug.Assert(
                customTypeResolver == null || readerBehavior.ApiBehaviorKind == ODataBehaviorKind.WcfDataServicesClient,
                "Custom type resolver can only be specified in WCF DS Client behavior.");

            return ResolveTypeName(model, expectedType, typeName, customTypeResolver, version, out typeKind);
        }

        /// <summary>
        /// Resolves the name of a primitive, complex, entity or collection type to the respective type.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="expectedType">The expected type for the type name being resolved, or null if none is available.</param>
        /// <param name="typeName">The name of the type to resolve.</param>
        /// <param name="customTypeResolver">Custom type resolver to use, if null the model is used directly.</param>
        /// <param name="version">The version to use when resolving the type name.</param>
        /// <param name="typeKind">The type kind of the type, if it could be determined. This will be None if we couldn't tell. It might be filled
        /// even if the method returns null, for example for Collection types with item types which are not recognized.</param>
        /// <returns>The <see cref="IEdmType"/> representing the type specified by the <paramref name="typeName"/>;
        /// or null if no such type could be found.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0003:MethodCallNotAllowed", Justification = "IEdmModel.FindType is allowed here and all other places should call this method to get to the type.")]
        internal static IEdmType ResolveTypeName(
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
            string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(typeName);
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

        /// <summary>
        /// Calculates the operations that are bindable to the given type.
        /// </summary>
        /// <param name="bindingType">The binding type in question.</param>
        /// <param name="model">The model to search for operations.</param>
        /// <param name="edmTypeResolver">The edm type resolver to get the parameter type.</param>
        /// <returns>An enumeration of operations that are always bindable to the given type.</returns>
        [SuppressMessage("DataWeb.Usage", "AC0014:DoNotHandleProhibitedExceptionsRule", Justification = "ExceptionUtils.IsCatchableExceptionType is being used correctly")] 
        internal static IEdmOperation[] CalculateBindableOperationsForType(IEdmType bindingType, IEdmModel model, EdmTypeResolver edmTypeResolver)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(edmTypeResolver != null, "edmTypeResolver != null");

            List<IEdmOperation> operations = null;
            try
            {
                operations = model.FindBoundOperations(bindingType).ToList();
            }
            catch (Exception exc)
            {
                if (!ExceptionUtils.IsCatchableExceptionType(exc))
                {
                    throw;
                }

                throw new ODataException(Strings.MetadataUtils_CalculateBindableOperationsForType(bindingType.FullTypeName()), exc);
            }

            List<IEdmOperation> operationsFound = new List<IEdmOperation>();
            foreach (IEdmOperation operation in operations.EnsureOperationsBoundWithBindingParameter())
            {
                IEdmOperationParameter bindingParameter = operation.Parameters.FirstOrDefault();
                IEdmType resolvedBindingType = edmTypeResolver.GetParameterType(bindingParameter).Definition;
                if (resolvedBindingType.IsAssignableFrom(bindingType))
                {
                    operationsFound.Add(operation);
                }
            }

            return operationsFound.ToArray();
        }

        /// <summary>
        /// Looks up the given term name in the given model, and returns the term's type if a matching term was found.
        /// </summary>
        /// <param name="qualifiedTermName">The name of the term to lookup, including the namespace.</param>
        /// <param name="model">The model to look in.</param>
        /// <returns>The type of the term in the model, or null if no matching term was found.</returns>
        internal static IEdmTypeReference LookupTypeOfValueTerm(string qualifiedTermName, IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            IEdmTypeReference typeFromModel = null;
            IEdmValueTerm termFromModel = model.FindValueTerm(qualifiedTermName);
            if (termFromModel != null)
            {
                typeFromModel = termFromModel.Type;
            }

            return typeFromModel;
        }

        /// <summary>
        /// Gets the nullable type reference for a payload type; if the payload type is null, uses Edm.String.
        /// </summary>
        /// <param name="payloadType">The payload type to get the type reference for.</param>
        /// <returns>The nullable <see cref="IEdmTypeReference"/> for the <paramref name="payloadType"/>.</returns>
        internal static IEdmTypeReference GetNullablePayloadTypeReference(IEdmType payloadType)
        {
            return payloadType == null ? null : payloadType.ToTypeReference(true);
        }
    }
}
