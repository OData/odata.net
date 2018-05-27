//---------------------------------------------------------------------
// <copyright file="PathParserModelUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Metadata;

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Set of helpers and extensions to make it easier to convert the semantic path parser
    /// to using <see cref="IEdmType"/> and the related classes.
    /// </summary>
    internal static class PathParserModelUtils
    {
        /// <summary>
        /// Gets the target entity set for the given operation import.
        /// </summary>
        /// <param name="operationImport">The operation import.</param>
        /// <param name="sourceEntitySet">The source entity set.</param>
        /// <param name="model">The model.</param>
        /// <returns>The target entity set of the operation import or null if it could not be determined.</returns>
        internal static IEdmEntitySetBase GetTargetEntitySet(this IEdmOperationImport operationImport, IEdmEntitySetBase sourceEntitySet, IEdmModel model)
        {
            IEdmEntitySetBase targetEntitySet;
            if (operationImport.TryGetStaticEntitySet(model, out targetEntitySet))
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
                Dictionary<IEdmNavigationProperty, IEdmPathExpression> path;
                IEnumerable<EdmError> errors;

                if (operationImport.TryGetRelativeEntitySetPath(model, out parameter, out path, out errors))
                {
                    IEdmEntitySetBase currentEntitySet = sourceEntitySet;

                    foreach (var navigation in path)
                    {
                        currentEntitySet = currentEntitySet.FindNavigationTarget(navigation.Key, navigation.Value) as IEdmEntitySetBase;
                        if (currentEntitySet is IEdmUnknownEntitySet)
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
                Dictionary<IEdmNavigationProperty, IEdmPathExpression> path;
                IEdmEntityType lastEntityType;
                IEnumerable<EdmError> errors;

                if (operation.TryGetRelativeEntitySetPath(model, out parameter, out path, out lastEntityType, out errors))
                {
                    IEdmNavigationSource target = source;

                    foreach (var navigation in path)
                    {
                        target = target.FindNavigationTarget(navigation.Key, navigation.Value);
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

        /// <summary>Determines a matching target kind from the specified type.</summary>
        /// <param name="type">ResourceType of element to get kind for.</param>
        /// <returns>An appropriate <see cref="RequestTargetKind"/> for the specified <paramref name="type"/>.</returns>
        internal static RequestTargetKind GetTargetKindFromType(this IEdmType type)
        {
            Debug.Assert(type != null, "type != null");

            switch (type.TypeKind)
            {
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                    return RequestTargetKind.Resource;
                case EdmTypeKind.Collection:
                    if (type.IsStructuredCollectionType())
                    {
                        return RequestTargetKind.Resource;
                    }

                    return RequestTargetKind.Collection;
                case EdmTypeKind.Enum:
                    return RequestTargetKind.Enum;
                case EdmTypeKind.TypeDefinition:
                    return RequestTargetKind.Primitive;
                default:
                    Debug.Assert(type.TypeKind == EdmTypeKind.Primitive, "typeKind == ResourceTypeKind.Primitive");
                    return RequestTargetKind.Primitive;
            }
        }
    }
}
