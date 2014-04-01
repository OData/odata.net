//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for writing OData content.
    /// </summary>
    internal static class WriterUtils
    {
        /// <summary>
        /// Determines if a property should be written or skipped.
        /// </summary>
        /// <param name="projectedProperties">The projected properties annotation to use (can be null).</param>
        /// <param name="propertyName">The name of the property to check.</param>
        /// <returns>true if the property should be skipped, false to write the property.</returns>
        internal static bool ShouldSkipProperty(this ProjectedPropertiesAnnotation projectedProperties, string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();

            if (projectedProperties == null)
            {
                return false;
            }
            else if (object.ReferenceEquals(ProjectedPropertiesAnnotation.EmptyProjectedPropertiesInstance, projectedProperties))
            {
                return true;
            }
            else if (object.ReferenceEquals(ProjectedPropertiesAnnotation.AllProjectedPropertiesInstance, projectedProperties))
            {
                return false;
            }

            return !projectedProperties.IsPropertyProjected(propertyName);
        }
    }
}
