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
    /// Represents an EDM function parameter.
    /// </summary>
    public class EdmFunctionParameter : EdmNamedElement, IEdmFunctionParameter
    {
        private readonly IEdmTypeReference type;
        private readonly EdmFunctionParameterMode mode;
        private readonly IEdmFunctionBase declaringFunction;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionParameter"/> class.
        /// </summary>
        /// <param name="declaringFunction">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        public EdmFunctionParameter(IEdmFunctionBase declaringFunction, string name, IEdmTypeReference type)
            : this(declaringFunction, name, type, EdmFunctionParameterMode.In)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionParameter"/> class.
        /// </summary>
        /// <param name="declaringFunction">Declaring function of the parameter.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="mode">Mode of the parameter.</param>
        public EdmFunctionParameter(IEdmFunctionBase declaringFunction, string name, IEdmTypeReference type, EdmFunctionParameterMode mode)
            : base(name)
        {
            EdmUtil.CheckArgumentNull(declaringFunction, "declaringFunction");
            EdmUtil.CheckArgumentNull(name, "name");
            EdmUtil.CheckArgumentNull(type, "type");

            this.type = type;
            this.mode = mode;
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
        /// Gets the function or function import that declared this parameter.
        /// </summary>
        public IEdmFunctionBase DeclaringFunction
        {
            get { return this.declaringFunction; }
        }

        /// <summary>
        /// Gets the mode of this parameter.
        /// </summary>
        public EdmFunctionParameterMode Mode
        {
            get { return this.mode; }
        }
    }
}
