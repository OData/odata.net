//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary>
    /// An enumeration representing the result of a scan operation through 
    /// the batch reader stream's buffer.
    /// </summary>
    internal enum ODataBatchReaderStreamScanResult
    {
        /// <summary>No match with the requested boundary was found (not even a partial one).</summary>
        NoMatch,

        /// <summary>A partial match with the requested boundary was found.</summary>
        PartialMatch,

        /// <summary>A complete match with the requested boundary was found.</summary>
        /// <remarks>
        /// This is only returned if we could also check whether the boundary is an end
        /// boundary or not; otherwise a partial match is returned.
        /// </remarks>
        Match,
    }
}
