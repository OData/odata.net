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
