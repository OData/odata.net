//---------------------------------------------------------------------
// <copyright file="CodeTernaryExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Code expression representing the ternary or conditional operator. In C# it is written as: (condition ? ifTrue : ifFalse)
    /// </summary>
    public class CodeTernaryExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeTernaryExpression"/> class.
        /// </summary>
        /// <param name="condition">An expression for the condition to test for.</param>
        /// <param name="ifTrue">An expression for the value if condition is true.</param>
        /// <param name="ifFalse">An expression for the value if condition is false.</param>
        public CodeTernaryExpression(CodeExpression condition, CodeExpression ifTrue, CodeExpression ifFalse)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(condition, "condition");
            ExceptionUtilities.CheckArgumentNotNull(ifTrue, "ifTrue");
            ExceptionUtilities.CheckArgumentNotNull(ifFalse, "ifFalse");

            this.Condition = condition;
            this.IfTrue = ifTrue;
            this.IfFalse = ifFalse;
        }

        /// <summary>
        /// Gets the expression for the condition.
        /// </summary>
        public CodeExpression Condition { get; private set; }

        /// <summary>
        /// Gets the expression for the true side of the conditional.
        /// </summary>
        public CodeExpression IfTrue { get; private set; }

        /// <summary>
        /// Gets expression for the false side of the conditional.
        /// </summary>
        public CodeExpression IfFalse { get; private set; }
    }
}