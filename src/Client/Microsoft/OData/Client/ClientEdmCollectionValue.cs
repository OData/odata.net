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

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Values;

    /// <summary>
    /// Implementation of <see cref="IEdmCollectionValue"/> which wraps client-side objects.
    /// </summary>
    internal sealed class ClientEdmCollectionValue : IEdmCollectionValue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientEdmCollectionValue"/> class.
        /// </summary>
        /// <param name="type">The type of the collection.</param>
        /// <param name="elements">The elements of the collection.</param>
        public ClientEdmCollectionValue(IEdmTypeReference type, IEnumerable<IEdmValue> elements)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(elements != null, "values != null");

            this.Type = type;
            this.Elements = elements.Select(v => (IEdmDelayedValue)new NullEdmDelayedValue(v));
        }

        /// <summary>
        /// Gets the type of this value.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// Gets the kind of this value.
        /// </summary>
        public EdmValueKind ValueKind
        {
            get { return EdmValueKind.Collection; }
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmDelayedValue> Elements { get; private set; }

        /// <summary>
        /// Non-delayed implementation of <see cref="IEdmDelayedValue"/>
        /// </summary>
        private class NullEdmDelayedValue : IEdmDelayedValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="NullEdmDelayedValue"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public NullEdmDelayedValue(IEdmValue value)
            {
                this.Value = value;   
            }

            /// <summary>
            /// Gets the data stored in this value.
            /// </summary>
            public IEdmValue Value { get; private set; }
        }
    }
}
