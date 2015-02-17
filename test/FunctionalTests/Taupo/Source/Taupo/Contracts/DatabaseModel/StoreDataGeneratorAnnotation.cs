//---------------------------------------------------------------------
// <copyright file="StoreDataGeneratorAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.DataGeneration;

    /// <summary>
    /// Annotation which represents data generator.
    /// </summary>
    public class StoreDataGeneratorAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the StoreDataGeneratorAnnotation class.
        /// </summary>
        /// <param name="dataGenerator">The data generator</param>
        public StoreDataGeneratorAnnotation(IDataGenerator dataGenerator)
        {
            ExceptionUtilities.CheckArgumentNotNull(dataGenerator, "dataGenerator");
            this.DataGenerator = dataGenerator;
        }

        /// <summary>
        /// Gets data generator.
        /// </summary>
        public IDataGenerator DataGenerator { get; private set; }
    }
}