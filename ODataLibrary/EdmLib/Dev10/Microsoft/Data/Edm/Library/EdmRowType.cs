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

using System;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a definition of an EDM row type.
    /// </summary>
    public class EdmRowType : EdmStructuredType, IEdmRowType
    {
        /// <summary>
        /// Initializes a new instance of the EdmRowType class.
        /// </summary>
        public EdmRowType()
            : base(EdmTypeKind.Row)
        {
        }

        /// <summary>
        /// Ensures the new base type is the correct type and sets the base type of the instance.
        /// </summary>
        /// <param name="newBaseType">New base type for this type.</param>
        protected override void SetBaseType(IEdmStructuredType newBaseType)
        {
            throw new InvalidOperationException(Edm.Strings.SetBaseType_RowCantHaveBaseType);
        }
    }
}
