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

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents a reference to an EDM type definition.
    /// </summary>
    public class EdmTypeDefinitionReference : EdmTypeReference, IEdmTypeDefinitionReference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmTypeDefinitionReference"/> class.
        /// </summary>
        /// <param name="typeDefinition">The definition refered to by this reference.</param>
        /// <param name="isNullable">Denotes whether the type can be nullable.</param>
        public EdmTypeDefinitionReference(IEdmTypeDefinition typeDefinition, bool isNullable)
            : base(typeDefinition, isNullable)
        {
        }
    }
}
