//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents an EDM operation parameter.
    /// </summary>
    public class EdmOperationParameter : EdmNamedElement, IEdmOperationParameter
    {
        private readonly IEdmTypeReference type;
        private readonly IEdmFunctionBase declaringFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmOperationParameter"/> class.
        /// </summary>
        /// <param name="declaringFunction">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmOperationParameter(IEdmFunctionBase declaringFunction, string name, IEdmTypeReference type)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringFunction, "declaringFunction");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(type, "type");

            this.type = type;
            this.declaringFunction = declaringFunction;
        }

        /// <summary>
        /// Gets the type of this parameter.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the function or operation import that declared this parameter.
        /// </summary>
        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }
    }
}
