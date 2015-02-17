//---------------------------------------------------------------------
// <copyright file="EntityModelDataGenerationHintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Resolves data generation hints in EntityModel by applying the specific data generation hints resolver.
    /// </summary>
    [ImplementationName(typeof(IEntityModelDataGenerationHintsResolver), "Default")]
    public class EntityModelDataGenerationHintsResolver : IEntityModelDataGenerationHintsResolver
    {
        private const int DefaultFixedLengthForUnlimitedData = 32;

        /// <summary>
        /// Resolves data generation hints for the entity model.
        /// </summary>
        /// <param name="model">The entity model to resolve data generation hints for.</param>
        /// <param name="dataGenerationHintsResolver">The data generation hints resolver.</param>
        public void ResolveDataGenerationHints(EntityModelSchema model, IPrimitiveDataTypeToDataGenerationHintsResolver dataGenerationHintsResolver)
        {
            ExceptionUtilities.CheckArgumentNotNull(model, "model");
            ExceptionUtilities.CheckArgumentNotNull(dataGenerationHintsResolver, "dataGenerationHintsResolver");
            
            foreach (ComplexType comlexType in model.ComplexTypes)
            {
                Resolve(comlexType.Properties, dataGenerationHintsResolver);
            }

            foreach (EntityType entityType in model.EntityTypes)
            {
                Resolve(entityType.Properties, dataGenerationHintsResolver);
            }
        }

        private static void Resolve(IEnumerable<MemberProperty> properties, IPrimitiveDataTypeToDataGenerationHintsResolver dataGenerationHintsResolver)
        {
            foreach (MemberProperty property in properties)
            {
                var collectionType = property.PropertyType as CollectionDataType;

                var elementType = collectionType != null ? collectionType.ElementDataType : property.PropertyType;

                var primitiveDataType = elementType as PrimitiveDataType;
                if (primitiveDataType != null)
                {
                    var hints = dataGenerationHintsResolver.ResolveDataGenerationHints(primitiveDataType).ToList();
                    property.WithDataGenerationHints(hints.ToArray());
                }
            }
        }
    }
}
