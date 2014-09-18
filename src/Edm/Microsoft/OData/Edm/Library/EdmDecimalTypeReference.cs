//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Csdl;

    /// <summary>
    /// Represents a reference to an EDM decimal type.
    /// </summary>
    public class EdmDecimalTypeReference : EdmPrimitiveTypeReference, IEdmDecimalTypeReference
    {
        private readonly int? precision;
        private readonly int? scale;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, null, CsdlConstants.Default_Scale)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmDecimalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="precision">Precision of values with this type.</param>
        /// <param name="scale">Scale of values with this type.</param>
        public EdmDecimalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision, int? scale)
            : base(definition, isNullable)
        {
            this.precision = precision;
            this.scale = scale;
        }

        /// <summary>
        /// Gets the precision of this type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }

        /// <summary>
        /// Gets the scale of this type.
        /// </summary>
        public int? Scale
        {
            get { return this.scale; }
        }
    }
}
