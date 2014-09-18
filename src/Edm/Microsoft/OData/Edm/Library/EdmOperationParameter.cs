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
    /// Represents an EDM operation parameter.
    /// </summary>
    public class EdmOperationParameter : EdmNamedElement, IEdmOperationParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationParameter"/> class.
        /// </summary>
        /// <param name="declaringOperation">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmOperationParameter(IEdmOperation declaringOperation, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringOperation, "declaringFunction");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(type, "type");

            this.Type = type;
            this.DeclaringOperation = declaringOperation;
        }

        /// <summary>
        /// Gets the type of this parameter.
        /// </summary>
        public IEdmTypeReference Type { get; private set; }

        /// <summary>
        /// Gets the operation that declared this parameter.
        /// </summary>
        public IEdmOperation DeclaringOperation { get; private set; }
    }
}
