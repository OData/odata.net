//---------------------------------------------------------------------
// <copyright file="CodeLambdaExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;

    /// <summary>
    /// Lambda expression.
    /// </summary>
    public class CodeLambdaExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the CodeLambdaExpression class.
        /// </summary>
        public CodeLambdaExpression()
        {
            this.Parameters = new CodeParameterDeclarationExpressionCollection();
            this.BodyStatements = new CodeStatementCollection();
            this.HasReturnValue = true;
        }

        /// <summary>
        /// Gets the lambda parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public CodeParameterDeclarationExpressionCollection Parameters { get; private set; }

        /// <summary>
        /// Gets or sets the lambda expression body.
        /// </summary>
        /// <value>The expression body.</value>
        public CodeExpression Body { get; set; }

        /// <summary>
        /// Gets the lambda body statements.
        /// </summary>
        /// <value>The body statements.</value>
        public CodeStatementCollection BodyStatements { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CodeLambdaExpression"/> has a return value.
        /// </summary>
        public bool HasReturnValue { get; set; }
    }
}