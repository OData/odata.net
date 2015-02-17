//---------------------------------------------------------------------
// <copyright file="ISpatialDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Spatial
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Spatial type data generator resolver
    /// </summary>
    [ImplementationSelector("SpatialDataGeneratorResolver", DefaultImplementation = "Default")]
    public interface ISpatialDataGeneratorResolver
    {
        /// <summary>
        /// Finds and returns the data generator for given spatial type
        /// </summary>
        /// <returns>the data generator for given spatial type</returns>
        /// <param name="spatialType">the spatial type to get data generator for</param>
        /// <param name="isUnique">whether the generator should produce unique data.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="hints">The datageneration hints.</param>
        IDataGenerator GetDataGenerator(SpatialDataType spatialType, bool isUnique, IRandomNumberGenerator random, params DataGenerationHint[] hints);
    }
}
