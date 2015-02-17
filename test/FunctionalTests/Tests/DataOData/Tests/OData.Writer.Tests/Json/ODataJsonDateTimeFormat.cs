//---------------------------------------------------------------------
// <copyright file="ODataJsonDateTimeFormat.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.Json
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Test Enumeration describing the various serialization formats for dates in JSON
    /// </summary>
    internal enum ODataJsonDateTimeFormat
    {
        /// <summary>
        /// Represents a DateTime value in the OData format of \/Date(ticksrepresentingdatetime)\/
        /// </summary>
        ODataDateTime,

        /// <summary>
        /// Represents a DateTime value in the ISO 8601 format of YYYY-MM-DDThh:mm:ss.sTZD eg 1997-07-16T19:20:30.45+01:00
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "ISO is a standards body and should be represented as all-uppercase in the API.")]
        ISO8601DateTime
    }
}
