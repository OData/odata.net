//---------------------------------------------------------------------
// <copyright file="INamedValueToQueryValueUpdater.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Component that can update  a queryValue based on a list of NamedValues
    /// </summary>
    [ImplementationSelector("NamedValueToQueryValueUpdater", DefaultImplementation = "Default")]
    public interface INamedValueToQueryValueUpdater
    {
        /// <summary>
        /// Updates query values based on the named values
        /// </summary>
        /// <param name="queryValue">value to update</param>
        /// <param name="namedValues">values to update to</param>
        void UpdateValues(QueryValue queryValue, IEnumerable<NamedValue> namedValues);
    }
}
