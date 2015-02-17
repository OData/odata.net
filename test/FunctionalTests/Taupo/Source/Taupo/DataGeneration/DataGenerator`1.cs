//---------------------------------------------------------------------
// <copyright file="DataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Base class for a data generator of a specific type.
    /// </summary>
    /// <typeparam name="TData">Type of generated data.</typeparam>
    public abstract class DataGenerator<TData> : IDataGenerator<TData>
    {
        /// <summary>
        /// Generates data of the <typeparamref name="TData"/> type.
        /// </summary>
        /// <returns>Data of the <typeparamref name="TData"/> type.</returns>
        public abstract TData GenerateData();

        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Generated data.</returns>
        object IDataGenerator.GenerateData()
        {
            return this.GenerateData();
        }
    }
}
