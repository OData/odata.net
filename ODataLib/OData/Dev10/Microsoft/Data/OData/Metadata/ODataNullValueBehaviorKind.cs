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

namespace Microsoft.Data.OData.Metadata
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Behavior of readers when reading property with null value.
    /// </summary>
    public enum ODataNullValueBehaviorKind
    {
        /// <summary>
        /// The default behavior - this means validate the null value against the declared type
        /// and then report the null value.
        /// </summary>
        Default = 0,

        /// <summary>
        /// This means to not report the value and not validate it against the model.
        /// </summary>
        /// <remarks>
        /// This setting can be used to correctly work with clients that send null values
        /// for uninitialized properties in requests instead of omitting them altogether.
        /// </remarks>
        IgnoreValue = 1,

        /// <summary>
        /// This means to report the value, but not validate it against the model.
        /// </summary>
        DisableValidation = 2,
    }
}
