//---------------------------------------------------------------------
// <copyright file="CodeExpressionWithFreeVariables.cs" company="Microsoft">
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
    /// Expression along with the free variables bound to it.
    /// </summary>
    public class CodeExpressionWithFreeVariables
    {
        /// <summary>
        /// Initializes a new instance of the CodeExpressionWithFreeVariables class.
        /// </summary>
        /// <param name="expression">Expression that has free variables.</param>
        /// <param name="freeVariables">Free variables bound to the query.</param>
        public CodeExpressionWithFreeVariables(CodeExpression expression, IEnumerable<CodeFreeVariable> freeVariables)
        {
            ExceptionUtilities.CheckArgumentNotNull(expression, "expression");
            ExceptionUtilities.CheckArgumentNotNull(freeVariables, "freeVariables");

            this.Expression = expression;
            this.FreeVariables = freeVariables.ToList().AsReadOnly();
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public CodeExpression Expression { get; private set; }

        /// <summary>
        /// Gets free variables bound to the expression.
        /// </summary>
        public ReadOnlyCollection<CodeFreeVariable> FreeVariables { get; private set; }
    }
}