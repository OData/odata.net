//---------------------------------------------------------------------
// <copyright file="ClrMetadataProviderTypesStrategy.cs" company="Microsoft">
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
    /// Metadata provider typing strategy that uses strongly-backed types
    /// </summary>
    [ImplementationName(typeof(IMetadataProviderTypingStrategy), "CLR")]
    public class ClrMetadataProviderTypesStrategy : IMetadataProviderTypingStrategy
    {
        /// <summary>
        /// Returns an entity-model fixup that sets up the appropriate annotations for this strategy
        /// </summary>
        /// <returns>An entity model fixup</returns>
        public IEntityModelFixup GetModelFixup()
        {
            return new ClrMetadataProviderTypesFixup();
        }

        /// <summary>
        /// Model fixup that makes all types be strongly-backed
        /// </summary>
        private class ClrMetadataProviderTypesFixup : IEntityModelFixup
        {
            /// <summary>
            /// Sets all types and properties to be type-backed and metadata-declared
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

                    entityType.MakeTypeBacked();
                    foreach (var property in properties)
                    {
                        property.MakeTypeBacked();
                        property.MakeMetadataDeclared();
                    }

                    foreach (var property in navigations)
                    {
                        property.MakeTypeBacked();
                        property.MakeMetadataDeclared();
                    }
                }

                foreach (var complexType in model.ComplexTypes)
                {
                    complexType.MakeTypeBacked();
                    foreach (var property in complexType.Properties)
                    {
                        property.MakeTypeBacked();
                        property.MakeMetadataDeclared();
                    }
                }
            }
        }
    }
}