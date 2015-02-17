//---------------------------------------------------------------------
// <copyright file="IQueryTypeLibraryBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Contracts.EntityModel;

    /// <summary>
    /// Builder for query type library
    /// </summary>
    public interface IQueryTypeLibraryBuilder
    {
        /// <summary>
        /// Builds the query type library with out Clr type mapping information, given the entity model  
        /// </summary>
        /// <param name="model">The given entity model</param>
        /// <returns>The query type library</returns>
        QueryTypeLibrary BuildLibraryWithoutClrTypeMapping(EntityModelSchema model);
    }
}
