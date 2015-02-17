//---------------------------------------------------------------------
// <copyright file="IObjectToQueryValueConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Converts the given object into QueryValue
    /// </summary>
    [ImplementationSelector("IObjectToQueryValueConverter", DefaultImplementation = "Default")]
    public interface IObjectToQueryValueConverter
    {
        /// <summary>
        /// Converts the given object into QueryValue.
        /// </summary>
        /// <param name="value">Object that will be converted.</param>
        /// <param name="queryType">QueryType of the result.</param>
        /// <returns>QueryValue representing the given object.</returns>
        QueryValue ConvertToQueryValue(object value, QueryType queryType);
    }
}