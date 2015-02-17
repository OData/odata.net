//---------------------------------------------------------------------
// <copyright file="IEntityModelDataGenerationHintsResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Resolves data generation hints for an entity model.
    /// </summary>
    [ImplementationSelector("EntityModelDataGenerationHintsResolver", DefaultImplementation = "Default")]
    public interface IEntityModelDataGenerationHintsResolver
    {
        /// <summary>
        /// Resolves data generation hints for the entity model.
        /// </summary>
        /// <param name="model">The entity model to resolve data generation hints for.</param>
        /// <param name="dataGenerationHintsResolver">The primitive data type to data generation hints resolver.</param>
        void ResolveDataGenerationHints(EntityModelSchema model, IPrimitiveDataTypeToDataGenerationHintsResolver dataGenerationHintsResolver);
    }
}
