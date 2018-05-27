//---------------------------------------------------------------------
// <copyright file="ExpressionTypeChecker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Validation
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
        public static bool TryCast(this IEdmExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            return TryCast(expression, type, null, false, out discoveredErrors);
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
        public static bool TryCast(this IEdmExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            EdmUtil.CheckArgumentNull(expression, "expression");
            type = type.AsActualTypeReference();

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
                case EdmExpressionKind.DateTimeOffsetConstant:
                case EdmExpressionKind.DecimalConstant:
                case EdmExpressionKind.FloatingConstant:
                case EdmExpressionKind.GuidConstant:
                case EdmExpressionKind.DurationConstant:
                case EdmExpressionKind.DateConstant:
                case EdmExpressionKind.TimeOfDayConstant:
                    IEdmPrimitiveValue primitiveValue = (IEdmPrimitiveValue)expression;
                    if (primitiveValue.Type != null)
                    {
                        return TestTypeReferenceMatch(primitiveValue.Type, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryCastPrimitiveAsType(primitiveValue, type, out discoveredErrors);
                case EdmExpressionKind.Null:
                    return TryCastNullAsType((IEdmNullExpression)expression, type, out discoveredErrors);
                case EdmExpressionKind.Path:
                case EdmExpressionKind.PropertyPath:
                case EdmExpressionKind.NavigationPropertyPath:
                    return TryCastPathAsType((IEdmPathExpression)expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.FunctionApplication:
                    IEdmApplyExpression applyExpression = (IEdmApplyExpression)expression;
                    if (applyExpression.AppliedFunction != null)
                    {
                        IEdmOperation operation = applyExpression.AppliedFunction as IEdmOperation;
                        if (operation != null)
                        {
                            return TestTypeReferenceMatch(operation.ReturnType, type, expression.Location(), matchExactly, out discoveredErrors);
                        }
                    }

                    // If we don't have the applied function we just assume that it will work.
                    discoveredErrors = Enumerable.Empty<EdmError>();
                    return true;
                case EdmExpressionKind.If:
                    return TryCastIfAsType((IEdmIfExpression)expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.IsType:
                    return TestTypeReferenceMatch(EdmCoreModel.Instance.GetBoolean(false), type, expression.Location(), matchExactly, out discoveredErrors);
                case EdmExpressionKind.Record:
                    IEdmRecordExpression recordExpression = (IEdmRecordExpression)expression;
                    if (recordExpression.DeclaredType != null)
                    {
                        return TestTypeReferenceMatch(recordExpression.DeclaredType, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryCastRecordAsType(recordExpression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.Collection:
                    IEdmCollectionExpression collectionExpression = (IEdmCollectionExpression)expression;
                    if (collectionExpression.DeclaredType != null)
                    {
                        return TestTypeReferenceMatch(collectionExpression.DeclaredType, type, expression.Location(), matchExactly, out discoveredErrors);
                    }

                    return TryCastCollectionAsType(collectionExpression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.Labeled:
                    return TryCast(((IEdmLabeledExpression)expression).Expression, type, context, matchExactly, out discoveredErrors);
                case EdmExpressionKind.Cast:
                    return TestTypeReferenceMatch(((IEdmCastExpression)expression).Type, type, expression.Location(), matchExactly, out discoveredErrors);
                case EdmExpressionKind.LabeledExpressionReference:
                    return TryCast(((IEdmLabeledExpressionReferenceExpression)expression).ReferencedLabeledExpression, type, out discoveredErrors);
                case EdmExpressionKind.EnumMember:
                    return TryCastEnumConstantAsType((IEdmEnumMemberExpression)expression, type, matchExactly, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionNotValidForTheAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType) };
                    return false;
            }
        }

        internal static bool TryCastPrimitiveAsType(this IEdmPrimitiveValue expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsPrimitive())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.PrimitiveConstantExpressionNotValidForNonPrimitiveType, Edm.Strings.EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType) };
                return false;
            }

            switch (expression.ValueKind)
            {
                case EdmValueKind.Binary:
                    return TryCastBinaryConstantAsType((IEdmBinaryConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Boolean:
                    return TryCastBooleanConstantAsType((IEdmBooleanConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.DateTimeOffset:
                    return TryCastDateTimeOffsetConstantAsType((IEdmDateTimeOffsetConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Decimal:
                    return TryCastDecimalConstantAsType((IEdmDecimalConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Floating:
                    return TryCastFloatingConstantAsType((IEdmFloatingConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Guid:
                    return TryCastGuidConstantAsType((IEdmGuidConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Integer:
                    return TryCastIntegerConstantAsType((IEdmIntegerConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.String:
                    return TryCastStringConstantAsType((IEdmStringConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Duration:
                    return TryCastDurationConstantAsType((IEdmDurationConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.Date:
                    return TryCastDateConstantAsType((IEdmDateConstantExpression)expression, type, out discoveredErrors);
                case EdmValueKind.TimeOfDay:
                    return TryCastTimeOfDayConstantAsType((IEdmTimeOfDayConstantExpression)expression, type, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                    return false;
            }
        }

        internal static bool TryCastNullAsType(this IEdmNullExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsNullable)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.NullCannotBeAssertedToBeANonNullableType, Edm.Strings.EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        internal static bool TryCastPathAsType(this IEdmPathExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            IEdmStructuredType structuredContext = context as IEdmStructuredType;
            if (structuredContext != null)
            {
                IEdmType result = context;

                // [EdmLib] Need to handle paths that bind to things other than properties.
                foreach (string segment in expression.PathSegments)
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

        internal static bool TryCastIfAsType(this IEdmIfExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            IEnumerable<EdmError> ifTrueErrors;
            IEnumerable<EdmError> ifFalseErrors;
            bool success = TryCast(expression.TrueExpression, type, context, matchExactly, out ifTrueErrors);
            success = TryCast(expression.FalseExpression, type, context, matchExactly, out ifFalseErrors) && success;
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

        internal static bool TryCastRecordAsType(this IEdmRecordExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
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
                    if (!expressionProperty.Value.TryCast(typeProperty.Type, context, matchExactly, out recursiveErrors))
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

        internal static bool TryCastCollectionAsType(this IEdmCollectionExpression expression, IEdmTypeReference type, IEdmType context, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
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
                success = TryCast(element, collectionElementType, context, matchExactly, out recursiveErrors) && success;
                errors.AddRange(recursiveErrors);
            }

            discoveredErrors = errors;
            return success;
        }

        private static bool TryCastGuidConstantAsType(IEdmGuidConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsGuid())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastFloatingConstantAsType(IEdmFloatingConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsFloating())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastDecimalConstantAsType(IEdmDecimalConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDecimal())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastDateTimeOffsetConstantAsType(IEdmDateTimeOffsetConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDateTimeOffset())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastDurationConstantAsType(IEdmDurationConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDuration())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastDateConstantAsType(IEdmDateConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsDate())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastTimeOfDayConstantAsType(IEdmTimeOfDayConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsTimeOfDay())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastBooleanConstantAsType(IEdmBooleanConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsBoolean())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastStringConstantAsType(IEdmStringConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
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

        private static bool TryCastIntegerConstantAsType(IEdmIntegerConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsIntegral())
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                return false;
            }

            switch (type.PrimitiveKind())
            {
                case EdmPrimitiveTypeKind.Int64:
                    return TryCastIntegerConstantInRange(expression, Int64.MinValue, Int64.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Int32:
                    return TryCastIntegerConstantInRange(expression, Int32.MinValue, Int32.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Int16:
                    return TryCastIntegerConstantInRange(expression, Int16.MinValue, Int16.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.Byte:
                    return TryCastIntegerConstantInRange(expression, Byte.MinValue, Byte.MaxValue, out discoveredErrors);
                case EdmPrimitiveTypeKind.SByte:
                    return TryCastIntegerConstantInRange(expression, SByte.MinValue, SByte.MaxValue, out discoveredErrors);
                default:
                    discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.ExpressionPrimitiveKindNotValidForAssertedType, Edm.Strings.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType) };
                    return false;
            }
        }

        private static bool TryCastIntegerConstantInRange(IEdmIntegerConstantExpression expression, long min, long max, out IEnumerable<EdmError> discoveredErrors)
        {
            if (expression.Value < min || expression.Value > max)
            {
                discoveredErrors = new EdmError[] { new EdmError(expression.Location(), EdmErrorCode.IntegerConstantValueOutOfRange, Edm.Strings.EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange) };
                return false;
            }

            discoveredErrors = Enumerable.Empty<EdmError>();
            return true;
        }

        private static bool TryCastBinaryConstantAsType(IEdmBinaryConstantExpression expression, IEdmTypeReference type, out IEnumerable<EdmError> discoveredErrors)
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

        private static bool TryCastEnumConstantAsType(IEdmEnumMemberExpression expression, IEdmTypeReference type, bool matchExactly, out IEnumerable<EdmError> discoveredErrors)
        {
            if (!type.IsEnum())
            {
                discoveredErrors = new EdmError[]
                {
                    new EdmError(
                        expression.Location(),
                        EdmErrorCode.ExpressionEnumKindNotValidForAssertedType,
                        Edm.Strings.EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType)
                };
                return false;
            }

            foreach (var member in expression.EnumMembers)
            {
                if (!TestTypeMatch(member.DeclaringType, type.Definition, expression.Location(), matchExactly, out discoveredErrors))
                {
                    return false;
                }
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
