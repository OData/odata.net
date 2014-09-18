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
    using System.Diagnostics;

    /// <summary>
    /// Common base class for all named EDM elements.
    /// </summary>
    [DebuggerDisplay("Name:{Name}")]
    public abstract class EdmNamedElement : EdmElement, IEdmNamedElement
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmNamedElement"/> class.
        /// </summary>
        /// <param name="name">Name of the element.</param>
        protected EdmNamedElement(string name)
        {
            EdmUtil.CheckArgumentNull(name, "name");

            this.name = name;
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }
    }
}
