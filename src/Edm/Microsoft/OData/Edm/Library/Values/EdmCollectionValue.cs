//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Library.Values
{
    /// <summary>
    /// Represents an EDM collection value.
    /// </summary>
    public class EdmCollectionValue : EdmValue, IEdmCollectionValue
    {
        private readonly IEnumerable<IEdmDelayedValue> elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmCollectionValue"/> class. 
        /// </summary>
        /// <param name="type">A reference to a collection type that describes this collection value</param>
        /// <param name="elements">The collection of values stored in this collection value</param>
        public EdmCollectionValue(IEdmCollectionTypeReference type, IEnumerable<IEdmDelayedValue> elements)
            : base(type)
        {
            this.elements = elements;
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements
        {
            get { return this.elements; }
        }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public override EdmValueKind ValueKind
        {
            get { return EdmValueKind.Collection; }
        }
    }
}
