//---------------------------------------------------------------------
// <copyright file="MetadataBindingUtils.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Diagnostics;
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
                if (constantNode != null && constantNode.Value != null && source.TypeReference.IsString() && targetTypeReference.IsEnum())
                {
                    return new ConstantNode(new ODataEnumValue(constantNode.Value.ToString(), targetTypeReference.Definition.ToString()), constantNode.Value.ToString(), targetTypeReference);
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
        /// <param name="segment">The node to retrive the type reference from.</param>
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