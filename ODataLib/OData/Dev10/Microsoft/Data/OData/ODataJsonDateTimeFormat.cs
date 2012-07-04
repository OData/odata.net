//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.Data.OData
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Enumeration describing the various serialization formats for dates in JSON
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
