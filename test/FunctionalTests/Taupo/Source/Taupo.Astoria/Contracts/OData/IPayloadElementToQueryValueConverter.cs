//---------------------------------------------------------------------
// <copyright file="IPayloadElementToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for converting from a payload element to a QueryValue
    /// </summary>
    [ImplementationSelector("PayloadElementToQueryValueConverter", DefaultImplementation = "Default")]
    public interface IPayloadElementToQueryValueConverter
    {
        /// <summary>
        /// Converts the given payload into a series of named value pairs
        /// </summary>
        /// <param name="element">The payload to convert</param>
        /// <param name="queryTypeToBuild">Query Type to build</param>
        /// <returns>The QueryValue that represents the given payload</returns>
        QueryValue Convert(ODataPayloadElement element, QueryType queryTypeToBuild);
    }
}