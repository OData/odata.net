//---------------------------------------------------------------------
// <copyright file="EdmPropertyPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM property path expression.
    /// </summary>
    public class EdmPropertyPathExpression : EdmPathExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments seperated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmPropertyPathExpression(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmPropertyPathExpression(params string[] pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmPropertyPathExpression(IEnumerable<string> pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.PropertyPath; }
        }
    }
}
