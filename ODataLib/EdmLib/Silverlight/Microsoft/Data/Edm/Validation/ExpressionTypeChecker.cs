//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Validation
{
    /// <summary>
    /// Collection of extension methods to assert that an expression is of the required type.
    /// </summary>
    public static class ExpressionTypeChecker
    {
        private static readonly bool[,] promotionMap = InitializePromotionMap();

        /// <summary>
        /// Determines if the type of an expression is compatible with the provided type
        /// </summary>
        /// <param name="expression">The expression to assert the type of.</param>
        /// <param name="type">The type to assert the expression as.</param>
        /// <param name="discoveredErrors">Errors produced if the expression does not match the specified type.</param>
        /// <returns>A value indicating whether the expression is valid for the given type or not.</returns>
        /// <remarks>If the expression has an associated type, this function will check that it matches the expected type and stop looking further. 
        /// If an expression claims a type, it must be validated that the type is valid for the expression. If the expression does not claim a type
        /// this method will attempt to check the validity of the expression itself with the asserted type.</remarks>
        public static bool TryAssertType(this IEdmExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            return TryAssertType(expression, type, null, false, out discoveredErrors);
        }
        
        /// <summary>
        /// Determines if the type of an expression is compatible with the provided type
        /// </summary>
        /// <param name="expression">The expression to assert the type of.</param>
        /// <param name="type">The type to assert the expression as.</param>
        /// <param name="context">The context paths are to be evaluated in.</param>
        /// <param name="matchExactly">A value indicating whether the expression must match the asserted type exactly, or simply be compatible.</param>
        /// <param name="discoveredErrors">Errors produced if the expression does not match the specified type.</param>
        /// <returns>A value indicating whether the expression is valid for the given type or not.</returns>
        /// <remarks>If the expression has an associated type, this function will check that it matches the expected type and stop looking further. 
        /// If an expression claims a type, it must be validated that the type is valid for the expression. If the expression does not claim a type
        /// this method will attempt to check the validity of the expression itself with the asserted type.</remarks>
        public static bool TryAssertType(this IEdmExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");

            // If we don't have a type to assert this passes vacuously.
            if (type == null || type.TypeKind() == EdmTypeKind.None)
            {
                discoveredErrors = Enumerable.Empty<EdmError>();
                return true;
            }

            switch (expression.ExpressionKind)
            {
                case EdmExpressionKind.IntegerConstant:
                case EdmExpressionKind.StringConstant:
                case EdmExpressionKind.BinaryConstant:
                case EdmExpressionKind.BooleanConstant:
                case EdmExpressionKind.DateTimeConstant:
                case EdmExpressionKind.DateTimeOffsetConstant:
                case EdmExpressionKind.DecimalConstant:
                case EdmExpressionKind.FloatingConstant:
                case EdmExpressionKind.GuidConstant:
                case EdmExpressionKind.TimeConstant:
                    IEdmPrimitiveValue primitiveValue = (IEdmPrimitiveValue)expression;
                    if (primitiveValue.Type != null)
                    {
                        return TestTypeReferenceMatch(primitiveValue.Type, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryAssertPrimitiveAsType(primitiveValue, type, out discoveredErrors);
                case EdmExpressionKind.Null:
                    return TryAssertNullAsType((IEdmNullExpression)expression, type, out discoveredErrors);
                case EdmExpressionKind.Path:
                    return TryAssertPathAsType((IEdmPathExpression)expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.FunctionApplication:
                    IEdmApplyExpression applyExpression = (IEdmApplyExpression)expression;
                    if (applyExpression.AppliedFunction != null)
                    {
                        IEdmFunctionBase function = applyExpression.AppliedFunction as IEdmFunctionBase;
                        if (function != null)
                        {
                            return TestTypeReferenceMatch(function.ReturnType, type, expression.Location(), matchExactly, out discoveredErrors);
                        }
                    }

                    // If we don't have the applied function we just assume that it will work.
                    discoveredErrors = Enumerable.Empty<EdmError>();
                    return true;
                case EdmExpressionKind.If:
                    return TryAssertIfAsType((IEdmIfExpression)expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.IsType:
                    return TestTypeReferenceMatch(EdmCoreModel.Instance.GetBoolean(false), type, expression.Location(), matchExactly, out discoveredErrors);
                case EdmExpressionKind.Record:
                    IEdmRecordExpression recordExpression = (IEdmRecordExpression)expression;
                    if (recordExpression.DeclaredType != null)
                    {
                        return TestTypeReferenceMatch(recordExpression.DeclaredType, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryAssertRecordAsType(recordExpression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.Collection:
                    IEdmCollectionExpression collectionExpression = (IEdmCollectionExpression)expression;
                    if (collectionExpression.DeclaredType != null)
                    {
                        return TestTypeReferenceMatch(collectionExpression.DeclaredType, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryAssertCollectionAsType(collectionExpression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.Labeled:
                    return TryAssertType(((IEdmLabeledExpression)expression).Expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.AssertType:
                    return TestTypeReferenceMatch(((IEdmAssertTypeExpression)expression).Type, type, expression.Location(), matchExactly, out discoveredErrors);
                case EdmExpressionKind.LabeledExpressionReference:
                    return TryAssertType(((IEdmLabeledExpressionReferenceExpression)expression).ReferencedLabeledExpression, type, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionNotValidForTheAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType) };
                    return false;
            }
        }

        internal static bool TryAssertPrimitiveAsType(this IEdmPrimitiveValue expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsPrimitive())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, Edm.Strings.EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType) };
                return false;
            }

            switch (expression.ValueKind)
            {
                case EdmValueKind.Binary:
                    return TryAssertBinaryConstantAsType((IEdmBinaryConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Boolean:
                    return TryAssertBooleanConstantAsType((IEdmBooleanConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.DateTime:
                    return TryAssertDateTimeConstantAsType((IEdmDateTimeConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.DateTimeOffset:
                    return TryAssertDateTimeOffsetConstantAsType((IEdmDateTimeOffsetConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Decimal:
                    return TryAssertDecimalConstantAsType((IEdmDecimalConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Floating:
                    return TryAssertFloatingConstantAsType((IEdmFloatingConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Guid:
                    return TryAssertGuidConstantAsType((IEdmGuidConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Integer:
                    return TryAssertIntegerConstantAsType((IEdmIntegerConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.String:
                    return TryAssertStringConstantAsType((IEdmStringConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Time:
                    return TryAssertTimeConstantAsType((IEdmTimeConstantExpression)expression, type, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                    return false;
            }
        }

        internal static bool TryAssertNullAsType(this IEdmNullExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsNullable)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, Edm.Strings.EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static bool TryAssertPathAsType(this IEdmPathExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            IEdmStructuredType structuredContext = context as IEdmStructuredType;
            if (structuredContext != null)
            {
                IEdmType result = context;

                foreach (string segment in expression.Path)
                {
                    IEdmStructuredType structuredResult = result as IEdmStructuredType;
                    if (structuredResult == null)
                    {
                        discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.PathIsNotValidForTheGivenContext, Edm.Strings.EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext(segment)) };
                        return false;
                    }

                    IEdmProperty resultProperty = structuredResult.FindProperty(segment);
                    result = (resultProperty != null) ? resultProperty.Type.Definition : null;

                    // If the path is not resolved, it could refer to an open type, and we cannot make an assertion about its type.
                    if (result == null)
                    {
                        discoveredErrors = Enumerable.Empty<EdmError>();
                        return true;
                    }
                }

                return TestTypeMatch(result, type.Definition, expression.Location(), matchExactly, out discoveredErrors);
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static bool TryAssertIfAsType(this IEdmIfExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            IEnumerable<EdmError> ifTrueErrors;
            IEnumerable<EdmError> ifFalseErrors;
            bool success = TryAssertType(expression.TrueExpression, type, context, matchExactly, out ifTrueErrors);
            success = TryAssertType(expression.FalseExpression, type, context, matchExactly, out ifFalseErrors) && success;
            if (!success)
            {
                List<EdmError> errorList = new List<EdmError>(ifTrueErrors);
                errorList.AddRange(ifFalseErrors);
                discoveredErrors = errorList;
            }
            else
            {
                discoveredErrors = Enumerable.Empty<EdmError>();
            }

            return success;
        }

        internal static bool TryAssertRecordAsType(this IEdmRecordExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");
            EdmUtil.CheckArgumentNull(type, "type");

            if (!type.IsStructured())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.RecordExpressionNotValidForNonStructuredType, Edm.Strings.EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType) };
                return false;
            }

            HashSetInternal<string> foundProperties = new HashSetInternal<string>();
            List<EdmError> errors = new List<EdmError>();

            IEdmStructuredTypeReference structuredType = type.AsStructured();
            foreach (IEdmProperty typeProperty in structuredType.StructuredDefinition().Properties())
            {
                IEdmPropertyConstructor expressionProperty = expression.Properties.FirstOrDefault(p => p.Name == typeProperty.Name);
                if (expressionProperty == null)
                {
                    errors.Add(new EdmError(expression.Location(), EdmErrorCode.RecordExpressionMissingRequiredProperty, Edm.Strings.EdmModel_Validator_Semantic_RecordExpressionMissingProperty(typeProperty.Name)));
                }
                else
                {
                    IEnumerable<EdmError> recursiveErrors;
                    if (!expressionProperty.Value.TryAssertType(typeProperty.Type, context, matchExactly, out recursiveErrors))
                    {
                        foreach (EdmError error in recursiveErrors)
                        {
                            errors.Add(error);
                        }
                    }

                    foundProperties.Add(typeProperty.Name);
                }
            }

            if (!structuredType.IsOpen())
            {
                foreach (IEdmPropertyConstructor property in expression.Properties)
                {
                    if (!foundProperties.Contains(property.Name))
                    {
                        errors.Add(new EdmError(expression.Location(), EdmErrorCode.RecordExpressionHasExtraProperties, Edm.Strings.EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties(property.Name)));
                    }
                }
            }

            if (errors.FirstOrDefault() != null)
            {
                discoveredErrors = errors;
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static bool TryAssertCollectionAsType(this IEdmCollectionExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsCollection())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.CollectionExpressionNotValidForNonCollectionType, Edm.Strings.EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType) };
                return false;
            }

            IEdmTypeReference collectionElementType = type.AsCollection().ElementType();
            bool success = true;
            List<EdmError> errors = new List<EdmError>();
            IEnumerable<EdmError> recursiveErrors;
            foreach (IEdmExpression element in expression.Elements)
            {
                success = TryAssertType(element, collectionElementType, context, matchExactly, out recursiveErrors) && success;
                errors.AddRange(recursiveErrors);
            }

            discoveredErrors = errors;
            return success;
        }

        private static bool TryAssertGuidConstantAsType(IEdmGuidConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsGuid())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertFloatingConstantAsType(IEdmFloatingConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsFloating())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertDecimalConstantAsType(IEdmDecimalConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDecimal())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertDateTimeOffsetConstantAsType(IEdmDateTimeOffsetConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDateTimeOffset())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertDateTimeConstantAsType(IEdmDateTimeConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDateTime())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertTimeConstantAsType(IEdmTimeConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsTime())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertBooleanConstantAsType(IEdmBooleanConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsBoolean())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertStringConstantAsType(IEdmStringConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsString())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            IEdmStringTypeReference stringType = type.AsString();
            if (stringType.MaxLength.HasValue && expression.Value.Length > stringType.MaxLength.Value)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.StringConstantLengthOutOfRange, Edm.Strings.EdmModel_Validator_Semantic_StringConstantLengthOutOfRange(expression.Value.Length, stringType.MaxLength.Value)) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertIntegerConstantAsType(IEdmIntegerConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsIntegral())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Int64:
                    return TryAssertIntegerConstantInRange(expression, Int64.MinValue, Int64.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Int32:
                    return TryAssertIntegerConstantInRange(expression, Int32.MinValue, Int32.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Int16:
                    return TryAssertIntegerConstantInRange(expression, Int16.MinValue, Int16.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Byte:
                    return TryAssertIntegerConstantInRange(expression, Byte.MinValue, Byte.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.SByte:
                    return TryAssertIntegerConstantInRange(expression, SByte.MinValue, SByte.MaxValue, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                    return false;
            }
        }

        private static bool TryAssertIntegerConstantInRange(IEdmIntegerConstantExpression expression, long min, long max, out IEnumerable<EdmError> discoveredErrors)
        {
            if (expression.Value < min || expression.Value > max)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.IntegerConstantValueOutOfRange, Edm.Strings.EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryAssertBinaryConstantAsType(IEdmBinaryConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsBinary())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            IEdmBinaryTypeReference binaryType = type.AsBinary();
            if (binaryType.MaxLength.HasValue && expression.Value.Length > binaryType.MaxLength.Value)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.BinaryConstantLengthOutOfRange, Edm.Strings.EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange(expression.Value.Length, binaryType.MaxLength.Value)) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TestTypeReferenceMatch(this IEdmTypeReference expressionType, IEdmTypeReference assertedType, EdmLocation location, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!TestNullabilityMatch(expressionType, assertedType, location, out discoveredErrors))
            {
                return false;
            }

            // A bad type reference matches anything (so as to avoid generating spurious errors).
            if (expressionType.IsBad())
            {
                discoveredErrors = Enumerable.Empty<EdmError>();
                return true;
            }

            return TestTypeMatch(expressionType.Definition, assertedType.Definition, location, matchExactly, out discoveredErrors);
        }

        private static bool TestTypeMatch(this IEdmType expressionType, IEdmType assertedType, EdmLocation location, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            if (matchExactly)
            {
                if (!expressionType.IsEquivalentTo(assertedType))
                {
                    discoveredErrors = new EdmError[] { new EdmError(location, EdmErrorCode.ExpressionNotValidForTheAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType) };
                    return false;
                }
            }
            else
            {
                // A bad type matches anything (so as to avoid generating spurious errors).
                if (expressionType.TypeKind == EdmTypeKind.None || expressionType.IsBad())
                {
                    discoveredErrors = Enumerable.Empty<EdmError>();
                    return true;
                }

                if (expressionType.TypeKind == EdmTypeKind.Primitive && assertedType.TypeKind == EdmTypeKind.Primitive)
                {
                    IEdmPrimitiveType primitiveExpressionType = expressionType as IEdmPrimitiveType;
                    IEdmPrimitiveType primitiveAssertedType = assertedType as IEdmPrimitiveType;
                    if (!primitiveExpressionType.PrimitiveKind.PromotesTo(primitiveAssertedType.PrimitiveKind))
                    {
                        discoveredErrors = new EdmError[] { new EdmError(location, EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType(expressionType.ToTraceString(), assertedType.ToTraceString())) };
                        return false;
                    }
                }
                else
                {
                    if (!expressionType.IsOrInheritsFrom(assertedType))
                    {
                        discoveredErrors = new EdmError[] { new EdmError(location, EdmErrorCode.ExpressionNotValidForTheAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType) };
                        return false;
                    }
                }
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TestNullabilityMatch(this IEdmTypeReference expressionType, IEdmTypeReference assertedType, EdmLocation location, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!assertedType.IsNullable && expressionType.IsNullable)
            {
                discoveredErrors = new EdmError[] { new EdmError(location, EdmErrorCode.CannotAssertNullableTypeAsNonNullableType, Edm.Strings.EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType(expressionType.FullName())) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool PromotesTo(this EdmPrimitiveTypeKind startingKind, EdmPrimitiveTypeKind target)
        {
            return startingKind == target || promotionMap[(int)startingKind, (int)target];
        }

        private static bool[,] InitializePromotionMap()
        {
            int typeKindCount = typeof(EdmPrimitiveTypeKind).GetFields().Where(f => f.IsLiteral).Count();
            bool[,] promotionMap = new bool[typeKindCount, typeKindCount];

            promotionMap[(int)EdmPrimitiveTypeKind.Byte, (int)EdmPrimitiveTypeKind.Int16] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.Byte, (int)EdmPrimitiveTypeKind.Int32] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.Byte, (int)EdmPrimitiveTypeKind.Int64] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.SByte, (int)EdmPrimitiveTypeKind.Int16] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.SByte, (int)EdmPrimitiveTypeKind.Int32] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.SByte, (int)EdmPrimitiveTypeKind.Int64] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.Int16, (int)EdmPrimitiveTypeKind.Int32] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.Int16, (int)EdmPrimitiveTypeKind.Int64] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.Int32, (int)EdmPrimitiveTypeKind.Int64] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.Single, (int)EdmPrimitiveTypeKind.Double] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.GeographyCollection, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyLineString, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyMultiLineString, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyMultiPoint, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyMultiPolygon, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyPoint, (int)EdmPrimitiveTypeKind.Geography] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeographyPolygon, (int)EdmPrimitiveTypeKind.Geography] = true;

            promotionMap[(int)EdmPrimitiveTypeKind.GeometryCollection, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryLineString, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryMultiLineString, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryMultiPoint, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryMultiPolygon, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryPoint, (int)EdmPrimitiveTypeKind.Geometry] = true;
            promotionMap[(int)EdmPrimitiveTypeKind.GeometryPolygon, (int)EdmPrimitiveTypeKind.Geometry] = true;

            return promotionMap;
        }
    }
}
