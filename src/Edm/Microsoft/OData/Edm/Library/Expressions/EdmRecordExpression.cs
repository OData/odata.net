//   OData .NET Libraries ver. 6.9
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

using System.Collections.Generic;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Expressions
{
    /// <summary>
    /// Represents an EDM record construction expression.
    /// </summary>
    public class EdmRecordExpression : EdmElement, IEdmRecordExpression
    {
        private readonly IEdmStructuredTypeReference declaredType;
        private readonly IEnumerable<IEdmPropertyConstructor> properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(params IEdmPropertyConstructor[] properties)
            : this((IEnumerable<IEdmPropertyConstructor>)properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Declared type of the record.</param>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEdmStructuredTypeReference declaredType, params IEdmPropertyConstructor[] properties)
            : this(declaredType, (IEnumerable<IEdmPropertyConstructor>)properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEnumerable<IEdmPropertyConstructor> properties)
            : this(null, properties)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRecordExpression"/> class.
        /// </summary>
        /// <param name="declaredType">Optional declared type of the record.</param>
        /// <param name="properties">Property constructors.</param>
        public EdmRecordExpression(IEdmStructuredTypeReference declaredType, IEnumerable<IEdmPropertyConstructor> properties)
        {
            EdmUtil.CheckArgumentNull(properties, "properties");

            this.declaredType = declaredType;
            this.properties = properties;
        }

        /// <summary>
        /// Gets the declared type of the record, or null if there is no declared type.
        /// </summary>
        public IEdmStructuredTypeReference DeclaredType
        {
            get { return this.declaredType; }
        }

        /// <summary>
        /// Gets the constructed property values.
        /// </summary>
        public IEnumerable<IEdmPropertyConstructor> Properties
        {
            get { return this.properties; }
        }

        /// <summary>
        /// Gets the kind of this expression.
        /// </summary>
        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Record; }
        }
    }
}
