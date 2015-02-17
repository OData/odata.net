//---------------------------------------------------------------------
// <copyright file="CodeTypeOperationExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents the Type operators in CLR
    /// </summary>
    public abstract class CodeTypeOperationExpression : CodeExpression
    {
        /// <summary>
        /// Initializes a new instance of the CodeTypeOperationExpression class.
        /// </summary>
        /// <param name="source">The source expression.</param>
        /// <param name="targetType">The target type reference expression.</param>
        protected CodeTypeOperationExpression(CodeExpression source, CodeTypeReferenceExpression targetType)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");
            ExceptionUtilities.CheckArgumentNotNull(targetType, "targetType");
            this.Source = source;
            this.TargetType = targetType;
        }

        /// <summary>
        /// Gets the source expression.
        /// </summary>
        /// <value>The source expression.</value>
        public CodeExpression Source { get; private set; }

        /// <summary>
        /// Gets the target type referece expression.
        /// </summary>
        /// <value>The type reference expression.</value>
        public CodeTypeReferenceExpression TargetType { get; private set; }
    }
}
