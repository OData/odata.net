//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using Microsoft.OData.Edm;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM property.
    /// </summary>
    public abstract class EdmProperty : EdmNamedElement, IEdmProperty
    {
        private readonly IEdmStructuredType declaringType;
        private readonly HashSetInternal<IDependent> dependents = new HashSetInternal<IDependent>();
        private readonly IEdmTypeReference type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmProperty"/> class.
        /// </summary>
        /// <param name="declaringType">The type that declares this property.</param>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        protected EdmProperty(IEdmStructuredType declaringType, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringType, "declaringType");
            EdmUtil.CheckArgumentNull(type, "type");

            this.declaringType = declaringType;
            this.type = type;
        }

        /// <summary>
        /// Gets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }
        
        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public abstract EdmPropertyKind PropertyKind
        {
            get;
        }
    }
}
