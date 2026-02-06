//---------------------------------------------------------------------
// <copyright file="EdmBinaryOperatorKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Enumeration of binary operators.
    /// </summary>
    public enum EdmBinaryOperatorKind
    {
        /// <summary>
        /// The logical or operator.
        /// </summary>
        Or = 0,

        /// <summary>
        /// The logical and operator.
        /// </summary>
        And = 1,

        /// <summary>
        /// The Equal operator.
        /// </summary>
        Eq = 2,

        /// <summary>
        /// The Not equal operator.
        /// </summary>
        Ne = 3,

        /// <summary>
        /// The Greater than operator.
        /// </summary>
        Gt = 4,

        /// <summary>
        /// The Greater than or equal operator.
        /// </summary>
        Ge = 5,

        /// <summary>
        /// The Less than operator.
        /// </summary>
        Lt = 6,

        /// <summary>
        /// The Less than or equal operator.
        /// </summary>
        Le = 7,

        /// <summary>
        /// The Addition operator.
        /// </summary>
        Add = 8,

        /// <summary>
        /// The Subtraction operator.
        /// </summary>
        Sub = 9,

        /// <summary>
        /// The Multiplication operator.
        /// </summary>
        Mul = 10,

        /// <summary>
        /// The Division operator. With integer result for integer operands
        /// </summary>
        Div = 11,

        /// <summary>
        /// The Division operator. With fractional result also for integer operands)
        /// </summary>
        DivBy = 12,

        /// <summary>
        /// The Modulo operator.
        /// </summary>
        Mod = 13,

        /// <summary>
        /// The has operator.
        /// </summary>
        Has = 14,

        /// <summary>
        /// The in operator.
        /// </summary>
        In = 15
    }

    internal static class EdmBinaryOperatorKindExtensions
    {
        public static string ToJsonName(this EdmBinaryOperatorKind kind)
        {
            return $"${kind}";
        }

        public static string ToXmlName(this EdmBinaryOperatorKind kind)
        {
            return $"{kind}";
        }
    }
}
