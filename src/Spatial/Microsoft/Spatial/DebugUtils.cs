//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.Data.Spatial
{
    /// <summary>
    /// Dummy class for code that is shared with ODataLib.
    /// The ODataLib version of this class has an implementation, but this version is just provided
    /// so that we don't have to conditionally compile all references to it in the shared code.
    /// Since it is debug-only anyway, there is no harm in leaving this no-op version so that the shared code is cleaner.
    /// </summary>
    internal static class DebugUtils
    {
        /// <summary>
        /// Dummy method to allow shared code to compile.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        internal static void CheckNoExternalCallers()
        {
        }
    }
}
