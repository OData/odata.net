//---------------------------------------------------------------------
// <copyright file="EdmSingleton.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
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
        public override IEdmPathExpression Path
        {
            get { return this.path; }
        }
    }
}
