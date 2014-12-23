//   OData .NET Libraries ver. 6.9
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

namespace Microsoft.OData.Core.UriParser.Parsers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Validation;

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
                    if (errors.Any(
                        e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName))
                    {
                        throw ExceptionUtil.CreateSyntaxError();
                    }
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
                    if (errors.Any(
                        e => e.ErrorCode == EdmErrorCode.InvalidPathFirstPathParameterNotMatchingFirstParameterName))
                    {
                        throw ExceptionUtil.CreateSyntaxError();
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get element type if possible
        /// </summary>
        /// <param name="type">The type, can be a collection or not.</param>
        /// <returns>Element type if <paramref name="type"/> is a collection type, <paramref name="type"/> itself if not.</returns>
        internal static IEdmType AsElementType(this IEdmType type)
        {
            IEdmCollectionType collectionType = type as IEdmCollectionType;
            return (collectionType != null) ? collectionType.ElementType.Definition : type;
        }
    }
}
