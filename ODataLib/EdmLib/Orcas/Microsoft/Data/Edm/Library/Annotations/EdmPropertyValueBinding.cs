//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Expressions;

namespace Microsoft.Data.Edm.Library.Annotations
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
