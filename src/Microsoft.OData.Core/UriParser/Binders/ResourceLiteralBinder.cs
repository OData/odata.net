//---------------------------------------------------------------------
// <copyright file="ResourceLiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Provides functionality to bind resource literal tokens into query nodes during OData URI parsing.
    /// A resource literal represents a structured object (entity or complex type) in the URI, typically
    /// expressed as a JSON-like collection of property name-value pairs.
    /// </summary>
    /// <remarks>
    /// This binder handles type resolution, property binding, and nested resource/collection literals.
    /// It supports both typed and untyped properties, and performs compatibility checks between expected
    /// types and types specified in the literal (e.g., via @odata.type or @type annotations).
    /// </remarks>
    internal sealed class ResourceLiteralBinder
    {
        /// <summary>
        /// Binds a ResourceLiteral token, a resource literal token is a collection of key value pairs.
        /// </summary>
        /// <param name="resourceLiteralToken">The ResourceLiteral token to bind.</param>
        /// <param name="bindMethod">The method to use for binding nested resource literals.</param>
        /// <param name="bindingState">The current state of the binding process, including model and settings.</param>
        /// <returns>The bound ResourceLiteral token.</returns>
        public static QueryNode BindResourceLiteralToken(ResourceLiteralToken resourceLiteralToken, Func<QueryToken, QueryNode> bindMethod, BindingState bindingState)
        {
            ExceptionUtils.CheckArgumentNotNull(resourceLiteralToken, nameof(resourceLiteralToken));
            ExceptionUtils.CheckArgumentNotNull(bindMethod, nameof(bindMethod));
            ExceptionUtils.CheckArgumentNotNull(bindingState, nameof(bindingState));

            IEdmStructuredType typeFromLiteral = ResolveActualType(resourceLiteralToken, bindingState);

            if (resourceLiteralToken.ExpectedType == null)
            {
                // If the expected type is not set, then we should use the type from the literal as the expected type if the type from the literal is not null.
                // If the type from the literal is null, then we let the rest of the binding process figure it out from the literal value.
                return BindResourceLiteral(resourceLiteralToken, typeFromLiteral, bindMethod, bindingState);
            }
            else if (typeFromLiteral == null)
            {
                // If the expected type is set, meanwhile the type from the literal is not set, then we should use the expected type as the type to bind the resource literal.
                return BindResourceLiteral(resourceLiteralToken, resourceLiteralToken.ExpectedType.StructuredDefinition(), bindMethod, bindingState);
            }
            else
            {
                IEdmStructuredType edmStructuredType = resourceLiteralToken.ExpectedType.StructuredDefinition();

                // If both the expected type and the type from the literal are set, test whether they are the same type first.
                // We don't care about the nullability of the expected type here, since the nullability of the expected type doesn't really matter for the binding of the resource literal.
                if (edmStructuredType.IsEquivalentTo(typeFromLiteral))
                {
                    return BindResourceLiteral(resourceLiteralToken, typeFromLiteral, bindMethod, bindingState);
                }

                // Second, they should be compatible. That is, the expected type should be a base type of the type from the literal. Otherwise, we should throw an exception.
                if (!edmStructuredType.IsAssignableFrom(typeFromLiteral))
                {
                    throw new ODataException("SRResources.ResourceLiteralBinder_TypeFromLiteralNotCompatibleWithExpectedType(typeFromLiteral.FullName(), resourceLiteralToken.ExpectedType.FullName())");
                }

                // Finally, we should use the type from the literal as the type to bind the resource literal, since it's more specific.
                ResourceConstantNode node = BindResourceLiteral(resourceLiteralToken, typeFromLiteral, bindMethod, bindingState);

                // Last, we should create a convert node to refresh the inheritance.
                return new ConvertNode(node, resourceLiteralToken.ExpectedType);
            }
        }

        /// <summary>
        /// Binds a ResourceLiteral Token, a resource literal token is a collection of key value pairs.
        /// </summary>
        /// <param name="resourceLiteralToken">The resource literal token to bind.</param>
        /// <param name="structuredType">The structured type to bind the resource literal token to. This can be null if the type cannot be determined from the literal token.</param>
        /// <param name="bindMethod">The method to use for binding nested resource literals.</param>
        /// <param name="bindingState">The current state of the binding process, including model and settings.</param>
        /// <returns>The bound resource literal token.</returns>
        private static ResourceConstantNode BindResourceLiteral(ResourceLiteralToken resourceLiteralToken, IEdmStructuredType structuredType, Func<QueryToken, QueryNode> bindMethod, BindingState bindingState)
        {
            Debug.Assert(resourceLiteralToken != null, "resourceLiteralToken != null");
            Debug.Assert(bindMethod != null, "bindMethod != null");
            Debug.Assert(bindingState != null, "bindingState != null");

            ResourceConstantNode singleResourceNode = new ResourceConstantNode(structuredType, resourceLiteralToken.OriginalText);

            foreach (var item in resourceLiteralToken)
            {
                IEdmTypeReference propertyType = ResolvePropertyType(structuredType, item.Key, bindingState); // This could be null.
                bool isNullOrUntyped = propertyType == null || propertyType.IsUntyped();

                if (item.Value is LiteralToken literalToken)
                {
                    literalToken.ExpectedEdmTypeReference = propertyType;
                    QueryNode node = bindMethod(literalToken);
                    singleResourceNode.AddProperty(item.Key, node);
                }
                else if (item.Value is ResourceLiteralToken subResourceLiteralToken)
                {
                    // If the property type is null or untyped, we don't know the type of the sub resource literal, so we will try to resolve it based on the properties inside the sub resource literal.
                    // It's valid for an untyped property to have a resource literal, in which case we will just treat it as an untyped resource literal.
                    if (!isNullOrUntyped && !propertyType.IsStructured())
                    {
                        throw new ODataException("SRResources.ResourceLiteralBinder_NonStructuredProperty(propertyType.FullName())");
                    }

                    subResourceLiteralToken.ExpectedType = propertyType?.AsStructured();

                    QueryNode resourceConstant = bindMethod(subResourceLiteralToken);
                    singleResourceNode.AddProperty(item.Key, resourceConstant);
                }
                else if (item.Value is CollectionLiteralToken subCollectionToken)
                {
                    // It's valid for an untyped property to have a collection literal value.
                    // If the property is declared untyped, we don't know the type of the sub resource literal, so we will try to resolve it based on the properties inside the sub resource literal.
                    if (!isNullOrUntyped && !propertyType.IsCollection())
                    {
                        throw new ODataException("SRResources.ResourceLiteralBinder_NonStructuredProperty(propertyType.FullName())");
                    }

                    subCollectionToken.CollectionType = propertyType?.AsCollection();
                    QueryNode collectionNode = bindMethod(subCollectionToken);
                    singleResourceNode.AddProperty(item.Key, collectionNode);
                }
                else
                {
                    throw new ODataException(Error.Format(SRResources.MetadataBinder_UnsupportedQueryNodeForConstant, item.Value.ToString(), item.Value.Kind, "Collection"));
                }
            }

            return singleResourceNode;
        }

        /// <summary>
        /// Resolves the actual structured type from the resource literal token's type name annotation.
        /// </summary>
        /// <param name="resourceLiteralToken">The resource literal token containing the type name annotation (e.g., @odata.type or @type).</param>
        /// <param name="bindingState">The binding state.</param>
        /// <returns>
        /// The resolved <see cref="IEdmStructuredType"/> if the type name is specified and can be resolved;
        /// otherwise, null if no type name is specified in the literal.
        /// </returns>
        private static IEdmStructuredType ResolveActualType(ResourceLiteralToken resourceLiteralToken, BindingState bindingState)
        {
            if (string.IsNullOrEmpty(resourceLiteralToken.TypeNameFromLiteral))
            {
                return null;
            }

            IEdmModel model = bindingState.Model;

            IEdmStructuredType actualType = bindingState.Configuration.Resolver.ResolveType(model, resourceLiteralToken.TypeNameFromLiteral) as IEdmStructuredType;
            if (actualType == null)
            {
                // or create a untyped structured type: actualType = new EdmUntypedStructuredTypeReference()
                throw new ODataException("Cannot resolve the type using type name .... in a JSON object using \"@odata.type\" or \"@type\".");
            }

            return actualType;
        }

        /// <summary>
        /// Resolves the type reference for a property within a structured type.
        /// </summary>
        /// <param name="structuredType">The structured type containing the property to resolve.</param>
        /// <param name="propertyName">The name of the property to resolve.</param>
        /// <param name="bindingState">The binding state containing the model and configuration for the binding process.</param>
        /// <returns>
        private static IEdmTypeReference ResolvePropertyType(IEdmStructuredType structuredType, string propertyName, BindingState bindingState)
        {
            if (structuredType == null || structuredType.IsUntyped())
            {
                return null;
            }

            IEdmProperty edmProperty = bindingState.Configuration.Resolver.ResolveProperty(structuredType, propertyName);
            if (edmProperty != null)
            {
                return edmProperty.Type;
            }

            return null;
        }
    }
}