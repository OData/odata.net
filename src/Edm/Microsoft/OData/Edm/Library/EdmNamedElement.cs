//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Common base class for all named EDM elements.
    /// </summary>
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
