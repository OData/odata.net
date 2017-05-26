//---------------------------------------------------------------------
// <copyright file="StubEdmEntitySet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib.StubEdm
{
    using System.Collections.Generic;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    /// <summary>
    /// Stub implementation of EdmEntitySet
    /// </summary>
    public class StubEdmEntitySet : StubEdmElement, IEdmEntitySet
    {
        private Dictionary<IEdmNavigationProperty, IEdmEntitySet> navigationTargets = new Dictionary<IEdmNavigationProperty, IEdmEntitySet>();
        
        /// <summary>
        /// Initializes a new instance of the StubEdmEntitySet class.
        /// </summary>
        /// <param name="name">the name of entity set</param>
        /// <param name="container">The container of the entity set</param>
        public StubEdmEntitySet(string name, IEdmEntityContainer container)
        {
            this.Name = name;
            this.Container = container;
        }

        /// <summary>
        /// Gets or sets the type of this navigation source.
        /// </summary>
        public IEdmType Type { get; set; }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the container element kind
        /// </summary>
        public EdmContainerElementKind ContainerElementKind 
        { 
            get { return EdmContainerElementKind.EntitySet; } 
        }

        /// <summary>
        /// Gets or sets the container
        /// </summary>
        public IEdmEntityContainer Container { get; set; }

        /// <summary>
        /// Gets the navigation targets of this entity set.
        /// </summary>
        public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings
        {
            get
            {
                foreach (KeyValuePair<IEdmNavigationProperty, IEdmEntitySet> mapping in this.navigationTargets)
                {
                    yield return new EdmNavigationPropertyBinding(mapping.Key, mapping.Value);
                }
            }
        }

        /// <summary>
        /// Gets the path that a navigation property targets.
        /// </summary>
        public IEdmPathExpression Path
        {
            get { return null; }
        }

		/// <summary>
		/// Sets or gets whether to include in the service document
		/// </summary>
		public bool IncludeInServiceDocument
		{
			get; set;
		}

		/// <summary>
		/// Sets the navigation target for a particular navigation property.
		/// </summary>
		/// <param name="navigationProperty">The navigation property.</param>
		/// <param name="target">The target entity set.</param>
		public void SetNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmEntitySet target)
        {
            this.navigationTargets[navigationProperty] = target;

            var stubTarget = (StubEdmEntitySet)target;
            if (stubTarget.FindNavigationTarget(navigationProperty.Partner) != this) 
            {
                stubTarget.SetNavigationTarget(navigationProperty.Partner, this);
            }
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The entity set that the navigation propertion targets, or null if no such entity set exists.</returns>
        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            if (this.navigationTargets.ContainsKey(navigationProperty))
            {
                return this.navigationTargets[navigationProperty];
            }
            else
            {
                return null;
            }
        }

        public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty, IEdmPathExpression bindingPath)
        {
            return FindNavigationTarget(navigationProperty);
        }

        public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(IEdmNavigationProperty navigationProperty)
        {
            return null;
        }
    }
}
