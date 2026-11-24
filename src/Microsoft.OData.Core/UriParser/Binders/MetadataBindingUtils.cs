//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData;
    using Microsoft.OData.Metadata;
    using ODataErrorStrings = Microsoft.OData.Strings;

    /// <summary>
    /// Helper methods for metadata binding.
    /// </summary>
    internal static class MetadataBindingUtils
    {
        /// <summary>
        /// If the source node is not of the specified type, then we check if type promotion is possible and inject a convert node.
        /// If the source node is the same type as the target type (or if the target type is null), we just return the source node as is.
        /// </summary>
        /// <param name="source">The source node to apply the conversion to.</param>
        /// <param name="targetTypeReference">The target primitive type. May be null - this method will do nothing in that case.</param>
        /// <param name="enableCaseInsensitive">Whether enum member matching ignores case.</param>
        /// <returns>The converted query node, or the original source node unchanged.</returns>
        internal static SingleValueNode ConvertToTypeIfNeeded(SingleValueNode source, IEdmTypeReference targetTypeReference, bool enableCaseInsensitive = false)
        {
            Debug.Assert(source != null, "source != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            if (source.TypeReference != null)
            {
                if (source.TypeReference.IsEquivalentTo(targetTypeReference))
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

                ConstantNode constantNode = source as ConstantNode;
                // Check if the source node is a constant node, not null, and the source type is either string or integral
                // and the target type is an enum.
                if (constantNode != null && constantNode.Value != null && (source.TypeReference.IsString() || source.TypeReference.IsIntegral()) && targetTypeReference.IsEnum())
                {
                    string memberName = constantNode.Value.ToString();
                    IEdmEnumType enumType = targetTypeReference.Definition as IEdmEnumType;
                    StringComparison comparison = enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                    if (long.TryParse(memberName, out long integralValue))
                    {
                        string name = enumType.TryParse(integralValue, out IEdmEnumMember member)
                            ? member.Name : enumType.IsFlags ? enumType.ParseFlagsFromIntegralValue(integralValue) : null;
                        if (name != null)
                        {
                            return new ConstantNode(new ODataEnumValue(name, enumType.FullName()),
                                ODataUriUtils.ConvertToUriLiteral(name, ODataVersion.V4), targetTypeReference);
                        }
                    }

                    var namedMember = enumType.FindMember(memberName, comparison);
                    string flagsName = namedMember != null ? namedMember.Name :
                        enumType.IsFlags ? enumType.ParseFlagsFromStringValue(memberName, comparison) : null;
                    if (flagsName != null)
                    {
                        return new ConstantNode(new ODataEnumValue(flagsName, enumType.FullName()),
                            ODataUriUtils.ConvertToUriLiteral(flagsName, ODataVersion.V4), targetTypeReference);
                    }

                    throw new ODataException(ODataErrorStrings.Binder_IsNotValidEnumConstant(memberName));
                }

                if (!TypePromotionUtils.CanConvertTo(source, source.TypeReference, targetTypeReference))
                {
                    throw new ODataException(ODataErrorStrings.MetadataBinder_CannotConvertToType(source.TypeReference.FullName(), targetTypeReference.FullName()));
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

                        if (string.IsNullOrEmpty(constantNode.LiteralText))
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

        internal static void VerifyCollectionNode(CollectionNode node, bool enableCaseInsensitive)
        {
            var collection = node as CollectionConstantNode;
            if (collection == null || !collection.ItemType.IsEnum())
            {
                return;
            }

            var enumType = (IEdmEnumType)collection.ItemType.Definition;
            var comparison = enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var item in collection.Collection)
            {
                var value = item.Value as ODataEnumValue;
                if (value == null || enumType.ContainsMember(value.Value, comparison))
                {
                    continue;
                }

                if (long.TryParse(value.Value, out long number) &&
                    (enumType.TryParse(number, out IEdmEnumMember member) || enumType.IsValidFlagsEnumValue(number)))
                {
                    continue;
                }

                if (enumType.IsFlags && enumType.ParseFlagsFromStringValue(value.Value, comparison) != null)
                {
                    continue;
                }

                throw new ODataException(ODataErrorStrings.Binder_IsNotValidEnumConstant(value.Value));
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
    }
}