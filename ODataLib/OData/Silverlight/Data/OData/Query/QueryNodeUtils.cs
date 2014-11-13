//   OData .NET Libraries ver. 5.6.3
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

namespace Microsoft.Data.OData.Query
{
    #region Namespaces
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query.SemanticAst;
    using ODataErrorStrings = Microsoft.Data.OData.Strings;
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
            DebugUtils.CheckNoExternalCallers();
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
