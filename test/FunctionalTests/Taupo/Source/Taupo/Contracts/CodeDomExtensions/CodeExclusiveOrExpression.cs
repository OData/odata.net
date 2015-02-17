//---------------------------------------------------------------------
// <copyright file="CodeExclusiveOrExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections.Generic;

    /// <summary>
    /// ExclusiveOr expression.
    /// </summary>
    public class CodeExclusiveOrExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the CodeExclusiveOrExpression class.
        /// </summary>
        /// <param name="left">The left expression.</param>
        /// <param name="right">The right expression.</param>
        public CodeExclusiveOrExpression(CodeExpression left, CodeExpression right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Gets the left operand of the expression.
        /// </summary>
        public CodeExpression Left { get; private set; }

        /// <summary>
        /// Gets the right operand of the expression.
        /// </summary>
        public CodeExpression Right { get; private set; }
    }
}