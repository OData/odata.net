//---------------------------------------------------------------------
// <copyright file="IDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Data generator for the TData type.
    /// </summary>
    /// <typeparam name="TData">Type of the generated data.</typeparam>
    public interface IDataGenerator<TData> : IDataGenerator
    {
        /// <summary>
        /// Generates data of the TData type.
        /// </summary>
        /// <returns>Generated data.</returns>
        new TData GenerateData();
    }
}