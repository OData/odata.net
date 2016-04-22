//---------------------------------------------------------------------
// <copyright file="PathParserModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CORE
namespace Microsoft.OData.Core.Query
#else
namespace Microsoft.OData.Service
#endif
{
    using System.Collections.Generic;
#if !ODATA_CORE
    using Microsoft.OData.Service.Providers;
#endif
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
#if ODATA_CORE
    using ErrorStrings = Microsoft.OData.Core.Strings;
    using ExceptionUtil = ODataUriParserException;
    using VersionUtil = TemporaryVersionUtil;
    using WebUtil = TemporaryWebUtil;
#else
    using Microsoft.OData.Core;
    using ErrorStrings = Microsoft.OData.Service.Strings;
    using ExceptionUtil = DataServiceException;
#endif

    /// <summary>
    /// Set of helpers and extensions to make it easier to convert the semantic path parser
    /// to using <see cref="IEdmType"/> and the related classes.
    /// </summary>
    internal static class PathParserModelUtils
    {
        /// <summary>
        /// Returns whether the given type is a structural type that is open.
        /// </summary>
        /// <param name="edmType">The type to check.</param>
        /// <returns>Whether the type is both structural and open.</returns>
        internal static bool IsOpenType(this IEdmType edmType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(edmType != null, "edmType != null");
            
            var structuredType = edmType as IEdmStructuredType;
            if (structuredType != null)
            {
                ThrowIfOpenComplexType(edmType);
                return structuredType.IsOpen;
            }

            // If its a collection, return whether its element type is open. 
            // This is because when processing a navigation property, the target type
            // may be a collection type even though a key expression has been applied.
            // This will be cleaned up in a subsequent change.
            // TODO: when SingleResult is removed from the semantic path parser, change this to return false.
            var collectionType = edmType as IEdmCollectionType;
            if (collectionType == null)
            {
                return false;
            }

            return collectionType.ElementType.Definition.IsOpenType();
        }

        /// <summary>
        /// Returns whether or not the type is an entity or entity collection type.
        /// </summary>
        /// <param name="edmType">The type to check.</param>
        /// <returns>Whether or not the type is an entity or entity collection type.</returns>
        internal static bool IsEntityOrEntityCollectionType(this IEdmType edmType)
        {
            DebugUtils.CheckNoExternalCallers();
            IEdmEntityType entityType;
            return edmType.IsEntityOrEntityCollectionType(out entityType);
        }

        /// <summary>
        /// Returns whether or not the type is an entity or entity collection type.
        /// </summary>
        /// <param name="edmType">The type to check.</param>
        /// <param name="entityType">The entity type. If the given type was a collection, this will be the element type.</param>
        /// <returns>Whether or not the type is an entity or entity collection type.</returns>
        internal static bool IsEntityOrEntityCollectionType(this IEdmType edmType, out IEdmEntityType entityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(edmType != null, "targetEdmType != null");
            if (edmType.TypeKind == EdmTypeKind.Entity)
            {
                entityType = (IEdmEntityType)edmType;
                return true;
            }

            if (edmType.TypeKind != EdmTypeKind.Collection)
            {
                entityType = null;
                return false;
            }

            IEdmEntityTypeReference entityReference = ((IEdmCollectionType)edmType).ElementType as IEdmEntityTypeReference;
            if (entityReference == null)
            {
                entityType = null;
                return false;
            }

            entityType = entityReference.EntityDefinition();
            return true;
        }

        /// <summary>
        /// Gets the target entity set for the given function import.
        /// </summary>
        /// <param name="functionImport">The function import.</param>
        /// <param name="sourceEntitySet">The source entity set.</param>
        /// <param name="model">The model.</param>
        /// <returns>The target entity set of the function import or null if it could not be determined.</returns>
        internal static IEdmEntitySet GetTargetEntitySet(this IEdmFunctionImport functionImport, IEdmEntitySet sourceEntitySet, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            IEdmEntitySet targetEntitySet;
            if (functionImport.TryGetStaticEntitySet(out targetEntitySet))
            {
                return targetEntitySet;
            }

            if (sourceEntitySet == null)
            {
                return null;
            }

            if (functionImport.IsBindable && functionImport.Parameters.Any())
            {
                IEdmFunctionParameter parameter;
                IEnumerable<IEdmNavigationProperty> path;
                if (functionImport.TryGetRelativeEntitySetPath(model, out parameter, out path))
                {
                    // TODO: throw better exception
                    WebUtil.CheckSyntaxValid(parameter == functionImport.Parameters.First());
                    targetEntitySet = sourceEntitySet;
                    foreach (var navigation in path)
                    {
                        targetEntitySet = targetEntitySet.FindNavigationTarget(navigation);
                        if (targetEntitySet == null)
                        {
                            return null;
                        }
                    }

                    return targetEntitySet;
                }
            }

            return null;
        }

        /// <summary>
        /// Throws an exception if the given type is an open complex type.
        /// </summary>
        /// <param name="edmType">The type to check.</param>
        private static void ThrowIfOpenComplexType(IEdmType edmType)
        {
            Debug.Assert(edmType != null, "edmType != null");
            if (edmType.TypeKind == EdmTypeKind.Complex)
            {
                var complexType = ((IEdmComplexType)edmType);
                if (complexType.IsOpen)
                {
                    throw new InvalidOperationException(ErrorStrings.ResourceType_ComplexTypeCannotBeOpen(complexType.FullName()));
                }
            }
        }
    }
}
