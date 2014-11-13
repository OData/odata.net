//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
