//---------------------------------------------------------------------
// <copyright file="StructuralDataGenerator`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DataGeneration
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Structural data generator.
    /// </summary>
    /// <typeparam name="TData">Type of the data.</typeparam>
    internal class StructuralDataGenerator<TData> : DataGenerator<TData>
    {
        private INamedValuesGenerator memberValuesGenerator;
        private IStructuralDataAdapter dataAdapter;

        /// <summary>
        /// Initializes a new instance of the StructuralDataGenerator class.
        /// </summary>
        /// <param name="dataAdapter">The data adapter.</param>
        /// <param name="dataGenerator">The data generator.</param>
        public StructuralDataGenerator(IStructuralDataAdapter dataAdapter, INamedValuesGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataAdapter, "dataAdapter");
            ExceptionUtilities.CheckArgumentNotNull(dataGenerator, "dataGenerator");
            this.dataAdapter = dataAdapter;
            this.memberValuesGenerator = dataGenerator;
        }

        /// <summary>
        /// Generates data.
        /// </summary>
        /// <returns>Generated data.</returns>
        public override TData GenerateData()
        {
            IEnumerable<NamedValue> memberValues = this.memberValuesGenerator.GenerateData();

            TData data = (TData)this.dataAdapter.CreateData(memberValues);

            return data;
        }
    }
}
