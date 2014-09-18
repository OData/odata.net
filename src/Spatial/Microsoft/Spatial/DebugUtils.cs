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
