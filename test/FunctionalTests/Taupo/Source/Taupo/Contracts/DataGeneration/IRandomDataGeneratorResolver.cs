//---------------------------------------------------------------------
// <copyright file="IRandomDataGeneratorResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Resolves data generator which generates random data.
    /// </summary>
    [ImplementationSelector("RandomDataGeneratorResolver", DefaultImplementation = "Default")]
    public interface IRandomDataGeneratorResolver
    {
        /// <summary>
        /// Resolves data generator which generates random data.
        /// </summary>
        /// <param name="clrType">The type of the data.</param>
        /// <param name="random">Random number generator.</param>
        /// <param name="hints">Data generation hints.</param>
        /// <returns>The random data generator.</returns>
        IDataGenerator ResolveRandomDataGenerator(Type clrType, IRandomNumberGenerator random, params DataGenerationHint[] hints);
    }
}
