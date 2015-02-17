//---------------------------------------------------------------------
// <copyright file="IEntityModelConceptualDataServices.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts
{
    using Microsoft.Test.Taupo.Contracts.DataGeneration;
    
    /// <summary>
    /// Entity model structural data services.
    /// </summary>
    public interface IEntityModelConceptualDataServices
    {
        /// <summary>
        /// Gets structural data generator for the specified entity set and type.
        /// </summary>
        /// <param name="entityTypeFullName">An entity type full name.</param>
        /// <param name="entitySetName">A name of the entity set for which data needs to be generated.</param>
        /// <returns>An <see cref="IDataGenerator"/> that generates data for entity type in the form of collection of named values.</returns>
        INamedValuesGenerator GetStructuralGenerator(string entityTypeFullName, string entitySetName);

        /// <summary>
        /// Gets structural data generator for the specified complex type.
        /// </summary>
        /// <param name="complexTypeFullName">A complex type full name.</param>
        /// <returns>An <see cref="IDataGenerator"/> that generates data for complex type in the form of collection of named values.</returns>
        INamedValuesGenerator GetStructuralGenerator(string complexTypeFullName);
    }
}
