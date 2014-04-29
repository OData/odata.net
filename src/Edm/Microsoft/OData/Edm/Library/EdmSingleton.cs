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
    /// Represents an EDM singleton.
    /// </summary>
    public class EdmSingleton : EdmNavigationSource, IEdmSingleton
    {
        private readonly IEdmEntityContainer container;
        private readonly IEdmEntityType entityType;
        private IEdmPathExpression path;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmSingleton"/> class.
        /// </summary>
        /// <param name="container">An <see cref="IEdmEntityContainer"/> containing this entity set.</param>
        /// <param name="name">Name of the singleton.</param>
        /// <param name="entityType">The entity type of the element of this singleton.</param>
        public EdmSingleton(IEdmEntityContainer container, string name, IEdmEntityType entityType)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(container, "container");
            EdmUtil.CheckArgumentNull(entityType, "entityType");

            this.container = container;
            this.entityType = entityType;
            this.path = new EdmPathExpression(name);
        }

        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.Singleton; }
        }

        /// <summary>
        /// Gets the container of this singleton.
        /// </summary>
        public IEdmEntityContainer Container
        {
            get { return this.container; }
        }

        /// <summary>
        /// Gets the type of this navigation source.
        /// </summary>
        public override IEdmType Type
        {
            get { return this.entityType; }
        }

        /// <summary>
        /// Gets the path that a navigation property targets.
        /// </summary>
        public override Edm.Expressions.IEdmPathExpression Path
        {
            get { return this.path; }
        }
    }
}
