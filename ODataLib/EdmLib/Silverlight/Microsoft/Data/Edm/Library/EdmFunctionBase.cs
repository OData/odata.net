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

using System.Collections.Generic;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Represents a base class for EDM functions and function imports.
    /// </summary>
    public abstract class EdmFunctionBase : EdmNamedElement, IEdmFunctionBase
    {
        private readonly List<IEdmFunctionParameter> parameters = new List<IEdmFunctionParameter>();
        private IEdmTypeReference returnType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdmFunctionBase"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="returnType">The return type of the function.</param>
        protected EdmFunctionBase(string name, IEdmTypeReference returnType)
            : base(name)
        {
            this.returnType = returnType;
        }

        /// <summary>
        /// Gets the return type of this function.
        /// </summary>
        public IEdmTypeReference ReturnType
        {
            get { return this.returnType; }
        }

        /// <summary>
        /// Gets the parameters of this function.
        /// </summary>
        public IEnumerable<IEdmFunctionParameter> Parameters
        {
            get { return this.parameters; }
        }

        /// <summary>
        /// Searches for a parameter with the given name in this function and returns null if no such parameter exists.
        /// </summary>
        /// <param name="name">The name of the parameter to be found.</param>
        /// <returns>The requested parameter, or null if no such parameter exists.</returns>
        public IEdmFunctionParameter FindParameter(string name)
        {
            foreach (IEdmFunctionParameter parameter in this.parameters)
            {
                if (parameter.Name == name)
                {
                    return parameter;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates and adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <returns>Created parameter.</returns>
        public EdmFunctionParameter AddParameter(string name, IEdmTypeReference type)
        {
            EdmFunctionParameter parameter = new EdmFunctionParameter(this, name, type);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Creates and adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="name">The name of the parameter being added.</param>
        /// <param name="type">The type of the parameter being added.</param>
        /// <param name="mode">Mode of the parameter.</param>
        /// <returns>Created parameter.</returns>
        public EdmFunctionParameter AddParameter(string name, IEdmTypeReference type, EdmFunctionParameterMode mode)
        {
            EdmFunctionParameter parameter = new EdmFunctionParameter(this, name, type, mode);
            this.parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Adds a parameter to this function (as the last parameter).
        /// </summary>
        /// <param name="parameter">The parameter being added.</param>
        public void AddParameter(IEdmFunctionParameter parameter)
        {
            EdmUtil.CheckArgumentNull(parameter, "parameter");

            this.parameters.Add(parameter);
        }
    }
}
