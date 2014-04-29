//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Edm.Library.Expressions;

    /// <summary>
    /// Represents an EDM entity set.
    /// </summary>
    public class EdmEntitySet : EdmEntitySetBase, IEdmEntitySet
    {
        private readonly IEdmEntityContainer container;
        private readonly IEdmCollectionType type;
        private IEdmPathExpression path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySet"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this entity set.</param>
        /// <param name="name">Name of the entity set.</param>
        /// <param name="elementType">The entity type of the elements in this entity set.</param>
        public EdmEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType)
            : base(name, elementType)
        {
            EdmUtil.CheckArgumentNull(container, "container");

            this.container = container;
            this.type = new EdmCollectionType(new EdmEntityTypeReference(elementType, false));
            this.path = new EdmPathExpression(this.container.FullName() + "." + this.Name);
        }

        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        /// <summary>
        /// Gets the container of this entity set.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        /// <summary>
        /// Gets the type of this entity set.
        /// </summary>
        public override IEdmType Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the path that a navigation property targets. 
        /// </summary>
        public override IEdmPathExpression Path
        {
            get { return this.path; }
        }
    }
}
