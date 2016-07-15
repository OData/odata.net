//---------------------------------------------------------------------
// <copyright file="EdmNavigationPropertyPathExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
    /// <summary>
    /// Represents an EDM navigation property path expression.
    /// </summary>
    public class EdmNavigationPropertyPathExpression : EdmPathExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNavigationPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="path">Path string containing segments separated by '/'. For example: "A.B/C/D.E/Func1(NS.T,NS.T2)/P1".</param>
        public EdmNavigationPropertyPathExpression(string path)
            : base(path)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNavigationPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmNavigationPropertyPathExpression(params string[] pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNavigationPropertyPathExpression"/> class.
        /// </summary>
        /// <param name="pathSegments">Path segments.</param>
        public EdmNavigationPropertyPathExpression(IEnumerable<string> pathSegments)
            : base(pathSegments)
        {
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.NavigationPropertyPath; }
        }
    }
}
