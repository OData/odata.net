//---------------------------------------------------------------------
// <copyright file="ResourcePropertyAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using Microsoft.OData.Service.Providers;
    using System.Reflection;

    /// <summary>Helper class for extension methods on the <see cref="ResourceProperty"/>.</summary>
    internal static class ResourcePropertyExtensions
    {
        /// <summary>Helper method to get annotation from the specified resource property.</summary>
        /// <param name="resourceType">The resource property to get annotation for.</param>
        /// <returns>The annotation for the resource property or null if the resource property doesn't have annotation.</returns>
        /// <remarks>We store the annotation in the <see cref="ResourceProperty.CustomState"/>, so this is just a simple helper
        /// which allows strongly typed access.</remarks>
        internal static ResourcePropertyAnnotation GetAnnotation(this ResourceProperty resourceProperty)
        {
            return resourceProperty.CustomState as ResourcePropertyAnnotation;
        }
    }

    /// <summary>Class used to annotate <see cref="ResourceProperty"/> instances with DSP specific data.</summary>
    public class ResourcePropertyAnnotation
    {
        /// <summary>
        /// If this property is a reference property this stores the Func that would return the resource association set which describes that reference.
        /// 
        /// We store a Func that returns the association set rather than the association set itself to support lazy loading of properties.
        /// This is because the constructor of ResourceAssociationSetEnd validates that the navigation property does exist in the resource type,
        /// this validation would trigger all properties for that type to be loaded.  Therefore we don't want to instanciate the ResourceAssociationSet
        /// until it is actually needed to delay the population of the properties.
        /// </summary>
        public Func<ResourceAssociationSet> ResourceAssociationSet { get; set; }

        /// <summary>If the property is backed by a real CLR property this is the info about that property.</summary>
        public PropertyInfo PropertyInfo { get; set; }
    }
}
