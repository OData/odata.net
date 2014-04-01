//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.OData.Core.UriParser.TreeNodeKinds;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Library;
    using Microsoft.OData.Core.Metadata;
    using Microsoft.OData.Core.UriParser.Semantic;
    using ODataErrorStrings = Microsoft.OData.Core.Strings;

    #endregion Namespaces

    /// <summary>
    /// Helper methods for working with query nodes.
    /// </summary>
    internal static class QueryNodeUtils
    {
        /// <summary>
        /// Checks whether a query node is a collection query node representing a collection of entities.
        /// </summary>
        /// <param name="query">The <see cref="QueryNode"/> to check.</param>
        /// <returns>The converted <see cref="CollectionNode"/> or null if <paramref name="query"/> is not an entity collection node.</returns>
        internal static EntityCollectionNode AsEntityCollectionNode(this QueryNode query)
        {
            Debug.Assert(query != null, "query != null");

            EntityCollectionNode collectionNode = query as EntityCollectionNode;
            if (collectionNode != null &&
                collectionNode.ItemType != null &&
                collectionNode.ItemType.IsODataEntityTypeKind())
            {
                return collectionNode;
            }

            return null;
        }

        /// <summary>
        /// Checks whether a query node is a collection query node representing a collection.
        /// </summary>
        /// <param name="query">The <see cref="QueryNode"/> to check.</param>
        /// <returns>The converted <see cref="CollectionNode"/> or null if <paramref name="query"/> is not a collection node.</returns>
        internal static CollectionNode AsCollectionNode(this QueryNode query)
        {
            Debug.Assert(query != null, "query != null");

            CollectionNode collectionNode = query as CollectionNode;
            if (collectionNode != null &&
                collectionNode.ItemType != null)
            {
                return collectionNode;
            }

            return null;
        }

        /// <summary>
        /// Compute the result type of a binary operator based on the type of its operands and the operator kind.
        /// </summary>
        /// <param name="typeReference">The type reference of the operators.</param>
        /// <param name="operatorKind">The kind of operator.</param>
        /// <returns>The result type reference of the binary operator.</returns>
        internal static IEdmPrimitiveTypeReference GetBinaryOperatorResultType(IEdmPrimitiveTypeReference typeReference, BinaryOperatorKind operatorKind)
        {
            Debug.Assert(typeReference != null, "type != null");

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
                    return EdmCoreModel.Instance.GetBoolean(typeReference.IsNullable);

                case BinaryOperatorKind.Add:        // fall through
                case BinaryOperatorKind.Subtract:   // fall through
                case BinaryOperatorKind.Multiply:   // fall through
                case BinaryOperatorKind.Divide:     // fall through
                case BinaryOperatorKind.Modulo:
                    return typeReference;

                default:
                    throw new ODataException(ODataErrorStrings.General_InternalError(InternalErrorCodes.QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath));
            }
        }
    }
}
