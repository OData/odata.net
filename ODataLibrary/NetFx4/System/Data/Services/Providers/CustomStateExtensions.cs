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

namespace System.Data.Services.Providers
{
    #region Namespaces.
    using System.Data.OData;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Helper extension methods for CustomStateAnnotation.
    /// </summary>
    internal static class CustomStateExtensions
    {
        /// <summary>
        /// Returns the value of the CustomState for the specified annotatable.
        /// </summary>
        /// <param name="annotatable">The annotatable instance to get the CustomState for.</param>
        /// <returns>The value of the CustomState.</returns>
        internal static object GetCustomState(this ODataAnnotatable annotatable)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");

            CustomStateAnnotation annotation = annotatable.GetAnnotation<CustomStateAnnotation>();
            if (annotation == null)
            {
                return null;
            }
            else
            {
                return annotation.Value;
            }
        }

        /// <summary>
        /// Sets the value of the CustomState for the specified annotatable.
        /// </summary>
        /// <param name="annotatable">The annotatable instance to set the CustomState for.</param>
        /// <param name="value">The value of the CustomState to set, will overwrite any existing value.</param>
        internal static void SetCustomState(this ODataAnnotatable annotatable, object value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(annotatable != null, "annotatable != null");

            CustomStateAnnotation annotation = annotatable.GetAnnotation<CustomStateAnnotation>();
            if (annotation == null)
            {
                annotation = new CustomStateAnnotation();
            }

            annotation.Value = value;
            annotatable.SetAnnotation(annotation);
        }
    }
}
