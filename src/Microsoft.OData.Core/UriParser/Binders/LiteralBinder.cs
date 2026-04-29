//---------------------------------------------------------------------
// <copyright file="LiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Class that knows how to bind literal token.
    /// If the literal token has the expected type from metadata, then bind the literal token to that type. Otherwise, bind the literal token to the type inferred from the literal token text.
    /// </summary>
    internal sealed class LiteralBinder
    {
        /// <summary>
        /// Binds a literal token to a ConstantNode
        /// </summary>
        /// <param name="literalToken">Literal token to bind.</param>
        /// <returns>Bound query node.</returns>
        internal static QueryNode BindLiteral(LiteralToken literalToken, BindingState bindingState = null)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (literalToken.Value is UriTemplateExpression templateExpr)
            {
                return CreateConstantNode(literalToken.Value, literalToken.OriginalText ?? templateExpr.LiteralText, templateExpr.ExpectedType);
            }

            // 'null' -> ConstantNode(null, type)
            if (TryBindNullConstantNode(literalToken, out SingleValueNode nullNode))
            {
                return nullNode;
            }

            // 'null' is handled above, so we can be sure that literalToken.Value is not null here.
            if (literalToken.InferredType == null)
            {
                literalToken.InferredType = EdmLibraryExtensions.GetPrimitiveTypeReference(literalToken.Value.GetType());
            }

            // 'red' or '1' -> ConstantNode(ODataEnumValue, type)
            if (TryCreateEnumConstantNode(literalToken, bindingState, out ConstantNode enumNode))
            {
                return enumNode;
            }

            if (TryRewriteIntegralConstantValue(literalToken, out object rewrittenValue))
            {
                return CreateConstantNode(rewrittenValue, literalToken.OriginalText, literalToken.ExpectedType);
            }

            if (literalToken.ExpectedType != null)
            {
                return CreateNodeWithTargetTypeReference(literalToken);
            }
            else if (literalToken.InferredType != null)
            {
                return CreateConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.InferredType);
            }
            else
            {
                return CreateConstantNode(literalToken.Value, literalToken.OriginalText, null);
            }
        }

        private static ConstantNode CreateConstantNode(object value, string literal, IEdmTypeReference typeReference)
        {
            bool isNullOrEmptyLiteral = string.IsNullOrEmpty(literal);
            if (typeReference == null && isNullOrEmptyLiteral)
            {
                return new ConstantNode(value);
            }
            else if (typeReference == null)
            {
                return new ConstantNode(value, literal);
            }
            else if (isNullOrEmptyLiteral)
            {
                return new ConstantNode(value, typeReference);
            }
            else
            {
                return new ConstantNode(value, literal, typeReference);
            }
        }

        /// <summary>
        /// Binds the 'null' literal to a ConstantNode.
        /// A 'null' literal can be bound to any type, so the expected type is critical in this case.
        /// </summary>
        /// <param name="literalToken">The literal token to bind.</param>
        /// <param name="node">The resulting SingleValueNode if binding is successful.</param>
        /// <returns>True if the binding was successful; otherwise, false.</returns>
        private static bool TryBindNullConstantNode(LiteralToken literalToken, out SingleValueNode node)
        {
            node = null;
            if (literalToken.Value != null)
            {
                return false;
            }

            if (literalToken.ExpectedType == null)
            {
                node = CreateConstantNode(null, literalToken.OriginalText, literalToken.InferredType);
                return true;
            }

            if (!literalToken.ExpectedType.IsNullable)
            {
                throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotBindNullLiteralToNonNullableType, literalToken.OriginalText, literalToken.ExpectedType.FullName()));
            }

            // Be noted, a 'null' literal may have inferred type, for example in a custom type prefix. But, it's uncommon.
            if (literalToken.InferredType != null)
            {
                if (literalToken.InferredType.Definition.IsEquivalentTo(literalToken.ExpectedType.Definition))
                {
                    node = CreateConstantNode(null, literalToken.OriginalText, literalToken.ExpectedType);
                    return true;
                }

                if (!literalToken.ExpectedType.Definition.IsAssignableFrom(literalToken.InferredType.Definition))
                {
                    throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotBindLiteralToIncompatibleType, literalToken.OriginalText, literalToken.InferredType.FullName(), literalToken.ExpectedType.FullName()));
                }

                node = new ConvertNode(
                    CreateConstantNode(null, literalToken.OriginalText, literalToken.InferredType),
                    literalToken.ExpectedType);
                return true;
            }

            // It's also allowed for Edm.Untyped.
            node = CreateConstantNode(null, literalToken.OriginalText, literalToken.ExpectedType);
            return true;
        }

        /// <summary>
        /// Binds the string or integral literal to an Enum ConstantNode if the expected type is an Enum type.
        /// </summary>
        /// <param name="literalToken">The literal token to bind.</param>
        /// <param name="bindingState">The current binding state. It could be null.</param>
        /// <param name="enumNode">The resulting ConstantNode if binding is successful.</param>
        /// <returns>True if the binding was successful; otherwise, false.</returns>
        /// <exception cref="ODataException">Throws if the literal value is not a valid enum constant.</exception>
        private static bool TryCreateEnumConstantNode(LiteralToken literalToken, BindingState bindingState, out ConstantNode enumNode)
        {
            Debug.Assert(literalToken != null);

            IEdmTypeReference inferredType = literalToken.InferredType;
            IEdmTypeReference targetType = literalToken.ExpectedType;

            enumNode = null;
            if (!(inferredType.IsString() || inferredType.IsIntegral()) ||
                targetType == null ||
                !targetType.IsEnum())
            {
                return false;
            }

            IEdmEnumType enumType = targetType.AsEnum().EnumDefinition();
            Debug.Assert(enumType != null);

            StringComparison stringComparison = bindingState != null ?
                bindingState.Configuration.Resolver.EnableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal :
                StringComparison.Ordinal;

            // Be noted: for the enum literal, it's single enum value.
            // Enum literal can also be combined values (e.g. "Red, Green, Blue", "1,2,4"), it's valid for example: "~/MyFunction(myColor=NS.MyColor'Red, Green, Blue')". So far, it's handled by DottedIndenfierBinder.
            // For literal token, we also allow the combined enum values, but we don't support the mixed form (e.g. "Red, 1", "Red, Green, 1").
            // "Red, Green, Blue", "1,2,4"   --> valid for flag enum, will support it in the next step.
            // ['Red','Green', 'Blue'] or [1,2,4] --> valid for collection (it's not single literal).
            if (inferredType.IsString())
            {
                // "Red" or "1", or "Red, Green, Blue", or "1, 2, 4"
                string memberName = literalToken.Value.ToString(); // don't use the OriginalText here since it contains the quotes for string literals.
                if (enumType.ContainsMember(memberName, stringComparison))
                {
                    enumNode = CreateConstantNode(new ODataEnumValue(memberName, enumType.FullTypeName()), literalToken.OriginalText, targetType);
                    return true;
                }
            }
            else
            {
                long enumValue = Convert.ToInt64(literalToken.Value, CultureInfo.InvariantCulture);
                if (enumType.TryParse(enumValue, out _))
                {
                    // if the literal value is integer, use that integer value to construct the ODataEnumValue.
                    enumNode = CreateConstantNode(new ODataEnumValue(literalToken.Value.ToString(), enumType.FullTypeName()), literalToken.OriginalText, targetType);
                    return true;
                }
            }

            throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, literalToken.OriginalText));
        }

        /// <summary>
        /// Try to rewrite an Edm.Int32 constant node if its value is within the valid range of the target integer type.
        /// </summary>
        /// <param name="literalToken">The literal token to be rewritten.</param>
        /// <param name="newValue">The rewritten value if successful.</param>
        /// <returns>If the node is successfully rewritten.</returns>
        private static bool TryRewriteIntegralConstantValue(LiteralToken literalToken, out object newValue)
        {
            newValue = null;
            IEdmTypeReference sourceType = literalToken.InferredType;
            if (sourceType == null || !sourceType.IsInt32())
            {
                return false;
            }

            IEdmTypeReference targetType = literalToken.ExpectedType;
            if (targetType == null || !targetType.IsByte() && !targetType.IsSByte() && !targetType.IsInt16())
            {
                return false;
            }

            int sourceValue = (int)literalToken.Value;
            object targetValue = null;
            switch (targetType.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Byte:
                    if (sourceValue >= byte.MinValue && sourceValue <= byte.MaxValue)
                    {
                        targetValue = (byte)sourceValue;
                    }

                    break;
                case EdmPrimitiveTypeKind.SByte:
                    if (sourceValue >= sbyte.MinValue && sourceValue <= sbyte.MaxValue)
                    {
                        targetValue = (sbyte)sourceValue;
                    }

                    break;
                case EdmPrimitiveTypeKind.Int16:
                    if (sourceValue >= short.MinValue && sourceValue <= short.MaxValue)
                    {
                        targetValue = (short)sourceValue;
                    }

                    break;
            }

            if (targetValue == null)
            {
                return false;
            }

            newValue = targetValue;
            return true;
        }

        private static SingleValueNode CreateNodeWithTargetTypeReference(LiteralToken literalToken)
        {
            Debug.Assert(literalToken.ExpectedType != null);
            Debug.Assert(literalToken.Value != null); // The 'null' literal value is handled ahead

            // The 'null' literal value is handled ahead, so, it's safe to assume the literalToken has a value, then it has an inferred type associated.
            IEdmTypeReference inferredType = literalToken.InferredType;
            IEdmTypeReference targetTypeReference = literalToken.ExpectedType;

            // don't care about the 'nullability' since it's a literal (why a literal has a nullable attribute?)
            if (inferredType.Definition.IsEquivalentTo(targetTypeReference.Definition))
            {
                // If the inferred type is type definition. it's uncommon but could be valid in the custom prefix.
                // We create a conversion node from inferred type to its underlying type (target type)
                // so that the service can convert value of source clr type to underlying clr type.
                if (inferredType.IsTypeDefinition())
                {
                    ConstantNode node = CreateConstantNode(literalToken.Value, literalToken.OriginalText, inferredType);
                    return new ConvertNode(node, targetTypeReference);
                }

                // be noted, should use 'targetTypeReference' here to use the same nullability
                return CreateConstantNode(literalToken.Value, literalToken.OriginalText, targetTypeReference);
            }

            var source = CreateConstantNode(literalToken.Value, literalToken.OriginalText, inferredType);
            if (!TypePromotionUtils.CanConvertTo(source, inferredType, targetTypeReference))
            {
                throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, targetTypeReference.FullName()));
            }
            else
            {
                // Be noted, the referred type is not same as target type here, but they are compatible. for example, an inferred float value can be converted to the expected decimal.
                if (TryGetConstantNodePrimitiveLDMF(literalToken.Value, inferredType, out object originalPrimitiveValue) && (originalPrimitiveValue != null))
                {
                    object targetPrimitiveValue = ODataUriConversionUtils.CoerceNumericType(originalPrimitiveValue, targetTypeReference.AsPrimitive().Definition as IEdmPrimitiveType);
                    if (targetPrimitiveValue == null)
                    {
                        throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, targetTypeReference.FullName()));
                    }

                    return CreateConstantNode(targetPrimitiveValue, literalToken.OriginalText, targetTypeReference);
                }

                // other type conversion : ConvertNode
                return new ConvertNode(source, targetTypeReference);
            }
        }

        internal static bool TryGetConstantNodePrimitiveLDMF(object value, IEdmTypeReference inferredType, out object primitiveValue)
        {
            primitiveValue = null;
            if (inferredType == null)
            {
                return false;
            }

            IEdmPrimitiveType primitiveType = inferredType.AsPrimitiveOrNull().Definition as IEdmPrimitiveType;
            if (primitiveType != null)
            {
                switch (primitiveType.PrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Int32:
                    case EdmPrimitiveTypeKind.Int64:
                    case EdmPrimitiveTypeKind.Single:
                    case EdmPrimitiveTypeKind.Double:
                    case EdmPrimitiveTypeKind.Decimal:
                        primitiveValue = value;
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}