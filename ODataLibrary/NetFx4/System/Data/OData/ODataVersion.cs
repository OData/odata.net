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

namespace System.Data.OData
{
    /// <summary>
    /// Enumeration representing the OData protocol version.
    /// </summary>
#if INTERNAL_DROP
    internal enum ODataVersion
#else
    public enum ODataVersion
#endif
    {
        /// <summary>Version 1.0.</summary>
        V1,

        /// <summary>Version 2.0.</summary>
        V2,

        /// <summary>Version 3.0.</summary>
        V3,
    }
}
