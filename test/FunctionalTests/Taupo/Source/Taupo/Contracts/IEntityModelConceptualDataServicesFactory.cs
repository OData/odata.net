//---------------------------------------------------------------------
// <copyright file="IEntityModelConceptualDataServicesFactory.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Factory that creates structural data services for the entity model schema.
    /// </summary>
    [ImplementationSelector("EntityModelStructuralDataServicesFactory", DefaultImplementation = "Default")]
    public interface IEntityModelConceptualDataServicesFactory
    {
        /// <summary>
        /// Creates structural data services for the specified entity model schema.
        /// </summary>
        /// <param name="modelSchema">Entity model schema.</param>
        /// <returns>An <see cref="IEntityModelConceptualDataServices"/>.</returns>
        IEntityModelConceptualDataServices CreateConceptualDataServices(EntityModelSchema modelSchema);
     }
}
