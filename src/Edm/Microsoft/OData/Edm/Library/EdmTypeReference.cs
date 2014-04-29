//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM type.
    /// </summary>
    public abstract class EdmTypeReference : EdmElement, IEdmTypeReference
    {
        private readonly IEdmType definition;
        private readonly bool isNullable;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeReference"/> class.
        /// </summary>
        /// <param name="definition">Type that describes this value.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        protected EdmTypeReference(IEdmType definition, bool isNullable)
        {
            EdmUtil.CheckArgumentNull(definition, "definition");

            this.definition = definition;
            this.isNullable = isNullable;
        }

        /// <summary>
        /// Gets a value indicating whether this type is nullable.
        /// </summary>
        public bool IsNullable
        {
            get { return this.isNullable; }
        }

        /// <summary>
        /// Gets the definition to which this type refers.
        /// </summary>
        public IEdmType Definition
        {
            get { return this.definition; }
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <returns>The text representation of the current object.</returns>
        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
