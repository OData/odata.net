//---------------------------------------------------------------------
// <copyright file="LazyResourceType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Framework.TestProviders.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Service.Providers;

    /// <summary>
    /// LazyResourceType delays the loading of CLR Type information
    /// </summary>
    public class LazyResourceType : ResourceType
    {
        /// <summary>
        /// Initializes a new instance of the LazyResourceType class
        /// </summary>
        /// <param name="instanceType">Instance type that will back this metadata type</param>
        /// <param name="resourceTypeKind">The Type, complex or Resource</param>
        /// <param name="baseType">BaseType of the ResourceTYpe</param>
        /// <param name="namespaceName">Namespace of the type</param>
        /// <param name="name">Name of the type</param>
        /// <param name="isAbstract">Whether the type is abstract or not</param>
        public LazyResourceType(
            Type instanceType, 
            ResourceTypeKind resourceTypeKind, 
            ResourceType baseType,
            string namespaceName, 
            string name, 
            bool isAbstract)
            : base(instanceType, resourceTypeKind, baseType, namespaceName, name, isAbstract)
        {
            this.LazyProperties = new List<ResourceProperty>();
        }

        internal List<ResourceProperty> LazyProperties { get; private set; }

        /// <summary>
        /// Adds a property to a LazyType
        /// </summary>
        /// <param name="property">Property to add to type</param>
        public void AddLazyProperty(ResourceProperty property)
        {
            this.LazyProperties.Add(property);
        }

        /// <summary>
        /// LoadPropertiesDeclaredOnThisType to load types
        /// </summary>
        /// <returns>IEnumerable of ResourceProperties</returns>
        protected override IEnumerable<ResourceProperty> LoadPropertiesDeclaredOnThisType()
        {
            return this.LazyProperties.AsEnumerable();
        }
    }
}