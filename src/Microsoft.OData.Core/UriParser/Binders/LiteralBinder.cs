//---------------------------------------------------------------------
// <copyright file="LiteralBinder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Class that knows how to bind literal values.
    /// </summary>
    internal sealed class LiteralBinder
    {
        private static bool TryCreateNullConstantNode(LiteralToken literalToken, out ConstantNode node)
        {
            node = null;
            if (literalToken.Value != null)
            {
                return false;
            }

            if (literalToken.ExpectedEdmTypeReference == null)
            {
                node = new ConstantNode(null, literalToken.OriginalText);
                return true;
            }

            if (!literalToken.ExpectedEdmTypeReference.IsNullable)
            {
                throw new ODataException("Cannot create a null constant node for a non-nullable type.");
            }

            node = new ConstantNode(null, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
            return true;
        }

        private static bool TryCreateEnumConstantNode(LiteralToken literalToken, out ConstantNode enumNode)
        {
            IEdmTypeReference inferredType = literalToken.InferredEdmTypeReference;
            IEdmTypeReference targetType = literalToken.ExpectedEdmTypeReference;

            enumNode = null;
            if (inferredType == null ||
                !(inferredType.IsString() || inferredType.IsIntegral()) ||
                targetType == null ||
                !targetType.IsEnum())
            {
                return false;
            }

            IEdmEnumType enumType = targetType.AsEnum().EnumDefinition();
            Debug.Assert(enumType != null);

            // singleEnumValue = enumerationMember / enumMemberValue
            // enumMemberValue = int64Value
            string memberName = literalToken.Value.ToString();
            if (enumType.ContainsMember(memberName, StringComparison.Ordinal))
            {
                enumNode = new ConstantNode(new ODataEnumValue(memberName, enumType.ToString()), literalToken.OriginalText, targetType);
                return true;
            }

            // If the member name is an integral value, we should try to convert it to the enum member name and find the enum member with the matching integral value
            if (long.TryParse(memberName, out long memberIntegralValue) && enumType.TryParse(memberIntegralValue, out IEdmEnumMember enumMember))
            {
                enumNode = new ConstantNode(new ODataEnumValue(enumMember.Name, enumType.ToString()), literalToken.OriginalText, targetType);
                return true;
            }

            throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, memberName));
        }

        /// <summary>
        /// Try to rewrite an Edm.Int32 constant node if its value is within the valid range of the target integer type.
        /// </summary>
        /// <param name="boundNode">The node to be rewritten.</param>
        /// <param name="targetType">The target type reference.</param>
        /// <returns>If the node is successfully rewritten.</returns>
        private static (bool, object) TryRewriteIntegralConstantValue(object value, IEdmTypeReference sourceType, IEdmTypeReference targetType)
        {
            if (targetType == null || !targetType.IsByte() && !targetType.IsSByte() && !targetType.IsInt16())
            {
                return (false, null);
            }

            if (sourceType == null || !sourceType.IsInt32())
            {
                return (false, null);
            }

            int sourceValue = (int)value;
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
                return (false, null);
            }

            return (true, targetValue);
        }

        /// <summary>
        /// Binds a literal value to a ConstantNode
        /// </summary>
        /// <param name="literalToken">Literal token to bind.</param>
        /// <returns>Bound query node.</returns>
        internal static QueryNode BindLiteral(LiteralToken literalToken)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (TryCreateNullConstantNode(literalToken, out ConstantNode nullNode))
            {
                return nullNode;
            }

            if (TryCreateEnumConstantNode(literalToken, out ConstantNode enumNode))
            {
                return enumNode;
            }

            if (!literalToken.OriginalText.IsEmpty)
            {
                if (literalToken.ExpectedEdmTypeReference != null)
                {
                    (bool success, object rewrittenValue) = TryRewriteIntegralConstantValue(literalToken.Value, literalToken.InferredEdmTypeReference, literalToken.ExpectedEdmTypeReference);
                    if (success)
                    {
                        return new ConstantNode(rewrittenValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                    }

                    ConstantNode node = new ConstantNode(literalToken.Value, literalToken.OriginalText);

                    return MetadataBindingUtils.ConvertToTypeIfNeeded(node, literalToken.ExpectedEdmTypeReference);
                }

                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            if (literalToken.OriginalText.IsEmpty)
            {
                return new ConstantNode(literalToken.Value);
            }

            return new ConstantNode(literalToken.Value, literalToken.OriginalText);
        }

        /// <summary>
        /// Binds a literal value to a ConstantNode
        /// </summary>
        /// <param name="literalToken">Literal token to bind.</param>
        /// <returns>Bound query node.</returns>
        internal static QueryNode BindInLiteral(LiteralToken literalToken)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            if (!literalToken.OriginalText.IsEmpty)
            {
                if (literalToken.ExpectedEdmTypeReference != null)
                {
                    OData.Edm.IEdmCollectionTypeReference collectionReference =
                        literalToken.ExpectedEdmTypeReference as OData.Edm.IEdmCollectionTypeReference;
                    if (collectionReference != null)
                    {
                        ODataCollectionValue collectionValue = literalToken.Value as ODataCollectionValue;
                        if (collectionValue != null)
                        {
                            return new CollectionConstantNode(collectionValue.Items, literalToken.OriginalText, collectionReference);
                        }
                    }

                    return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                }

                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }

            return new ConstantNode(literalToken.Value);
        }

        //public static ConstantNode BindStringLiteral(StringLiteralToken stringLiteral)
        //{

        //}

        public static QueryNode BindTypedLiteral(LiteralToken literalToken, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            // for example: myprefix'abc...'
            //if (literalToken.LiteralKind == LiteralKind.CustomTypeLiteral)
            //{
            //    // First, let's parse the custom prefix literal using the CustomEdmTypeReference from the literal string.
            //    object value = literalParser.ParseUriStringToType(literalToken.OriginalText, literalToken.CustomEdmTypeReference, out UriLiteralParsingException exception);

            //    VerifyParsedResult(literalToken, value, exception);

            //    if (literalToken.ExpectedEdmTypeReference == null ||
            //        literalToken.ExpectedEdmTypeReference.IsUntyped() ||
            //        literalToken.ExpectedEdmTypeReference.IsAssignableFrom(literalToken.CustomEdmTypeReference))
            //    {
            //        return new ConstantNode(value, literalToken.OriginalText, literalToken.CustomEdmTypeReference);
            //    }

            //    throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName()));
            //}

            if (literalToken.ExpectedEdmTypeReference != null)
            {
                // for 'null' literal, we can only bind it to any nullable type, otherwise it's invalid.
                if (literalToken.Value == null)
                {
                    if (literalToken.ExpectedEdmTypeReference.IsNullable)
                    {
                        return new ConstantNode(null, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                    }
                    else
                    {
                        // throw if we have a non-nullable type and the literal is null, since this is not valid.
                        throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotBindNullLiteralToNonNullableType, literalToken.ExpectedEdmTypeReference.FullName()));
                    }
                }

                if (literalToken.ExpectedEdmTypeReference.IsUntyped())
                {
                    // Any value can be assigned to untyped, just return the value from the token without parsing again.
                    return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.InferredEdmTypeReference);
                }

                QueryNode node = ConvertToTypeIfNeeded(literalToken, literalToken.ExpectedEdmTypeReference, model);
                if (node != null)
                {
                    return node;
                }

                // for pre-parsed string literal,
                if (literalToken.Value is string)
                {
                    if (literalToken.ExpectedEdmTypeReference.IsString())
                    {
                        return new ConstantNode(literalToken.Value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                    }
                    else
                    {
                        throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName()));
                    }
                }

                //literalToken.ExpectedEdmTypeReference.IsAssignableFrom(literalToken.CustomEdmTypeReference)

                // for all other pre-parsed literals, we still need to parse the literal text to get the value.
                object value = DefaultUriLiteralParser.GetOrCreate(model).ParseUriStringToType(literalToken.OriginalText.ToString(), literalToken.ExpectedEdmTypeReference, out UriLiteralParsingException exception);

                VerifyParsedResult(literalToken, value, exception);

                return new ConstantNode(value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
            }
            else
            {
                return new ConstantNode(literalToken.Value, literalToken.OriginalText);
            }
        }

        /// <summary>
        /// If the source node is not of the specified type, then we check if type promotion is possible and inject a convert node.
        /// If the source node is the same type as the target type (or if the target type is null), we just return the source node as is.
        /// </summary>
        /// <param name="source">The source node to apply the conversion to.</param>
        /// <param name="targetTypeReference">The target primitive type. May be null - this method will do nothing in that case.</param>
        /// <returns>The converted query node, or the original source node unchanged.</returns>
        private static QueryNode ConvertToTypeIfNeeded(LiteralToken source, IEdmTypeReference targetTypeReference, IEdmModel model)
        {
            Debug.Assert(source != null, "source != null");
            Debug.Assert(targetTypeReference != null, "targetTypeReference != null");
            Debug.Assert(source.InferredEdmTypeReference != null); // We check the null value ahead.

            if (source.InferredEdmTypeReference.IsEquivalentTo(targetTypeReference))
            {
                return new ConstantNode(source.Value, source.OriginalText, source.InferredEdmTypeReference);
            }

            // Structured type in url will be translated into a node with raw string value.
            // We create a conversion node from string to structured type.
            if (targetTypeReference.IsStructured() || targetTypeReference.IsStructuredCollectionType())
            {
                throw new ODataException("Should never be here.");
            }

            // Check if the source node is a constant node, not null, and the source type is either string or integral
            // and the target type is an enum.
            if ((source.InferredEdmTypeReference.IsString() || source.InferredEdmTypeReference.IsIntegral()) && targetTypeReference.IsEnum())
            {
                string memberName = source.Value.ToString();
                IEdmEnumType enumType = targetTypeReference.Definition as IEdmEnumType;
                if (enumType.ContainsMember(memberName, StringComparison.Ordinal))
                {
                    // string literalText = ODataUriUtils.ConvertToUriLiteral(constantNode.Value, default(ODataVersion));
                    return new ConstantNode(new ODataEnumValue(memberName, enumType.FullTypeName()), source.OriginalText, targetTypeReference);
                }

                // Saxu: how about the isFlag?
                // If the member name is an integral value, we should try to convert it to the enum member name and find the enum member with the matching integral value
                if (long.TryParse(memberName, out long memberIntegralValue) && enumType.TryParse(memberIntegralValue, out IEdmEnumMember enumMember))
                {
                    string literalText = ODataUriUtils.ConvertToUriLiteral(enumMember.Name, default(ODataVersion));
                    return new ConstantNode(new ODataEnumValue(enumMember.Name, enumType.FullTypeName()), literalText, targetTypeReference);
                }

                throw new ODataException(Error.Format(SRResources.Binder_IsNotValidEnumConstant, memberName));
            }

            if (!TypePromotionUtils.CanConvertTo(null, source.InferredEdmTypeReference, targetTypeReference))
            {
                object value = DefaultUriLiteralParser.GetOrCreate(model).ParseUriStringToType(source.OriginalText.ToString(), source.ExpectedEdmTypeReference, out UriLiteralParsingException exception);

                VerifyParsedResult(source, value, exception);

                return new ConstantNode(value, source.OriginalText, source.ExpectedEdmTypeReference);

                //throw new ODataException(Error.Format(SRResources.MetadataBinder_CannotConvertToType, source.InferredEdmTypeReference.FullName(), targetTypeReference.FullName()));
            }
            else
            {
                object originalPrimitiveValue;
                if (TryGetConstantNodePrimitiveLDMF(source, out originalPrimitiveValue) && (originalPrimitiveValue != null))
                {
                    // L F D M types : directly create a ConvertNode.
                    // 1. NodeToExpressionTranslator.cs won't allow implicitly converting single/double to decimal, which should be done here at Node tree level.
                    // 2. And prevent losing precision in float -> double, e.g. (double)1.234f => 1.2339999675750732d not 1.234d
                    object targetPrimitiveValue = ODataUriConversionUtils.CoerceNumericType(originalPrimitiveValue, targetTypeReference.AsPrimitive().Definition as IEdmPrimitiveType);

                    var candidate = new ConstantNode(targetPrimitiveValue, source.OriginalText, targetTypeReference);
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
                    return new ConvertNode(new ConstantNode(source.Value, source.OriginalText, source.InferredEdmTypeReference), targetTypeReference);
                }
            }
        }

        internal static bool TryGetConstantNodePrimitiveLDMF(LiteralToken literalToken, out object primitiveValue)
        {
            primitiveValue = null;

            IEdmPrimitiveType primitiveType = literalToken.InferredEdmTypeReference.AsPrimitiveOrNull().Definition as IEdmPrimitiveType;
            if (primitiveType != null)
            {
                switch (primitiveType.PrimitiveKind)
                {
                    case EdmPrimitiveTypeKind.Int32:
                    case EdmPrimitiveTypeKind.Int64:
                    case EdmPrimitiveTypeKind.Single:
                    case EdmPrimitiveTypeKind.Double:
                    case EdmPrimitiveTypeKind.Decimal:
                        primitiveValue = literalToken.Value;
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }



#if false
        public static ConstantNode BindTypedLiteral2(LiteralToken literalToken, IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(literalToken, "literalToken");

            DefaultUriLiteralParser literalParser = DefaultUriLiteralParser.GetOrCreate(model);

            // for example: myprefix'abc...'
            if (literalToken.LiteralKind == LiteralKind.CustomTypeLiteral)
            {
                // First, let's parse the custom prefix literal using the CustomEdmTypeReference from the literal string.
                object value = literalParser.ParseUriStringToType(literalToken.OriginalText.ToString(), literalToken.CustomEdmTypeReference, out UriLiteralParsingException exception);

                VerifyParsedResult(literalToken, value, exception);

                if (literalToken.ExpectedEdmTypeReference == null ||
                    literalToken.ExpectedEdmTypeReference.IsUntyped() ||
                    literalToken.ExpectedEdmTypeReference.IsAssignableFrom(literalToken.CustomEdmTypeReference))
                {
                    return new ConstantNode(value, literalToken.OriginalText, literalToken.CustomEdmTypeReference);
                }

                throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName()));
            }

            // if we have an expected type, we need to parse the literal text to get the value.  If we don't have an expected type, then we just use the value from the token.
            // If the expected type is untyped, it means we don't have context to bind this literal. Just to use the literal kind to bind.
            if (literalToken.ExpectedEdmTypeReference != null && !literalToken.ExpectedEdmTypeReference.IsUntyped())
            {
                if (literalToken.OriginalText.Span.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    if (literalToken.ExpectedEdmTypeReference.IsNullable)
                    {
                        return new ConstantNode(null, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
                    }
                    else
                    {
                        // throw if we have a non-nullable type and the literal is null, since this is not valid.
                        throw new ODataException("Error.Format(SRResources.MetadataBinder_UnsupportedQueryTokenKind, token.Kind)");
                    }
                }

                bool quoted = IsStringQuoted(literalToken.OriginalText.Span);
                if (quoted)
                {
                    bool isString = literalToken.ExpectedEdmTypeReference.IsString();
                    if (isString || literalToken.ExpectedEdmTypeReference.IsUntyped())
                    {
                        string escapedString = GetStringByRemoveQuotes(literalToken.OriginalText.Span);
                        IEdmTypeReference edmType = literalToken.ExpectedEdmTypeReference;
                        if (!isString)
                        {
                            edmType = EdmCoreModel.Instance.GetString(true);
                        }

                        return new ConstantNode(escapedString, literalToken.OriginalText, edmType);
                    }
                    else
                    {
                        throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName()));
                    }
                }

                // for all other literals
                object value = literalParser.ParseUriStringToType(literalToken.OriginalText.ToString(), literalToken.ExpectedEdmTypeReference, out UriLiteralParsingException exception);

                VerifyParsedResult(literalToken, value, exception);

                return new ConstantNode(value, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference);
            }
            else
            {
                if (literalToken.LiteralKind == LiteralKind.Null)
                {
                    return new ConstantNode(null, literalToken.OriginalText, null);
                }

                IEdmTypeReference typeReference = UriParserHelper.GetLiteralEdmTypeReference(literalToken.LiteralKind);

                object value = literalParser.ParseUriStringToType(literalToken.OriginalText.ToString(), typeReference, out UriLiteralParsingException exception);

                VerifyParsedResult(literalToken, value, exception);

                // Update the real type reference using the actual value, typically for Geograph/geometry.
                typeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType());

                return new ConstantNode(value, literalToken.OriginalText, typeReference);
            }
        }
#endif

        internal static void VerifyParsedResult(LiteralToken literalToken, object value, UriLiteralParsingException exception)
        {
            if (value == null)
            {
                if (exception == null)
                {
                    throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValue, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName()));
                }
                else
                {
                    throw new ODataException(Error.Format(SRResources.LiteralBinder_CannotConvertPrimitiveValueWithReason, literalToken.OriginalText, literalToken.ExpectedEdmTypeReference.FullName(), exception.Message));
                }
            }
        }

        internal static bool IsStringQuoted(ReadOnlySpan<char> text)
        {
            if (text.Length >= 2 && text[0] == '\'' && text[^1] == '\'')
            {
                return true;
            }

            if (text.Length >= 2 && text[0] == '"' && text[^1] == '"')
            {
                return true;
            }

            return false;
        }

        internal static string GetStringByRemoveQuotes(ReadOnlySpan<char> text)
        {
            if (text.Length < 2)
            {
                return text.ToString();
            }

            string escaped;
            char ch;
            if (text[0] == '\'' && text[^1] == '\'')
            {
                // single-quoted, the single quote is escaped by doubling it, for example: 'abc''def' represents abc'def
                escaped = "''";
                ch = '\'';
            }
            else if (text[0] == '"' && text[^1] == '"')
            {
                // double-quoted, the double quote is escaped by backslash, for example: "abc\"def" represents abc"def
                escaped = "\\\"";
                ch = '"';
            }
            else
            {
                // for none quoted, just return the original text, for example: abc, which is not a valid string literal but we just return it.
                return text.ToString();
            }

            ReadOnlySpan<char> s = text.Slice(1, text.Length - 2);
            ReadOnlySpan<char> t = s;

            StringBuilder sb = null;

            while (true)
            {
                int i = t.IndexOf(escaped, StringComparison.Ordinal);
                if (i >= 0)
                {
                    if (sb == null)
                    {
                        sb = new StringBuilder();
                    }

                    sb.Append(t.Slice(0, i)).Append(ch);
                    t = t.Slice(i + 2);
                }
                else
                {
                    break;
                }
            }

            if (sb != null)
            {
                sb.Append(t);
                return sb.ToString();
            }

            return s.ToString();
        }
    }
}