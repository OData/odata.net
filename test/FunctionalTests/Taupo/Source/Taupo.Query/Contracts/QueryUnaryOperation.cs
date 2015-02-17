//---------------------------------------------------------------------
// <copyright file="QueryUnaryOperation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    /// <summary>
    /// Query unary operations
    /// </summary>
    public enum QueryUnaryOperation
    {
        /// <summary>
        /// Bitwise: not
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// Logical: negate
        /// </summary>
        LogicalNegate,

        /// <summary>
        /// Negate operation
        /// </summary>
        Negate,
    }
}
