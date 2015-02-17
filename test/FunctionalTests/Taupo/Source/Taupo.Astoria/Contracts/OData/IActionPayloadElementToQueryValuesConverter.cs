//---------------------------------------------------------------------
// <copyright file="IActionPayloadElementToQueryValuesConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Contract for converting from a parameters payload element into a QueryValues
    /// </summary>
    [ImplementationSelector("ActionPayloadElementToQueryValuesConverter", DefaultImplementation = "Default")]
    public interface IActionPayloadElementToQueryValuesConverter
    {
        /// <summary>
        /// Converts the given payload into a series of named value pairs
        /// </summary>
        /// <param name="parametersPayload"> parameters payload</param>
        /// <param name="action">Function that the parameters are of</param>
        /// <returns>Returns a dictionary of QueryValues and parameter names representing the parameters from the payload</returns>
        IDictionary<string, QueryValue> Convert(ComplexInstance parametersPayload, Function action);
    }
}