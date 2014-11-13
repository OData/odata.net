//   OData .NET Libraries ver. 6.8.1
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

using Microsoft.OData.Edm.Annotations;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Library.Annotations
{
    /// <summary>
    /// Represents a property binding specified as part of an EDM type annotation.
    /// </summary>
    public class EdmPropertyValueBinding : EdmElement, IEdmPropertyValueBinding
    {
        private readonly IEdmProperty boundProperty;
        private readonly IEdmExpression value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyValueBinding"/> class.
        /// </summary>
        /// <param name="boundProperty">Property that is given a value by the annotation.</param>
        /// <param name="value">Expression producing the value of the annotation.</param>
        public EdmPropertyValueBinding(IEdmProperty boundProperty, IEdmExpression value)
        {
            EdmUtil.CheckArgumentNull(boundProperty, "boundProperty");
            EdmUtil.CheckArgumentNull(value, "value");

            this.boundProperty = boundProperty;
            this.value = value;
        }

        /// <summary>
        /// Gets the property that is given a value by the annotation.
        /// </summary>
        public IEdmProperty BoundProperty
        {
            get { return this.boundProperty; }
        }

        /// <summary>
        /// Gets the expression producing the value of the annotation.
        /// </summary>
        public IEdmExpression Value
        {
            get { return this.value; }
        }
    }
}
