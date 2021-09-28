//---------------------------------------------------------------------
// <copyright file="IEdmTermExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmTerm"/> interfaces.
    /// </summary>
    internal static class IEdmTermExtensions
    {
        /// <summary>
        /// Gets the default value expression for the given <see cref="IEdmTerm"/>.
        /// </summary>
        /// <param name="term">The given term.</param>
        /// <returns>Null or the build Edm value expression.</returns>
        public static IEdmExpression GetDefaultValueExpression(this IEdmTerm term)
        {
            EdmUtil.CheckArgumentNull(term, "term");

            if (term.DefaultValue == null)
            {
                return null;
            }

            return BuildEdmExpression(term.Type.Definition, term.DefaultValue);
        }

        /// <summary>
        /// Parses a <paramref name="value"/> into an <see cref="IEdmExpression"/> value of the correct EDM type.
        /// </summary>
        /// <param name="edmType">The type of value.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// An IEdmExpression of type <paramref name="edmType" /> or null if the type is not supported/implemented.
        /// </returns>
        public static IEdmExpression BuildEdmExpression(IEdmType edmType, string value)
        {
            EdmUtil.CheckArgumentNull(edmType, "edmType");
            EdmUtil.CheckArgumentNull(value, "value");

            EdmTypeKind termTypeKind = edmType.TypeKind;

            // Create expressions/constants for the corresponding types
            switch (termTypeKind)
            {
                case EdmTypeKind.Primitive:
                    IEdmPrimitiveType primitiveTypeReference = (IEdmPrimitiveType)edmType;
                    return BuildEdmPrimitiveValueExp(primitiveTypeReference, value);

                case EdmTypeKind.TypeDefinition:
                    IEdmTypeDefinition typeDefinitionReference = (IEdmTypeDefinition)edmType;
                    return BuildEdmPrimitiveValueExp(typeDefinitionReference.UnderlyingType(), value);

                case EdmTypeKind.Path:
                    return BuildEdmPathExp((IEdmPathType)edmType, value);

                case EdmTypeKind.Enum:
                case EdmTypeKind.Complex:
                case EdmTypeKind.Entity:
                case EdmTypeKind.Collection:
                case EdmTypeKind.EntityReference:
                case EdmTypeKind.Untyped:
                default:
                    throw new NotSupportedException(Strings.EdmVocabularyAnnotations_TermTypeNotSupported(edmType.FullTypeName()));
            }
        }

        /// <summary>
        /// Returns an IEdmExpression for an EDM primitive value.
        /// </summary>
        /// <param name="typeReference">Reference to the type of term.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// Primitive expression for the value of the according type or null if the type is not supported/implemented.
        /// </returns>
        private static IEdmExpression BuildEdmPrimitiveValueExp(IEdmPrimitiveType typeReference, string value)
        {
            // From OData spec, the following primitive types are supported as Constant Expression
            switch (typeReference.PrimitiveKind)
            {
                case EdmPrimitiveTypeKind.Binary:
                    if (EdmValueParser.TryParseBinary(value, out byte[] binary))
                    {
                        return new EdmBinaryConstant(binary);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidBinary(value));

                case EdmPrimitiveTypeKind.Boolean:
                    if (EdmValueParser.TryParseBool(value, out bool? bl))
                    {
                        return new EdmBooleanConstant(bl.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidBoolean(value));

                case EdmPrimitiveTypeKind.Date:
                    if (EdmValueParser.TryParseDate(value, out Date? dt))
                    {
                        return new EdmDateConstant(dt.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidDate(value));

                case EdmPrimitiveTypeKind.DateTimeOffset:
                    if (EdmValueParser.TryParseDateTimeOffset(value, out DateTimeOffset? dto))
                    {
                        return new EdmDateTimeOffsetConstant(dto.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidDateTimeOffset(value));

                case EdmPrimitiveTypeKind.Decimal:
                    if (EdmValueParser.TryParseDecimal(value, out decimal? dec))
                    {
                        return new EdmDecimalConstant(dec.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidDecimal(value));

                case EdmPrimitiveTypeKind.Duration:
                    if (EdmValueParser.TryParseDuration(value, out TimeSpan? ts))
                    {
                        return new EdmDurationConstant(ts.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidDuration(value));

                case EdmPrimitiveTypeKind.Single:
                case EdmPrimitiveTypeKind.Double:
                    if (EdmValueParser.TryParseFloat(value, out double? dbl))
                    {
                        return new EdmFloatingConstant(dbl.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidFloatingPoint(value));

                case EdmPrimitiveTypeKind.Guid:
                    if (EdmValueParser.TryParseGuid(value, out Guid? gd))
                    {
                        return new EdmGuidConstant(gd.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidGuid(value));

                case EdmPrimitiveTypeKind.Int16:
                case EdmPrimitiveTypeKind.Int32:
                    if (EdmValueParser.TryParseInt(value, out int? intNum))
                    {
                        return new EdmIntegerConstant(intNum.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidInteger(value));

                case EdmPrimitiveTypeKind.Int64:
                    if (EdmValueParser.TryParseLong(value, out long? longNum))
                    {
                        return new EdmIntegerConstant(longNum.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidLong(value));

                case EdmPrimitiveTypeKind.String:
                    return new EdmStringConstant(value);

                case EdmPrimitiveTypeKind.TimeOfDay:
                    if (EdmValueParser.TryParseTimeOfDay(value, out TimeOfDay? tod))
                    {
                        return new EdmTimeOfDayConstant(tod.Value);
                    }

                    throw new FormatException(Strings.ValueParser_InvalidTimeOfDay(value));
            }

            throw new NotSupportedException(Strings.EdmVocabularyAnnotations_TermTypeNotSupported(typeReference.FullName()));
        }

        /// <summary>
        /// Returns an IEdmExpression for an EDM path type.
        /// </summary>
        /// <param name="pathType">Reference to the type of term.</param>
        /// <param name="value">The value to parse.</param>
        /// <returns>
        /// Expression for the value.
        /// </returns>
        private static IEdmExpression BuildEdmPathExp(IEdmPathType pathType, string value)
        {
            switch (pathType.PathKind)
            {
                case EdmPathTypeKind.AnnotationPath:
                    return new EdmAnnotationPathExpression(value);

                case EdmPathTypeKind.PropertyPath:
                    return new EdmPropertyPathExpression(value);

                case EdmPathTypeKind.NavigationPropertyPath:
                    return new EdmNavigationPropertyPathExpression(value);

                default:
                    return new EdmPathExpression(value);
            }
        }

        public static bool IsEqual(this IEdmExpression left, IEdmExpression right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            else if (left == null || right == null)
            {
                return false;
            }

            // now, we have "left != null and right != null"
            if (left.ExpressionKind != right.ExpressionKind)
            {
                return false;
            }

            switch (left.ExpressionKind)
            {
                case EdmExpressionKind.BinaryConstant:
                    IEdmBinaryConstantExpression leftBinary = (IEdmBinaryConstantExpression)left;
                    IEdmBinaryConstantExpression rightBinary = (IEdmBinaryConstantExpression)right;
                    return leftBinary.Value.SequenceEqual(rightBinary.Value);

                case EdmExpressionKind.BooleanConstant:
                    IEdmBooleanConstantExpression leftBoolean = (IEdmBooleanConstantExpression)left;
                    IEdmBooleanConstantExpression rightBoolean = (IEdmBooleanConstantExpression)right;
                    return leftBoolean.Value == rightBoolean.Value;

                case EdmExpressionKind.DateTimeOffsetConstant:
                    IEdmDateTimeOffsetConstantExpression leftDateTimeOffset = (IEdmDateTimeOffsetConstantExpression)left;
                    IEdmDateTimeOffsetConstantExpression rightDateTimeOffset = (IEdmDateTimeOffsetConstantExpression)right;
                    return leftDateTimeOffset.Value == rightDateTimeOffset.Value;

                case EdmExpressionKind.DecimalConstant:
                    IEdmDecimalConstantExpression leftDecimal = (IEdmDecimalConstantExpression)left;
                    IEdmDecimalConstantExpression rightDecimal = (IEdmDecimalConstantExpression)right;
                    return leftDecimal.Value == rightDecimal.Value;

                case EdmExpressionKind.FloatingConstant:
                    IEdmFloatingConstantExpression leftFloating = (IEdmFloatingConstantExpression)left;
                    IEdmFloatingConstantExpression rightFloating = (IEdmFloatingConstantExpression)right;
                    return leftFloating.Value.Equals(rightFloating.Value);

                case EdmExpressionKind.GuidConstant:
                    IEdmGuidConstantExpression leftGuid = (IEdmGuidConstantExpression)left;
                    IEdmGuidConstantExpression rightGuid = (IEdmGuidConstantExpression)right;
                    return leftGuid.Value == rightGuid.Value;

                case EdmExpressionKind.IntegerConstant:
                    IEdmIntegerConstantExpression leftInteger = (IEdmIntegerConstantExpression)left;
                    IEdmIntegerConstantExpression rightInteger = (IEdmIntegerConstantExpression)right;
                    return leftInteger.Value == rightInteger.Value;

                case EdmExpressionKind.Path:
                case EdmExpressionKind.PropertyPath:
                case EdmExpressionKind.NavigationPropertyPath:
                case EdmExpressionKind.AnnotationPath:
                    IEdmPathExpression leftPath = (IEdmPathExpression)left;
                    IEdmPathExpression rightPath = (IEdmPathExpression)right;
                    return leftPath.Path == rightPath.Path;

                case EdmExpressionKind.StringConstant:
                    IEdmStringConstantExpression leftString = (IEdmStringConstantExpression)left;
                    IEdmStringConstantExpression rightString = (IEdmStringConstantExpression)right;
                    return leftString.Value == rightString.Value;

                case EdmExpressionKind.DurationConstant:
                    IEdmDurationConstantExpression leftDuration = (IEdmDurationConstantExpression)left;
                    IEdmDurationConstantExpression rightDuration = (IEdmDurationConstantExpression)right;
                    return leftDuration.Value == rightDuration.Value;

                case EdmExpressionKind.DateConstant:
                    IEdmDateConstantExpression leftDate = (IEdmDateConstantExpression)left;
                    IEdmDateConstantExpression rightDate = (IEdmDateConstantExpression)right;
                    return leftDate.Value == rightDate.Value;

                case EdmExpressionKind.TimeOfDayConstant:
                    IEdmTimeOfDayConstantExpression leftTimeOfDay = (IEdmTimeOfDayConstantExpression)left;
                    IEdmTimeOfDayConstantExpression rightTimeOfDay = (IEdmTimeOfDayConstantExpression)right;
                    return leftTimeOfDay.Value == rightTimeOfDay.Value;

                default:
                    break;
            }

            return false;
        }
    }
}
