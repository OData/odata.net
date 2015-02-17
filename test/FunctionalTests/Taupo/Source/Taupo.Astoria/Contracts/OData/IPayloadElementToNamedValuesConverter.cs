//---------------------------------------------------------------------
// <copyright file="IPayloadElementToNamedValuesConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for converting from a payload element into a series of named-value pairs
    /// </summary>
    [ImplementationSelector("PayloadElementToNamedValuesConverter", DefaultImplementation = "Default")]
    public interface IPayloadElementToNamedValuesConverter
    {
        /// <summary>
        /// Converts the given payload into a series of named value pairs
        /// </summary>
        /// <param name="payload">The payload to convert</param>
        /// <returns>The named-value pairs represented by the given payload</returns>
        IEnumerable<NamedValue> ConvertToNamedValues(ODataPayloadElement payload);
    }
}