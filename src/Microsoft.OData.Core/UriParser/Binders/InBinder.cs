//---------------------------------------------------------------------
// <copyright file="InBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Class that knows how to bind the In operator.
    /// </summary>
    internal sealed class InBinder
    {
        /// <summary>
        /// Method to use for binding the parent node, if needed.
        /// </summary>
        private readonly Func<QueryToken, QueryNode> bindMethod;

        /// <summary>
        /// Resolver for parsing
        /// </summary>
        private readonly ODataUriResolver resolver;

        /// <summary>
        /// Constructs a InBinder with the given method to be used binding the parent token if needed.
        /// </summary>
        /// <param name="bindMethod">Method to use for binding the parent token, if needed.</param>
        /// <param name="resolver">Resolver for parsing.</param>
        internal InBinder(Func<QueryToken, QueryNode> bindMethod, ODataUriResolver resolver)
        {
            this.bindMethod = bindMethod;
            this.resolver = resolver;
        }

        /// <summary>
        /// Binds an In operator token.
        /// </summary>
        /// <param name="inToken">The In operator token to bind.</param>
        /// <returns>The bound In operator token.</returns>
        internal QueryNode BindInOperator(InToken inToken)
        {
            ExceptionUtils.CheckArgumentNotNull(inToken, "inToken");

            SingleValueNode left = this.GetSingleValueOperandFromToken(inToken.Left);
            CollectionNode right = null;
            if (left.TypeReference != null)
            {
                right = this.GetCollectionOperandFromToken(
                    inToken.Right, new EdmCollectionTypeReference(new EdmCollectionType(left.TypeReference)));
            }
            else 
            {
                right = this.GetCollectionOperandFromToken(
                    inToken.Right, new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetUntyped())));
            }

            // If the left operand is either an integral or a string type and the right operand is a collection of enums,
            // Calls the MetadataBindingUtils.ConvertToTypeIfNeeded() method to convert the left operand to the same enum type as the right operand.
            if ((!(right is CollectionConstantNode) && right.ItemType.IsEnum()) && (left.TypeReference != null && (left.TypeReference.IsString() || left.TypeReference.IsIntegral())))
            {
                left = MetadataBindingUtils.ConvertToTypeIfNeeded(left, right.ItemType);
            }

            MetadataBindingUtils.VerifyCollectionNode(right, this.resolver.EnableCaseInsensitive);

            return new InNode(left, right);
        }

        /// <summary>
        /// Retrieve SingleValueNode bound with given query token.
        /// </summary>
        /// <param name="queryToken">The query token</param>
        /// <returns>The corresponding SingleValueNode</returns>
        private SingleValueNode GetSingleValueOperandFromToken(QueryToken queryToken)
        {
            SingleValueNode operand = this.bindMethod(queryToken) as SingleValueNode;
            if (operand == null)
            {
                throw new ODataException(SRResources.MetadataBinder_LeftOperandNotSingleValue);
            }

            return operand;
        }

        /// <summary>
        /// Retrieve CollectionNode bound with given query token.
        /// </summary>
        /// <param name="queryToken">The query token</param>
        /// <param name="expectedType">The expected type that this collection holds</param>
        /// <param name="model">The Edm model</param>
        /// <returns>The corresponding CollectionNode</returns>
        private CollectionNode GetCollectionOperandFromToken(QueryToken queryToken, IEdmCollectionTypeReference expectedType)
        {
            Debug.Assert(queryToken != null);
            Debug.Assert(expectedType != null);

            if (queryToken is CollectionLiteralToken collectionToken)
            {
                collectionToken.ExpectedCollectionType = expectedType;
            }

            var node = this.bindMethod(queryToken);

            CollectionNode operand = null;
            if (node is SingleValueOpenPropertyAccessNode openNode)
            {
                operand = new CollectionOpenPropertyAccessNode(openNode.Source, openNode.Name, expectedType);
            }
            else
            {
                operand = node as CollectionNode;
            }
            
            if (operand == null)
            {
                throw new ODataException(SRResources.MetadataBinder_RightOperandNotCollectionValue);
            }

            return operand;
        }
    }
}
