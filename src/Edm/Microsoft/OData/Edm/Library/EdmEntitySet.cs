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
