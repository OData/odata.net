//---------------------------------------------------------------------
// <copyright file="DelegatedDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System;
    using Microsoft.Test.Taupo.Common;
    
    /// <summary>
    /// Data generator that uses delegate to generate data.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class DelegatedDataGenerator<TData> : DataGenerator<TData>
    {
        private Func<TData> generateDataDelegate;

        /// <summary>
        /// Initializes a new instance of the DelegatedDataGenerator class.
        /// </summary>
        /// <param name="generateDataDelegate">Delegate to generate data.</param>
        public DelegatedDataGenerator(Func<TData> generateDataDelegate)
        {
            ExceptionUtilities.CheckArgumentNotNull(generateDataDelegate, "generateDataDelegate");

            this.generateDataDelegate = generateDataDelegate;
        }

        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Data generated using delegate specified during construction.</returns>
        public override TData GenerateData()
        {
            return this.generateDataDelegate();
        }
    }
}
