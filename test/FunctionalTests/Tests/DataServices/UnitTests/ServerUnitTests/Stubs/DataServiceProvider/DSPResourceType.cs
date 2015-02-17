//---------------------------------------------------------------------
// <copyright file="DSPResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// A Resource Type class that allows lazy load of properties.
    /// </summary>
    public class DSPResourceType : ResourceType
    {
        /// <summary>
        /// Delegate to lazy load properties on this type.
        /// </summary>
        public Func<IEnumerable<ResourceProperty>> LoadPropertiesDeclaredOnThisTypeDelegate { get; set; }

        /// <summary>
        /// Constructs a new instance of Astoria type using the specified clr type
        /// </summary>
        /// <param name="instanceType">clr type that represents the flow format inside the Astoria runtime</param>
        /// <param name="resourceTypeKind"> kind of the resource type</param>
        /// <param name="baseType">base type of the resource type</param>
        /// <param name="namespaceName">Namespace name of the given resource type.</param>
        /// <param name="name">name of the given resource type.</param>
        /// <param name="isAbstract">whether the resource type is an abstract type or not.</param>
        public DSPResourceType(Type instanceType, ResourceTypeKind resourceTypeKind, ResourceType baseType, string namespaceName, string name, bool isAbstract)
            : base(instanceType, resourceTypeKind, baseType, namespaceName, name, isAbstract)
        {
        }

        /// <summary>
        /// Return the list of properties declared by this resource type. This method gives a chance to lazy load the properties
        /// of a resource type, instead of loading them upfront. This property will be called once and only once, whenever
        /// ResourceType.Properties or ResourceType.PropertiesDeclaredOnThisType property is accessed.
        /// </summary>
        /// <returns>the list of properties declared on this type.</returns>
        protected override IEnumerable<ResourceProperty> LoadPropertiesDeclaredOnThisType()
        {
            if (this.LoadPropertiesDeclaredOnThisTypeDelegate != null)
            {
                return this.LoadPropertiesDeclaredOnThisTypeDelegate();
            }
            else
            {
                return base.LoadPropertiesDeclaredOnThisType();
            }
        }
    }
}
