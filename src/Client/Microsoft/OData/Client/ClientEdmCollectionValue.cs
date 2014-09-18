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
