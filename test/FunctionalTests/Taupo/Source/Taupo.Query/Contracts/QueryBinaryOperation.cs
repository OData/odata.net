//---------------------------------------------------------------------
// <copyright file="QueryBinaryOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    /// <summary>
    /// Query binary operations
    /// </summary>
    public enum QueryBinaryOperation
    {
        /// <summary>
        /// String concatenation
        /// </summary>
        Concat,

        /// <summary>
        /// Arithmetic: Add
        /// </summary>
        Add,

        /// <summary>
        /// Arithmetic: Subtract
        /// </summary>
        Subtract,

        /// <summary>
        /// Arithmetic: Multiply
        /// </summary>
        Multiply,

        /// <summary>
        /// Arithmetic: Divide
        /// </summary>
        Divide,

        /// <summary>
        /// Arithmetic: Modulo
        /// </summary>
        Modulo,

        /// <summary>
        /// Bitwise: and
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// Bitwise: or
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// Bitwise: exclusive or
        /// </summary>
        BitwiseExclusiveOr,

        /// <summary>
        /// Logical: and
        /// </summary>
        LogicalAnd,

        /// <summary>
        /// Logical: or
        /// </summary>
        LogicalOr,

        /// <summary>
        /// Comparison: EqualTo
        /// </summary>
        EqualTo,

        /// <summary>
        /// Comparison: NotEqualTo
        /// </summary>
        NotEqualTo,

        /// <summary>
        /// Comparison: LessThan
        /// </summary>
        LessThan,

        /// <summary>
        /// Comparison: GreaterThan
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Comparison: LessThanOrEqualTo
        /// </summary>
        LessThanOrEqualTo,

        /// <summary>
        /// Comparison: GreaterThanOrEqualTo
        /// </summary>
        GreaterThanOrEqualTo,
    }
}
