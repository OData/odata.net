//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM multi-value construction expression.
    /// </summary>
    public class EdmCollectionExpression : EdmElement, IEdmCollectionExpression
    {
        private readonly IEdmTypeReference declaredType;
        private readonly IEnumerable<IEdmExpression> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(params IEdmExpression[] elements)
            : this((IEnumerable<IEdmExpression>)elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the collection.</param>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEdmTypeReference declaredType, params IEdmExpression[] elements)
            : this(declaredType, (IEnumerable<IEdmExpression>)elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEnumerable<IEdmExpression> elements)
            : this(null, elements)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the collection.</param>
        /// <param name="elements">The constructed element values.</param>
        public EdmCollectionExpression(IEdmTypeReference declaredType, IEnumerable<IEdmExpression> elements)
        {
            EdmUtil.CheckArgumentNull(elements, "elements");

            this.declaredType = declaredType;
            this.elements = elements;
        }

        /// <summary>
        /// Gets the declared type of the collection.
        /// </summary>
        public IEdmTypeReference DeclaredType
        {
            get { return this.declaredType; }
        }

        /// <summary>
        /// Gets the constructed element values.
        /// </summary>
        public IEnumerable<IEdmExpression> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Collection; }
        }
    }
}
