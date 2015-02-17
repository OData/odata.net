//---------------------------------------------------------------------
// <copyright file="IUniqueDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Resolves data generator which generates unique data.
    /// </summary>
    [ImplementationSelector("UniqueDataGeneratorResolver", DefaultImplementation = "Default")]
    public interface IUniqueDataGeneratorResolver
    {
        /// <summary>
        /// Resolves data generator which generates unique data.
        /// </summary>
        /// <param name="clrType">The type of the data.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>The unique data generator.</returns>
        IDataGenerator ResolveUniqueDataGenerator(Type clrType, IRandomNumberGenerator random, params DataGenerationHint[] hints);
    }
}
