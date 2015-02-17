//---------------------------------------------------------------------
// <copyright file="CodeAsExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.CodeDomExtensions
{
    using System.CodeDom;

    /// <summary>
    /// Represents the Type As operator in CLR
    /// </summary>
    public class CodeAsExpression : CodeTypeOperationExpression
    {
        /// <summary>
        /// Initializes a new instance of the CodeAsExpression class.
        /// </summary>
        /// <param name="source">The source expression.</param>
        /// <param name="targetType">The target type reference expression.</param>
        public CodeAsExpression(CodeExpression source, CodeTypeReferenceExpression targetType)
            : base(source, targetType)
        {
        }
    }
}