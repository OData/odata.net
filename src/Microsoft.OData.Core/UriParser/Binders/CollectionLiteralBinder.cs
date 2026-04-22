//---------------------------------------------------------------------
// <copyright file="CollectionLiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Provides functionality to bind collection literal tokens into query nodes during OData URI parsing.
    /// A collection literal represents a collection of values in the URI, typically expressed as a JSON-like array.
    /// </summary>
    internal struct CollectionLiteralBinder
    {
        /// <summary>
        /// Binds a CollectionLiteral token.
        /// </summary>
        /// <param name="collectionLiteralToken">The CollectionLiteral token to bind.</param>
        /// <param name="bindMethod">The method to use to bind the items in the collection.</param>
        /// <param name="bindingState">The state of the binding.</param>
        /// <returns>The bound CollectionLiteral token.</returns>
        public static QueryNode BindCollectionLiteralToken(CollectionLiteralToken collectionLiteralToken, Func<QueryToken, QueryNode> bindMethod, BindingState bindingState)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionLiteralToken, nameof(collectionLiteralToken));
            ExceptionUtils.CheckArgumentNotNull(bindMethod, nameof(bindMethod));
            ExceptionUtils.CheckArgumentNotNull(bindingState, nameof(bindingState));

            if (collectionLiteralToken.Count == 0)
            {
                // for empty collection.
                return new CollectionConstantNode(collectionLiteralToken.CollectionType, collectionLiteralToken.OriginalText);
            }

            IEdmTypeReference itemType = collectionLiteralToken.CollectionType?.ElementType(); // expected item type for this collection, it could be null, untyped or real type.
            bool isNullOrUntyped = itemType == null || itemType.IsUntyped();

            bool hasLiteral = false;
            bool hasResourceLiteral = false;
            bool hasCollectionLiteral = false;
            CollectionConstantNode collectionConstants = null;
            CollectionRootPathNode collectionRootPathNode = null;
            foreach (var item in collectionLiteralToken)
            {
                // Be noted, the collection literal could be collection of root path tokens. If it is the case, make sure all items are root path tokens.
                if (item is RootPathToken rootPathNode)
                {
                    RootPathNode rootNode = (RootPathNode)bindMethod(rootPathNode);
                    if (collectionConstants != null && collectionConstants.Count > 0)
                    {
                        throw new ODataException("Mix the rootExp with other constant within the collection is invalid.");
                    }

                    if (collectionRootPathNode == null)
                    {
                        collectionRootPathNode = new CollectionRootPathNode(collectionLiteralToken.CollectionType, collectionLiteralToken.OriginalText);
                    }

                    collectionRootPathNode.Add(rootNode);
                }
                else
                {
                    // If the collection literal is non-root path token collection, then its item could be primitive literal (literal token), resource literal and collection literal.
                    if (collectionRootPathNode != null && collectionRootPathNode.Count > 0)
                    {
                        throw new ODataException("Mix the rootExp with other constant within the collection is invalid.");
                    }

                    if (collectionConstants == null)
                    {
                        collectionConstants = new CollectionConstantNode(collectionLiteralToken.CollectionType, collectionLiteralToken.OriginalText);
                    }

                    if (item is LiteralToken literalToken)
                    {
                        if ((hasResourceLiteral || hasCollectionLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException("Mix the literal with resource literal or collection literal within the collection is invalid.");
                        }

                        literalToken.ExpectedEdmTypeReference = itemType;
                        QueryNode node = bindMethod(literalToken);

                        collectionConstants.AddItem(node);
                        hasLiteral = true;
                    }
                    else if (item is ResourceLiteralToken resourceLiteralToken)
                    {
                        if ((hasLiteral || hasCollectionLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException("Mix the literal with resource literal or collection literal within the collection is invalid.");
                        }

                        if (!isNullOrUntyped)
                        {
                            if (!itemType.IsStructured())
                            {
                                throw new ODataException("The item type of a collection of collection must be either untyped or collection type.");
                            }

                            resourceLiteralToken.ExpectedType = itemType.AsStructured();
                        }

                        QueryNode resourceConstant = bindMethod(resourceLiteralToken);
                        collectionConstants.AddItem(resourceConstant);
                        hasResourceLiteral = true;
                    }
                    else if (item is CollectionLiteralToken nestedCollectionToken)
                    {
                        if ((hasLiteral || hasResourceLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException("Mix the literal with resource literal or collection literal within the collection is invalid.");
                        }

                        if (!isNullOrUntyped)
                        {
                            if (!itemType.IsCollection())
                            {
                                throw new ODataException("The item type of a collection of collection must be either untyped or collection type.");
                            }

                            nestedCollectionToken.CollectionType = itemType.AsCollection();
                        }

                        QueryNode collectionNode = bindMethod(nestedCollectionToken);
                        collectionConstants.AddItem(collectionNode);
                        hasCollectionLiteral = true;
                    }
                    else
                    {
                        // Is there a special logic for the 'ConvertNode' if the source node is *Constant*Node?
                        throw new ODataException(Error.Format(SRResources.MetadataBinder_UnsupportedQueryNodeForConstant, item.ToString(), item.Kind, "Collection"));
                    }
                }
            }

            return collectionConstants == null ? collectionRootPathNode : collectionConstants;
        }
    }
}
