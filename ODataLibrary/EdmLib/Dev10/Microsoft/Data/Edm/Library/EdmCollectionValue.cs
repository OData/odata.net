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

using System;
using System.Collections.Generic;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents an EDM collection value.
    /// </summary>
    public class EdmCollectionValue : EdmValue, IEdmCollectionValue
    {
        private readonly IEnumerable<IEdmValue> elementValues;

        /// <summary>
        /// Initializes a new instance of the EdmCollectionValue class. 
        /// </summary>
        /// <param name="type">A reference to the MultiValue type that describes this collection value</param>
        /// <param name="elementValues">The collection of values stored in this collection value</param>
        public EdmCollectionValue(IEdmCollectionTypeReference type, IEnumerable<IEdmValue> elementValues)
            : base(type)
        {
            if (!type.IsCollection())
            {
                throw new InvalidOperationException(Edm.Strings.EdmCollectionValueType);
            }

            this.elementValues = elementValues;
        }

        /// <summary>
        /// Gets the values stored in this collection.
        /// </summary>
        public IEnumerable<IEdmValue> ElementValues
        {
            get { return this.elementValues; }
        }
    }
}
