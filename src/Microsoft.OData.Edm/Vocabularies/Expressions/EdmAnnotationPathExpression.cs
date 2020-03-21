//---------------------------------------------------------------------
// <copyright file="EdmAnnotationPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM annotation path expression.
    /// </summary>
    public class EdmAnnotationPathExpression : EdmPathExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotationPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments separated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmAnnotationPathExpression(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmAnnotationPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmAnnotationPathExpression(params string[] pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmAnnotationPathExpression(IEnumerable<string> pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.AnnotationPath; }
        }
    }
}
