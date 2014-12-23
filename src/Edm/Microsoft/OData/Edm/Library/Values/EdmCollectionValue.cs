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
