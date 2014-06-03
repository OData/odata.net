//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;
    using ErrorStrings = Microsoft.OData.Core.Strings;

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
            Debug.Assert(edmType != null, "edmType != null");
            
            var structuredType = edmType as IEdmStructuredType;
            if (structuredType != null)
            {
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

            entityType = ((IEdmCollectionType)edmType).ElementType.Definition as IEdmEntityType;
            return entityType != null;
        }

        /// <summary>
        /// Gets the target entity set for the given operation import.
        /// </summary>
        /// <param name="operationImport">The operation import.</param>
        /// <param name="sourceEntitySet">The source entity set.</param>
        /// <param name="model">The model.</param>
        /// <returns>The target entity set of the operation import or null if it could not be determined.</returns>
        internal static IEdmEntitySetBase GetTargetEntitySet(this IEdmOperationImport operationImport, IEdmEntitySetBase sourceEntitySet, IEdmModel model)
        {
            IEdmEntitySet targetEntitySet;
            if (operationImport.TryGetStaticEntitySet(out targetEntitySet))
            {
                return targetEntitySet;
            }

            if (sourceEntitySet == null)
            {
                return null;
            }

            if (operationImport.Operation.IsBound && operationImport.Operation.Parameters.Any())
            {
                IEdmOperationParameter parameter;
                IEnumerable<IEdmNavigationProperty> path;
                IEnumerable<EdmError> errors;

                if (operationImport.TryGetRelativeEntitySetPath(model, out parameter, out path, out errors))
                {
                    IEdmEntitySetBase currentEntitySet = sourceEntitySet;
                    foreach (var navigation in path)
                    {
                        currentEntitySet = currentEntitySet.FindNavigationTarget(navigation) as IEdmEntitySetBase;
                        if (currentEntitySet == null || currentEntitySet is IEdmUnknownEntitySet)
                        {
                            return currentEntitySet;
                        }
                    }

                    return currentEntitySet;
                }
                else
                {
                    bool foundSyntaxError = !errors.Any(e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName);
                    ExceptionUtil.ThrowSyntaxErrorIfNotValid(foundSyntaxError);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the target entity set for the given operation import.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="source">The source of operation.</param>
        /// <param name="model">The model.</param>
        /// <returns>The target entity set of the operation import or null if it could not be determined.</returns>
        internal static IEdmEntitySetBase GetTargetEntitySet(this IEdmOperation operation, IEdmNavigationSource source, IEdmModel model)
        {
            if (source == null)
            {
                return null;
            }

            if (operation.IsBound && operation.Parameters.Any())
            {
                IEdmOperationParameter parameter;
                IEnumerable<IEdmNavigationProperty> path;
                IEdmEntityType lastEntityType;
                IEnumerable<EdmError> errors;

                if (operation.TryGetRelativeEntitySetPath(model, out parameter, out path, out lastEntityType, out errors))
                {
                    IEdmNavigationSource target = source;
                    foreach (var navigation in path)
                    {
                        target = target.FindNavigationTarget(navigation);
                        if (target == null)
                        {
                            return null;
                        }
                    }

                    return target as IEdmEntitySetBase;
                }
                else
                {
                    bool foundSyntaxError = errors.Any(e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName);
                    ExceptionUtil.ThrowSyntaxErrorIfNotValid(!foundSyntaxError);
                }
            }

            return null;
        }
    }
}
