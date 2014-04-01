//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an abstract EDM entity set base.
    /// </summary>
    public abstract class EdmEntitySetBase : EdmNavigationSource, IEdmEntitySetBase
    {
        private readonly IEdmCollectionType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySetBase"/> class.
        /// </summary>
        /// <param name="name">Name of the entity set base.</param>
        /// <param name="elementType">The entity type of the elements in this entity set base.</param>
        protected EdmEntitySetBase(string name, IEdmEntityType elementType)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(elementType, "elementType");

            this.type = new EdmCollectionType(new EdmEntityTypeReference(elementType, false));
        }

        /// <summary>
        /// Gets the type of this navigation source.
        /// </summary>
        public override IEdmType Type
        {
            get { return this.type; }
        }
    }
}
