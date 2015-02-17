//---------------------------------------------------------------------
// <copyright file="CodeArrayInitializerExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents array initializer expression "{ a, b, c }" in C#" usable in conjunction with
    /// <see cref="CodeCreateAndInitializeObjectExpression" />
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class CodeArrayInitializerExpression : CodeExpression, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the CodeArrayInitializerExpression class.
        /// </summary>
        public CodeArrayInitializerExpression()
        {
            this.Values = new CodeExpressionCollection();
        }

        /// <summary>
        /// Gets the collection of values.
        /// </summary>
        /// <value>The values.</value>
        public CodeExpressionCollection Values { get; private set; }

        /// <summary>
        /// Adds the specified expression to the collection of values.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Add(CodeExpression expression)
        {
            this.Values.Add(expression);
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}