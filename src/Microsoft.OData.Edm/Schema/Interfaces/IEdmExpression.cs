//---------------------------------------------------------------------
// <copyright file="IEdmExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Defines EDM expression kinds.
    /// </summary>
    public enum EdmExpressionKind
    {
        /// <summary>
        /// Represents an expression with unknown or error kind.
        /// </summary>
        None = 0,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmBinaryConstantExpression"/>.
        /// </summary>
        BinaryConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmBooleanConstantExpression"/>.
        /// </summary>
        BooleanConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDateTimeOffsetConstantExpression"/>.
        /// </summary>
        DateTimeOffsetConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDecimalConstantExpression"/>.
        /// </summary>
        DecimalConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmFloatingConstantExpression"/>.
        /// </summary>
        FloatingConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmGuidConstantExpression"/>.
        /// </summary>
        GuidConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIntegerConstantExpression"/>.
        /// </summary>
        IntegerConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmStringConstantExpression"/>.
        /// </summary>
        StringConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDurationConstantExpression"/>.
        /// </summary>
        DurationConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmNullExpression"/>.
        /// </summary>
        Null,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmRecordExpression"/>.
        /// </summary>
        Record,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmCollectionExpression"/>.
        /// </summary>
        Collection,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPathExpression"/>.
        /// </summary>
        Path,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIfExpression"/>.
        /// </summary>
        If,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmCastExpression"/>.
        /// </summary>
        Cast,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIsTypeExpression"/>.
        /// </summary>
        IsType,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmApplyExpression"/>.
        /// </summary>
        FunctionApplication,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmLabeledExpressionReferenceExpression"/>.
        /// </summary>
        LabeledExpressionReference,

        /// <summary>
        /// Represents an expression implementing <see cref=" IEdmLabeledExpression"/>
        /// </summary>
        Labeled,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPathExpression"/>.
        /// </summary>
        PropertyPath,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPathExpression"/>.
        /// </summary>
        NavigationPropertyPath,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDateConstantExpression"/>.
        /// </summary>
        DateConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmTimeOfDayConstantExpression"/>.
        /// </summary>
        TimeOfDayConstant,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmEnumMemberExpression"/>.
        /// </summary>
        EnumMember,

        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPathExpression"/>.
        /// </summary>
        AnnotationPath
    }

    /// <summary>
    /// Represents an EDM expression.
    /// </summary>
    public interface IEdmExpression : IEdmElement
    {
        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        EdmExpressionKind ExpressionKind { get; }
    }
}
