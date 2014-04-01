//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
