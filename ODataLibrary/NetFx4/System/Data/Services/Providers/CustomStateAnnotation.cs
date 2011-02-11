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
    #endregion Namespaces.

    /// <summary>
    /// Annotation used on the metadata instances to store the value of CustomState property.
    /// </summary>
    internal class CustomStateAnnotation
    {
        /// <summary>
        /// The value of the CustomState property.
        /// </summary>
        private object customState;

        /// <summary>
        /// The value of the CustomState property.
        /// </summary>
        internal object Value
        {
            get
            {
                DebugUtils.CheckNoExternalCallers();

                return this.customState;
            }

            set
            {
                DebugUtils.CheckNoExternalCallers();

                this.customState = value;
            }
        }
    }
}
