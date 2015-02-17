//---------------------------------------------------------------------
// <copyright file="LazyMetadataProviderTypesStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    
    /// <summary>
    /// Metadata provider types strategy that makes all types lazy-loaded and backed by weak types
    /// </summary>
    [ImplementationName(typeof(IMetadataProviderTypingStrategy), "Lazy")]
    public class LazyMetadataProviderTypesStrategy : IMetadataProviderTypingStrategy
    {
        /// <summary>
        /// Returns an entity-model fixup that sets up the appropriate annotations for this strategy
        /// </summary>
        /// <returns>An entity model fixup</returns>
        public IEntityModelFixup GetModelFixup()
        {
            return new LazyMetadataProviderTypesFixup();
        }

        /// <summary>
        /// Fixes up model to make all types lazy-loaded and backed by weak types
        /// </summary>
        private class LazyMetadataProviderTypesFixup : IEntityModelFixup
        {
            /// <summary>
            /// Sets all types and properties to be lazy loaded and metadata-declared, but not type backed
            /// </summary>
            /// <param name="model">The model to fix up</param>
            public void Fixup(EntityModelSchema model)
            {
                foreach (var entityType in model.EntityTypes)
                {
                    IEnumerable<MemberProperty> properties;
                    IEnumerable<NavigationProperty> navigations;
                    if (entityType.BaseType == null)
                    {
                        properties = entityType.AllProperties;
                        navigations = entityType.AllNavigationProperties;
                    }
                    else
                    {
                        properties = entityType.Properties;
                        navigations = entityType.NavigationProperties;
                    }

                    entityType.MakeLazyLoadedType();
                    foreach (var property in properties)
                    {
                        property.MakeMetadataDeclared();
                    }

                    foreach (var property in navigations)
                    {
                        property.MakeMetadataDeclared();
                    }
                }

                foreach (var complexType in model.ComplexTypes)
                {
                    complexType.MakeLazyLoadedType();
                    foreach (var property in complexType.Properties)
                    {
                        property.MakeMetadataDeclared();
                    }
                }
            }
        }
    }
}