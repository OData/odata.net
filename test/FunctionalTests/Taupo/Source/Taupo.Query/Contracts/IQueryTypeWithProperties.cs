//---------------------------------------------------------------------
// <copyright file="IQueryTypeWithProperties.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Query type with properties that can be projected in query
    /// </summary> 
    public interface IQueryTypeWithProperties
    {
        /// <summary>
        /// Gets the collection of type properties.
        /// </summary>
        IList<QueryProperty> Properties { get; }
    }
}
