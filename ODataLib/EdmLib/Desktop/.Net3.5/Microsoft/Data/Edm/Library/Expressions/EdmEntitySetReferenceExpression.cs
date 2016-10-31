//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM entity set reference expression.
    /// </summary>
    public class EdmEntitySetReferenceExpression : EdmElement, IEdmEntitySetReferenceExpression
    {
        private readonly IEdmEntitySet referencedEntitySet;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySetReferenceExpression"/> class.
        /// </summary>
        /// <param name="referencedEntitySet">Referenced entity set.</param>
        public EdmEntitySetReferenceExpression(IEdmEntitySet referencedEntitySet)
        {
            EdmUtil.CheckArgumentNull(referencedEntitySet, "referencedEntitySet");
            this.referencedEntitySet = referencedEntitySet;
        }

        /// <summary>
        /// Gets the referenced entity set.
        /// </summary>
        public IEdmEntitySet ReferencedEntitySet
        {
            get { return this.referencedEntitySet; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.EntitySetReference; }
        }
    }
}
