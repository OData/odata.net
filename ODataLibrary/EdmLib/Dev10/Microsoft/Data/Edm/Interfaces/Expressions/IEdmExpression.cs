//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.Edm.Expressions
{
    /// <summary>
    /// Defines EDM expression kinds.
    /// </summary>
    public enum EdmExpressionKind
    {
        /// <summary>
        /// Represents an expression with unknown or error kind.
        /// </summary>
        None,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmStringConstantExpression"/>.
        /// </summary>
        StringConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIntegerConstantExpression"/>.
        /// </summary>
        IntegerConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmFloatingConstantExpression"/>.
        /// </summary>
        FloatingConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDecimalConstantExpression"/>.
        /// </summary>
        DecimalConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmBooleanConstantExpression"/>.
        /// </summary>
        BooleanConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmDateTimeConstantExpression"/>.
        /// </summary>
        DateTimeConstant,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmRecordExpression"/>.
        /// </summary>
        Record,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmMultiValueExpression"/>.
        /// </summary>
        MultiValue,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPathExpression"/>.
        /// </summary>
        Path,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmParameterReferenceExpression"/>.
        /// </summary>
        ParameterReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmFunctionReferenceExpression"/>.
        /// </summary>
        FunctionReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmPropertyReferenceExpression"/>.
        /// </summary>
        PropertyReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmValueTermReferenceExpression"/>.
        /// </summary>
        TermPropertyReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmEntitySetReferenceExpression"/>.
        /// </summary>
        EntitySetReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmEnumConstantReferenceExpression"/>.
        /// </summary>
        EnumConstantReference,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIfExpression"/>.
        /// </summary>
        If,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmAssertTypeExpression"/>.
        /// </summary>
        AssertType,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmIsTypeExpression"/>.
        /// </summary>
        IsType,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmAnonymousFunctionExpression"/>.
        /// </summary>
        AnonymousFunction,
        /// <summary>
        /// Represents an expression implementing <see cref="IEdmFunctionApplicationExpression"/>.
        /// </summary>
        FunctionApplication
    }

    /// <summary>
    /// Represents an EDM expression.
    /// </summary>
    public interface IEdmExpression
    {
        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        EdmExpressionKind ExpressionKind { get; }
    }
}
