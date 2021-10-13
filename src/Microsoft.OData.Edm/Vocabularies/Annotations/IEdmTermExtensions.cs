//---------------------------------------------------------------------
// <copyright file="IEdmTermExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Edm.Csdl;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Contains extension methods for <see cref="IEdmTerm"/> interfaces.
    /// </summary>
    public static class IEdmTermExtensions
    {
        /// <summary>
        /// Creates <see cref="IEdmVocabularyAnnotation"/> using the <see cref="IEdmTerm"/> and its default value.
        /// </summary>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="target">Element the annotation applies to.</param>
        /// <returns>The <see cref="IEdmVocabularyAnnotation"/> built.</returns>
        public static IEdmVocabularyAnnotation CreateVocabularyAnnotation(this IEdmTerm term, IEdmVocabularyAnnotatable target)
        {
            return term.CreateVocabularyAnnotation(target, null);
        }

        /// <summary>
        /// Creates <see cref="IEdmVocabularyAnnotation"/> using the <see cref="IEdmTerm"/> and its default value.
        /// </summary>
        /// <param name="term">Term bound by the annotation.</param>
        /// <param name="target">Element the annotation applies to.</param>
        /// <param name="qualifier">Qualifier used to discriminate between multiple bindings of the same property or type.</param>
        /// <returns>The <see cref="IEdmVocabularyAnnotation"/> built.</returns>
        public static IEdmVocabularyAnnotation CreateVocabularyAnnotation(this IEdmTerm term, IEdmVocabularyAnnotatable target, string qualifier)
        {
            EdmUtil.CheckArgumentNull(term, "term");
            EdmUtil.CheckArgumentNull(target, "target");

            IEdmExpression value = term.GetDefaultValueExpression();
            if (value == null)
            {
                throw new InvalidOperationException(Strings.EdmVocabularyAnnotations_DidNotFindDefaultValue(term.Type));
            }

            return new EdmVocabularyAnnotation(target, term, qualifier, value)
            {
                UseDefault = true
            };
        }

        /// <summary>
        /// Gets the default value expression for the given <see cref="IEdmTerm"/>.
        /// </summary>
        /// <param name="term">The given term.</param>
        /// <returns>Null or the build Edm value expression.</returns>
        internal static IEdmExpression GetDefaultValueExpression(this IEdmTerm term)
        {
            EdmUtil.CheckArgumentNull(term, "term");

            if (string.IsNullOrEmpty(term.DefaultValue))
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
        internal static IEdmExpression BuildEdmExpression(IEdmType edmType, string value)
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
    }
}
