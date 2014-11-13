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

using System;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents a value of an EDM property.
    /// </summary>
    public class EdmPropertyValue : IEdmPropertyValue
    {
        private readonly string name;
        private IEdmValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyValue"/> class with non-initialized <see cref="Value"/> property.
        /// This constructor allows setting <see cref="Value"/> property once after <see cref="EdmPropertyValue"/> has been constructed.
        /// </summary>
        /// <param name="name">Name of the property for which this provides a value.</param>
        public EdmPropertyValue(string name)
        {
            EdmUtil.CheckArgumentNull(name, "name");
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmPropertyValue"/> class.
        /// This constructor will not allow changing <see cref="Value"/> property after the EdmPropertyValue instance has been constructed.
        /// </summary>
        /// <param name="name">Name of the property for which this provides a value.</param>
        /// <param name="value">Value of the property.</param>
        public EdmPropertyValue(string name, IEdmValue value)
        {
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(value, "value");

            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Gets the name of the property for which this provides a value.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets or sets the value of the property.
        /// The value can be initialized only once either using the <see cref="EdmPropertyValue(string,IEdmValue)"/> constructor or by assigning value directly to this property.
        /// </summary>
        public IEdmValue Value
        {
            get
            {
                return this.value;
            }

            set
            {
                EdmUtil.CheckArgumentNull(value, "value");

                if (this.value != null)
                {
                    throw new InvalidOperationException(Strings.ValueHasAlreadyBeenSet);
                }

                this.value = value;
            }
        }
    }
}
