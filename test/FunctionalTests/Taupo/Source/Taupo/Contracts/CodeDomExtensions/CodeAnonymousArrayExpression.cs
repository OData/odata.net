//---------------------------------------------------------------------
// <copyright file="CodeAnonymousArrayExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Code expression for an anonymous array
    /// </summary>
    public class CodeAnonymousArrayExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeAnonymousArrayExpression"/> class.
        /// </summary>
        /// <param name="elements">The elements of the array.</param>
        internal CodeAnonymousArrayExpression(params CodeExpression[] elements)
        {
            ExceptionUtilities.CheckArgumentNotNull(elements, "elements");
            this.Elements = elements.AsEnumerable();
        }

        /// <summary>
        /// Gets the array element expressions.
        /// </summary>
        public IEnumerable<CodeExpression> Elements { get; private set; }
    }
}
