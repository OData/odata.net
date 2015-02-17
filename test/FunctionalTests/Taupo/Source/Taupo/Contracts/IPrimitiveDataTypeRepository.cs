//---------------------------------------------------------------------
// <copyright file="IPrimitiveDataTypeRepository.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Primitive data types repository.
    /// </summary>
    public interface IPrimitiveDataTypeRepository
    {
        /// <summary>
        /// Gets primitive data types available in the repository.
        /// </summary>
        /// <returns>Primitive data types available in the repository.</returns>
        IEnumerable<PrimitiveDataType> GetPrimitiveDataTypes();

        /// <summary>
        /// Creates a data generator for the given data type from the repository.
        /// </summary>
        /// <param name="dataType">The data type from the repository to get data generator for.</param>
        /// <param name="random">The random number generator.</param>
        /// <param name="isUnique">The value indicating whether the generator should produce unique data.</param>
        /// <returns>The data generator for the given data type.</returns>
        IDataGenerator CreateDataGenerator(PrimitiveDataType dataType, IRandomNumberGenerator random, bool isUnique);
    }
}
