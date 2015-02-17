//---------------------------------------------------------------------
// <copyright file="IODataUriToStringConverter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A contract to use when converting an OData uri into a string
    /// </summary>
    [ImplementationSelector("ODataUriToStringConverter", DefaultImplementation = "StrictlyOrdered", 
        HelpText = "Responsible for converting OData URI's into raw string URIs")]
    public interface IODataUriToStringConverter
    {
        /// <summary>
        /// Converts the given OData uri into a string
        /// </summary>
        /// <param name="uri">The uri to convert</param>
        /// <returns>The converted uri</returns>
        string ConvertToString(ODataUri uri);

        /// <summary>
        /// Converts the given OData uri segment into a string
        /// </summary>
        /// <param name="segment">The segment to convert</param>
        /// <returns>The converted segment</returns>
        string ConvertToString(ODataUriSegment segment);
    }
}
