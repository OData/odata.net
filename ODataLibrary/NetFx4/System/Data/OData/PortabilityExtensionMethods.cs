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
    #region Namespaces
    using System.Data.Services.Providers;
    #endregion Namespaces

#if WINDOWS_PHONE || ORCAS
    /// <summary>
    /// Helper extension methods to hide differences in platforms.
    /// </summary>
    internal static class PortabilityExtensionMethods
    {
        /// <summary>
        /// Returns true if the specified kind enum has the specified flags set.
        /// </summary>
        /// <param name="kind">The kind enum to test.</param>
        /// <param name="flag">The flag to look for.</param>
        /// <returns>true if the flag is set; false otherwise.</returns>
        internal static bool HasFlag(this ResourcePropertyKind kind, ResourcePropertyKind flag)
        {
            DebugUtils.CheckNoExternalCallers();
            return (kind & flag) == flag;
        }
    }
#endif
}
