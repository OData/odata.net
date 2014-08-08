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
    /// Represents a definition of an EDM row type.
    /// </summary>
    public class EdmRowType : EdmStructuredType, IEdmRowType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmRowType"/> class.
        /// </summary>
        public EdmRowType()
            : base(false, false, null)
        {
        }

        /// <summary>
        /// Gets the kind of this type.
        /// </summary>
        public override EdmTypeKind TypeKind
        {
            get { return EdmTypeKind.Row; }
        }
    }
}
