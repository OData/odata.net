//---------------------------------------------------------------------
// <copyright file="OpenMetadataProviderTypesStrategy.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria
{
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Metadata provider types strategy that makes all types open, and omits all properties besides keys, navigations, and etags
    /// </summary>
    [ImplementationName(typeof(IMetadataProviderTypingStrategy), "Open")]
    public class OpenMetadataProviderTypesStrategy : IMetadataProviderTypingStrategy
    {
        /// <summary>
        /// Returns an entity-model fixup that sets up the appropriate annotations for this strategy
        /// </summary>
        /// <returns>An entity model fixup</returns>
        public IEntityModelFixup GetModelFixup()
        {
            return new OpenMetadataProviderTypesFixup();
        }

        /// <summary>
        /// Fixes up the model to make all types open, and omit properties that are not keys, etags, or naviations
        /// </summary>
        private class OpenMetadataProviderTypesFixup : IEntityModelFixup
        {
            /// <summary>
            /// Sets all types and properties to be open and not metadata-declared, unless they are keys, etags, or navigations
            /// </summary>
            /// <param name="model">The model to fix up</param>
            public void Fixup(EntityModelSchema model)
            {
                foreach (var entityType in model.EntityTypes)
                {
                    entityType.IsOpen = true;
                    entityType.MakeMetadataDeclared();
                    entityType.MakeAllRequiredPropertiesMetadataDeclared();
                }

                foreach (var complexType in model.ComplexTypes)
                {
                    complexType.MakeMetadataDeclared();
                    foreach (var property in complexType.Properties)
                    {
                        property.MakeMetadataDeclared();
                    }
                }
            }
        }
    }
}