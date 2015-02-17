//---------------------------------------------------------------------
// <copyright file="IEntityContainerDataPopulationDriver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Data;
    
    /// <summary>
    /// Entity container data population driver.
    /// </summary>
    [ImplementationSelector("EntityContainerDataPopulationDriver", DefaultImplementation = "Default")]
    public interface IEntityContainerDataPopulationDriver
    {
        /// <summary>
        /// Gets or sets the entity container.
        /// </summary>
        /// <value>The entity container.</value>
        EntityContainer EntityContainer { get; set; }

        /// <summary>
        /// Gets or sets the random number generator.
        /// </summary>
        /// <value>The random.</value>
        IRandomNumberGenerator Random { get; set; }

        /// <summary>
        /// Gets or sets the structural data services.
        /// </summary>
        /// <value>The structural data services.</value>
        IEntityModelConceptualDataServices StructuralDataServices { get; set; }

        /// <summary>
        /// Gets or sets the threshold for number of entities populated by <see cref="TryPopulateNextData"/>.
        /// </summary>
        /// <value>The threshold for number of entities populated by <see cref="TryPopulateNextData"/>.</value>
        /// <remarks>Set the threshold to -1 if all data need to be populated in one call to <see cref="TryPopulateNextData"/>.</remarks>
        int ThresholdForNumberOfEntities { get; set; } 

        /// <summary>
        /// Tries to populate next portion of data.
        /// </summary>
        /// <param name="data">The populated data.</param>
        /// <returns>True if data was populated, false otherwise.</returns>
        bool TryPopulateNextData(out EntityContainerData data);
    }
}
