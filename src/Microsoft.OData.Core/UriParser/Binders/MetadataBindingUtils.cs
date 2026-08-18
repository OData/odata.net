//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.OData;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

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
        /// <param name="enableCaseInsensitive">Whether to enable case insensitive comparison for enum member names.</param>
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
                    // String comparison for enum member names is case-sensitive by default.
                    StringComparison comparison = enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

                    string memberName = constantNode.Value.ToString();
                    IEdmEnumType enumType = targetTypeReference.Definition as IEdmEnumType;

                    // If the member name is an integral value, we should try to convert it to the enum member name and find the enum member with the matching integral value
                    if (long.TryParse(memberName, out long memberIntegralValue))
                    {
                        if(enumType.TryParse(memberIntegralValue, out IEdmEnumMember enumMember))
                        {
                            string literalText = ODataUriUtils.ConvertToUriLiteral(enumMember.Name, default);
                            return new ConstantNode(new ODataEnumValue(enumMember.Name, enumType.ToString()), literalText, targetTypeReference);
                        }
                        
                        if(enumType.IsFlags)
                        {
                            string flagsValue = enumType.ParseFlagsFromIntegralValue(memberIntegralValue);
                            if(!string.IsNullOrEmpty(flagsValue))
                            {
                                string literalText = ODataUriUtils.ConvertToUriLiteral(flagsValue, default);
                                return new ConstantNode(new ODataEnumValue(flagsValue, enumType.ToString()), literalText, targetTypeReference);
                            }
                        }
                    }

                    // Check if the member name is a valid enum member name
                    IEdmEnumMember edmEnumMember = enumType.FindMember(memberName, comparison);
                    if (edmEnumMember != null)
                    {
                        string literalText = ODataUriUtils.ConvertToUriLiteral(constantNode.Value, default(ODataVersion));
                        return new ConstantNode(new ODataEnumValue(edmEnumMember.Name, enumType.ToString()), literalText, targetTypeReference);
                    }

                    // If the member name is a string representation of a flags value,
                    // we should try to convert it to the enum member name and find the enum member with the matching flags value
                    if (enumType.IsFlags)
                    {
                        string flagsValue = enumType.ParseFlagsFromStringValue(memberName, comparison);
                        if (!string.IsNullOrEmpty(flagsValue))
                        {
                            string literalText = ODataUriUtils.ConvertToUriLiteral(flagsValue, default(ODataVersion));
                            return new ConstantNode(new ODataEnumValue(flagsValue, enumType.ToString()), literalText, targetTypeReference);
                        }
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
                                   candidate :
                                   new ConvertNode(candidate, targetTypeReference);
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
        /// Converts a collection node's element type to <paramref name="targetTypeReference"/> when possible,
        /// materializing a new <see cref="CollectionConstantNode"/> for constant collections;
        /// leaves non-constant/open or non-convertible collections unchanged.
        /// </summary>
        /// <param name="source">Source collection node.</param>
        /// <param name="targetTypeReference">Desired collection type (must be a collection).</param>
        /// <returns>Converted collection node or original source.</returns>
        internal static CollectionNode ConvertToTypeIfNeeded(CollectionNode source, IEdmTypeReference targetTypeReference)
        {
            Debug.Assert(source != null, "source != null");

            if (targetTypeReference == null)
            {
                return source;
            }

            IEdmCollectionTypeReference sourceCollectionType = source.CollectionType;
            if (sourceCollectionType == null) // Open collection? Leave as is
            {
                return source;
            }

            if (!targetTypeReference.IsCollection())
            {
                throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.CollectionType.FullName(), targetTypeReference.FullName()));
            }

            IEdmCollectionTypeReference targetCollectionType = targetTypeReference.AsCollection();

            if (sourceCollectionType.IsEquivalentTo(targetCollectionType))
            {
                IEdmTypeReference sourceElemType = sourceCollectionType.ElementType();
                IEdmTypeReference targetElemType = targetCollectionType.ElementType();
                if (source is CollectionConstantNode colConstantNode
                    && sourceElemType.IsTypeDefinition()
                    && targetElemType.IsPrimitive()
                    && sourceElemType.AsPrimitive().PrimitiveKind() == targetElemType.AsPrimitive().PrimitiveKind())
                {
                    List<ConstantNode> convertedNodes = ConvertNodes(colConstantNode.Collection, targetElemType);
                    
                    return new CollectionConstantNode(convertedNodes, BuildCollectionLiteral(convertedNodes, targetElemType), targetCollectionType);
                }

                return source;
            }

            IEdmTypeReference sourceElementType = sourceCollectionType.ElementType();
            IEdmTypeReference targetElementType = targetCollectionType.ElementType();

            if (!TypePromotionUtils.CanConvertTo(null, sourceElementType, targetElementType))
            {
                throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, sourceElementType.FullName(), targetElementType.FullName()));
            }
            
            if (source is CollectionConstantNode collectionConstantNode)
            {
                List<ConstantNode> convertedNodes = ConvertNodes(collectionConstantNode.Collection, targetElementType);

                return new CollectionConstantNode(convertedNodes, BuildCollectionLiteral(convertedNodes, targetElementType), targetCollectionType);
            }

            // Non-constant collections: leave as-is (conversion implicit)
            return source;
        }

        /// <summary>
        /// Converts each constant value to <paramref name="targetElementType"/>, applying enum/numeric coercion; preserves null items.
        /// </summary>
        /// <param name="nodes">Original constant value nodes.</param>
        /// <param name="targetElementType">The target primitive type.</param>
        /// <returns>List of converted constant nodes.</returns>
        private static List<ConstantNode> ConvertNodes(IList<ConstantNode> nodes, IEdmTypeReference targetElementType)
        {
            List<ConstantNode> convertedNodes = new List<ConstantNode>(nodes.Count);

            for (int i = 0; i < nodes.Count; i++)
            {
                ConstantNode item = nodes[i];
                if (item == null)
                {
                    // Preserve null
                    convertedNodes.Add(new ConstantNode(null, "null", targetElementType));
                    continue;
                }

                ConstantNode convertedNode = ConvertToTypeIfNeeded(item, targetElementType) as ConstantNode;

                // If ConvertToTypeIfNeeded returned a ConvertNode, force materialization into a ConstantNode
                if (convertedNode == null)
                {
                    // Try to keep original literal text if meaningful
                    string literal = item.LiteralText ?? ODataUriUtils.ConvertToUriLiteral(item.Value, ODataVersion.V4);
                    convertedNode = new ConstantNode(item.Value, literal, targetElementType);
                }

                convertedNodes.Add(convertedNode);
            }

            return convertedNodes;
        }

        /// <summary>
        /// Builds a bracketed collection literal (e.g. [1,2,3]) from constant nodes, quoting/escaping strings and preserving nulls.
        /// </summary>
        /// <param name="nodes">Constant nodes representing items.</param>
        /// <param name="typeReference">Element type for string quoting rules.</param>
        /// <returns>OData collection literal text.</returns>
        private static string BuildCollectionLiteral(List<ConstantNode> nodes, IEdmTypeReference typeReference)
        {
            Debug.Assert(typeReference != null, $"{nameof(typeReference)} != null");

            List<string> list = new List<string>();
            for (int i = 0; i < nodes.Count; i++)
            {
                ConstantNode node = nodes[i];
                if (node == null || node.Value == null)
                {
                    list.Add("null");
                    continue;
                }
                
                string literal = node.LiteralText ?? ODataUriUtils.ConvertToUriLiteral(node.Value, ODataVersion.V4);
                if (typeReference.IsString() && !(literal.Length > 1 && literal[0] == '\'' && literal[^1] == '\''))
                {
                    literal = $"'{literal.Replace("'", "''", StringComparison.Ordinal)}'";
                }

                list.Add(literal);
            }

            return "(" + string.Join(",", list) + ")";
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
                    // Check if the enum value is a valid enum member name
                    if (enumType.HasMember(enumValue.Value, comparison))
                    {
                        continue;
                    }

                    if (long.TryParse(enumValue.Value, out long memberIntegralValue))
                    {
                        // Check if the enum value is a valid integral value
                        if (enumType.TryParse(memberIntegralValue, out IEdmEnumMember _))
                        {
                            continue;
                        }

                        // Check if the enum value is a valid flags value
                        if (enumType.IsFlags && enumType.IsValidFlagsEnumValue(memberIntegralValue))
                        {
                            continue;
                        }
                    }

                    if (enumType.IsFlags && !string.IsNullOrEmpty(enumType.ParseFlagsFromStringValue(enumValue.Value, comparison)))
                    {
                        continue;
                    }

                    throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, enumValue.Value));
                }
            }
        }
    }
}