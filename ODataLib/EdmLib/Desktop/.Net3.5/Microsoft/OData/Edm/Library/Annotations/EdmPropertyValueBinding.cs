//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
