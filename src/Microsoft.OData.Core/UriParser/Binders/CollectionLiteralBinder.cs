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
        /// <returns>The bound CollectionLiteral token.</returns>
        public static QueryNode BindCollectionLiteralToken(CollectionLiteralToken collectionLiteralToken, Func<QueryToken, QueryNode> bindMethod)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionLiteralToken, nameof(collectionLiteralToken));
            ExceptionUtils.CheckArgumentNotNull(bindMethod, nameof(bindMethod));

            if (collectionLiteralToken.Items.Count == 0)
            {
                // for empty collection.
                return new CollectionConstantNode(collectionLiteralToken.ExpectedCollectionType)
                {
                    LiteralText = collectionLiteralToken.OriginalText
                };
            }

            IEdmTypeReference itemType = collectionLiteralToken.ExpectedCollectionType?.ElementType(); // expected item type for this collection, it could be null, untyped or real type.
            bool isNullOrUntyped = itemType == null || itemType.IsUntyped();

            bool hasLiteral = false;
            bool hasResourceLiteral = false;
            bool hasCollectionLiteral = false;
            CollectionConstantNode collectionConstants = null;
            CollectionRootPathNode collectionRootPathNode = null;
            foreach (var item in collectionLiteralToken.Items)
            {
                // Be noted, the collection literal could be collection of root path tokens. If it is the case, make sure all items are root path tokens.
                if (item is RootPathToken rootPathNode)
                {
                    RootPathNode rootNode = (RootPathNode)bindMethod(rootPathNode);
                    if (collectionConstants != null && collectionConstants.Items.Count > 0)
                    {
                        throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_MixedLiterals, "literal or resource literal or collection literal", "root path"));
                    }

                    if (collectionRootPathNode == null)
                    {
                        collectionRootPathNode = new CollectionRootPathNode(collectionLiteralToken.ExpectedCollectionType)
                        {
                            LiteralText = collectionLiteralToken.OriginalText
                        };
                    }

                    collectionRootPathNode.Collection.Add(rootNode);
                }
                else
                {
                    // If the collection literal is non-root path token collection, then its item could be primitive literal (literal token), resource literal and collection literal.
                    if (collectionRootPathNode != null && collectionRootPathNode.Count > 0)
                    {
                        throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_MixedLiterals, "root path", "literal or resource literal or collection literal"));
                    }

                    if (collectionConstants == null)
                    {
                        collectionConstants = new CollectionConstantNode(collectionLiteralToken.ExpectedCollectionType)
                        {
                            LiteralText = collectionLiteralToken.OriginalText
                        };
                    }

                    if (item is LiteralToken literalToken)
                    {
                        if ((hasResourceLiteral || hasCollectionLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_MixedLiterals, "literal", "resource literal or collection literal"));
                        }

                        literalToken.ExpectedType = isNullOrUntyped ? null : itemType;
                        QueryNode node = bindMethod(literalToken);

                        collectionConstants.Items.Add(node);
                        hasLiteral = true;
                    }
                    else if (item is ResourceLiteralToken resourceLiteralToken)
                    {
                        if ((hasLiteral || hasCollectionLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_MixedLiterals, "resource literal", "literal or collection literal"));
                        }

                        BindingHelpers.SetExpectedType(resourceLiteralToken, itemType);

                        QueryNode resourceConstant = bindMethod(resourceLiteralToken);
                        collectionConstants.Items.Add(resourceConstant);
                        hasResourceLiteral = true;
                    }
                    else if (item is CollectionLiteralToken nestedCollectionToken)
                    {
                        if ((hasLiteral || hasResourceLiteral) && !isNullOrUntyped)
                        {
                            throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_MixedLiterals, "collection", "literal or resource literal"));
                        }

                        BindingHelpers.SetExpectedType(nestedCollectionToken, itemType);

                        QueryNode collectionNode = bindMethod(nestedCollectionToken);
                        collectionConstants.Items.Add(collectionNode);
                        hasCollectionLiteral = true;
                    }
                    else
                    {
                        throw new ODataException(Error.Format(SRResources.CollectionLiteralBinder_InvalidTokenItem, item.Kind));
                    }
                }
            }

            return collectionConstants == null ? collectionRootPathNode : collectionConstants;
        }
    }
}
