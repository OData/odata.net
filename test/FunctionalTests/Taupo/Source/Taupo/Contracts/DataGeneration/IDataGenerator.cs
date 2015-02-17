//---------------------------------------------------------------------
// <copyright file="IDataGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Data generator.
    /// </summary>
    public interface IDataGenerator
    {
        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Generated data.</returns>
        object GenerateData();
    }
}