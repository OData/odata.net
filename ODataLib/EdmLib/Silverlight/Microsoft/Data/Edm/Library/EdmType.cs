//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents the definition of an EDM type.
    /// </summary>
    public abstract class EdmType : EdmElement, IEdmType
    {
        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public abstract EdmTypeKind TypeKind
        {
            get;
        }

        /// <summary>
        /// Returns the text representation of the current object.
        /// </summary>
        /// <returns>The text representation of the current object.</returns>
        public override string ToString()
        {
            return this.ToTraceString();
        }
    }
}
