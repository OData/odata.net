//---------------------------------------------------------------------
// <copyright file="EdmEntitySetFacade.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Wraps an entity set from the server-side model and restricts which APIs can be called.
    /// </summary>
    internal class EdmEntitySetFacade : IEdmEntitySet
    {
        /// <summary>Storage for the entity set from the server model.</summary>
        private readonly IEdmEntitySet serverEntitySet;

        /// <summary>Storage for the model facade.</summary>
        private readonly EdmModelFacade modelFacade;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmEntitySetFacade"/> class.
        /// </summary>
        /// <param name="serverEntitySet">The entity set from the server model.</param>
        /// <param name="containerFacade">The entity container facade to which the set belongs.</param>
        /// <param name="modelFacade">The model facade.</param>
        internal EdmEntitySetFacade(IEdmEntitySet serverEntitySet, EdmEntityContainerFacade containerFacade, EdmModelFacade modelFacade)
        {
            Debug.Assert(serverEntitySet != null, "serverEntitySet != null");
            Debug.Assert(containerFacade != null, "container != null");
            Debug.Assert(modelFacade != null, "modelFacade != null");

            this.serverEntitySet = serverEntitySet;
            this.Container = containerFacade;
            this.modelFacade = modelFacade;
            
            this.Name = this.serverEntitySet.Name;
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the kind of element of this container element.
        /// </summary>
        public EdmContainerElementKind ContainerElementKind
        {
            get { return EdmContainerElementKind.EntitySet; }
        }

        /// <summary>
        /// Gets the container that contains this element.
        /// </summary>
        public IEdmEntityContainer Container { get; private set; }

        /// <summary>
        /// Gets the entity type contained in this entity set.
        /// </summary>
        public IEdmEntityType ElementType
        {
            get { return this.modelFacade.GetOrCreateEntityTypeFacade(this.serverEntitySet.ElementType); }
        }

        /// <summary>
        /// Gets the navigation targets of this entity set.
        /// </summary>
        public IEnumerable<IEdmNavigationTargetMapping> NavigationTargets
        {
            get { throw CreateExceptionForUnsupportedPublicMethod("NavigationTargets"); }
        }

        /// <summary>
        /// Finds the entity set that a navigation property targets.
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        /// <returns>The entity set that the navigation propertion targets, or null if no such entity set exists.</returns>
        public IEdmEntitySet FindNavigationTarget(IEdmNavigationProperty navigationProperty)
        {
            Util.CheckArgumentNull(navigationProperty, "navigationProperty");

            var navigationFacade = navigationProperty as EdmNavigationPropertyFacade;
            if (navigationFacade == null)
            {
                return null;
            }

            return navigationFacade.FindNavigationTarget(this.serverEntitySet);
        }

        /// <summary>
        /// Unit test method for determining whether two facades are equivalent (ie: wrap the same server entity set).
        /// </summary>
        /// <param name="other">The other facade.</param>
        /// <returns>
        ///   <c>true</c> if the two facades wrap the same entity sets; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsEquivalentTo(EdmEntitySetFacade other)
        {
            return other != null && ReferenceEquals(other.serverEntitySet, this.serverEntitySet);
        }

        /// <summary>
        /// Creates an exception for a intentionally-unsupported part of the API.
        /// This is used to prevent new code from calling previously-unused API, which could be a breaking change
        /// for user implementations of the interface.
        /// </summary>
        /// <param name="methodName">Name of the unsupported method.</param>
        /// <returns>The exception</returns>
        private static Exception CreateExceptionForUnsupportedPublicMethod(string methodName)
        {
            return new NotSupportedException(Microsoft.OData.Service.Client.Strings.EdmModelFacade_UnsupportedMethod("IEdmEntitySet", methodName));
        }
    }
}