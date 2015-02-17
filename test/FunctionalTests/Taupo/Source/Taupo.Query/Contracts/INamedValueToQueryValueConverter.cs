//---------------------------------------------------------------------
// <copyright file="INamedValueToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Component that can update query structural values based on a list of NamedValues
    /// </summary>
    [ImplementationSelector("INamedValueToQueryValue", DefaultImplementation = "Default")]
    public interface INamedValueToQueryValueConverter
    {
        /// <summary>
        /// Updates query structuralvalues 
        /// </summary>
        /// <param name="namedValues">values to update to</param>
        /// <param name="queryTypeToBuild">Query Type to build</param>
        /// <returns>Returns a new queryValue with the named values tranferred into it</returns>
        QueryValue Convert(IEnumerable<NamedValue> namedValues, QueryType queryTypeToBuild);
    }
}
