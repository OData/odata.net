//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM temporal (Duration, DateTime, DateTimeOffset) type.
    /// </summary>
    public class EdmTemporalTypeReference : EdmPrimitiveTypeReference, IEdmTemporalTypeReference
    {
        private readonly int? precision;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTemporalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable)
            : this(definition, isNullable, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTemporalTypeReference"/> class.
        /// </summary>
        /// <param name="definition">The type this reference refers to.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        /// <param name="precision">Precision of values with this type.</param>
        public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision)
            : base(definition, isNullable)
        {
            this.precision = precision;
        }

        /// <summary>
        /// Gets the precision of this temporal type.
        /// </summary>
        public int? Precision
        {
            get { return this.precision; }
        }
    }
}
