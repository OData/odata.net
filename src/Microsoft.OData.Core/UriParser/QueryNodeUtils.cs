//---------------------------------------------------------------------
// <copyright file="QueryNodeUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using ODataErrorStrings = Microsoft.OData.Strings;
    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query nodes.
    /// </summary>
    internal static class QueryNodeUtils
    {
        /// <summary>
        /// Pre-look map for BinaryOperator kind resolver
        /// </summary>
        private static Dictionary<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>, EdmPrimitiveTypeKind> additionalMap =
                new Dictionary<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>, EdmPrimitiveTypeKind>(EqualityComparer<Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>>.Default)
                {
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Add,
                            EdmPrimitiveTypeKind.DateTimeOffset,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.DateTimeOffset
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Add,
                            EdmPrimitiveTypeKind.Duration,
                            EdmPrimitiveTypeKind.DateTimeOffset),
                        EdmPrimitiveTypeKind.DateTimeOffset
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Add,
                            EdmPrimitiveTypeKind.Date,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.Date
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Add,
                            EdmPrimitiveTypeKind.Duration,
                            EdmPrimitiveTypeKind.Date),
                        EdmPrimitiveTypeKind.Date
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Add,
                            EdmPrimitiveTypeKind.Duration,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.Duration
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Subtract,
                            EdmPrimitiveTypeKind.DateTimeOffset,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.DateTimeOffset
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Subtract,
                            EdmPrimitiveTypeKind.DateTimeOffset,
                            EdmPrimitiveTypeKind.DateTimeOffset),
                        EdmPrimitiveTypeKind.Duration
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Subtract,
                            EdmPrimitiveTypeKind.Date,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.Date
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Subtract,
                            EdmPrimitiveTypeKind.Date,
                            EdmPrimitiveTypeKind.Date),
                        EdmPrimitiveTypeKind.Duration
                    },
                    {
                        new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(
                            BinaryOperatorKind.Subtract,
                            EdmPrimitiveTypeKind.Duration,
                            EdmPrimitiveTypeKind.Duration),
                        EdmPrimitiveTypeKind.Duration
                    },
                };

        /// <summary>
        /// Compute the result type of a binary operator based on the type of its operands and the operator kind.
        /// </summary>
        /// <param name="left">The type reference on the left hand.</param>
        /// <param name="right">The type reference on the right hand.</param>
        /// <param name="operatorKind">The kind of operator.</param>
        /// <returns>The result type reference of the binary operator.</returns>
        internal static IEdmPrimitiveTypeReference GetBinaryOperatorResultType(IEdmPrimitiveTypeReference left, IEdmPrimitiveTypeReference right, BinaryOperatorKind operatorKind)
        {
            Debug.Assert(left != null, "type != null");
            Debug.Assert(right != null, "type != null");

            EdmPrimitiveTypeKind kind;
            if (additionalMap.TryGetValue(new Tuple<BinaryOperatorKind, EdmPrimitiveTypeKind, EdmPrimitiveTypeKind>(operatorKind, left.PrimitiveKind(), right.PrimitiveKind()), out kind))
            {
                return EdmCoreModel.Instance.GetPrimitive(kind, left.IsNullable);
            }

            switch (operatorKind)
            {
                case BinaryOperatorKind.Or:                 // fall through
                case BinaryOperatorKind.And:                // fall through
                case BinaryOperatorKind.Equal:              // fall through
                case BinaryOperatorKind.NotEqual:           // fall through
                case BinaryOperatorKind.GreaterThan:        // fall through
                case BinaryOperatorKind.GreaterThanOrEqual: // fall through
                case BinaryOperatorKind.LessThan:           // fall through
                case BinaryOperatorKind.LessThanOrEqual:
                case BinaryOperatorKind.Has:
                    return EdmCoreModel.Instance.GetBoolean(left.IsNullable);

                case BinaryOperatorKind.Add:        // fall through
                case BinaryOperatorKind.Subtract:   // fall through
                case BinaryOperatorKind.Multiply:   // fall through
                case BinaryOperatorKind.Divide:     // fall through
                case BinaryOperatorKind.Modulo:
                    return left;

                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath));
            }
        }
    }
}