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

namespace System.Data.OData.Atom
{
    #region Namespaces.
    using System.Data.Services.Providers;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Extension methods to make it easier to work with EPM.
    /// </summary>
    internal static class EpmExtensionMethods
    {
        /// <summary>
        /// Returns the EPM annotation for a resource type.
        /// </summary>
        /// <param name="resourceType">The resource type to get the EPM annotation for.</param>
        /// <returns>Returns the EPM annotation for a resource type. If there's no such annotation this returns null.</returns>
        internal static EpmResourceTypeAnnotation Epm(this ResourceType resourceType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(resourceType != null, "resourceType != null");

            return resourceType.GetAnnotation<EpmResourceTypeAnnotation>();
        }
    }
}
