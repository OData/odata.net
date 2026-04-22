//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using Microsoft.OData;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using static System.Net.Mime.MediaTypeNames;

    /// <summary>
    /// Helper methods for metadata binding.
    /// </summary>
    internal static class MetadataBindingUtils
    {
        internal static SingleValueNode ConvertConstantNodeIfNeeded(ConstantNode source, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(source != null, "constantNode != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            // We don't have type information, espically for 'null' literal case
            // But, we cannot say any 'null' literal without a type. Especially, for type prefied 'null' literal.
            if (source.TypeReference == null)
            {
                if (source.Value == null)
                {
                    if (targetTypeReference.IsNullable)
                    {
                        return new ConvertNode(source, targetTypeReference);
                    }

                    throw new ODataException($"Cannot 'null' to non-null type of {targetTypeReference.FullName()}");
                }

                // For safety, create a convert node for non-null value constant node without a type. It looks weird.
                return new ConvertNode(source, targetTypeReference);
            }

            // Any literal value can convert to Edm.Untyped
            if (targetTypeReference.IsUntyped())
            {
                return new ConvertNode(source, targetTypeReference);
            }

            // Why using the 'Definition'? Because, we should not care about the nullablity since the ConstantNode is from literal, we don't know about the nullablity of the literal
            if (source.TypeReference.Definition.IsEquivalentTo(targetTypeReference.Definition))
            {
                if (targetTypeReference.IsTypeDefinition())
                {
                    return new ConvertNode(source, targetTypeReference);
                }

                return source;
            }

            IEdmTypeReference sourceTypeReference = source.TypeReference;
            if (source.TypeReference.IsNullable != targetTypeReference.IsNullable)
            {
                sourceTypeReference = source.TypeReference.Definition.ToTypeReference(targetTypeReference.IsNullable);
            }

            if (!TypePromotionUtils.CanConvertTo(source, sourceTypeReference, targetTypeReference))
            {
                throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName()));
            }

            return new ConvertNode(source, targetTypeReference);
        }

#if false
        internal static SingleValueNode ConvertResourceConstantNodeIfNeeded(ResourceConstantNode source, IEdmTypeReference targetTypeReference, BindingState bindingState)
        {
            Debug.Assert(source != null, "source != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            // Any source value can convert to Edm.Untyped
            if (targetTypeReference.IsUntyped())
            {
                return new ConvertNode(source, targetTypeReference);
            }

            if (!targetTypeReference.IsStructured())
            {
                throw new UriLiteralParsingException(Error.Format(SRResources.UriPrimitiveTypeParsers_FailedToParseJsonObjectToNonStrucutredType, source.LiteralText, targetTypeReference.FullName()));
            }

            IEdmStructuredTypeReference targetStructuredTypeReference = targetTypeReference.AsStructured();
            if (source.ExpectedStructuredType != null)
            {
                if (source.ExpectedStructuredType.StructuredDefinition().IsEquivalentTo(targetStructuredTypeReference.StructuredDefinition()))
                {
                    return source;
                }

                if (targetStructuredTypeReference.IsAssignableFrom(source.ExpectedStructuredType))
                {
                    return new ConvertNode(source, targetTypeReference);
                }

                throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName()));
            }

            ResourceConstantNode newNode = new ResourceConstantNode(targetStructuredTypeReference, source.LiteralText);
            foreach (var property in source.Properties)
            {
                IEdmTypeReference propertyType = GetPropertyType(targetStructuredTypeReference, property.Key, bindingState); // This could be null.
                if (propertyType == null)
                {
                    newNode.Add(property.Key, property.Value);
                    continue;
                }

                if (property.Value is ConstantNode constantNode)
                {
                    newNode.Add(property.Key, ConvertConstantNodeIfNeeded(constantNode, propertyType));
                }
                else if (property.Value is ResourceConstantNode subResourceConstantNode)
                {
                    newNode.Add(property.Key, ConvertResourceConstantNodeIfNeeded(subResourceConstantNode, propertyType));
                }
                else if (property.Value is CollectionConstantNode subCollectionNode)
                {
                    newNode.Add(property.Key, ConvertCollectionConstantNodeIfNeeded(subCollectionNode, propertyType, bindingState));
                }
                else
                {
                    throw new ODataException(Error.Format(SRResources.MetadataBinder_UnsupportedQueryNodeForConstant, property.Value.ToString(), property.Value.Kind, "Resource Constant"));
                }
            }

            return newNode;
        }

        internal static CollectionConstantNode ConvertCollectionConstantNodeIfNeeded(CollectionConstantNode source, IEdmTypeReference targetTypeReference, BindingState bindingState)
        {
            Debug.Assert(source != null, "source != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            // Any Collection Constant value can convert to Edm.Untyped?? No, Be noted, the target type should be collection of Edm.Untyped.
            //if (targetTypeReference.IsUntyped())
            //{
            //    return new ConvertNode(source, targetTypeReference);
            //}

            if (!targetTypeReference.IsCollection())
            {
                throw new UriLiteralParsingException(Error.Format(SRResources.UriPrimitiveTypeParsers_FailedToParseJsonArrayToNonCollectionType, source.LiteralText, targetTypeReference.FullName()));
            }

            IEdmCollectionTypeReference targetCollectionTypeReference = targetTypeReference.AsCollection();
            IEdmTypeReference targetElementTypeReference = targetCollectionTypeReference.ElementType();


            if (source.CollectionType != null)
            {
                if (source.ItemType.Definition.IsEquivalentTo(targetElementTypeReference.Definition))
                {
                    return source;
                }

                if (targetStructuredTypeReference.IsAssignableFrom(source.ExpectedStructuredType))
                {
                    return new ConvertNode(source, targetTypeReference);
                }

                throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName()));
            }

            CollectionConstantNode newNode = new CollectionConstantNode(targetCollectionTypeReference, source.LiteralText);
            foreach (var item in source.Items)
            {
                if (item is ConstantNode constantNode)
                {
                    newNode.Add(ConvertConstantNodeIfNeeded(constantNode, targetElementTypeReference));
                }
                else if (item is ResourceConstantNode nestedResourceConstant)
                {
                    newNode.Add(ConvertConstantNodeIfNeeded(nestedResourceConstant, targetElementTypeReference, bindingState));
                }
                else if (item is CollectionConstantNode nestedCollectionNode)
                {
                    newNode.Add(ConvertCollectionConstantNodeIfNeeded(nestedCollectionNode, targetElementTypeReference, bindingState));
                }
                else
                {
                    // Is there a special logic for the 'ConvertNode' if the source node is *Constant*Node?
                    throw new ODataException(Error.Format(SRResources.MetadataBinder_UnsupportedQueryNodeForConstant, item.ToString(), item.Kind, "Collection"));
                }
            }

            return newNode;
        }

#endif
        private static IEdmTypeReference GetPropertyType(IEdmStructuredTypeReference structuredTypeRef, string propertyName, BindingState bindingState)
        {
            IEdmStructuredType structuredType = structuredTypeRef.StructuredDefinition();

            IEdmProperty edmProperty = bindingState.Configuration.Resolver.ResolveProperty(structuredType, propertyName);
            if (edmProperty != null)
            {
                return edmProperty.Type;
            }

            return null;
        }

        /// <summary>
        /// If the source node is not of the specified type, then we check if type promotion is possible and inject a convert node.
        /// If the source node is the same type as the target type (or if the target type is null), we just return the source node as is.
        /// </summary>
        /// <param name="source">The source node to apply the conversion to.</param>
        /// <param name="targetTypeReference">The target primitive type. May be null - this method will do nothing in that case.</param>
        /// <returns>The converted query node, or the original source node unchanged.</returns>
        internal static SingleValueNode ConvertToTypeIfNeeded(SingleValueNode source, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(source != null, "source != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            if (source.TypeReference != null)
            {
                ConstantNode constantNode = source as ConstantNode;
                if (constantNode != null && constantNode.Value == null && !targetTypeReference.IsNullable)
                {
                    throw new ODataException($"Can not covert 'null' literal to a non-nullable type {targetTypeReference.FullName()}");
                }

                // don't care about the 'nullability'
                if (source.TypeReference.Definition.IsEquivalentTo(targetTypeReference.Definition))
                {
                    // For source is type definition, if source's underlying type == target type.
                    // We create a conversion node from source to its underlying type (target type)
                    // so that the service can convert value of source clr type to underlying clr type.
                    if (source.TypeReference.IsTypeDefinition())
                    {
                        return new ConvertNode(source, targetTypeReference);
                    }

                    return source;
                }

                // Structured type in url will be translated into a node with raw string value.
                // We create a conversion node from string to structured type.
                if (targetTypeReference.IsStructured() || targetTypeReference.IsStructuredCollectionType())
                {
                    return new ConvertNode(source, targetTypeReference);
                }

               
                // Check if the source node is a constant node, not null, and the source type is either string or integral
                // and the target type is an enum.
                if (constantNode != null && constantNode.Value != null && (source.TypeReference.IsString() || source.TypeReference.IsIntegral()) && targetTypeReference.IsEnum())
                {
                    string memberName = constantNode.Value.ToString();
                    IEdmEnumType enumType = targetTypeReference.Definition as IEdmEnumType;
                    if (enumType.ContainsMember(memberName, StringComparison.Ordinal))
                    {
                        string literalText = ODataUriUtils.ConvertToUriLiteral(constantNode.Value, default(ODataVersion));
                        return new ConstantNode(new ODataEnumValue(memberName, enumType.ToString()), literalText, targetTypeReference);
                    }

                    // If the member name is an integral value, we should try to convert it to the enum member name and find the enum member with the matching integral value
                    if (long.TryParse(memberName, out long memberIntegralValue) && enumType.TryParse(memberIntegralValue, out IEdmEnumMember enumMember))
                    {
                        string literalText = ODataUriUtils.ConvertToUriLiteral(enumMember.Name, default(ODataVersion));
                        return new ConstantNode(new ODataEnumValue(enumMember.Name, enumType.ToString()), literalText, targetTypeReference);
                    }

                    throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, memberName));
                }

                if (!TypePromotionUtils.CanConvertTo(source, source.TypeReference, targetTypeReference))
                {
                    throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.TypeReference.FullName(), targetTypeReference.FullName()));
                }
                else
                {
                    if (source.TypeReference.IsEnum() && constantNode != null)
                    {
                        return new ConstantNode(constantNode.Value, ODataUriUtils.ConvertToUriLiteral(constantNode.Value, ODataVersion.V4), targetTypeReference);
                    }

                    object originalPrimitiveValue;
                    if (MetadataUtilsCommon.TryGetConstantNodePrimitiveLDMF(source, out originalPrimitiveValue) && (originalPrimitiveValue != null))
                    {
                        // L F D M types : directly create a ConvertNode.
                        // 1. NodeToExpressionTranslator.cs won't allow implicitly converting single/double to decimal, which should be done here at Node tree level.
                        // 2. And prevent losing precision in float -> double, e.g. (double)1.234f => 1.2339999675750732d not 1.234d
                        object targetPrimitiveValue = ODataUriConversionUtils.CoerceNumericType(originalPrimitiveValue, targetTypeReference.AsPrimitive().Definition as IEdmPrimitiveType);

                        if (constantNode.LiteralText.IsEmpty)
                        {
                            return new ConstantNode(targetPrimitiveValue);
                        }

                        var candidate = new ConstantNode(targetPrimitiveValue, constantNode.LiteralText);
                        var decimalType = candidate.TypeReference as IEdmDecimalTypeReference;
                        if (decimalType != null)
                        {
                            var targetDecimalType = (IEdmDecimalTypeReference)targetTypeReference;
                            return decimalType.Precision == targetDecimalType.Precision &&
                                   decimalType.Scale == targetDecimalType.Scale ?
                                   (SingleValueNode)candidate :
                                   (SingleValueNode)(new ConvertNode(candidate, targetTypeReference));
                        }
                        else
                        {
                            return candidate;
                        }
                    }
                    else
                    {
                        // other type conversion : ConvertNode
                        return new ConvertNode(source, targetTypeReference);
                    }
                }
            }
            else
            {
                // If the source doesn't have a type (possibly an open property), then it's possible to convert it
                // cause we don't know for sure.
                return new ConvertNode(source, targetTypeReference);
            }
        }

        /// <summary>
        /// Retrieves type associated to a segment.
        /// </summary>
        /// <param name="segment">The node to retrieve the type from.</param>
        /// <returns>The type of the node, or item type for collections.</returns>
        internal static IEdmType GetEdmType(this QueryNode segment)
        {
            SingleValueNode singleNode = segment as SingleValueNode;

            if (singleNode != null)
            {
                IEdmTypeReference typeRef = singleNode.TypeReference;
                return (typeRef != null) ? typeRef.Definition : null;
            }

            CollectionNode collectionNode = segment as CollectionNode;

            if (collectionNode != null)
            {
                IEdmTypeReference typeRef = collectionNode.ItemType;
                return (typeRef != null) ? typeRef.Definition : null;
            }

            return null;
        }

        /// <summary>
        /// Retrieves the type reference associated to a segment.
        /// </summary>
        /// <param name="segment">The node to retrieve the type reference from.</param>
        /// <returns>The Type reference of the node (item type reference for collections).</returns>
        internal static IEdmTypeReference GetEdmTypeReference(this QueryNode segment)
        {
            SingleValueNode singleNode = segment as SingleValueNode;

            if (singleNode != null)
            {
                return singleNode.TypeReference;
            }

            CollectionNode collectionNode = segment as CollectionNode;

            if (collectionNode != null)
            {
                return collectionNode.ItemType;
            }

            return null;
        }

        internal static void VerifyCollectionNode(CollectionNode node, bool enableCaseInsensitive = false)
        {
            if (node == null ||
                !(node is CollectionConstantNode collectionConstantNode) ||
                !collectionConstantNode.ItemType.IsEnum()
                )
            {
                return;
            }

            IEdmEnumType enumType = collectionConstantNode.ItemType.Definition as IEdmEnumType;

            StringComparison comparison = enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (ConstantNode item in collectionConstantNode.Collection)
            {
                if (item != null && item.Value != null && item.Value is ODataEnumValue enumValue)
                {
                    if (!enumType.ContainsMember(enumValue.Value, comparison))
                    {
                        throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, enumValue.Value));
                    }
                }
            }
        }
    }
}